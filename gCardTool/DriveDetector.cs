using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;             // required for Message
using System.Runtime.InteropServices;   // required for Marshal
using System.IO;
using Microsoft.Win32.SafeHandles;     

namespace Dolinay
{

    // Delegate for event handler to handle the device events 
    public delegate void DriveDetectorEventHandler(Object sender, DriveDetectorEventArgs e);
    
    /// <summary>
    /// Our class for passing in custom arguments to our event handlers 
    /// 
    /// </summary>
    public class DriveDetectorEventArgs : EventArgs 
    {
        public DriveDetectorEventArgs()
        {
            Cancel = false;
            Drive = "";
            HookQueryRemove = false;
        }

        /// <summary>
        /// Get/Set the value indicating that the event should be cancelled 
        /// Only in QueryRemove handler.
        /// </summary>
        public bool Cancel;

        /// <summary>
        /// Drive letter for the device which caused this event 
        /// </summary>
        public string Drive;

        /// <summary>
        /// Set to true in your DeviceArrived event handler if you wish to receive the 
        /// QueryRemove event for this drive. 
        /// </summary>
        public bool HookQueryRemove;

    }



    /// <summary>
    /// Detects insertion or removal of removable drives.
    /// Use it in 2 steps:
    /// 1) Create instance of this class in your project and add handlers for the
    /// DeviceArrived, DeviceRemoved and QueryRemove events.
    /// 2) Override WndProc in your form and call DriveDetector's WndProc from there. 
    /// </summary>
    class DriveDetector : IDisposable 
    {
        /// <summary>
        /// Events signalized to the client app.
        /// Add handlers for these events in your form to be notified of removable device events 
        /// </summary>
        public event DriveDetectorEventHandler DeviceArrived;
        public event DriveDetectorEventHandler DeviceRemoved;
        public event DriveDetectorEventHandler QueryRemove;


        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="control">object which will receive Windows messages. 
        /// Pass "this" as this argument from your form class.</param>
        public DriveDetector(Control control)
        {
            mFileToOpen = null;
            mDeviceNotifyHandle = IntPtr.Zero;
            mRecipientHandle = control.Handle;
            mCurrentDrive = "";
        }

        /// <summary>
        /// Consructs DriveDetector object setting also path to file which should be opened
        /// when registering for query remove.  
        /// </summary>
        ///<param name="control">object which will receive Windows messages. 
        /// Pass "this" as this argument from your form class.</param>
        /// <param name="FileToOpen">Optional. Name of a file on the removable drive which should be opened. 
        /// If null, any file on the drive will be opened. Opening a file is needed for us 
        /// to be able to register for the query remove message. TIP: Use relative path without drive letter.
        /// e.g. "SomeFolder\file_on_flash.txt"</param>
        public DriveDetector(Control control, string FileToOpen)
        {
            mFileToOpen = FileToOpen;
            mDeviceNotifyHandle = IntPtr.Zero;
            mRecipientHandle = control.Handle;
            mCurrentDrive = "";
        }


        /// <summary>
        /// Gets the value indicating whether the query remove event will be fired.
        /// </summary>
	    public bool IsQueryHooked
	    {
		    get
            {
                if (mDeviceNotifyHandle == IntPtr.Zero)
                    return false;
                else
                    return true;
            }
	    }

        /// <summary>
        /// Gets letter of drive which is currently hooked. Empty string if none.
        /// See also IsQueryHooked.
        /// </summary>
        public string HookedDrive
        {
            get
            {
                return mCurrentDrive;
            }
        }

        /// <summary>
        /// Gets the file stream for file which this class opened on a drive to be notified
        /// about it's removal. 
        /// </summary>
        public FileStream OpenedFile
        {
            get
            {
                return mFileOnFlash;
            }
        }

