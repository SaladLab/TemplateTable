using System;

namespace Basic
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("***** BasicUsage *****");
            var basicUsage = new BasicUsage();
            basicUsage.Run();
            Console.WriteLine();

            Console.WriteLine("***** Benchmark *****");
            var benchmark = new Benchmark();
            benchmark.Run();
            Console.WriteLine();
        }
    }
}
