using System;
using IFix;

public class RegistryFusLog : RegistryLocalMachine
{
    private const string logfolder = @"\IFix\Fuslog";
    private const string subkeyname = @"SOFTWARE\Microsoft\Fusion";
    private const string fullsubkeyname = @"HKEY_LOCAL_MACHINE\"+subkeyname;

    private const string EnableLog = nameof(EnableLog);
    private const string ForceLog = nameof(ForceLog);
    private const string LogFailures = nameof(LogFailures);
    private const string LogResourceBinds = nameof(LogResourceBinds);
    private const string LogFolder = nameof(LogFolder);
    public RegistryFusLog() : base(fullsubkeyname)
    {
    }

    public void Enable()
    {
        if (!ExistKey() || !Exist(LogFolder))
        {
            Write(LogFolder,$"{Environment.SpecialFolder.LocalApplicationData}{logfolder}");
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
        Write(LogFolder, path);
    }

    public int ReadFuslog()
    {
        return Read<int>(EnableLog);
    }

    public string ReadLogFolder()
    {
        return Read<string>(LogFolder);
    }

}