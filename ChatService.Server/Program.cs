namespace ChatService.Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Observer.IServer client = new Observer.Server();
            client.Accept();
            //Console.WriteLine("Hello, World!");
        }
    }
}