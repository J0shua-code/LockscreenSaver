using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LockscreenSaver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = Properties.Settings.Default.SaveDir;
        }

        private bool File_checker(DateTime time)
        {
            if (Properties.Settings.Default.LastModified < time)
            {
                return true;
            }
            return false;
        }

        private bool Is_wallpaper(string image_path)
        {
            if (Image.FromFile(image_path).Height >= 720 && Image.FromFile(image_path).Width >= 1080 && Image.FromFile(image_path).Width > Image.FromFile(image_path).Height)
            {
                return true;
            }
            return false;

        }

        private void Save_wallpaper(string file_path, DateTime filename)
        {
            Random rnd = new Random();
            string formatted_filename = filename.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + " " + rnd.Next(10, 1000);
            string new_file_path = Path.Combine(Properties.Settings.Default.SaveDir, formatted_filename + ".jpg");
            File.Copy(file_path, new_file_path, true);
        }

        private void Check_new_wallpaper()
        {
            string directory_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Packages\\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\\LocalState\\Assets\\");

            DirectoryInfo images_directory = new DirectoryInfo(directory_path); //Assuming Test is your Folder
            FileInfo[] Files = images_directory.GetFiles().OrderByDescending(p => p.CreationTime).ToArray(); //Getting Files in descending order w.r.t creation date

            foreach (FileInfo file in Files)
            {
                string file_path = Path.Combine(directory_path, file.Name);

                if (File_checker(file.LastWriteTime))
                {
                    if (Is_wallpaper(file_path))
                    {
                        Save_wallpaper(file_path, file.LastWriteTime);
                    }

                }
                else
                {
                    Properties.Settings.Default.LastModified = Directory.GetLastWriteTime(directory_path);
                    Properties.Settings.Default.Save();
                    Console.WriteLine("Not first run");

                    break;
                }

            }
            if (Properties.Settings.Default.FirstRun)
            {
                Properties.Settings.Default.FirstRun = false;
                Properties.Settings.Default.LastModified = Directory.GetLastWriteTime(directory_path);
                Properties.Settings.Default.Save();
                Console.WriteLine("First run");
            }
        }

        //-------------------------------------------------------------------------------

        private void Button1_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textBox1.Text = dialog.FileName;
                Properties.Settings.Default.SaveDir = textBox1.Text;
                Properties.Settings.Default.Save();


            }
            Check_new_wallpaper();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            createIconMenuStructure();
        }

        public NotifyIcon notifyIcon1 = new NotifyIcon();
        public ContextMenu contextMenu1 = new ContextMenu();

        public void createIconMenuStructure()
        {
            // Add menu items to shortcut menu.  
            contextMenu1.MenuItems.Add("Show", MenuItemMaximize_click);
            contextMenu1.MenuItems.Add("Exit", MenuItemNew_Click);

            // Set properties of NotifyIcon component.  
            notifyIcon1.Icon = Properties.Resources.white_icon;
            //notifyIcon1.Visible = true;
            notifyIcon1.Text = "Right-click me!";
            notifyIcon1.Visible = false;
            notifyIcon1.ContextMenu = contextMenu1;
        }

        private void MenuItemNew_Click(Object sender, System.EventArgs e)
        {

            Application.Exit();
        }

        private void MenuItemMaximize_click(Object sender, System.EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon1.Visible = true;

            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }


    }
}