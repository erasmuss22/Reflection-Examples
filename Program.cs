using System;
using System.Collections.Generic;

namespace DataDriven
{
    class Program
    {
        public static readonly int Iterations = 100000;
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

            Example1.Execute();
            Example2.Execute();
            Example3.Execute();
            Example3A.Execute();
        }
    }
}
