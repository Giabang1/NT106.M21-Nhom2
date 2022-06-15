using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace LuckyDice
{
    public partial class Summary : Form
    {
        public Character MainChar;
        public Character Char1;
        public Character Char2;

        Stream stream;
        IPEndPoint ServerIPE;
        TcpClient tcpClient = new TcpClient();
        void Connect()
        {
            try
            {
                ServerIPE = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
                tcpClient = new TcpClient();
                tcpClient.Connect(ServerIPE);
                stream = tcpClient.GetStream();
                Thread recv = new Thread(Receive);
                recv.IsBackground = true;
                recv.Start();
            }
            catch (Exception)
            {
                tcpClient.Close();
                MessageBox.Show("Không kết nối được đến Server. Ứng dụng sẽ tự động tắt.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }

        //receive func, receive message broadcasted from server
        void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    stream.Read(data, 0, data.Length);
                    string message = Encoding.UTF8.GetString(data);
                    if ((message.Substring(0, 16) == "3clientPlayagain"))
                    {
                        btnExit.Hide();
                    }
                    else if ((message.Substring(0, 11) == "3clientExit"))
                    {
                        button1.Hide();
                    }
                }
            }
            catch (Exception) { }
        }

        //send func, send message to Server
        void Send(String message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
        public Summary(Character mainchar, Character char1, Character char2)
        {
            this.MainChar = mainchar;
            this.Char1 = char1;
            this.Char2 = char2;
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            Connect();
        }

        private void Summary_Load(object sender, EventArgs e)
        {
            string dir = Path.GetDirectoryName(Application.ExecutablePath);

            string filenameMailChar = dir + this.MainChar.anhNV;
            ptbMainCharacter.Image = Image.FromFile(filenameMailChar);
            lbMainChar.Text = this.MainChar.diemNV.ToString();

            string filenameChar1 = dir + this.Char1.anhNV;
            ptbChar1.Image = Image.FromFile(filenameChar1);
            lbChar1.Text = this.Char1.diemNV.ToString();

            string filenameChar2 = dir + this.Char2.anhNV;
            ptbChar2.Image = Image.FromFile(filenameChar2);
            lbChar2.Text = this.Char2.diemNV.ToString();

            int Money = this.MainChar.diemNV + this.Char1.diemNV + this.Char2.diemNV;
            if (Money >= 15000)
            {
                string pictureWin = dir + @"\summary\win.png";
                ptbResult.Image = Image.FromFile(pictureWin);
            }
            else
            {
                string pictureLose = dir + @"\summary\lose.png";
                ptbResult.Image = Image.FromFile(pictureLose);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Send("SYSTEM_3clientPlayagain");
            new CharacterSelected().Show();
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Send("SYSTEM_3clientExit");
            this.Close();
        }
    }
}
