using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace Firefox_NewTab_Background_Image_Changer
{
    public partial class Form1 : Form
    {
        string appname = "Firefox NewTab Background Image Changer";
        string[] Dirct = new string[99];

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Wakeup load
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = path + "\\Mozilla\\Firefox\\Profiles";

            //フォルダ内のすべてのフォルダを取得
            string[] allDirectories = Directory.GetDirectories(path);
            int i = 0;
            foreach (string str in allDirectories)
            {
                string s = str.Replace(path + "\\", "");
                Dirct[i] += s;
                i++;
            }
            comboBox1.DataSource = Dirct;

            if (i >= 1)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //select image button clicked
            string s_image;
            openFileDialog1.Title = "Select background image";
            openFileDialog1.Filter = "Image (*.png;*.jpg)|*.png;*.jpg";
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                s_image = openFileDialog1.FileName;
            }
            else
            {
                return;
            }
            textBox2.Text = s_image;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //create chrome folder
            if (!Directory.Exists(textBox1.Text))
            {
                //folder not found
                MessageBox.Show("Selected profile cannot found!", appname, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Directory.Exists(textBox1.Text + "\\chrome\\img"))
            {
                //chrome already created
                MessageBox.Show("image folder already created!", appname, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                //make chrome folder
                if (!Directory.Exists(textBox1.Text + "\\chrome"))
                {
                    Directory.CreateDirectory(textBox1.Text + "\\chrome");
                }
                Directory.CreateDirectory(textBox1.Text + "\\chrome\\img");
                MessageBox.Show("image folder created successfully!", appname, MessageBoxButtons.OK, MessageBoxIcon.Information);
                checkBox4.Checked = true;
            }
            return;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //apply background image
            if (textBox2.Text != "")
            {
                if (!File.Exists(textBox2.Text))
                {
                    //image not found
                    MessageBox.Show("Selected image cannot found!", appname, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please select image!", appname, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!Directory.Exists(textBox1.Text + "\\chrome\\img"))
            {
                //image folder not found
                MessageBox.Show("image folder not found!\n[Create image folder] first.", appname, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string csstext = "";
            string target = "url(about:none)";
            string imgpath = "";

            if (checkBox1.Checked)
            {
                target += ", url(about:home)";
            }

            if (checkBox2.Checked)
            {
                target += ", url(about:newtab)";
            }

            if (checkBox3.Checked)
            {
                target += ", url(about:privatebrowsing)";
            }

            string imgname = System.IO.Path.GetExtension(textBox2.Text);

            csstext = "@-moz-document " + target + " { body{ content: ''; z-index: -1; position: fixed; top: 0; left: 0; background: #f9a no-repeat url(./img/bg" + imgname + ") center; background-size: cover; width: 100vw; height: 100vh;} }";

            if (File.Exists(textBox1.Text + "\\chrome\\img\\bg" + imgname))
            {
                //already created
                DialogResult dr = MessageBox.Show("Overwrite background image?", appname, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    FileSystem.CopyFile(textBox2.Text, textBox1.Text + "\\chrome\\img\\bg" + imgname, UIOption.OnlyErrorDialogs);
                }
                else
                {
                    return;
                }
            }
            else
            {
                //not created
                FileSystem.CopyFile(textBox2.Text, textBox1.Text + "\\chrome\\img\\bg" + imgname, UIOption.OnlyErrorDialogs);
            }

            Encoding enc = Encoding.GetEncoding("Shift_JIS");
            using (StreamWriter writer = new StreamWriter(textBox1.Text + "\\chrome\\userContent.css", false, enc))
            {
                writer.WriteLine(csstext);
            }

            MessageBox.Show("background image apply successfully!", appname, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //not working
            MessageBox.Show("1. Open Firefox\n2. Enter 'about:config' on URL bar\n3. Type 'toolkit.legacyUserProfileCustomizations.stylesheets' on search box\n4. Change 'False' to 'True'\n5. Restart Firefox", appname, MessageBoxButtons.OK);
            return;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //profile change event
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = path + "\\Mozilla\\Firefox\\Profiles";
            int i = comboBox1.SelectedIndex;
            textBox1.Text = path + "\\" + Dirct[i];

            if (Directory.Exists(textBox1.Text + "\\chrome"))
            {
                checkBox4.Checked = true;
            }
            else
            {
                checkBox4.Checked = false;
            }

            if (File.Exists(textBox1.Text + "\\chrome\\userContent.css"))
            {
                checkBox5.Checked = true;
            }
            else
            {
                checkBox5.Checked = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //what is profile?
            MessageBox.Show("Firefox stores user settings and other information in each profile. Here, select the profile you want to customize the background for.\nYou can see the profile in use at [about:profiles].\n\nFor example, if the root directory string ends with [thisisexample.default-esr], that is the profile in use.", appname, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
    }
}
