using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.IO;
using Dolinay;
using System.Diagnostics;
using System.Security.Principal;

namespace gCardTool
{
    [SuppressUnmanagedCodeSecurity]
    public partial class Form1 : Form
    {

        const int FILE_FLAG_NO_BUFFERING = unchecked((int)0x20000000);
        const int FILE_FLAG_SEQUENTIAL_SCAN = unchecked((int)0x08000000);

        uint SectorsPerCluster;
        uint BytesPerSector;
        uint NumberOfFreeClusters;
        uint TotalNumberOfClusters;

        FileStream fs;
        byte[] g;
        string tempPath = System.IO.Path.GetTempPath().ToString();
        string mmcSel = "0";

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern SafeFileHandle CreateFile(String fileName,
        int desiredAccess,
        System.IO.FileShare shareMode,
        IntPtr securityAttrs,
        System.IO.FileMode creationDisposition,
        int flagsAndAttributes,
        IntPtr templateFile);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool GetDiskFreeSpace(string lpRootPathName,
           out uint lpSectorsPerCluster,
           out uint lpBytesPerSector,
           out uint lpNumberOfFreeClusters,
           out uint lpTotalNumberOfClusters);

        private DriveDetector driveDetector = null;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (driveDetector != null)
            {
                driveDetector.WndProc(ref m);
            }
        }

        public Form1()
        {
            InitializeComponent();

            driveDetector = new DriveDetector(this);
            driveDetector.DeviceArrived += new DriveDetectorEventHandler(OnDriveArrived);
            driveDetector.DeviceRemoved += new DriveDetectorEventHandler(OnDriveRemoved);
        }

        private void OnDriveArrived(object sender, DriveDetectorEventArgs e)
        {
            listTargetDrives();
        }

        private void OnDriveRemoved(object sender, DriveDetectorEventArgs e)
        {
            listTargetDrives();
        }

