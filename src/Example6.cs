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

            var firstNameProperty = DynamicClassInitializer.GetSetterForObjectProperty("Person", "FirstName");
            var lastNameProperty = DynamicClassInitializer.GetSetterForObjectProperty("Person", "LastName");
            var addressProperty = DynamicClassInitializer.GetSetterForObjectProperty("Person", "Address");

            var address1Property = DynamicClassInitializer.GetSetterForObjectProperty("Address", "Address1");
            var postalCodeProperty = DynamicClassInitializer.GetSetterForObjectProperty("Address", "PostalCode");

            for (int i = 0; i < Program.Iterations; i++) {
                // use compiled and cached actions to set values for each property
                firstNameProperty(person, "John");
                lastNameProperty(person, "Smith");

                address1Property(address, "1234 Main St");
                postalCodeProperty(address, "12345");

                addressProperty(person, address);
            }

            stopWatch.Stop();
            
            Console.WriteLine("Example 6 setting properties using cached lambdas for setters Elapsed time {0} ms",stopWatch.ElapsedMilliseconds);
        }
    }
}