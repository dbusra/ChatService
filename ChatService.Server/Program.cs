namespace ChatService.Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Observer.IServer client = new Observer.Server();
            client.Accept();

            while (true)
                Console.ReadLine();
            //Console.WriteLine("Hello, World!");
        }
    }
}