using System;
using System.Diagnostics;

namespace DataDriven
{
    public class Example4
    {
        public static void Execute() 
        {
            Stopwatch stopWatch = Stopwatch.StartNew();

            Person person = new Person();
            Address address = new Address();
            for (int i = 0; i < Program.Iterations; i++) {
                person.FirstName = "John";
                person.LastName = "Smith";

                address.Address1 = "1234 Main St";
                address.PostalCode = "12345";

                person.Address = address;
            }

            stopWatch.Stop();
            Console.WriteLine("Example 4 Setting Properties on Class Elapsed time {0} ms",stopWatch.ElapsedMilliseconds);
        }
    }
}