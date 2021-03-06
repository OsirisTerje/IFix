﻿using Microsoft.Win32;

namespace IFix
{
    public abstract class RegistryBase
    {

        public string SubKeyName { get; }
        public string BaseKey { get; }

        protected RegistryBase(string basekey, string subkeyname)
        {

            BaseKey = basekey; // RegistryKey.OpenBaseKey(basekey, RegistryView.Default);
            SubKeyName = subkeyname;
        }

        public virtual T Read<T>(string property)
        {
            var result = Registry.GetValue(BaseKey + SubKeyName, property, null);
            if (result == null)
                return default(T);
            var value = (T)result;
            return value;
        }

        public virtual bool Exist(string property)
        {
            var value = Registry.GetValue(BaseKey + SubKeyName, property, null);
            return value != null;

        }

        public abstract bool ExistKey();


        public virtual void Write<T>(string property, T val)
        {
            Registry.SetValue(BaseKey + SubKeyName, property, val);
        }


       
    }



public class KeyValue
{
    public string Key { get; }
    public string Value { get; }

    public KeyValue(string key, string val)
    {
        Key = key;
        Value = val;
    }
}

}

