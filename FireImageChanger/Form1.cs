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

        string msg_about_profile = "Firefoxでは、履歴やアドオンなどといった情報はプロファイルに保存されています。\nここでは、背景画像を変更したいプロファイルを選択します。\n使用中のプロファイルについては、[about:profiles]で確認することができます。\n\n例えば、使用中のプロファイルの'ルートディレクトリー'欄の最後が[thisisexample.default-esr]で終わっている場合、それがプロファイル名になります。";
        string msg_profile_notfound = "プロファイルが見つかりません。\n既に削除されたか、名前が変更された可能性があります。";
        string msg_create_folder_already = "既に画像フォルダは作成済みです。";
        string msg_create_folder_done = "画像フォルダを作成しました。";
        string msg_image_notfound = "画像ファイルが存在しません。\n削除されたか、名前が変更された可能性があります。";
        string msg_image_select = "画像を選択してください。";
        string msg_folder_notfound = "画像フォルダを見つけられません。\n[フォルダを作成]ボタンを押してください。";
        string msg_background_overwrite_img = "既に設定されている背景画像を上書きしますか？";
        string msg_background_overwrite_ini = "設定ファイルを上書きしますか？";
        string msg_background_done = "背景画像を適用しました。\nFirefoxを再起動してください。";
        string msg_about_notworking = "1. Firefoxを開きます\n2. アドレスバーに 'about:config'と入力します。\n3. 検索ボックスに 'toolkit.legacyUserProfileCustomizations.stylesheets'と入力します。\n4. 'False'になっている場合、'True'にします。\n5. Firefoxを再起動します。";


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

            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //select image button clicked
            string s_image;
            openFileDialog1.Title = "背景画像を選択 (Select background image)";
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
                MessageBox.Show(msg_profile_notfound, appname, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Directory.Exists(textBox1.Text + "\\chrome\\img"))
            {
                //chrome already created
                MessageBox.Show(msg_create_folder_already, appname, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                //make chrome folder
                if (!Directory.Exists(textBox1.Text + "\\chrome"))
                {
                    Directory.CreateDirectory(textBox1.Text + "\\chrome");
                }
                Directory.CreateDirectory(textBox1.Text + "\\chrome\\img");
                MessageBox.Show(msg_create_folder_done, appname, MessageBoxButtons.OK, MessageBoxIcon.Information);
                checkBox4.Checked = true;
            }
            return;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double opac = 1;
            String encod = comboBox2.SelectedItem.ToString();

            //apply background image
            if (textBox2.Text != "")
            {
                if (!File.Exists(textBox2.Text))
                {
                    //image not found
                    MessageBox.Show(msg_image_notfound, appname, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show(msg_image_select, appname, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!Directory.Exists(textBox1.Text + "\\chrome\\img"))
            {
                //image folder not found
                MessageBox.Show(msg_folder_notfound, appname, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            csstext = "@-moz-document " + target + " { body{ z-index: 0; overflow: hidden; background-color: rgb(0, 163, 175); } body::before{ content: ''; z-index: -1; position: fixed; top: 0; left: 0; background: #f9a no-repeat url(./img/bg" + imgname + ") center; opacity: " + opac + "; background-size: cover; width: 100vw; height: 100vh; } }";

            if (File.Exists(textBox1.Text + "\\chrome\\img\\bg" + imgname))
            {
                //already created
                DialogResult dr = MessageBox.Show(msg_background_overwrite_img, appname, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    FileSystem.CopyFile(textBox2.Text, textBox1.Text + "\\chrome\\img\\bg" + imgname, UIOption.OnlyErrorDialogs);
                }

                DialogResult dr2 = MessageBox.Show(msg_background_overwrite_ini, appname, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr2 == DialogResult.No)
                {
                    return;
                }
            }
            else
            {
                //not created
                FileSystem.CopyFile(textBox2.Text, textBox1.Text + "\\chrome\\img\\bg" + imgname, UIOption.OnlyErrorDialogs);
            }

            Encoding enc = Encoding.GetEncoding(encod);
            using (StreamWriter writer = new StreamWriter(textBox1.Text + "\\chrome\\userContent.css", false, enc))
            {
                writer.WriteLine(csstext);
            }

            MessageBox.Show(msg_background_done, appname, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //not working
            MessageBox.Show(msg_about_notworking, appname, MessageBoxButtons.OK);
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
            MessageBox.Show(msg_about_profile, appname, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox3.SelectedIndex)
            {
                case 0:
                    //日本語
                    msg_about_profile = "Firefoxでは、履歴やアドオンなどといった情報はプロファイルに保存されています。\nここでは、背景画像を変更したいプロファイルを選択します。\n使用中のプロファイルについては、[about:profiles]で確認することができます。\n\n例えば、使用中のプロファイルの'ルートディレクトリー'欄の最後が[thisisexample.default-esr]で終わっている場合、それがプロファイル名になります。";
                    msg_profile_notfound = "プロファイルが見つかりません。\n既に削除されたか、名前が変更された可能性があります。";
                    msg_create_folder_already = "既に画像フォルダは作成済みです。";
                    msg_create_folder_done = "画像フォルダを作成しました。";
                    msg_image_notfound = "画像ファイルが存在しません。\n削除されたか、名前が変更された可能性があります。";
                    msg_image_select = "画像を選択してください。";
                    msg_folder_notfound = "画像フォルダを見つけられません。\n[フォルダを作成]ボタンを押してください。";
                    msg_background_overwrite_img = "既に設定されている背景画像を上書きしますか？";
                    msg_background_overwrite_ini = "設定ファイルを上書きしますか？";
                    msg_background_done = "背景画像を適用しました。\nFirefoxを再起動してください。";
                    msg_about_notworking = "1. Firefoxを開きます\n2. アドレスバーに 'about:config'と入力します。\n3. 検索ボックスに 'toolkit.legacyUserProfileCustomizations.stylesheets'と入力します。\n4. 'False'になっている場合、'True'にします。\n5. Firefoxを再起動します。";
                    label1.Text = "プロファイル:";
                    label2.Text = "パス:";
                    label3.Text = "画像パス:";
                    label5.Text = "文字コード:";
                    groupBox1.Text = "適用先";
                    groupBox2.Text = "ステータス";
                    checkBox4.Text = "画像フォルダ";
                    button2.Text = "画像フォルダ作成";
                    button3.Text = "背景画像を適用";
                    button4.Text = "適用されないときは";
                    break;

                case 1:
                    //English;
                    msg_about_profile = "Firefox stores user settings and other information in each profile. Here, select the profile you want to customize the background for.\nYou can see the profile in use at [about:profiles].\n\nFor example, if the root directory string ends with [thisisexample.default-esr], that is the profile in use.";
                    label1.Text = "Profile:";
                    label2.Text = "Path:";
                    label3.Text = "Image:";
                    label5.Text = "Encode:";
                    groupBox1.Text = "Enable";
                    groupBox2.Text = "Status";
                    checkBox4.Text = "Image folder";
                    button2.Text = "Create image folder";
                    button3.Text = "Apply background image";
                    button4.Text = "Not working?";
                    msg_profile_notfound = "Selected profile was not found!";
                    msg_create_folder_already = "Image folder was already created.";
                    msg_create_folder_done = "Create image folder successfull.";
                    msg_image_notfound = "Selected image was not found!";
                    msg_image_select = "Please select image!";
                    msg_folder_notfound = "Image folder was not found!\nSelect [Create image folder] to create folder.";
                    msg_background_overwrite_img = "Overwrite image?";
                    msg_background_overwrite_ini = "Overwrite INI?";
                    msg_background_done = "Successfull!\nRestart Firefox to apply.";
                    msg_about_notworking = "1. Start Firefox\n2. Type 'about:config' on address bar\n3. Type 'toolkit.legacyUserProfileCustomizations.stylesheets' on search box\n4. Change 'False' to 'True'\n5. Restart Firefox";
                    break;
            }
            return;
        }
    }
}
