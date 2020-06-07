using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KryptografiaClient
{
    public partial class Form1 : Form
    {
        private Client _Client;
        public Form1()
        {
            InitializeComponent();
            _Client = new Client(UpdateUI);
            _Client.Initialize();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(richTextBox2.Text))
                return;
            _Client.SendTextMessage(richTextBox2.Text);
            richTextBox1.Text += richTextBox2.Text + "\n";
            richTextBox2.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog() { Multiselect = false };
            fileDialog.ShowDialog();
            if (String.IsNullOrEmpty(fileDialog.FileName))
                return;
            var filePath = fileDialog.FileName;
            _Client.SendFileMessage(filePath);
            richTextBox1.Text += $"Wysłano plik {filePath.Split("\\").Last()}"+"\n";
        }
        private string Formater(string text)
        {
            string formatedText = "";
            int spaceLength = 130 - text.Length;
            for(int i = 0; i < spaceLength; i++)
            {
                formatedText += " ";
            }
            return formatedText + text + "\n";
        }

        private void UpdateUI(string data) => _ = richTextBox1.Invoke(method: (MethodInvoker)(() => richTextBox1.Text += Formater(data)));

        
        
    }
}
