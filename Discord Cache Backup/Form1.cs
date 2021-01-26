using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Discord_Cache_Backup
{
    public partial class Form1 : Form
    {

        public class UsrData
        {
            public string[] localStorageSplit { get; set; }
            public string localStorageLocation { get; set; }

            public string[] sessionStorageSplit { get; set; }
            public string sessionStorageLocation { get; set; }

        }
        public Form1()
        {
            InitializeComponent();

            if (!File.Exists("backup"))
            {
                Directory.CreateDirectory("backup");
            }
            if(!File.Exists("backup/Local Storage"))
            {
                Directory.CreateDirectory("backup/Local Storage");
            }
            if (!File.Exists("backup/Session Storage"))
            {
                Directory.CreateDirectory("backup/Session Storage");
            }
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;

            button2.FlatStyle = FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 0;

            button3.FlatStyle = FlatStyle.Flat;
            button3.FlatAppearance.BorderSize = 0;

            /*
            string[] files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\discord\\Local Storage\\leveldb", "*.log", SearchOption.AllDirectories); // Array: ["C:\\Users\\smashie420\\AppData\\Roaming\\discord\\Local Storage\\leveldb\\00003.log"]
            UsrData data = new UsrData
            {
                fileDir = String.Join("", files), // String : "C:\\Users\\smashie420\\AppData\\Roaming\\discord\\Local Storage\\leveldb\\00003.log"
                fileSplit = String.Join("", files).Split('\\'), // Array: ["C:","Users","Smashie420","AppData", "Roaming", "discord", "Local Storage", "leveldb", "00003.log"]
                fileName = String.Join("", files).Split('\\')[String.Join("", files).Split('\\').Length - 1] // String: 00003.log
            };

            
            string[] files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\discord\\Local Storage\\leveldb", "*.log", SearchOption.AllDirectories); // Array: ["C:\\Users\\smashie420\\AppData\\Roaming\\discord\\Local Storage\\leveldb\\00003.log"]
            fileDir = String.Join("", files); // String : "C:\\Users\\smashie420\\AppData\\Roaming\\discord\\Local Storage\\leveldb\\00003.log"
            fileSplit = fileDir.Split('\\'); // Array: ["C:","Users","Smashie420","AppData", "Roaming", "discord", "Local Storage", "leveldb", "00003.log"]
            fileName = fileSplit[fileSplit.Length - 1]; // String: 00003.log
            */
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x84:
                    base.WndProc(ref m);
                    if ((int)m.Result == 0x1)
                        m.Result = (IntPtr)0x2;
                    return;
            }

            base.WndProc(ref m);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            UsrData data = new UsrData
            {
                localStorageSplit = String.Join("", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\discord\\Local Storage").Split('\\'), // Array: ["C:","Users","Smashie420","AppData", "Roaming", "discord", "Local Storage", "leveldb", "00003.log"]
                localStorageLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\discord\\Local Storage",
                sessionStorageSplit = String.Join("", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\discord\\Session Storage").Split('\\'), // Array: ["C:","Users","Smashie420","AppData", "Roaming", "discord", "Local Storage", "leveldb", "00003.log"]
                sessionStorageLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\discord\\Session Storage"
            };
            File.WriteAllText("data.json", JsonConvert.SerializeObject(data));
            progressBar1.Value = 10;
            // Local storage stuff
            foreach (string dirPath in Directory.GetDirectories(data.localStorageLocation, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(data.localStorageLocation, "backup/Local Storage/"));
            progressBar1.Value = 30;
            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(data.localStorageLocation, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(data.localStorageLocation, "backup/Local Storage/"), true);
            progressBar1.Value = 40;
            //MessageBox.Show("Finished backing up data!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);



            // Session Storage Stuff
            foreach (string dirPath in Directory.GetDirectories(data.sessionStorageLocation, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(data.sessionStorageLocation, "backup/Session Storage/"));
            progressBar1.Value = 80;
            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(data.sessionStorageLocation, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(data.sessionStorageLocation, "backup/Session Storage/"), true);

            progressBar1.Value = 100;
            MessageBox.Show("Finished backing up data!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        
    private void button2_Click(object sender, EventArgs e)
        {
            
            if (Process.GetProcessesByName("Discord").Length > 0)
            {
                MessageBox.Show("Discord is running please close it");
                return;
            }
            string json = File.ReadAllText("data.json");
            UsrData data = JsonConvert.DeserializeObject<UsrData>(json);
            progressBar1.Value = 10;
            foreach (string dirPath in Directory.GetDirectories("backup/Local Storage/", "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace("backup/Local Storage/", data.localStorageLocation+"\\"));
            progressBar1.Value = 30;
            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles("backup/Local Storage/", "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace("backup/Local Storage/", data.localStorageLocation + "\\"), true);
            progressBar1.Value = 60;
            /* Dont need this because Session storage doesnt have any sub-directories
             foreach (string dirPath in Directory.GetDirectories("backup/Session Storage/", "*", SearchOption.AllDirectories))
                 Directory.CreateDirectory(dirPath.Replace("backup/Session Storage/", data.sessionStorageLocation)); //data.sessionStorageLocation, "backup/Session Storage/"
             */

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles("backup/Session Storage/", "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace("backup/Session Storage/", data.sessionStorageLocation), true);
            progressBar1.Value = 100;
            MessageBox.Show("Finished restoring up data!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
