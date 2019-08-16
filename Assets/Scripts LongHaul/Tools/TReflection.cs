using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

public static class TReflection
{
    public static T CreateInstance<T>(Type t,params object[] constructorArgs) => (T)Activator.CreateInstance(t, constructorArgs);
    public static void GetAllInheritClasses<T> (Action<Type,T> OnInstanceCreated,params object[] constructorArgs)
    {
        Type[] allTypes= Assembly.GetExecutingAssembly().GetTypes();
        Type parentType=typeof(T);
        for (int i = 0; i < allTypes.Length; i++)
        {
            if (allTypes[i].IsClass && !allTypes[i].IsAbstract && allTypes[i].IsSubclassOf(parentType))
                OnInstanceCreated(allTypes[i], CreateInstance<T>(allTypes[i], constructorArgs));
        }
    }
    public static void InovokeAllMethod<T>(List<Type> classes,string methodName,T template) where T:class
    {
        foreach (Type t in classes)
        {
            MethodInfo method = t.GetMethod(methodName);
            if (method != null)
                method.Invoke(null,new object[] {template });
            else
                UnityEngine.Debug.LogError("Null Method Found From:"+t.ToString()+"."+methodName);
        }
    }
}
