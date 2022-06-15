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
    public partial class CharacterSelected : Form
    {
        Stream stream;
        IPEndPoint ServerIPE;
        Character character;
        TcpClient tcpClient = new TcpClient();
        int numberCharacter = 0;
        public CharacterSelected()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            Connect();
        }

        private void CharacterSelected_Load(object sender, EventArgs e)
        {
            Import();
            btnNext.Enabled = false;
            btnDanielSelected.Enabled = false;
            btnLeonardoSelected.Enabled = false;
            btnChristineSelected.Enabled = false;
            btnMitnickSelected.Enabled = false;
        }
        void Import() 
        {
            string dir = Path.GetDirectoryName(Application.ExecutablePath);
            tbDaniel.Text = File.ReadAllText(dir + @"\character\Daniel.txt");
            tbLeonardo.Text = File.ReadAllText(dir + @"\character\Leonardo.txt");
            tbChristine.Text = File.ReadAllText(dir + @"\character\Christine.txt");
            tbMitnick.Text = File.ReadAllText(dir + @"\character\Mitnick.txt");
        }
        void Connect()
        {
            try
            {
                ServerIPE = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
                tcpClient = new TcpClient();
                tcpClient.Connect(ServerIPE);
                stream = tcpClient.GetStream();
                Send("SYSTEM_CLientConnected");
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
        void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    stream.Read(data, 0, data.Length);
                    string message = Encoding.UTF8.GetString(data);
                    PendingReceivedMessage(message);
                    if (numberCharacter == 3)
                        btnNext.Enabled = true;
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

        // Disable button
        void PendingReceivedMessage(String message)
        {
            tbView.Text = message;
            if (tbView.Text == "EnableSelect")
            {
                btnDanielSelected.Enabled = true;
                btnLeonardoSelected.Enabled = true;
                btnChristineSelected.Enabled = true;
                btnMitnickSelected.Enabled = true;
            }
            if (tbView.Text == "D")
            {
                btnDanielSelected.Enabled = false;
                numberCharacter++;
            }
            if (tbView.Text == "L")
            {
                btnLeonardoSelected.Enabled = false;
                numberCharacter++;
            }
            if (tbView.Text == "C")
            {
                btnChristineSelected.Enabled = false;
                numberCharacter++;
            }
            if (tbView.Text == "M")
            {
                btnMitnickSelected.Enabled = false;
                numberCharacter++;
            }
        }

        void PendingSendMessage(String message)
        {
            if (message == "D")
            {
                btnChristineSelected.Visible = false;
                btnLeonardoSelected.Visible = false;
                btnMitnickSelected.Visible = false;
            }
            if (message == "L")
            {
                btnChristineSelected.Visible = false;
                btnDanielSelected.Visible = false;
                btnMitnickSelected.Visible = false;
            }
            if (message == "M")
            {
                btnChristineSelected.Visible = false;
                btnLeonardoSelected.Visible = false;
                btnDanielSelected.Visible = false;
            }
            if (message == "C")
            {
                btnDanielSelected.Visible = false;
                btnLeonardoSelected.Visible = false;
                btnMitnickSelected.Visible = false;
            }
        }

        private void btnDanielSelected_Click(object sender, EventArgs e)
        {
            Send("D");
            character = new Character("1", "Daniel", @"\Play\Daniel.png", 1000);
            PendingSendMessage("D");
            btnNext.Enabled = true;
        }

        private void btnLeonardoSelected_Click(object sender, EventArgs e)
        {
            Send("L");
            character = new Character("2", "Leonardo", @"\Play\Leonardo.png", 1000);
            PendingSendMessage("L");
            btnNext.Enabled = true;
        }

        private void btnChristineSelected_Click(object sender, EventArgs e)
        {
            Send("C");
            character = new Character("3", "Christine", @"\Play\Christine.png", 1000);
            PendingSendMessage("C");
            btnNext.Enabled = true;
        }

        private void btnMitnickSelected_Click(object sender, EventArgs e)
        {
            Send("M");
            character = new Character("4", "Mitnick", @"\Play\Mitnick.png", 1000);
            PendingSendMessage("M");
            btnNext.Enabled = true;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            Playing playing = new Playing(character);
            playing.Location = this.Location;
            playing.Show();
            numberCharacter = 0;
            this.Close();
        }
    }
}
