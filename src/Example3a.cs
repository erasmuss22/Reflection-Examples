using System;
using System.Diagnostics;

namespace DataDriven
{
    public class Example3A
    {
        public static void Execute() 
        {
            Stopwatch stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < Program.Iterations; i++) {
                BaseObject address = DynamicClassInitializer.CreateObjectInstanceByTypeNameLambda("Address");

                // create a new Person
                BaseObject person = DynamicClassInitializer.CreateObjectInstanceByTypeNameLambda("Person");
            }

            stopWatch.Stop();
            
            Console.WriteLine("Example 3a Lambda Instantiation Cache By Type Elapsed time {0} ms",stopWatch.ElapsedMilliseconds);
        }
    }
}