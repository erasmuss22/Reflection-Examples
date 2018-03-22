using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace DataDriven
{
    public class DynamicClassInitializer
    {
        private static Dictionary<string, Type> dynamicTypesByName = new Dictionary<string, Type>(StringComparer.CurrentCultureIgnoreCase);
        private static AssemblyBuilder assemblyBuilder;
        private static ModuleBuilder moduleBuilder;

        private static Dictionary<string, Func<BaseObject>> creatorsByName = new Dictionary<string, Func<BaseObject>>();

        private static Dictionary<Type, Func<BaseObject>> creatorsByType = new Dictionary<Type, Func<BaseObject>>();

        private static Dictionary<string, Dictionary<string, Action<BaseObject, object>>> settersByTypeNameAndProperty = new Dictionary<string, Dictionary<string, Action<BaseObject, object>>>();

        public static bool IsInitialized { get; private set; }

        public static void InitializeDynamicClasses(List<DynamicClass> dynamicClasses)
        {
            if (IsInitialized)
            {
                throw new InvalidOperationException("Dynamic classes are already initialized. To add new classes use the CreateType method.");
            }

            // Tarjan's algorithm for topological sorting to make sure dynamic classes
            // that depend on other dynamic classes are initialized first
            List<DynamicClass> sortedDynamicClasses = ClassDependencyResolver.SortDependencyOrder(dynamicClasses);

            // Create the dynamic assembly
            var typeSignature = "DataDriven.Objects";
            var assembly = new AssemblyName(typeSignature);
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assembly, AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(typeSignature);

            // Now that the assembly exists, iterate over the sorted dynamic classes and create their types
            foreach (DynamicClass dynamicClass in sortedDynamicClasses)
            {
                CreateType(dynamicClass);
            }

            foreach (KeyValuePair<string, Type> kvp in dynamicTypesByName)
            {
                creatorsByName[kvp.Key] = Expression.Lambda<Func<BaseObject>>(Expression.New(kvp.Value)).Compile();
                creatorsByName[kvp.Key]();

                creatorsByType[kvp.Value] = Expression.Lambda<Func<BaseObject>>(Expression.New(kvp.Value)).Compile();
                creatorsByType[kvp.Value]();
            }

            IsInitialized = true;
        }

        public static BaseObject CreateObjectInstanceByName(string typeName)
        {
            if (dynamicTypesByName.ContainsKey(typeName))
            {
                return (BaseObject)Activator.CreateInstance(dynamicTypesByName[typeName]);
            }

            return null;
        }

        public static BaseObject CreateObjectInstanceByNameLambda(string typeName)
        {
            if (creatorsByName.ContainsKey(typeName))
            {
                return creatorsByName[typeName]();
            }

            return null;
        }

        public static BaseObject CreateObjectInstanceByTypeNameLambda(string typeName)
        {
            if (dynamicTypesByName.ContainsKey(typeName)) {
                Type type = dynamicTypesByName[typeName];
                if (creatorsByType.ContainsKey(type))
                {
                    return creatorsByType[type]();
                }
            }

            return null;
        }

        public static Action<BaseObject, object> GetSetterForObjectProperty(string typeName, string propertyName)
        {
            if (settersByTypeNameAndProperty.ContainsKey(typeName) && settersByTypeNameAndProperty[typeName].ContainsKey(propertyName))
            {
                return settersByTypeNameAndProperty[typeName][propertyName];
            }

            return null;
        }

        public static bool DynamicTypeExists(string typeName)
        {
            return dynamicTypesByName.ContainsKey(typeName);
        }

        public static void CreateType(DynamicClass dynamicClass)
        {
            if (dynamicTypesByName.ContainsKey(dynamicClass.ObjectName))
            {
                throw new InvalidOperationException(string.Format("A type with the name {0} is already loaded. Either change the name of the type or alter it in the database and reinitialize the system", dynamicClass.ObjectName));
            }

            if (assemblyBuilder == null)
            {
                throw new InvalidOperationException("The assembly has not been initialized. Call InitializeDynamicClasses before creating new types.");
            }

            // Build the type using the given name and make it inherit from BaseObject
            TypeBuilder tb = moduleBuilder.DefineType(
                dynamicClass.ObjectName,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout,
                typeof(BaseObject));

            // Build a default constructor for the type so that Newtonsoft knows how to handle it
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            // go over each field on the class and create a property
            foreach (var field in dynamicClass.Fields)
            {
                // if the field is using a system type (or custom model coded in C#), use that
                if (field.PropertyType != null)
                {
                    CreateProperty(tb, field.PropertyName, field.PropertyType);
                }
                else
                {
                    // if the field is using a dynamic type, use that and throw exception if it doesn't exist
                    if (!string.IsNullOrEmpty(field.DynamicRuntimeType) && dynamicTypesByName.ContainsKey(field.DynamicRuntimeType))
                    {
                        CreateProperty(tb, field.PropertyName, dynamicTypesByName[field.DynamicRuntimeType]);
                    }
                    else
                    {
                        throw new ArgumentException(string.Format("No dynamic type exists by the name {0}", field.DynamicRuntimeType));
                    }
                }
            }

            // now that the properties are attached, create the type and cache it
            Type objectType = tb.CreateType();
            dynamicTypesByName[dynamicClass.ObjectName] = objectType;

            foreach (var field in dynamicClass.Fields)
            {
                var propertyInfo = objectType.GetProperty(field.PropertyName);
                var instance = Expression.Parameter(propertyInfo.DeclaringType, "objInstance");
                var argument = Expression.Parameter(typeof(object), "valueToSet");

                var setterCall = Expression.Call(
                    instance,
                    propertyInfo.GetSetMethod(),
                    Expression.Convert(argument, propertyInfo.PropertyType));

                if (!settersByTypeNameAndProperty.ContainsKey(dynamicClass.ObjectName))
                {
                    settersByTypeNameAndProperty[dynamicClass.ObjectName] = new Dictionary<string, Action<BaseObject, object>>();
                }

                settersByTypeNameAndProperty[dynamicClass.ObjectName][field.PropertyName] = (Action<BaseObject, object>)Expression.Lambda(setterCall, instance, argument).Compile();
            }
        }

        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            // create a private backing for the field
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            // define the public property for the field using the type and make it use the default value for that type
            PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            // Create a getter method for the property
            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);

            // make the IL code generator for the getter and emit assembly code that returns the value currently in the private backing
            // of the property
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();
            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            // Create a setter method that accepts a single value of the type defined
            MethodBuilder setPropMthdBldr =
                tb.DefineMethod(
                  "set_" + propertyName,
                  MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                  null,
                  new[] { propertyType });

            // make the IL code generator for the setter and emit assembly code that sets the value in the private backing
            // of the property
            ILGenerator setIL = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIL.DefineLabel();
            Label exitSet = setIL.DefineLabel();

            // set the value
            setIL.MarkLabel(modifyProperty);
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, fieldBuilder);

            // exit the setter function
            setIL.Emit(OpCodes.Nop);
            setIL.MarkLabel(exitSet);
            setIL.Emit(OpCodes.Ret);

            // add the generated methods to the property
            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
    }
}
