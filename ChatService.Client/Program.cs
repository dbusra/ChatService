using ChatService.Observer;
namespace ChatService.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Observer.IClient client = new Observer.Client();
            client.BeginSendMessage();

            while(true)
                Console.ReadLine();
            //Console.WriteLine("Hello, World!");
        }
    }
}