        /// <summary>
        /// Hooks specified drive to receive a message when it is being removed.  
        /// This can be achieved also by setting e.HookQueryRemove to true in your 
        /// DeviceArrived event handler. In that case mFileToOpen (which can be set using second constructor)
        /// is used as the file to open.
        /// </summary>
        /// <param name="fileOnDrive">drive letter or relative path to a file on the drive which should be 
        /// used to get a handle - required for registering to receive query remove messages.
        /// If only drive letter is specified (e.g. "D:\\", any file found on the flash drive can be used.</param>
        /// <returns>true if hooked ok, false otherwise</returns>
        public bool EnableQueryRemove(string fileOnDrive)
        {
            if (fileOnDrive == null || fileOnDrive.Length == 0)
                throw new ArgumentException("Drive path must be supplied to register for Query remove.");
            if ( fileOnDrive.Length == 2 && fileOnDrive[1] == ':' )
                fileOnDrive += '\\';        // append "\\" if only drive letter with ":" was passed in.

            if (mDeviceNotifyHandle != IntPtr.Zero)
            {
                RegisterForDeviceChange(false, null);
            }

            if (!File.Exists(fileOnDrive))
                mFileToOpen = null;     // use any file
            else
                mFileToOpen = fileOnDrive;

            RegisterQuery(Path.GetPathRoot(fileOnDrive));
            if (mDeviceNotifyHandle == IntPtr.Zero)
                return false;   // failed to register

            return true;
        }

        /// <summary>
        /// Unhooks any currently hooked drive so that the query remove 
        /// message is not generated for it.
        /// </summary>
        public void DisableQueryRemove()
        {
            if (mDeviceNotifyHandle != IntPtr.Zero)
            {
                RegisterForDeviceChange(false, null);
            }
        }


        /// <summary>
        /// Unregister and close the file we may have opened on the removable drive. 
        /// Garbage collector will call this method.
        /// </summary>
        public void Dispose()
        {
            RegisterForDeviceChange(false, null);
        }


