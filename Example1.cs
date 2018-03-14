using System;
using System.Diagnostics;

namespace DataDriven
{
    public class Example1
    {
        public static void Execute() 
        {
            Stopwatch stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < 10000000; i++) {
                Person person = new Person();
                person.FirstName = "John";

                Address address = new Address();
                address.Address1 = "1234 Test St";
                person.Address = address;
            }

            stopWatch.Stop();
            Console.WriteLine("Example 1 Class Instantiation Elapsed time {0} ms",stopWatch.ElapsedMilliseconds);

            stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < 10000000; i++) {
                BaseObject address = DynamicClassInitializer.CreateObjectInstanceByName("Address");

                Type addressType = address.GetType();
                var address1 = addressType.GetProperty("Address1");

                // set a value on the address's Address1 property
                address1.SetValue(address, "1234 Test St");

                // create a new Person
                BaseObject person = DynamicClassInitializer.CreateObjectInstanceByName("Person");

                Type personType = person.GetType();
                var addressProperty = personType.GetProperty("Address");

                // set the dynamically created Address on the dynamically created Person's Address property
                addressProperty.SetValue(person, address);
            }

            stopWatch.Stop();
            Console.WriteLine("Example 1 Dynamic Instantiation Elapsed time {0} ms",stopWatch.ElapsedMilliseconds);
        }
    }
}