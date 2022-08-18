namespace ChatService.Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Observer.IServer server = new Observer.Server();
            server.Accept();

            while (true)
                Console.ReadLine();
        }
    }
}