        #region WindowProc
        /// <summary>
        /// Message handler which must be called from client form.
        /// Processes Windows messages and calls event handlers. 
        /// </summary>
        /// <param name="m"></param>
        public void WndProc(ref Message m)
        {
            int devType;
            char c;

            if (m.Msg == WM_DEVICECHANGE)
            {
                // WM_DEVICECHANGE can have several meanings depending on the WParam value...
                switch (m.WParam.ToInt32())
                {

                        //
                        // New device has just arrived
                        //
                    case DBT_DEVICEARRIVAL:

                        devType = Marshal.ReadInt32(m.LParam, 4);
                        if (devType == DBT_DEVTYP_VOLUME)
                        {
                            DEV_BROADCAST_VOLUME vol;
                            vol = (DEV_BROADCAST_VOLUME)
                                Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_VOLUME));

                            // Get the drive letter 
                            c = DriveMaskToLetter(vol.dbcv_unitmask);


                            //
                            // Call the client event handler
                            //
                            // We should create copy of the event before testing it and
                            // calling the delegate - if any
                            DriveDetectorEventHandler tempDeviceArrived = DeviceArrived;
                            if ( tempDeviceArrived != null )
                            {
                                DriveDetectorEventArgs e = new DriveDetectorEventArgs();
                                e.Drive = c + ":\\";
                                tempDeviceArrived(this, e);
                                
                                // Register for query remove if requested
                                if (e.HookQueryRemove)
                                {
                                    // If something is already hooked, unhook it now
                                    if (mDeviceNotifyHandle != IntPtr.Zero)
                                    {
                                        RegisterForDeviceChange(false, null);
                                    }
                                        
                                   RegisterQuery(c + ":\\");
                                }
                            }     // if  has event handler

                            
                        }
                        break;



                        //
                        // Device is about to be removed
                        // Any application can cancel the removal
                        //
                    case DBT_DEVICEQUERYREMOVE:

                        devType = Marshal.ReadInt32(m.LParam, 4);
                        if (devType == DBT_DEVTYP_HANDLE)
                        {

                            // TODO: we could get the handle for which this message is sent 
                            // from vol.dbch_handle and compare it agains a list of handles for 
                            // which we have registered the query remove message (?)                                                 
                            //DEV_BROADCAST_HANDLE vol;
                            //vol = (DEV_BROADCAST_HANDLE)
                            //   Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_HANDLE));
                            // if ( vol.dbch_handle ....
                            
  
                            //
                            // Call the event handler in client
                            //
                            DriveDetectorEventHandler tempQuery = QueryRemove;
                            if (tempQuery != null)
                            {
                                DriveDetectorEventArgs e = new DriveDetectorEventArgs();
                                e.Drive = mCurrentDrive;        // drive which is hooked
                                tempQuery(this, e);

                                // If the client wants to cancel, let Windows know
                                if (e.Cancel)
                                {                                    
                                    m.Result = (IntPtr)BROADCAST_QUERY_DENY;
                                }
                                else
                                {
                                    // Close the handle so that the drive can be unmounted
                                    if (mFileOnFlash != null)
                                    {
                                        mFileOnFlash.Close();
                                        mFileOnFlash = null;
                                    }
                                }

                           }                          
                        }
                        break;


                        //
                        // Device has been removed
                        //
                    case DBT_DEVICEREMOVECOMPLETE:

                        devType = Marshal.ReadInt32(m.LParam, 4);
                        if (devType == DBT_DEVTYP_VOLUME)
                        {
                            devType = Marshal.ReadInt32(m.LParam, 4);
                            if (devType == DBT_DEVTYP_VOLUME)
                            {
                                DEV_BROADCAST_VOLUME vol;
                                vol = (DEV_BROADCAST_VOLUME)
                                    Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_VOLUME));
                                c = DriveMaskToLetter(vol.dbcv_unitmask);

                                //
                                // Call the client event handler
                                //
                                DriveDetectorEventHandler tempDeviceRemoved = DeviceRemoved;
                                if (tempDeviceRemoved != null)
                                {
                                    DriveDetectorEventArgs e = new DriveDetectorEventArgs();
                                    e.Drive = c + ":\\";
                                    tempDeviceRemoved(this, e);
                                }

                                // TODO: we could unregister the notify handle here if we knew it is the
                                // right drive which has been just removed
                                //RegisterForDeviceChange(false, null);
                            }
                        }
                        break;
                }

            }

        }
        #endregion



        #region  Private Area

        /// <summary>
        /// Class which contains also handle to the file opened on the flash drive
        /// </summary>
        private FileStream mFileOnFlash = null;

        /// <summary>
        /// Name of the file to try to open on the removable drive for query remove registration
        /// </summary>
        private string mFileToOpen;

        /// <summary>
        /// Handle to file which we keep opened on the drive if query remove message is required by the client
        /// </summary>       
        private IntPtr mDeviceNotifyHandle;

        /// <summary>
        /// Handle of the window which receives messages from Windows. This will be a form.
        /// </summary>
        private IntPtr mRecipientHandle;

        /// <summary>
        /// Drive which is currently hooked for query remove
        /// </summary>
        private string mCurrentDrive;   


        // Win32 constants
        private const int DBT_DEVTYP_DEVICEINTERFACE = 5;
        private const int DBT_DEVTYP_HANDLE = 6;
        private const int BROADCAST_QUERY_DENY = 0x424D5144;
        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000; // system detected a new device
        private const int DBT_DEVICEQUERYREMOVE = 0x8001;   // Preparing to remove (any program can disable the removal)
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004; // removed 
        private const int DBT_DEVTYP_VOLUME = 0x00000002; // drive type is logical volume

        /// <summary>
        /// Registers for receiving the query remove message for a given drive.
        /// We need to open a handle on that drive and register with this handle. 
        /// CLient can specify this file in mFileToOpen or we will use any file we fing on the drive
        /// </summary>
        /// <param name="drive">drive for which to register. </param>
        private void RegisterQuery(string drive)
        {
            bool register = true;

            if (mFileToOpen == null)
            {
                // If client gave us no file, let's pick one on the drive... 
                mFileToOpen = GetAnyFile(drive);
                if (mFileToOpen.Length == 0)
                    return;     // no file found on the flash drive
            }
            else
            {
                // Make sure the path in mFileToOpen contains valid drive
                // If there is a drive letter in the path, it may be different from the  actual
                // letter assigned to the drive now. We will cut it off and merge the actual drive 
                // with the rest of the path.
                if (mFileToOpen.Contains(":"))
                {
                    string tmp = mFileToOpen.Substring(3);
                    string root = Path.GetPathRoot(drive);
                    mFileToOpen = Path.Combine(root, tmp);
                }
                else
                    mFileToOpen = Path.Combine(drive, mFileToOpen);
            }


            try
            {
                mFileOnFlash = new FileStream(mFileToOpen, FileMode.Open);
            }
            catch (Exception)
            {
                // just do not register if the file could not be opened
                register = false;
            }

            if (register)
            {
                RegisterForDeviceChange(true, mFileOnFlash.SafeFileHandle);
                mCurrentDrive = drive;
            }


        }

        /// <summary>
        /// Registers to be notified when the volume is about to be removed
        /// This is requierd if you want to get the QUERY REMOVE messages
        /// </summary>
        /// <param name="register">true to register, false to unregister</param>
        /// <param name="fileHandle">handle of a file opened on the removable drive</param>
        private void RegisterForDeviceChange(bool register, SafeFileHandle fileHandle)
        {
            if (register)
            {
                // Register for handle
                DEV_BROADCAST_HANDLE data = new DEV_BROADCAST_HANDLE();
                data.dbch_devicetype = DBT_DEVTYP_HANDLE;
                data.dbch_reserved = 0;
                data.dbch_nameoffset = 0;
                //data.dbch_data = null;
                //data.dbch_eventguid = 0;
                data.dbch_handle = fileHandle.DangerousGetHandle(); //Marshal. fileHandle; 
                data.dbch_hdevnotify = (IntPtr)0;
                int size = Marshal.SizeOf(data);
                data.dbch_size = size;
                IntPtr buffer = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(data, buffer, true);

                mDeviceNotifyHandle = Native.RegisterDeviceNotification(mRecipientHandle, buffer, 0);
            }
            else
            {
                // unregister
                if (mDeviceNotifyHandle != IntPtr.Zero)
                    Native.UnregisterDeviceNotification(mDeviceNotifyHandle);
                mDeviceNotifyHandle = IntPtr.Zero;
                mCurrentDrive = "";
                if (mFileOnFlash != null)
                {
                    mFileOnFlash.Close();
                    mFileOnFlash = null;
                }
            }

        }

        /// <summary>
        /// Gets drive letter from a bit mask where bit 0 = A, bit 1 = B etc.
        /// There can actually be more than one drive in the mask but we 
        /// just use the last one in this case.
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        private static char DriveMaskToLetter(int mask)
        {
            char letter;
            string drives = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            // 1 = A
            // 2 = B
            // 4 = C...
            int cnt = 0;
            int pom = mask / 2;     
            while (pom != 0)
            {
                // while there is any bit set in the mask
                // shift it to the righ...                
                pom = pom / 2;
                cnt++;
            }

            if (cnt < drives.Length)
                letter = drives[cnt];
            else
                letter = '?';

            return letter;
        }

        /// <summary>
        /// Searches for any file in a given path and returns its full path
        /// </summary>
        /// <param name="drive">drive to search</param>
        /// <returns>path of the file or empty string</returns>
        private string GetAnyFile(string drive)
        {
            string file = "";
            // First try files in the root
            string[] files = Directory.GetFiles(drive);
            if (files.Length == 0)
            {
                // if no file in the root, search whole drive
                files = Directory.GetFiles(drive, "*.*", SearchOption.AllDirectories);
            }
                
            if (files.Length > 0)
                file = files[0];        // get the first file

            // return empty string if no file found
            return file;
        }
        #endregion


        #region Native Win32 API
        /// <summary>
        /// WinAPI functions
        /// </summary>        
        private class Native
        {
            //   HDEVNOTIFY RegisterDeviceNotification(HANDLE hRecipient,LPVOID NotificationFilter,DWORD Flags);
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, uint Flags);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern uint UnregisterDeviceNotification(IntPtr hHandle);

        }

        
        // Structure with information for RegisterDeviceNotification.
        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_HANDLE
        {
            public int dbch_size;
            public int dbch_devicetype;
            public int dbch_reserved;
            public IntPtr dbch_handle;
            public IntPtr dbch_hdevnotify;
            public Guid dbch_eventguid;
            public long dbch_nameoffset;
            //public byte[] dbch_data[1]; // = new byte[1];
            public byte dbch_data;
            public byte dbch_data1; 
        }

        // Struct for parameters of the WM_DEVICECHANGE message
        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_VOLUME
        {
            public int dbcv_size;
            public int dbcv_devicetype;
            public int dbcv_reserved;
            public int dbcv_unitmask;
        }
        #endregion

    }
}
