using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Server
{
    public partial class MiniServer : Form
    {
        int curRound;
        int num_clientReady = 0;
        int characterSelected = 0;
        int nomoney1 = 0;
        int nomoney2 = 0;
        int nomoney3 = 0;
        int nomoney4 = 0;
        public MiniServer()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            Listen();
        }
        TcpListener server;
        Socket client;
        IPEndPoint IPE;
        List<Socket> clientList = new List<Socket>();
        void Listen()
        {
            IPE = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            server = new TcpListener(IPE);
            AddMess("Server running on " + IPE.ToString());
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    server.Start();
                    client = server.AcceptSocket();
                    clientList.Add(client);
                    tbView3.Text = clientList.Count.ToString();
                    AddMess("New client connected from: " + client.RemoteEndPoint);
                    Thread receive = new Thread(Receive);
                    receive.IsBackground = true;
                    receive.Start(client);
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }
        void Receive(object obj)
        {
            try
            {
                while (true)
                {
                    Socket client = obj as Socket;
                    byte[] data = new byte[1024 * 5000];
                    client.Receive(data);
                    string message = Encoding.UTF8.GetString(data);

                    if (message.Substring(0, 6) != "SYSTEM")
                    {
                        AddMess(message);
                        Broadcast(message);
                    }
                    else
                    {
                        if (message.Substring(7, 13) == "InfoCharacter")
                            Broadcast(message);
                        else if (message.Substring(7, 8) == "NOMONEY1")
                        {
                            nomoney1 = 1;
                        }
                        else if (message.Substring(7, 8) == "NOMONEY2")
                        {
                            nomoney2 = 1;
                        }
                        else if (message.Substring(7, 8) == "NOMONEY3")
                        {
                            nomoney3 = 1;
                        }
                        else if (message.Substring(7, 8) == "NOMONEY4")
                        {
                            nomoney4 = 1;
                        }
                        else if (message.Substring(7, 16) == "3clientPlayagain")
                        {
                            Broadcast("3clientPlayagain");
                        }
                        else if(message.Substring(7, 11) == "3clientExit")
                        {
                            Broadcast("3clientExit");
                        }    
                        else
                            SystemMessage(message);

                    }
                    if (nomoney1 + nomoney2 + nomoney3 + nomoney4 == 3)
                    {
                        Broadcast("3clientendgame");
                        nomoney1 = 0;
                        nomoney2 = 0;
                        nomoney3 = 0;
                        nomoney4 = 0;
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        private void SystemMessage(string rec_message)
        {
            tbView2.Text = rec_message.Substring(7, rec_message.Length - 7);
            if (tbView2.Text == "READY")
            {
                num_clientReady++;
                tbPlayer.Text = num_clientReady.ToString();
                if (num_clientReady == 3)
                {
                    AddMess("Round " + curRound.ToString() + " is started");
                    StartRound("Round " + curRound.ToString() + " is started");
                    curRound++;
                }
            }
            else
            {
                if (tbView2.Text == "BYE")
                {
                    characterSelected++;
                    tbView2.Text = "";
                    if (characterSelected == 3)
                    {
                        Broadcast("Server: Welcome to Lucky Dice!");
                        AddMess("Now " + clientList.Count.ToString());
                    }
                }
                else
                {
                    if (tbView2.Text == "CLientConnected")
                    {
                        characterSelected++;
                        tbPlayer.Text = characterSelected.ToString();
                    }
                    if (characterSelected == 3)
                        EnableSelected();
                }
            }
            AddMess(rec_message);
        }
        private void Broadcast(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            foreach (Socket clientSocket in clientList)
            {
                clientSocket.Send(data);
            }
        }

        private void StartRound(string mess)
        {
            Broadcast(mess);
            Thread.Sleep(100);
            Sendrand();
            num_clientReady = 0;
        }

        private void Sendrand()
        {
            Random rnd = new Random();
            int dice1 = rnd.Next(1, 7);
            int dice2 = rnd.Next(1, 7);
            int dice3 = rnd.Next(1, 7);
            String message1 = "resultdice1" + dice1.ToString() + "resultdice2" + dice2.ToString() + "resultdice3" + dice3.ToString();
            Broadcast(message1);

        }

        void AddMess(string mess)
        {
            tbView.Text += "\r\n" + mess;
        }
        private void EnableSelected()
        {
            Thread.Sleep(500);
            Broadcast("EnableSelect");
            AddMess("EnableSelect");
            characterSelected = 0;
            curRound = 1;
        }
       
        private void btnStop_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
