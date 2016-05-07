using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace MiniClient
{
    public partial class Form1 : Form
    {

        private TcpClient klient;
        private TcpClient klient2;
        private BinaryReader ReadFromServer;

        private byte[] byteData = new byte[1024];
        string odpowiedzSerwera = "";
        string wiadomoscKlienta = "";

        string host;
        int port;

        public Form1()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button5.Enabled = false;
            host = "127.0.0.1"; //textBox1.Text;
            port = 1000; //System.Convert.ToInt16(numericUpDown1.Value);
            try
            {
                klient = new TcpClient(host, port);
                dziennik.Invoke(new Action(delegate()
                {
                    dziennik.Items.Add("Nawiązano połączenie z " + host + " na porcie: " + port);
                }));
                klient.Client.BeginReceive(byteData,
                                       0,
                                       byteData.Length,
                                       SocketFlags.None,
                                       new AsyncCallback(OnReceive),
                                       null);
                ReadFromServer = new BinaryReader(klient.GetStream());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: Nie udało się nawiązać połączenia!");
                MessageBox.Show(ex.ToString());
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button6.Enabled = false;
            host = "127.0.0.1"; //textBox1.Text;
            port = 1000; //System.Convert.ToInt16(numericUpDown1.Value);
            try
            {
                klient2 = new TcpClient(host, port);
                dziennik.Invoke(new Action(delegate()
                {
                    dziennik.Items.Add("Nawiązano połączenie z " + host + " na porcie: " + port);
                }));
                klient2.Client.BeginReceive(byteData,
                                       0,
                                       byteData.Length,
                                       SocketFlags.None,
                                       new AsyncCallback(OnReceive2),
                                       null);
                ReadFromServer = new BinaryReader(klient2.GetStream());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: Nie udało się nawiązać połączenia!");
                MessageBox.Show(ex.ToString());
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                klient.Client.EndReceive(ar);
                odpowiedzSerwera = Encoding.ASCII.GetString(byteData);
                dziennik.Invoke(new Action(delegate()
                {
                    dziennik.Items.Add(odpowiedzSerwera);
                }));
                //Accordingly process the message received

                /*
                switch (odpowiedzSerwera)
                {
                    case Command.Login:
                        lstChatters.Items.Add(msgReceived.strName);
                        break;

                    case Command.Logout:
                        lstChatters.Items.Remove(msgReceived.strName);
                        break;

                    case Command.Message:
                        break;

                    case Command.List:
                        lstChatters.Items.AddRange(msgReceived.strMessage.Split('*'));
                        lstChatters.Items.RemoveAt(lstChatters.Items.Count - 1);
                        Invoke((MethodInvoker)delegate()
                        {
                            txtChatBox.Text += "<<<" + strName + " has joined the room>>>\r\n";
                        });
                        break;
                }

                if (msgReceived.strMessage != null && msgReceived.cmdCommand != Command.List)
                    Invoke((MethodInvoker)delegate()
                    {
                        txtChatBox.Text += msgReceived.strMessage + "\r\n";
                    });
                */

                byteData = new byte[1024];

                klient.Client.BeginReceive(byteData,
                                          0,
                                          byteData.Length,
                                          SocketFlags.None,
                                          new AsyncCallback(OnReceive),
                                          null);

            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclientTCP: " + host, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        

            private void OnReceive2(IAsyncResult ar)
        {
            try
            {
                klient2.Client.EndReceive(ar);
                odpowiedzSerwera = Encoding.ASCII.GetString(byteData);
                dziennik.Invoke(new Action(delegate()
                {
                    dziennik.Items.Add(odpowiedzSerwera);
                }));
                //Accordingly process the message received
                
                /*
                switch (odpowiedzSerwera)
                {
                    case Command.Login:
                        lstChatters.Items.Add(msgReceived.strName);
                        break;

                    case Command.Logout:
                        lstChatters.Items.Remove(msgReceived.strName);
                        break;

                    case Command.Message:
                        break;

                    case Command.List:
                        lstChatters.Items.AddRange(msgReceived.strMessage.Split('*'));
                        lstChatters.Items.RemoveAt(lstChatters.Items.Count - 1);
                        Invoke((MethodInvoker)delegate()
                        {
                            txtChatBox.Text += "<<<" + strName + " has joined the room>>>\r\n";
                        });
                        break;
                }

                if (msgReceived.strMessage != null && msgReceived.cmdCommand != Command.List)
                    Invoke((MethodInvoker)delegate()
                    {
                        txtChatBox.Text += msgReceived.strMessage + "\r\n";
                    });
                */

                byteData = new byte[1024];

                klient2.Client.BeginReceive(byteData,
                                          0,
                                          byteData.Length,
                                          SocketFlags.None,
                                          new AsyncCallback(OnReceive2),
                                          null);

            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclientTCP: " + host, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //Fill the info for the message to be send
                wiadomoscKlienta = "FLIPCARD";

                int temp1 = 0;
                int temp2 = 65;
                byte[] byteData = new byte[2];
                byteData[0] = Convert.ToByte(temp1);
                byteData[1] = Convert.ToByte(temp2);

                //Send it to the server
                klient.Client.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnSend), null);

                wiadomoscKlienta = "";
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to send message to the server.", "SGSclientTCP: " + host, MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //Fill the info for the message to be send
                wiadomoscKlienta = "GRABTOTE";

                byte[] byteData = new byte[8];
                byteData = Encoding.ASCII.GetBytes(wiadomoscKlienta);

                //Send it to the server
                klient.Client.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnSend), null);

                wiadomoscKlienta = "";
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to send message to the server.", "SGSclientTCP: " + host, MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //Fill the info for the message to be send
                wiadomoscKlienta = "FLIPCARD";

                byte[] byteData = new byte[8];
                byteData = Encoding.ASCII.GetBytes(wiadomoscKlienta);

                //Send it to the server
                klient2.Client.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnSend2), null);

                wiadomoscKlienta = "";
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to send message to the server.", "SGSclientTCP: " + host, MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                //Fill the info for the message to be send
                wiadomoscKlienta = "GRABTOTE";

                byte[] byteData = Encoding.ASCII.GetBytes(wiadomoscKlienta);

                //Send it to the server
                klient2.Client.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnSend2), null);

                wiadomoscKlienta = "";
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to send message to the server.", "SGSclientTCP: " + host, MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
        }

        private void OnSend(IAsyncResult ar)
        {
            try
            {
                klient.Client.EndSend(ar);
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclientTCP: " + host, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnSend2(IAsyncResult ar)
        {
            try
            {
                klient2.Client.EndSend(ar);
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclientTCP: " + host, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
