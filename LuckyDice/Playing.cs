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
    public partial class Playing : Form
    {
        public Character Nhanvat;
        int chon = 0;
        public Playing(Character nv)
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            Connect();
            this.Nhanvat = nv;
            tbTongtien.Text = "1000";
            lbName.Text = this.Nhanvat.tenNV;
            btnSend.Enabled = false;
            btnStart.BackColor = Color.FromArgb(73, 141, 162);
            timer.Start();
        }
        Stream stream;
        TcpClient tcpClient;
        IPEndPoint ServerIPE;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer()
        {
            Interval = 1000
        };
        int count = 60;
        int curRound = 1;
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

        int dice1;
        int dice2;
        int dice3;
        Character Nv1 = null;
        Character Nv2 = null;
        void Receive()
        {

            while (true)
            {
                byte[] data = new byte[1024 * 5000];
                stream.Read(data, 0, data.Length);
                string message = Encoding.UTF8.GetString(data);
                tbcmd.Text = message;
                if (tbcmd.Text == "D" || tbcmd.Text == "M" || tbcmd.Text == "C" || tbcmd.Text == "L" || tbcmd.Text == "EnableSelect")
                {
                    continue;
                }
                if (curRound <= 6)
                    if (tbcmd.Text == "Round " + curRound.ToString() + " is started")
                    {
                        tbcmd.Text = "Round " + curRound.ToString();
                        tbvan.Text = tbcmd.Text;
                        StartRound();
                    }
                    else if (message.Substring(0, 10) == "resultdice")
                    {
                        dice1 = Int32.Parse(message.Substring(11, 1));
                        dice2 = Int32.Parse(message.Substring(23, 1));
                        dice3 = Int32.Parse(message.Substring(35, 1));
                    }
                    else if (message.Substring(0, 14) == "3clientendgame")
                    {
                        btnStart.Hide();
                        string info_character = "SYSTEM_InfoCharacter" + this.Nhanvat.anhNV + "SYSTEM_InfoCharacter" + this.Nhanvat.diemNV.ToString();
                        byte[] character_data = Encoding.UTF8.GetBytes(info_character);
                        stream.Write(character_data, 0, character_data.Length);
                        btnkq.Enabled = true;
                    }
                    else
                    {
                        if (message.Substring(0, 20) != "SYSTEM_InfoCharacter")
                            AddMessage(message);
                    }

                if (message.Substring(0, 20) == "SYSTEM_InfoCharacter")
                {
                    string[] arrListStr = message.Split(new[] { "SYSTEM_InfoCharacter" }, StringSplitOptions.RemoveEmptyEntries);
                    arrListStr[1] = arrListStr[1].Trim();
                    int Diem = Int32.Parse(arrListStr[1]);
                    if (arrListStr[0] != this.Nhanvat.anhNV)
                    {
                        if (Nv1 == null)
                            Nv1 = new Character("", "", arrListStr[0], Diem);
                        else
                            Nv2 = new Character("", "", arrListStr[0], Diem);
                    }
                }
            }
        }
        void AddMessage(string mess)
        {
            tbChatBox.Text += "\r\n" + mess;
        }
        private void Playing_Load(object sender, EventArgs e)
        {
            tbMessage.Enabled = true;
            btnSend.Enabled = true;
            string dir = Path.GetDirectoryName(Application.ExecutablePath);
            string filename = dir + this.Nhanvat.anhNV;
            ptbCharacter.Image = Image.FromFile(filename);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            byte[] data = Encoding.UTF8.GetBytes("SYSTEM_READY");
            stream.Write(data, 0, data.Length);
            btnStart.Enabled = false;
            tbcuoc.Enabled = true;
            tbcuoc.Clear();
            if (Int32.Parse(tbTongtien.Text) < 100)
            {
                tbcuoc.Enabled = false;
                tbcuoc.Text = tbTongtien.Text;
            }
            if (Int32.Parse(tbTongtien.Text) == 0)
            {
                tbcuoc.Enabled = false;
                tbcuoc.Text = tbTongtien.Text;
                DisableButton();
            }
            btnskill.Enabled = true;
            if (Daniel == 0 || Christine == 0 || Mitnick == 0 || Leonardo == 0)
            {
                btnskill.Enabled = false;
            }
            chon = 0;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            byte[] data = Encoding.UTF8.GetBytes(this.Nhanvat.tenNV + ": " + tbMessage.Text);
            stream.Write(data, 0, data.Length);
            tbMessage.Clear();
        }
        private void StartRound()
        {
            EnableButton();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
            //enable playing button
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            count--;
            if (count == 0)
            {
                lbGiay.Text = "0";
                timer.Tick -= new EventHandler(timer_Tick);
                string mess = "SYSTEM_Round " + this.curRound.ToString() + " is ended";
                stream.Write(Encoding.UTF8.GetBytes(mess), 0, mess.Length);
                curRound++;
                Rand(ptbDice1, dice1);
                Rand(ptbDice2, dice2);
                Rand(ptbDice3, dice3);
                Play(chon);
                if (this.Nhanvat.diemNV == 0)
                {
                    string diem = "SYSTEM_NOMONEY" + this.Nhanvat.maNV;
                    byte[] character_data = Encoding.UTF8.GetBytes(diem);
                    stream.Write(character_data, 0, character_data.Length);
                }
                btnskill.Enabled = false;
                if (isChristineSkill == true)
                {
                    if (ChristineMoney > this.Nhanvat.diemNV)
                    {
                        this.Nhanvat.diemNV = (this.Nhanvat.diemNV + ChristineMoney) / 2;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    isChristineSkill = false;
                }
                btnStart.Enabled = true;
                DisableButton();
                tbSkill.Clear();
                count = 60;
                if (curRound > 6)
                {
                    btnStart.Hide();
                    string info_character = "SYSTEM_InfoCharacter" + this.Nhanvat.anhNV + "SYSTEM_InfoCharacter"
                    + this.Nhanvat.diemNV.ToString();
                    byte[] character_data = Encoding.UTF8.GetBytes(info_character);
                    stream.Write(character_data, 0, character_data.Length);
                    btnkq.Enabled = true;
                }
            }
            lbGiay.Text = count.ToString();
        }

        private void Rand(PictureBox Ptb, int n)
        {
            switch (n)
            {
                case 1:
                    Ptb.Image = Image.FromFile(@"dice\1.jpg");
                    break;
                case 2:
                    Ptb.Image = Image.FromFile(@"dice\2.jpg");
                    break;
                case 3:
                    Ptb.Image = Image.FromFile(@"dice\3.jpg");
                    break;
                case 4:
                    Ptb.Image = Image.FromFile(@"dice\4.jpg");
                    break;
                case 5:
                    Ptb.Image = Image.FromFile(@"dice\5.jpg");
                    break;
                case 6:
                    Ptb.Image = Image.FromFile(@"dice\6.jpg");
                    break;
            }
        }

        private void Play(int chon)
        {
            if (Int32.Parse(tbTongtien.Text) < 100)
            {
                tbcuoc.Enabled = false;
                tbcuoc.Text = tbTongtien.Text;
            }
            else if (tbcuoc.Text == "")
            {
                tbcuoc.Text = "-50%";
                this.Nhanvat.diemNV = this.Nhanvat.diemNV / 2;
                tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                chon = 24;
            }
            else if (Int32.Parse(tbcuoc.Text) < 100)
            {
                tbcuoc.Text = "-50%";
                this.Nhanvat.diemNV = this.Nhanvat.diemNV / 2;
                tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                chon = 24;
            }
            else if (Int32.Parse(tbcuoc.Text) > this.Nhanvat.diemNV)
            {
                this.Nhanvat.diemNV = this.Nhanvat.diemNV / 2;
                tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                tbcuoc.Text = "-50%";
                chon = 24;
            }
            switch (chon)
            {
                case 0:
                    this.Nhanvat.diemNV = this.Nhanvat.diemNV / 2;
                    tbcuoc.Text = "-50%";
                    tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    break;
                case 1:
                    if ((dice1 + dice2 + dice3) % 2 == 0)
                    {
                        this.Nhanvat.diemNV -= Int32.Parse(tbcuoc.Text);
                        //tongtien = tongtien - cuoc;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV += Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                case 2:
                    if ((dice1 + dice2 + dice3) % 2 != 0)
                    {
                        this.Nhanvat.diemNV -= Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV += Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;

                case 3:
                    if (dice1 + dice2 + dice3 > 10)
                    {
                        this.Nhanvat.diemNV -= Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV += Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;

                case 4:
                    if (dice1 + dice2 + dice3 < 11)
                    {
                        this.Nhanvat.diemNV -= Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV += Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                // Ba xúc xắc đều bằng 1 hoặc 2 hoặc 3
                case 5:
                    if ((dice1 == 1 && dice2 == 1 && dice3 == 1) || (dice1 == 2 && dice2 == 2 && dice3 == 2) || (dice1 == 3 && dice2 == 3 && dice3 == 3))
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV + Int32.Parse(tbcuoc.Text) * 60;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV -= Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                // Ba xúc xắc đều bằng 4 hoặc 5 hoặc 6 
                case 6:
                    if ((dice1 == 4 && dice2 == 4 && dice3 == 4) || (dice1 == 5 && dice2 == 5 && dice3 == 5) || (dice1 == 6 && dice2 == 6 && dice3 == 6))
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV + Int32.Parse(tbcuoc.Text) * 60;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV - Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;

                case 7:
                    if (dice1 + dice2 + dice3 == 4)
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV + Int32.Parse(tbcuoc.Text) * 60;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV - Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                case 8:
                    if (dice1 + dice2 + dice3 == 5)
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV + Int32.Parse(tbcuoc.Text) * 20;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV - Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                case 9:
                    if (dice1 + dice2 + dice3 == 6)
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV + Int32.Parse(tbcuoc.Text) * 18;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV - Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                case 10:
                    if (dice1 + dice2 + dice3 == 7)
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV + Int32.Parse(tbcuoc.Text) * 12;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV - Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                case 11:
                    if (dice1 + dice2 + dice3 == 8)
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV + Int32.Parse(tbcuoc.Text) * 8;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV - Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                case 12:
                    if (dice1 + dice2 + dice3 == 9)
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV + Int32.Parse(tbcuoc.Text) * 6;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV - Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                case 13:
                    if (dice1 + dice2 + dice3 == 10)
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV + Int32.Parse(tbcuoc.Text) * 6;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV - Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                case 14:
                    if (dice1 + dice2 + dice3 == 11)
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV + Int32.Parse(tbcuoc.Text) * 6;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV - Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                case 15:
                    if (dice1 + dice2 + dice3 == 12)
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV + Int32.Parse(tbcuoc.Text) * 6;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV - Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                case 16:
                    if (dice1 + dice2 + dice3 == 13)
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV + Int32.Parse(tbcuoc.Text) * 8;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV - Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                case 17:
                    if (dice1 + dice2 + dice3 == 14)
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV + Int32.Parse(tbcuoc.Text) * 12;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV - Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                case 18:
                    if (dice1 + dice2 + dice3 == 15)
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV + Int32.Parse(tbcuoc.Text) * 18;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV - Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                case 19:
                    if (dice1 + dice2 + dice3 == 16)
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV + Int32.Parse(tbcuoc.Text) * 20;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV - Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                case 20:
                    if (dice1 + dice2 + dice3 == 17)
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV + Int32.Parse(tbcuoc.Text) * 60;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV - Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                case 21:
                    if ((dice1 == 1 && dice2 == 1 && dice3 == 1) || (dice1 == 2 && dice2 == 2 && dice3 == 2) || (dice1 == 3 && dice2 == 3 && dice3 == 3) ||
                        (dice1 == 4 && dice2 == 4 && dice3 == 4) || (dice1 == 5 && dice2 == 5 && dice3 == 5) || (dice1 == 6 && dice2 == 6 && dice3 == 6))
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV + Int32.Parse(tbcuoc.Text) * 30;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV - Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                case 22:
                    if ((dice1 == 1 && dice2 == 1) || (dice1 == 1 && dice3 == 1) || (dice2 == 1 && dice3 == 1) ||
                        (dice1 == 2 && dice2 == 2) || (dice1 == 2 && dice3 == 2) || (dice2 == 2 && dice3 == 2) ||
                        (dice1 == 3 && dice2 == 3) || (dice1 == 3 && dice3 == 3) || (dice2 == 3 && dice3 == 3))
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV + Int32.Parse(tbcuoc.Text) * 11;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV - Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                case 23:
                    if ((dice1 == 4 && dice2 == 4) || (dice1 == 4 && dice3 == 4) || (dice2 == 4 && dice3 == 4) ||
                        (dice1 == 5 && dice2 == 5) || (dice1 == 5 && dice3 == 5) || (dice2 == 5 && dice3 == 5) ||
                        (dice1 == 6 && dice2 == 6) || (dice1 == 3 && dice3 == 6) || (dice2 == 3 && dice3 == 6))
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV + Int32.Parse(tbcuoc.Text) * 11;
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    else
                    {
                        this.Nhanvat.diemNV = this.Nhanvat.diemNV - Int32.Parse(tbcuoc.Text);
                        tbTongtien.Text = this.Nhanvat.diemNV.ToString();
                    }
                    break;
                default: break;
            }
        }
        private void DisableButton()
        {

            radioButton1.Enabled = false;
            radioButton2.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            radioButton5.Enabled = false;
            radioButton6.Enabled = false;
            radioButton7.Enabled = false;
            radioButton8.Enabled = false;
            radioButton9.Enabled = false;
            radioButton10.Enabled = false;
            radioButton11.Enabled = false;
            radioButton12.Enabled = false;
            radioButton13.Enabled = false;
            radioButton14.Enabled = false;
            radioButton15.Enabled = false;
            radioButton16.Enabled = false;
            radioButton17.Enabled = false;
            radioButton18.Enabled = false;
            radioButton19.Enabled = false;
            radioButton20.Enabled = false;
            radioButton21.Enabled = false;
            radioButton22.Enabled = false;
            radioButton23.Enabled = false;
        }
        private void EnableButton()
        {
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = false;
            radioButton7.Checked = false;
            radioButton8.Checked = false;
            radioButton9.Checked = false;
            radioButton10.Checked = false;
            radioButton11.Checked = false;
            radioButton12.Checked = false;
            radioButton13.Checked = false;
            radioButton14.Checked = false;
            radioButton15.Checked = false;
            radioButton16.Checked = false;
            radioButton17.Checked = false;
            radioButton18.Checked = false;
            radioButton19.Checked = false;
            radioButton20.Checked = false;
            radioButton21.Checked = false;
            radioButton22.Checked = false;
            radioButton23.Checked = false;
            //
            radioButton1.Enabled = true;
            radioButton2.Enabled = true;
            radioButton3.Enabled = true;
            radioButton4.Enabled = true;
            radioButton5.Enabled = true;
            radioButton6.Enabled = true;
            radioButton7.Enabled = true;
            radioButton8.Enabled = true;
            radioButton9.Enabled = true;
            radioButton10.Enabled = true;
            radioButton11.Enabled = true;
            radioButton12.Enabled = true;
            radioButton13.Enabled = true;
            radioButton14.Enabled = true;
            radioButton15.Enabled = true;
            radioButton16.Enabled = true;
            radioButton17.Enabled = true;
            radioButton18.Enabled = true;
            radioButton19.Enabled = true;
            radioButton20.Enabled = true;
            radioButton21.Enabled = true;
            radioButton22.Enabled = true;
            radioButton23.Enabled = true;
        }
        bool isChristineSkill = false;
        int Daniel = 3;
        int Mitnick = 2;
        int Christine = 3;
        int Leonardo = 3;
        int ChristineMoney;
        private void btnskill_Click(object sender, EventArgs e)
        {
            btnskill.Enabled = false;
            if (this.Nhanvat.maNV == "1")
            {
                tbSkill.Text = dice2.ToString();
                Daniel--;
                if (Daniel == 0)
                {
                    btnskill.Enabled = false;
                }
            }

            if (this.Nhanvat.maNV == "2")
            {
                string res = "";
                if (dice1 == 1 || dice1 == 4 || dice1 == 6)
                    res += "RED ";
                else
                    res += "BLACK ";
                if (dice2 == 1 || dice2 == 4 || dice2 == 6)
                    res += "RED ";
                else
                    res += "BLACK ";
                if (dice3 == 1 || dice3 == 4 || dice3 == 6)
                    res += "RED";
                else
                    res += "BLACK";
                tbSkill.Text = res;
                Leonardo--;
                if (Leonardo == 0)
                {
                    btnskill.Enabled = false;
                }
            }

            if (this.Nhanvat.maNV == "3")
            {
                tbSkill.Text = "Bảo toàn 50% tiền nếu cược sai.";
                Christine--;
                isChristineSkill = true;
                ChristineMoney = this.Nhanvat.diemNV;
                if (Christine == 0)
                {
                    btnskill.Enabled = false;
                }
            }

            if (this.Nhanvat.maNV == "4")
            {
                int tong = dice1 + dice2 + dice3;
                Random rnd = new Random();
                int tongrand1 = rnd.Next(3, 18);
                while (tongrand1 == tong)
                {
                    tongrand1 = rnd.Next(3, 18);
                }
                int tongrand2 = rnd.Next(3, 18);
                while (tongrand2 == tong || tongrand2 == tongrand1)
                {
                    tongrand2 = rnd.Next(3, 18);
                }
                int tongrand3 = rnd.Next(3, 18);
                while (tongrand3 == tong || tongrand3 == tongrand1 || tongrand3 == tongrand2)
                {
                    tongrand3 = rnd.Next(3, 18);
                }
                int[] arr = new int[4] { tong, tongrand1, tongrand2, tongrand3 };
                Random random = new Random();
                arr = arr.OrderBy(x => random.Next()).ToArray();
                foreach (var i in arr)
                {
                    tbSkill.Text += i.ToString() + "  ";
                }
                Mitnick--;
                if (Mitnick == 0)
                {
                    btnskill.Enabled = false;
                }
            }
        }

        private void btnkq_Click(object sender, EventArgs e)
        {
            Summary summary = new Summary(this.Nhanvat, Nv1, Nv2);
            summary.Location = this.Location;
            summary.Show();
            this.Hide();
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            chon = 1;
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            chon = 3;
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            chon = 22;
        }

        private void radioButton4_Click(object sender, EventArgs e)
        {
            chon = 5;
        }

        private void radioButton5_Click(object sender, EventArgs e)
        {
            chon = 21;
        }

        private void radioButton6_Click(object sender, EventArgs e)
        {
            chon = 6;
        }

        private void radioButton7_Click(object sender, EventArgs e)
        {
            chon = 23;
        }

        private void radioButton8_Click(object sender, EventArgs e)
        {
            chon = 2;
        }

        private void radioButton9_Click(object sender, EventArgs e)
        {
            chon = 4;
        }

        private void radioButton10_Click(object sender, EventArgs e)
        {
            chon = 7;
        }

        private void radioButton11_Click(object sender, EventArgs e)
        {
            chon = 8;
        }

        private void radioButton12_Click(object sender, EventArgs e)
        {
            chon = 9;
        }

        private void radioButton13_Click(object sender, EventArgs e)
        {
            chon = 10;
        }

        private void radioButton14_Click(object sender, EventArgs e)
        {
            chon = 11;
        }

        private void radioButton15_Click(object sender, EventArgs e)
        {
            chon = 12;
        }

        private void radioButton16_Click(object sender, EventArgs e)
        {
            chon = 13;
        }

        private void radioButton17_Click(object sender, EventArgs e)
        {
            chon = 14;
        }

        private void radioButton18_Click(object sender, EventArgs e)
        {
            chon = 15;
        }

        private void radioButton19_Click(object sender, EventArgs e)
        {
            chon = 16;
        }

        private void radioButton20_Click(object sender, EventArgs e)
        {
            chon = 17;
        }

        private void radioButton21_Click(object sender, EventArgs e)
        {
            chon = 18;
        }

        private void radioButton22_Click(object sender, EventArgs e)
        {
            chon = 19;
        }

        private void radioButton23_Click(object sender, EventArgs e)
        {
            chon = 20;
        }

        private void tbcuoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
