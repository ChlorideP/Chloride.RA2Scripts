namespace Chloride.RA2.IniExt.Scripts;
public static class Common
{
    public static void Main()
    {
        var config = InitWithInis("config.ini");
        // new ScriptName(config).Run();
        new ArrayValueSort(config).Run();
    }

    public static IniDoc InitWithInis(params string[] paths)
    {
        var ret = new IniDoc();
        ret.Deserialize(
            paths.Select(i => new FileInfo(i))
            .Where(i => i.Exists).ToArray());
        return ret;
    }

    public static string? GetValue(this IniSection sect, string key)
    {
        try { return sect[key].ToString(); }
        catch { return null; }
    }
}