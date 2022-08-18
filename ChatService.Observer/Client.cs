﻿using System;
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
        private readonly int _port = 3333;
        IPHostEntry host;
        IPAddress ipAddress;
        byte[] bytes = null; 
        private Message _message = null;

        public Client()
        {
            try
            {               
                #region get remote end point
                // get the remote ip address, it is used for establish connection, here localhost's ip : 127.0.0.1 
                host = Dns.GetHostEntry("localhost");
                ipAddress = host.AddressList[1];
                IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, _port);
                #endregion

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
                while(true){ 
                Console.WriteLine("Please enter a message: (to send message press enter) ");
                string message = Console.ReadLine();

                if (message != null)
                {
                    _message = new Message();

                    // convert message into byte array.
                    _message.ByteMessage = Encoding.ASCII.GetBytes(message);

                    // send message to server through socket
                    _clientSocket.BeginSend(_message.ByteMessage, 0, _message.ByteMessage.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);

                    ////receive response from server
                    //int bytesReceived = _clientSocket.Receive(_message.ByteMessage);

                    //Console.WriteLine("Echo message = {0} ", Encoding.ASCII.GetString(bytes, 0, bytesReceived));

                }
                else
                {
                    Close();
                }
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
        private void ConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                Console.WriteLine("in ConnectCallback");

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
                Console.WriteLine("in SendCallback");

                _clientSocket.EndSend(asyncResult);
            }
            catch (Exception e)
            {
                throw new Exception("an error occurred while sending message! Exception: " + e.Message);

            }
        }
    }
}