        private void listTargetDrives()
        {
            try
            {
                cmbMmc.Items.Clear();
                DriveInfo[] allDrives = DriveInfo.GetDrives();
                foreach (DriveInfo d in allDrives)
                {
                    if (d.IsReady)
                    {
                        if (d.DriveType == DriveType.Removable)
                        {
                            cmbMmc.Items.Add(d.Name + " [ Removable - " + calcDriveSize(d.TotalSize) + " ]");
                        }
                    }
                }
                if (cmbMmc.Items.Count > 0)
                {
                    cmbMmc.SelectedIndex = 0;
                    button3.Enabled = true;
                }
                else
                {
                    button3.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An error occured{0}", ex.Message.ToString()), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string calcDriveSize(long baseSize)
        {
            decimal driveSize = baseSize / 1024;
            string sizeType = "Bytes";
            if (driveSize > 1024)
            {
                driveSize = driveSize / 1024;
                sizeType = "MB";
            }
            if (driveSize > 1024)
            {
                driveSize = driveSize / 1024;
                sizeType = "GB";
            }
            string retSize = driveSize.ToString("N2") + sizeType;

            return retSize;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //try
            //{
                DriveNumber usb = new DriveNumber();
                string driveLetter = cmbMmc.SelectedItem.ToString().Substring(0, 2).ToUpper();

                GetDiskFreeSpace(driveLetter + "\\", out SectorsPerCluster, out BytesPerSector, out NumberOfFreeClusters, out TotalNumberOfClusters);

                string driveNum = usb.getdriveNumber(driveLetter);
                SafeFileHandle handle;
                int blockSize = int.Parse(BytesPerSector.ToString());
                int flags = FILE_FLAG_NO_BUFFERING | FILE_FLAG_SEQUENTIAL_SCAN;
                // Call the Windows CreateFile() API to open the file 
                handle = CreateFile(@"\\.\PHYSICALDRIVE" + driveNum, (int)FileAccess.ReadWrite, FileShare.None, IntPtr.Zero, FileMode.Open, flags, IntPtr.Zero);
                if (!handle.IsInvalid)
                {
                    bool isdone = true;
                    fs = new FileStream(handle, FileAccess.ReadWrite, blockSize, false);
                    byte[] b = new byte[blockSize];
                    byte[] c = new byte[blockSize];
                    txtDbgOut.Text += "Reading sector 0 of Drive" + driveLetter + "\r\n";
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.Read(b, 0, b.Length);
                    txtDbgOut.Text += "Converting to GoldCard Sector" + "\r\n";
                    for (int i = 0; i < g.Length; i++)
                    {
                        b[i] = g[i];
                    }
                    txtDbgOut.Text += "Writing sector 0 of Drive" + driveLetter + "\r\n";
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.Write(b, 0, b.Length);
                    txtDbgOut.Text += "Validating sector 0 of Drive" + driveLetter + "\r\n";
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.Read(c, 0, c.Length);

                    for (int i = 0; i < c.Length; i++)
                    {
                        if (b[i] != c[i])
                        {
                            isdone = false;
                        }
                    }

                    if (isdone)
                    {
                        txtDbgOut.Text += "Writing sector 0 of Drive" + driveLetter + " Success" + "\r\n";
                        MessageBox.Show(string.Format("Success {0}\\ is now a Gold Card", driveLetter), "All Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        txtDbgOut.Text += "Writing sector 0 of Drive" + driveLetter + " Failed" + "\r\n";
                        txtDbgOut.Text += "Sector 0 of Drive" + driveLetter + " does not match buffer" + "\r\n";
                        MessageBox.Show(string.Format("An error occured, Failed to write to {0}", driveLetter), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    //textBox1.Text += "Failed to open device";
                    MessageBox.Show(string.Format("An error occured {0}", "Could not get access to MMC"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            /*}
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An error occured {0}", ex.Message.ToString()), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           */
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text += " 0.0.7";
            this.Text += new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator) ? " (Admin)" : "";
            this.Icon = Properties.Resources.microsd;
            listTargetDrives();
            if (this.Text.IndexOf("Admin") == -1)
            {
                MessageBox.Show(string.Format("You must run this application as an {0}\nRight Click the icon and run as Administrator", "Administrator"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void btnRefreshMmc_Click(object sender, EventArgs e)
        {
            listTargetDrives();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "http://psas.revskills.de/?q=goldcard";
            p.Start();
        }

        private void btnGetCid_Click(object sender, EventArgs e)
        {
            txtMmcCid.Text = "Working...";
            try
            {
                string normCid = string.Empty;
                this.Cursor = Cursors.WaitCursor;
                btnGetCid.Enabled = false;
                if (radMMC1.Checked) { mmcSel = "1"; } else { mmcSel = "0"; }
                txtDbgOut.Text += "Executing adb shell cat /sys/class/mmc_host/mmc" + mmcSel + "/mmc" + mmcSel + ":*/cid" + "\r\n";
                File.WriteAllBytes(tempPath + "adb.exe", Properties.Resources.adb);
                File.WriteAllBytes(tempPath + "AdbWinApi.dll", Properties.Resources.AdbWinApi);
                File.WriteAllBytes(tempPath + "AdbWinUsbApi.dll", Properties.Resources.AdbWinUsbApi);

                if (File.Exists(tempPath + "runadb.cmd")) { File.Delete(tempPath + "runadb.cmd"); }
                if (File.Exists(tempPath + "killadb.cmd")) { File.Delete(tempPath + "killadb.cmd"); }

                using (StreamWriter sw = new StreamWriter(tempPath + "runadb.cmd"))
                {
                    sw.WriteLine("@echo off");
                    sw.WriteLine("adb shell cat /sys/class/mmc_host/mmc" + mmcSel + "/mmc" + mmcSel + ":*/cid>mycid.txt");
                    sw.Close();
                }

                using (StreamWriter sw = new StreamWriter(tempPath + "killadb.cmd"))
                {
                    sw.WriteLine("@echo off");
                    sw.WriteLine("adb kill-server");
                    sw.Close();
                }

                Process runAdb = new Process();
                runAdb.StartInfo.UseShellExecute = false;
                runAdb.StartInfo.FileName = tempPath + "runadb.cmd";
                runAdb.StartInfo.WorkingDirectory = tempPath;
                runAdb.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                runAdb.StartInfo.CreateNoWindow = true;
                runAdb.Start();
                runAdb.WaitForExit();

                Process killAdb = new Process();
                killAdb.StartInfo.FileName = tempPath + "killadb.cmd";
                killAdb.StartInfo.WorkingDirectory = tempPath;
                killAdb.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                killAdb.StartInfo.CreateNoWindow = true;
                killAdb.Start();
                killAdb.WaitForExit();

                if (File.Exists(tempPath + "runadb.cmd")) { File.Delete(tempPath + "runadb.cmd"); }
                if (File.Exists(tempPath + "killadb.cmd")) { File.Delete(tempPath + "killadb.cmd"); }

                File.Copy(tempPath + "mycid.txt", tempPath + "mycidcopy.txt", true);
                txtDbgOut.Text += "Reading adb output" + "\r\n";
                using (StreamReader sr = new StreamReader(tempPath + "mycidcopy.txt"))
                {
                    txtDbgOut.Text += "------------------------" + "\r\n";
                    string srLine = string.Empty;
                    while (sr.Peek() >= 0)
                    {
                        srLine = sr.ReadLine();
                        txtDbgOut.Text += srLine + "\r\n";
                        if (int.Parse(srLine.Length.ToString()) == 32)
                        {
                            normCid = srLine.ToUpper();
                            break;
                        }
                    }
                    txtDbgOut.Text += "------------------------" + "\r\n";
                }

                if (normCid == string.Empty)
                {
                    txtDbgOut.Text += "Could not locate CID in output" + "\r\n";
                    txtMmcCid.Text = "Failed to get CID";
                }
                else
                {
                    txtDbgOut.Text += "CID: "+ normCid + "\r\n";
                    txtDbgOut.Text += "Reversing CID" + "\r\n";
                    StringBuilder revCid = new StringBuilder(normCid.Length);
                    for (int i = normCid.Length - 1; i >= 0; i -= 2)
                    {
                        revCid.Append(normCid[i - 1]);
                        revCid.Append(normCid[i]);
                    }
                    txtMmcCid.Text = "00" + revCid.ToString().Remove(0, 2);
                    txtDbgOut.Text += "CID: " + txtMmcCid.Text + "\r\n";
                }

                this.Cursor = Cursors.Default;
                btnGetCid.Enabled = true;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(string.Format("An error occured {0}", ex.Message.ToString()), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String goldCardFile = string.Empty;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "GoldCard Image|*.img";
            dlg.Title = "Load GoldCard Image";
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                goldCardFile = dlg.FileName.ToString();
                g = File.ReadAllBytes(goldCardFile);
                if (g.Length != 384)
                {
                    button1.Enabled = false;
                    txtDbgOut.Text += goldCardFile + " is not 384 bytes" + "\r\n";
                    MessageBox.Show(string.Format("An error occured {0}", "Does not appear to be a valid GoldCard"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    txtDbgOut.Text += "Loaded " + goldCardFile + "\r\n";
                    button1.Enabled = true;
                }
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "http://l.vorbeth.co.uk/ieyrI4";
            p.Start();
        }

        private void formClosing(object sender, FormClosingEventArgs e)
        {
            if (File.Exists(tempPath + "adb.exe")) { File.Delete(tempPath + "adb.exe"); }
            if (File.Exists(tempPath + "AdbWinApi.dll")) { File.Delete(tempPath + "AdbWinApi.dll"); }
            if (File.Exists(tempPath + "AdbWinUsbApi.dll")) { File.Delete(tempPath + "AdbWinUsbApi.dll"); }

            if (File.Exists(tempPath + "mycid.txt")) { File.Delete(tempPath + "mycid.txt"); }
            if (File.Exists(tempPath + "mycidcopy.txt")) { File.Delete(tempPath + "mycidcopy.txt"); }
        }

        private void txtDbgOut_TextChanged(object sender, EventArgs e)
        {
            txtDbgOut.SelectionStart = txtDbgOut.TextLength;
            txtDbgOut.ScrollToCaret();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string driveLetter = cmbMmc.SelectedItem.ToString().Substring(0, 2).ToUpper();
            DialogResult r = MessageBox.Show("This will wipe all data on drive " + driveLetter + "\nAre you sure?", "Format Drive", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                txtDbgOut.Text += "Formatting drive " + driveLetter + " to FAT32" + "\r\n";

                using (StreamWriter sw = new StreamWriter(tempPath + "formatmmc.cmd"))
                {
                    sw.WriteLine("@echo off");
                    sw.WriteLine("echo |FORMAT " + driveLetter + " /V:GoldCard /FS:FAT32 /X /Q");
                    sw.Close();
                }

                Process fmMMC = new Process();
                fmMMC.StartInfo.UseShellExecute = false;
                fmMMC.StartInfo.FileName = tempPath + "formatmmc.cmd";
                fmMMC.StartInfo.WorkingDirectory = tempPath;
                fmMMC.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                fmMMC.StartInfo.CreateNoWindow = true;
                fmMMC.Start();
                fmMMC.WaitForExit();

                txtDbgOut.Text += "Formatting done." + "\r\n";
                txtDbgOut.Text += "Unmount MMC before reading CID." + "\r\n";
                txtDbgOut.Text += "------------------------" + "\r\n";
            }
            if (File.Exists(tempPath + "formatmmc.cmd")) { File.Delete(tempPath + "formatmmc.cmd"); }

        }
    }
}
