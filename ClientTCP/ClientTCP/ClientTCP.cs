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

namespace ClientTCP
{
    enum Command
    {
        Login,
        Logout,
        Message,
        List,
        Null
    }

    public partial class ClientTCP : Form
    {

        public ClientTCP()
        {
                InitializeComponent();
                CheckForIllegalCrossThreadCalls = false;
                Connect();
        }
        IPEndPoint IP;
        Socket client;

        void Connect()
        {
            IP = new IPEndPoint(IPAddress.Loopback, 10000);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                client.Connect(IP);
            }
            catch
            {
                MessageBox.Show("Không thể kết nối đến server!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            Thread listener = new Thread(Receive);
            listener.IsBackground = true;
            listener.Start();
        }

        void close()
        {
            client.Close();
        }

        void Send()
        {
            Data data = new Data();
            data.strName = null;
            data.cmdCommand = Command.Message;
            data.strMessage = textBox2.Text;


            if (client != null && textBox2.Text != string.Empty)
                client.Send(data.Serialize());
        }

        void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[1024 * 100];
                    client.Receive(buffer);

                    Data data = new Data(buffer);
                    

                    string message = data.strMessage;

                    AddMessage(message);

                }
            }
            catch
            {
                client.Close();
            }

        }

        void AddMessage(string s)
        {
            listView1.Items.Add(new ListViewItem() { Text = s });
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Send();
            AddMessage(textBox2.Text);
            textBox2.Clear();
        }
    }
}
