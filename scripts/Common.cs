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

    /// <summary>
    /// 从某一个ini文件开始，先序遍历子 ini 树。
    /// </summary>
    /// <param name="root">起始 ini 文件路径。</param>
    /// <returns></returns>
    public static List<FileInfo> TryGetIncludes(string root)
    {
        T Pop<T>(List<T> lst, int index = -1) {
            var ret = lst.Last();
            lst.RemoveAt(index >= 0 ? index : lst.Count + index);
            return ret;
        }

        List<FileInfo> ret = new();
        List<string> stack = new() { root };

        var rootdir = Path.GetDirectoryName(root);

        while (stack.Count > 0) {
            FileInfo ini = new(Pop(stack));
            IniDoc doc = new();

            try {
                doc.Deserialize(ini);
            } catch (Exception e) {
                Console.WriteLine($"!) Unable to open {ini.Name}: {e.ToString().Split(':').First()}");
                continue;
            }

            ret.Add(ini);

            if (doc.Contains("#include", out IniSection? inc))
                stack.AddRange(inc!.Values.Select(i => Path.Combine(rootdir!, i)).Reverse());
        }

        return ret;
    }
}