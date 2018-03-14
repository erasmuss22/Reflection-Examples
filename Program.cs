using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DataDriven
{
    class Program
    {
        static void Main(string[] args)
        {
            DynamicClass addressCLass = new DynamicClass()
            {
                ObjectName = "Address",
                Fields = new List<Field>()
                {
                    new Field() { PropertyName = "Address1", PropertyType = typeof(string) },
                    new Field() { PropertyName = "Address2", PropertyType = typeof(string) },
                    new Field() { PropertyName = "PostalCode", PropertyType = typeof(string) }
                }
            };

            DynamicClass dynamicClass = new DynamicClass()
            {
                ObjectName = "Person",
                Fields = new List<Field>()
                {
                    new Field() { PropertyName = "FirstName", PropertyType = typeof(string) },
                    new Field() { PropertyName = "LastName", PropertyType = typeof(string) },
                    new Field() { PropertyName = "Age", PropertyType = typeof(int) },
                    new Field() { PropertyName = "Address", DynamicRuntimeType = "Address" }
                }
            };

            DynamicClassInitializer.InitializeDynamicClasses(new List<DynamicClass>() { addressCLass, dynamicClass });

            Stopwatch stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < 50000000; i++) {
                Person person = new Person();
                person.FirstName = "John";

                Address address = new Address();
                address.Address1 = "1234 Test St";
                person.Address = address;
            }

            stopWatch.Stop();
            Console.WriteLine("Class Instantiation Elapsed time {0} ms",stopWatch.ElapsedMilliseconds);

            stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < 50000000; i++) {
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
            Console.WriteLine("Dynamic Instantiation Elapsed time {0} ms",stopWatch.ElapsedMilliseconds);
        }
    }
}
