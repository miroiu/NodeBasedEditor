using System;

namespace VEdit.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadKey();

            object x = 3.0;
            if(x is double)
            {
                Console.WriteLine(x);
            }
        }

        public static int Add(int x, int y)
        {
            return x + y;
        }

        public static void Write(int x)
        {
            Console.WriteLine(x);
        }
    }
}
