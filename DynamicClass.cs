using System;
using System.Collections.Generic;

namespace DataDriven
{
    public class DynamicClass
    {
        public List<Field> Fields { get; set; }

        public string ObjectName { get; set; }
    }

    public class Field
    {
        public string PropertyName { get; set; }

        public Type PropertyType { get; set; }

        public string DynamicRuntimeType { get; set; }
    }
}
