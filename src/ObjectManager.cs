using System;
using System.Collections.Generic;

namespace DataDriven
{
    public class ObjectManager
    {
        public void InitializeDynamicClasses(List<DynamicClass> dynamicClasses)
        {
            if (DynamicClassInitializer.IsInitialized)
            {
                return;
            }

            DynamicClassInitializer.InitializeDynamicClasses(dynamicClasses);
        }
    }
}
