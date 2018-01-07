using System;
using System.Diagnostics;
using System.IO;
using System.Security;

namespace IFix
{
    public class Diagnostics
    {
        public DiagnosticsCommands DiagnosticsCommands { get; private set; }
        public int Execute(DiagnosticsCommands command)
        {
            DiagnosticsCommands = command;
            if (command.Show)
            {
                ShowDump();
                ShowFuslog();
                return 0;
            }

            if (command.HasDumpCommand && command.Check)
            {
                ShowDump();
                
            }

            if (command.HasFuslogCommand && command.Check)
            {
                ShowFuslog();
            }

            try
            {

            
            if (command.HasDumpCommand && command.Fix)
            {
                var dump = new RegistryDump(true);
                var enable = command.EnableDisableDump == 1;
                if (enable)
                    dump.EnableDump();
                else
                    dump.DisableDump();
                Console.WriteLine(enable?"Dump enabled":"Dump disabled");
            }


            if (command.HasFuslogCommand && command.Fix)
            {
                var fuslog = new RegistryFusLog();
                var enable = command.EnableDisableFuslog == 1;
                if (enable)
                    fuslog.Enable();
                else
                    fuslog.Disable();
                Console.WriteLine(enable ? "Fuslog enabled" : "Fuslog disabled");

            }

            if (command.Fix && command.DumpFolder.Length > 0)
            {
                var dump = new RegistryDump();
                if (command.DumpFolder != dump.ReadDumpFolder())
                {
                    dump.SetDumpFolder(command.DumpFolder);
                    Console.WriteLine($"Dumpfolder set to {command.DumpFolder}");
                    if (!Directory.Exists(command.DumpFolder))
                        Directory.CreateDirectory(command.DumpFolder);
                }
                else
                {
                    Console.WriteLine($"Dump folder not changed, already set to {command.DumpFolder}");
                }
            }

            if (command.Fix && command.FuslogFolder.Length > 0)
            {
                var fusLog = new RegistryFusLog();
                if (command.DumpFolder != fusLog.ReadLogFolder())
                {
                    fusLog.SetLogFolder(command.FuslogFolder);
                    Console.WriteLine($"Fuslog folder set to {command.FuslogFolder}");
                    if (!Directory.Exists(command.FuslogFolder))
                        Directory.CreateDirectory(command.FuslogFolder);
                }
                else
                {
                    Console.WriteLine($"Fuslog folder  not changed, already set to {command.FuslogFolder}");
                }
            }
            }
            catch (SecurityException)
            {
                Console.WriteLine("IFix needs to change your registry, please run from an elevated (admin) command prompt");
            }


            return 0;
        }

        private static void ShowFuslog()
        {
            var fuslog = new RegistryFusLog();
            if (fuslog.ExistKey())
            {
                var state = fuslog.ReadFuslog() == 1 ? "enabled." : "disabled.";
                Console.WriteLine($"Fuslog is {state}");
                Console.WriteLine($"Current fuslog logfolder is: {fuslog.ReadLogFolder()}");
            }
            else
            {
                Console.WriteLine("No fuslog key in registry, thus no fuslog enabled");
            }
        }

        private static void ShowDump()
        {
            var dump = new RegistryDump();
            if (dump.ExistKey())
            {
                var state = dump.IsDumpEnabled? "enabled." : "disabled.";
                Console.WriteLine($"Dump is {state}");
                Console.WriteLine($"Current dumpfolder is: {dump.ReadDumpFolder()}");
            }
            else
            {
                Console.WriteLine("No dump key in registry, thus no dumps enabled");
            }
        }
    }
}
