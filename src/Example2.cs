using System;
using System.Diagnostics;

namespace DataDriven
{
    public class Example2
    {
        public static void Execute() 
        {
            Stopwatch stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < Program.Iterations; i++) {
                BaseObject address = DynamicClassInitializer.CreateObjectInstanceByName("Address");

                // create a new Person
                BaseObject person = DynamicClassInitializer.CreateObjectInstanceByName("Person");
            }

            stopWatch.Stop();
            
            Console.WriteLine("Example 2 Activator.CreateInstance Instantiation Elapsed time {0} ms",stopWatch.ElapsedMilliseconds);
        }
    }
}