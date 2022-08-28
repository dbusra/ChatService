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
        private const string HOST_NAME = "localhost";
        private readonly int _port = 3333;
        private readonly int _maxRequestNumber = 10;
        IPHostEntry host;
        IPAddress ipAddress;
        private Message _message = null;
        private byte[] _bytes;
        private DateTime _dateBeforeSend;
        private int _counterForReceiveMethodCall = 0;

        public static ManualResetEvent allDone = new ManualResetEvent(false);


        // make ready the server to listen clients.
        public Server()
        {
            try
            {
                IPEndPoint localEndPoint = GetEndpoint();

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
                allDone.Reset();
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

                // Wait until a connection is made before continuing.  
                allDone.WaitOne();
            }
            catch (Exception e)
            {
                throw new Exception("an error occurred while receiving message! Exception: " + e.Message);

            }
        }
        public void NotifyClients()
        {
            GetCallCount();

            if (_counterForReceiveMethodCall == 1)
            {
                //warn client
                //SendWarningMessage();
            }
            else if (_counterForReceiveMethodCall > 1)
            {
                //shutdown client;
                //_clientSocket.Close();

            }
        }
        public void Close()
        {
            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }
        private IPEndPoint GetEndpoint()
        {
            //get local end point for binding
            // get host ip address, it is used for establish connection, here localhost's ip : 127.0.0.1 
            host = Dns.GetHostEntry(HOST_NAME);
            ipAddress = host.AddressList[1];
            IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, _port);

            return iPEndPoint;
        }

        // when client connects to the server this method is called
        private void AcceptCallback(IAsyncResult asyncResult)
        {
            try
            {
                // Signal the main thread to continue.  
                allDone.Set();

                // server accepts current client 
                _clientSocket = _serverSocket.EndAccept(asyncResult);

                _message = new Message();

                _bytes = new byte[_clientSocket.ReceiveBufferSize];

                // TODO IF INTERVAL IS EQUAL OR LESS THAN 1 SEC, WARN THE CLIENT. IF ("WARNING_COUNT" > 1) THEN SHUTDOWN THE CLIENT
                // UNIT TEST : 

                _dateBeforeSend = DateTime.Now;

                // begin receiving data from client, put received data to _bytes
                //once data is received, ReceiveCallback method is called  
                _clientSocket.BeginReceive(_bytes, 0, _bytes.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), _clientSocket);
                             
                //accept from another/multiple client
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
                _clientSocket = (Socket)asyncResult.AsyncState;
                //int bufferSize = _clientSocket.EndReceive(asyncResult);

                SocketError error;
                _clientSocket.EndReceive(asyncResult, out error);


                if (error == SocketError.Success)
                {
                    Console.WriteLine("Messages from client:");

                    string message = Encoding.ASCII.GetString(_bytes);

                    Console.Write(message);

                    // receive messages from client continuously.
                    _clientSocket.BeginReceive(_bytes, 0, _bytes.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), _clientSocket);

                    NotifyClients();
                    
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

        private int GetCallCount()
        {
            DateTime now = DateTime.Now;
            TimeSpan  span = now - _dateBeforeSend;

            if (span.TotalMinutes <= 1) 
                _counterForReceiveMethodCall++;
            
            else
                _counterForReceiveMethodCall = 0;

            return _counterForReceiveMethodCall;
        }

        

        #region send message to client //not complete
        //public void SendWarningMessage()
        //{
        //    try
        //    {
        //        _message = new Message();

        //            string message = "Server sends message";

        //            if (message != null)
        //            {
        //                // convert message into byte array.
        //                _bytes = Encoding.ASCII.GetBytes(message);

        //                // send message to client through socket
        //                _serverSocket.BeginSend(_bytes, 0, _bytes.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);

        //            //receive response from client

        //            int bytesReceived = _clientSocket.Receive(_bytes);

        //            Console.WriteLine("Echo message = {0} ", Encoding.ASCII.GetString(_bytes, 0, bytesReceived));
        //        }
        //            else
        //            {
        //                Close();
        //            }


        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("an error occurred while sending message! Exception: " + e.Message);
        //    }
        //}
        //private void SendCallback(IAsyncResult asyncResult)
        //{
        //    try
        //    {
        //        _serverSocket.EndSend(asyncResult);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("an error occurred while sending message! Exception: " + e.Message);

        //    }
        //}
        #endregion
    }
}
