using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerTCP
{
    enum Command
    {
        Login,
        Logout,
        Message,
        List,
        Null
    }


    public partial class ServerTCP : Form
    {
        public ServerTCP()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Connect();
        }
        IPEndPoint IP;
        Socket server;
        List<Socket> clientList;

        void Connect()
        {
            clientList = new List<Socket>();

            IP = new IPEndPoint(IPAddress.Any, 10000);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            server.Bind(IP);

            Thread listen = new Thread(() => {
                try
                {
                    while (true)
                    {
                        server.Listen(100);
                        Socket client = server.Accept();
                        clientList.Add(client);

                        Thread receive = new Thread(Receive);
                        receive.IsBackground = true;
                        receive.Start(client);
                    }
                }
                catch
                {
                    IP = new IPEndPoint(IPAddress.Any, 10000);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
            });

            listen.IsBackground = true;
            listen.Start();
        }

        void close()
        {
            server.Close();
        }

        void Send(Socket client)
        {
            
        }

        void Receive(Object obj)
        {
            Socket client = obj as Socket;
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[1024 * 100];
                    client.Receive(buffer);

                    Data data = new Data(buffer);

                    string message = data.strMessage;

                    

                    foreach (Socket item in clientList)
                    {
                        if (item != null && item != client)
                            item.Send(data.Serialize());
                    }

                    AddMessage(message);
                }
            }
            catch
            {
                clientList.Remove(client);
                client.Close();
            }

        }

        byte[] Serialize(object obj)
        {
            MemoryStream Stream = new MemoryStream();
            BinaryFormatter Formatter = new BinaryFormatter();

            Formatter.Serialize(Stream, obj);
            return Stream.ToArray();
        }

        object Deserialize(byte[] data)
        {
            MemoryStream Stream = new MemoryStream(data);
            BinaryFormatter Formatter = new BinaryFormatter();

            return Formatter.Deserialize(Stream);

        }
        void AddMessage(string s)
        {
            listView1.Items.Add(new ListViewItem() { Text = s });
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            close();
        }
    }

}
