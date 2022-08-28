using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace ChatService.Observer
{
    public class Client : IClient
    {
        private Socket _clientSocket;
        private const string SERVER_HOST_NAME = "localhost";
        private readonly int _port = 3333;
        IPHostEntry host;
        IPAddress ipAddress;
        private Message _message = null;
        private byte[] _bytes;

        public List<string> MessageList = new List<string>();

        public Client()
        {
            try
            {
                IPEndPoint remoteEndPoint = GetEndpoint();

                // instantiating socket 
                _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                Connect(remoteEndPoint);

                Console.WriteLine("Client connected to: {0}  ", remoteEndPoint);
            }
            catch (Exception e)
            {
                throw new Exception("an error occurred while instantiating client! Exception: " + e.Message);
            }
        }
        public void Connect(IPEndPoint endPoint)
        {
            //connect to remote end point, once connection established ConnectCallback is called.
            _clientSocket.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), null);
        }
        public void BeginSendMessage()
        {
            try
            {
                bool sendMessage = true;
                _message = new Message();
                _message.MessageCount = 0;

                Console.WriteLine("Message: (to send message press enter) ");

                while (sendMessage) 
                {    

                    string message = Console.ReadLine();
                    //string message = "Client sends message";

                    
                    if (!string.IsNullOrEmpty(message))
                    {
                        // convert message into byte array.
                        _bytes = Encoding.ASCII.GetBytes(message);

                        // send message to server through socket
                        _clientSocket.BeginSend(_bytes, 0, _bytes.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);

                        //receive response from server
                        //int bytesReceived = _clientSocket.Receive(_bytes);

                        //Console.WriteLine("Echo message = {0} ", Encoding.ASCII.GetString(bytes, 0, bytesReceived));
                    }
                    //else
                    //{
                    //    Close();
                    //}
                   // _message.MessageCount++;

                }

            }
            catch (Exception e) 
            {
                throw new Exception("an error occurred while sending message! Exception: " + e.Message);
            }
        }
       
        public void Close()
        {
            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }
        private IPEndPoint GetEndpoint()
        {
            //get remote end point
            // get the remote ip address, it is used for establish connection, here localhost's ip : 127.0.0.1 
            host = Dns.GetHostEntry(SERVER_HOST_NAME);
            ipAddress = host.AddressList[1];

            IPEndPoint endPoint = new IPEndPoint(ipAddress, _port);
            return endPoint;  
        }
        private void ConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                _clientSocket.EndConnect(asyncResult);
            }
            catch(Exception e)
            {
                throw new Exception("an error occurred while connecting! Exception: " + e.Message);

            }
        }

        private void SendCallback(IAsyncResult asyncResult)
        {
            try
            {
                _clientSocket.EndSend(asyncResult);
                //_clientSocket.Close();
            }
            catch (Exception e)
            {
                throw new Exception("an error occurred while sending message! Exception: " + e.Message);

            }
        } 
        private string Message(int i)
        {
            string message = (i + 1) + ". message";
            MessageList.Add(message);

            return message;
        }
        private void UpdateList()
        {
            for (int i = 0; i < length; i++)
            {

            }
        }
    }
}
