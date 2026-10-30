using System;
using System.Collections.Generic;
namespace CCS.Core
{
    public static class ServiceContainer
    {
        private static readonly Dictionary<Type,object> items=new Dictionary<Type,object>();
        public static void Register<T>(T instance) where T:class
        {
            items[typeof(T)]=instance;
        }
        public static T Get<T>() where T:class
        {
            return (T)items[typeof(T)];
        }
    }
}