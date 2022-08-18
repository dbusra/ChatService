using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Observer
{
    public class Server : IServer
    {        
        private Socket _serverSocket, _clientSocket;
        private readonly int _port = 3333;
        private readonly int _maxRequestNumber = 10;
        IPHostEntry host;
        IPAddress ipAddress;
        private Message _message = null;


        // make ready the server to listen clients.
        public Server()
        {
            try
            {            
                #region get  local end point for binding
                // get host ip address, it is used for establish connection, here localhost's ip : 127.0.0.1 
                host = Dns.GetHostEntry("localhost");
                ipAddress = host.AddressList[1];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, _port);
                #endregion

                // instantiating socket 
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                Bind(localEndPoint);
                Listen();

                Console.WriteLine("waiting for a connection from client...");

            }
            catch (Exception e)
            {
                throw new Exception("an error occurred while instantiating server! Exception: " + e.Message);
            }   

        }
        public void Bind(IPEndPoint endPoint)
        {
            _serverSocket.Bind(endPoint);
        }
        public void Listen()
        {
            _serverSocket.Listen(_maxRequestNumber);
        }
        public void Accept()
        {
            try
            {
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);               
                //Console.WriteLine("Text received: {0}", _data);
            }
            catch(Exception e)
            {
                throw new Exception("an error occurred while receiving message! Exception: " + e.Message);

            }
        }
        // when client connects to the server this method is called
        private void AcceptCallback(IAsyncResult asyncResult)
        {
            try
            {
                Console.WriteLine("in AcceptCallback");
                // server accepts current client 
                _clientSocket = _serverSocket.EndAccept(asyncResult);

                _message = new Message();

                _message.ByteMessage = new byte[_clientSocket.ReceiveBufferSize];

                // begin receiving data from client, put received data to _bytes
                //once data is received, ReceiveCallback method is called  
                _clientSocket.BeginReceive(_message.ByteMessage, 0, _message.ByteMessage.Length,SocketFlags.None,new AsyncCallback(ReceiveCallback), _clientSocket);
                Accept();
            }
            catch (Exception e)
            {
                throw new Exception("an error occurred while receiving message! Exception: " + e.Message);

            }
        }
        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {

                Console.WriteLine("in ReceiveCallback");
                
                _clientSocket = (Socket)asyncResult.AsyncState;
                //int bufferSize = _clientSocket.EndReceive(asyncResult);

                SocketError error;
                _clientSocket.EndReceive(asyncResult,out error);
                
                if (error == SocketError.Success)
                {
                    _message.StringMessage = Encoding.ASCII.GetString(_message.ByteMessage);

                    Console.WriteLine(_message.StringMessage);
                    //bool control = true;

                    _clientSocket.BeginReceive(_message.ByteMessage, 0, _message.ByteMessage.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), _clientSocket);

                    //_serverSocket.SendAsync(_buffer,SocketFlags.None);

                    //if (!control)
                    //{
                    //    Close();
                    //}
                }
                else 
                {
                    Close();
                } 

            }
            catch (Exception e)
            {
                throw new Exception("an error occurred while receiving message! Exception: " + e.Message);

            }
        }

        public void Close()
        {
           _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }
    }
}
