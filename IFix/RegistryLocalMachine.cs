using System.Collections.Generic;
using Microsoft.Win32;

namespace IFix
{
    public class RegistryLocalMachine : RegistryBase
    {
        public RegistryLocalMachine(string subkeyname)
            : base(@"HKEY_LOCAL_MACHINE\", subkeyname)
        {

        }

        public override bool ExistKey()
        {
            var res = Registry.LocalMachine.OpenSubKey(SubKeyName);
            return res != null;
        }

        public override T Read<T>(string property)
        {
            var thebase = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            using (var res = thebase.OpenSubKey(SubKeyName))
            {
                if (res != null)
                {
                    var val = res.GetValue(property);
                    return (T) val;
                }
                return default(T);
            }
        }

        public override void Write<T>(string property, T val)
        {
            var thebase = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            var res = thebase.OpenSubKey(SubKeyName,true);
            res.SetValue(property, val);
        }
        public override bool Exist(string property)
        {
            var thebase = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            var res = thebase.OpenSubKey(SubKeyName);
            var val = res?.GetValue(property);
            return val != null;
        }


        public IEnumerable<KeyValue> Properties
        {
            get
            {
                var list = new List<KeyValue>();
                var key = Registry.LocalMachine.OpenSubKey(SubKeyName);

                foreach (var property in key.GetValueNames())
                {
                    var val = Read<string>(property);
                    list.Add(new KeyValue(property,val));

                }
                return list;
            }
        }


    }
}