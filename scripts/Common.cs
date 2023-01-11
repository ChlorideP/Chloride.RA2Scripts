namespace Chloride.RA2.IniExt.Scripts;
public static class Common
{
    public static void Main()
    {
        var config = InitIni("config.ini");
        // new ScriptName(config).Run();
        new ArrayValueSort(config).Run();
    }

    public static IniDoc InitIni(string path)
    {
        IniDoc ret = new();
        FileInfo fi = new(path);
        ret.Deserialize(fi);
        // 只读一层捏。不会真有人套娃 [#include] 罢？
        ret.Deserialize(
            ret.GetTypeList("#include")
            .Select(i => new FileInfo(Path.Combine(fi.DirectoryName!, i)))
            .Where(i => i.Exists)
            .ToArray());
        return ret;
    }

    public static string? GetValue(this IniSection sect, string key)
    {
        try { return sect[key].ToString(); }
        catch { return null; }
    }
}