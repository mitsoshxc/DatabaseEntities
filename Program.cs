using System;

namespace DatabaseEntitiesCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            char _read;
            bool _continue = true;

            Console.WriteLine("####################################################");
            Console.WriteLine("###### Welcome to Database Entities Creation #######");
            Console.WriteLine("## Press 'c' to Continue or any other key to Quit ##");
            Console.Write("#> ");
            _read = Console.ReadKey().KeyChar;
            if (_read != 'c' && _read != 'C')
            {
                Environment.Exit(1);
            }
            else
            {
                Entity _Entity = new Entity();

                while (_continue)
                {
                    _Entity.NewEntity();

                    Console.WriteLine("###################################################");
                    Console.WriteLine("## Press 'c' to Add new or any other key to Quit ##");
                    _read = Console.ReadKey().KeyChar;
                    if (_read != 'c' && _read != 'C')
                    {
                        _continue = false;
                    }
                }
            }
        }
    }
}
