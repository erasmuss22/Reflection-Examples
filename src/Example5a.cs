using System;
using System.Diagnostics;

namespace DataDriven
{
    public class Example5A
    {
        public static void Execute() 
        {
            Stopwatch stopWatch = Stopwatch.StartNew();

            BaseObject person = DynamicClassInitializer.CreateObjectInstanceByName("Person");
            BaseObject address = DynamicClassInitializer.CreateObjectInstanceByName("Address");

            Type personType = person.GetType();
            var firstNameProperty = personType.GetProperty("FirstName");
            var lastNameProperty = personType.GetProperty("LastName");
            var addressProperty = personType.GetProperty("Address");

            Type addressType = address.GetType();
            var address1Property = addressType.GetProperty("Address1");
            var postalCodeProperty = addressType.GetProperty("PostalCode");

            for (int i = 0; i < Program.Iterations; i++) {
                // use reflected property information to set values on the instance
                firstNameProperty.SetValue(person, "John");
                lastNameProperty.SetValue(person, "Smith");

                address1Property.SetValue(address, "1234 Main St");
                postalCodeProperty.SetValue(address, "12345");

                addressProperty.SetValue(person, address);
            }

            stopWatch.Stop();
            
            Console.WriteLine("Example 5a setting properties through reflection with reflection outside loop Elapsed time {0} ms",stopWatch.ElapsedMilliseconds);
        }
    }
}