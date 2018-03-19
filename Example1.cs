using System;
using System.Diagnostics;

namespace DataDriven
{
    public class Example1
    {
        public static void Execute() 
        {
            Stopwatch stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < Program.Iterations; i++) {
                Person person = new Person();
                person.FirstName = "John";

                Address address = new Address();
                address.Address1 = "1234 Test St";
                person.Address = address;
            }

            stopWatch.Stop();
            Console.WriteLine("Example 1 Class Instantiation Elapsed time {0} ms",stopWatch.ElapsedMilliseconds);
        }
    }
}