using System;
using System.Reflection;
public static class ObjectExtensions
{
    public static void ForEachProperty(this Type target, Action<PropertyInfo> forEachCallback)
    {
        PropertyInfo[] elementFields = target.GetProperties();
        var l = elementFields.Length;
        for(int i = 0; i < l; i++)
        {
            if(elementFields[i].GetCustomAttributes(typeof(HideAttribute), true).Length > 0) continue;
            
            forEachCallback?.Invoke(elementFields[i]);
        }
    }
}