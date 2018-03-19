using System;
using System.Diagnostics;

namespace DataDriven
{
    public class Example3
    {
        public static void Execute() 
        {
            Stopwatch stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < Program.Iterations; i++) {
                BaseObject address = DynamicClassInitializer.CreateObjectInstanceByNameLambda("Address");

                // create a new Person
                BaseObject person = DynamicClassInitializer.CreateObjectInstanceByNameLambda("Person");
            }

            stopWatch.Stop();
            
            Console.WriteLine("Example 3 Lambda Instantiation Caches By Name Elapsed time {0} ms",stopWatch.ElapsedMilliseconds);
        }
    }
}