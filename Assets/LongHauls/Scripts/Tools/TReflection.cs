using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

public static class TReflection
{
    public static T CreateInstance<T>(Type t,params object[] constructorArgs) => (T)Activator.CreateInstance(t, constructorArgs);
    public static void TraversalAllInheritedClasses<T> (Action<Type,T> OnInstanceCreated,params object[] constructorArgs)
    {
        Type[] allTypes= Assembly.GetExecutingAssembly().GetTypes();
        Type parentType=typeof(T);
        for (int i = 0; i < allTypes.Length; i++)
        {
            if (allTypes[i].IsClass && !allTypes[i].IsAbstract && allTypes[i].IsSubclassOf(parentType))
                OnInstanceCreated(allTypes[i], CreateInstance<T>(allTypes[i], constructorArgs));
        }
    }
    public static void InvokeAllMethod<T>(List<Type> classes,string methodName,T template) where T:class
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

    public static class UI
    {
        static Type m_UIBaseAttribute = typeof(UnityEngine.EventSystems.UIBehaviour);
        static Dictionary<Type, Func<Transform, object>> m_BasePropertyHelper = new Dictionary<Type, Func<Transform, object>>()
    {
        { typeof(Button),(Transform transform)=>transform.GetComponent<Button>() },
        { typeof(Text),(Transform transform)=>transform.GetComponent<Text>() },
        { typeof(InputField),(Transform transform)=>transform.GetComponent<InputField>() },
        { typeof(Image),(Transform transform)=>transform.GetComponent<Image>() },
    };
        public static void UIPropertyFill<T>(T target) where T : Component
        {
            try
            {
                var properties = target.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).Where(p => p.PropertyType.IsSubclassOf(m_UIBaseAttribute));
                var fieldInfos = target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).Where(p => p.FieldType.IsSubclassOf(m_UIBaseAttribute));
                object objValue = null;
                foreach (var property in properties)
                {
                    if (GetProperty(target, property.Name, property.PropertyType, out objValue))
                        property.SetValue(target, objValue, null);
                }

                foreach (var filedInfo in fieldInfos)
                {
                    if (GetProperty(target, filedInfo.Name, filedInfo.FieldType, out objValue))
                        filedInfo.SetValue(target, objValue);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error!Property Should Be Named Like: x_Xxxx_xxx! Transfered To Xxxx/xxx \n" + e.Message + "\n" + e.StackTrace);
            }
        }
        static bool GetProperty<T>(T target, string name, Type type, out object obj) where T : Component
        {
            obj = null;
            string[] propertySplitPath = name.Split('_');
            string path = "";
            for (int i = 1; i < propertySplitPath.Length; i++)
            {
                path += propertySplitPath[i];
                if (i < propertySplitPath.Length - 1)
                    path += "/";
            }
            if (!m_BasePropertyHelper.ContainsKey(type))
                throw new Exception("Type Helper Not Contains:" + type);
            Transform targetTrans = target.transform.Find(path);
            if (targetTrans == null)
                throw new Exception("Folder:" + path);
            obj = m_BasePropertyHelper[type](targetTrans);
            return true;
        }
    }
}
