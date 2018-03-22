using System;
using System.Diagnostics;

namespace DataDriven
{
    public class Example6
    {
        public static void Execute() 
        {
            Stopwatch stopWatch = Stopwatch.StartNew();

            BaseObject person = DynamicClassInitializer.CreateObjectInstanceByName("Person");
            BaseObject address = DynamicClassInitializer.CreateObjectInstanceByName("Address");

            for (int i = 0; i < Program.Iterations; i++) {
                // use compiled and cached actions to set values for each property
                DynamicClassInitializer.SetPropertyForObject("Person", "FirstName", person, "John");
                DynamicClassInitializer.SetPropertyForObject("Person", "LastName", person, "Smith");

                DynamicClassInitializer.SetPropertyForObject("Address", "Address1", address, "1234 Main St");
                DynamicClassInitializer.SetPropertyForObject("Address", "PostalCode", address, "12345");
                DynamicClassInitializer.SetPropertyForObject("Person", "Address", person, address);
            }

            stopWatch.Stop();

            Console.WriteLine("Example 6 setting properties using cached lambdas for setters Elapsed time {0} ms",stopWatch.ElapsedMilliseconds);
        }
    }
}