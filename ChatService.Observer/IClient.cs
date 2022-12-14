using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Observer
{
    public interface IClient
    {
        //connect to remote end point
        void Connect(IPEndPoint endPoint);
        void BeginSendMessage();
        void Close();
    }
}
