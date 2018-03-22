using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataDriven
{
    public class PropertyWrapper<T> : PropertyWrapper where T : BaseObject
    {
        public PropertyWrapper() : base(typeof(T))
        {
            this.BuildSetters();
        }

        private Dictionary<string, Action<T, object>> settersByProperty = new Dictionary<string, Action<T, object>>();

        public override void ExecuteSetter(BaseObject objInstance, string propertyName, object objValue)
        {
            this.settersByProperty[propertyName]((T)objInstance, objValue);
        }

        public void BuildSetters()
        {
            foreach (PropertyInfo propertyInfo in this.Type.GetProperties())
            {
                var instance = Expression.Parameter(propertyInfo.DeclaringType, "objInstance");
                var argument = Expression.Parameter(typeof(object), "valueToSet");

                var setterCall = Expression.Call(
                    instance,
                    propertyInfo.GetSetMethod(),
                    Expression.Convert(argument, propertyInfo.PropertyType));

                this.settersByProperty[propertyInfo.Name] = (Action<T, object>)Expression.Lambda(setterCall, instance, argument).Compile();
            }
        }
    }

    public abstract class PropertyWrapper
    {
        public PropertyWrapper(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; set; }

        public static PropertyWrapper CreateFromTypeName(string typeName)
        {
            Type openType = typeof(PropertyWrapper<>);

            Type objectType = DynamicClassInitializer.CreateObjectInstanceByName(typeName).GetType();
            Type genericListType = openType.MakeGenericType(objectType);

            object instance = Activator.CreateInstance(genericListType);
            return (PropertyWrapper)instance;
        }

        public abstract void ExecuteSetter(BaseObject objInstance, string propertyName, object objValue);
    }
}