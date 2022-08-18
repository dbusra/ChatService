using System.Net;
using System.Net.Sockets;

namespace ChatService.Observer
{
    public interface IServer
    {
        //associate socket with local end point.
        void Bind(IPEndPoint endPoint);               
        //start listening messages from client
        void Listen();
        //accept connection from client
        void Accept();
        // close socket
        void Close();
    }
}