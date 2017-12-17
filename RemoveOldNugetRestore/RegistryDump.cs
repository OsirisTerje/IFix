using IFix;
using Microsoft.Win32;

public class RegistryDump : RegistryLocalMachine
{

    private const string parentkey = @"SOFTWARE\Microsoft\Windows\Windows Error Reporting\";
    private const string LocalDumps = nameof(LocalDumps);
    private const string DumpType = nameof(DumpType);
    private const string DumpFolder = nameof(DumpFolder);
    public RegistryDump(bool create = false) : base($"{parentkey}{LocalDumps}")
    {
        if (create)
            EnsureExist();
    }


    private void EnsureExist()
    {
        if (ExistKey())
            return;
        var pkey = Registry.LocalMachine.OpenSubKey(parentkey);
        pkey.CreateSubKey(LocalDumps);
    }


    public bool ExistDumpType => Exist(DumpType);
    public bool ExistDumpFolder => Exist(DumpFolder);


    public void EnableDump()
    {
        Write(DumpType, 2);
    }

    public void DisableDump()
    {
        Write(DumpType, 0);
    }

    public void SetDumpFolder(string path)
    {
        Write(DumpFolder, path);
    }

    public int ReadDump()
    {
        return Read<int>(DumpType);
    }

    public bool IsDumpEnabled => ReadDump() != 0;


    public string ReadDumpFolder()
    {
        return Read<string>(DumpFolder);
    }

}