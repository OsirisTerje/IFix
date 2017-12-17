using System;
using IFix;

namespace IFix
{
    public class RegistryFusLog : RegistryLocalMachine
    {
        private const string Logfolder = @"\IFix\Fuslog";
        private const string Subkeyname = @"SOFTWARE\Microsoft\Fusion";
        private const string EnableLog = nameof(EnableLog);
        private const string ForceLog = nameof(ForceLog);
        private const string LogFailures = nameof(LogFailures);
        private const string LogResourceBinds = nameof(LogResourceBinds);
        private const string LogPath = nameof(LogPath);

        public RegistryFusLog() : base(Subkeyname)
        {
        }

        public void Enable()
        {
            if (!ExistKey() || !Exist(LogPath))
            {
                Write(LogPath, $"{Environment.SpecialFolder.LocalApplicationData}{Logfolder}");
            }
            Write(EnableLog, 1);
            Write(ForceLog, 1);
            Write(LogFailures, 1);
            Write(LogResourceBinds, 1);

        }

        public void Disable()
        {
            Write(EnableLog, 0);
            Write(ForceLog, 0);
            Write(LogFailures, 0);
            Write(LogResourceBinds, 0);
        }

        public void SetLogFolder(string path)
        {
            Write(LogPath, path);
        }

        public int ReadFuslog()
        {
            return Read<int>(EnableLog);
        }

        public string ReadLogFolder()
        {
            return Read<string>(LogPath);
        }

    }
}