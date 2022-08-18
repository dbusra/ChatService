using ChatService.Observer;
namespace ChatService.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Observer.Client client = new Observer.Client();
            client.SendMessages();
            
            Observer.Client client2 = new Observer.Client();
            client2.SendMessages();

            //Console.WriteLine("Hello, World!");
        }
    }
}