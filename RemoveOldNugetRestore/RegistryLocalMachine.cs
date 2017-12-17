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