using System.Text;

namespace Chloride.RA2Scripts.Formats;
public static class IniSerializer
{
    static IniSerializer() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    /// <summary>
    /// INI tree Pre-order traversal.
    /// </summary>
    /// <returns>A sequence of ini paths. Same as how <c>Ares.DLL</c> reads.</returns>
    public static List<FileInfo> TryGetIncludes(string root)
    {
        static T Pop<T>(List<T> lst, int index = -1)
        {
            index = index >= 0 ? index : lst.Count + index;
            var ret = lst[index];
            lst.RemoveAt(index);
            return ret;
        }

        List<FileInfo> ret = new();
        List<string> stack = new() { root };

        var rootdir = Path.GetDirectoryName(root);

        while (stack.Count > 0)
        {
            FileInfo ini = new(Pop(stack));
            IniDoc doc = new();

            try
            {
                doc.Deserialize(ini);
            }
            catch (Exception e)
            {
                Console.WriteLine($"!) Unable to open {ini.Name}: {e.ToString().Split(':').First()}");
                continue;
            }

            ret.Add(ini);

            if (doc.Contains("#include", out IniSection? inc))
                stack.AddRange(inc!.Values.Select(i => Path.Combine(rootdir!, i)).Reverse());
        }

        return ret;
    }

    public static void Deserialize(this IniDoc doc, params FileInfo[] inis)
    {
        foreach (var ini in inis)
        {
            if (!ini.Exists)
                throw new FileNotFoundException(ini.FullName);
            using var fs = ini.OpenRead();
            using var sr = new StreamReader(fs);
            Deserialize(doc, sr);
        }
    }

    public static void Deserialize(IniDoc doc, TextReader tr)
    {
        IniSection? self = doc.Default;

        while (tr.Peek() > 0)
        {
            // there must be faster ways to read ini chars while its structure still be kept...
            // though I didn't figure out yet.

            var line = tr.ReadLine();
            var strip = line?.Trim();

            if (string.IsNullOrEmpty(strip))
                continue;

            if (strip.StartsWith(';'))
                self!.Add(
                    $";{Guid.NewGuid().ToString()[0..8]}",
                    new() { Comment = line });
            else if (strip.StartsWith('['))
                self = doc.ParseSection(strip);
            else if (strip.Contains('='))
                doc.ParseEntry(self, strip);
        }

        tr.Close();
    }

    private static IniSection ParseSection(this IniDoc doc, string sectionLine)
    {
        IniSection? super = null;

        var sect = sectionLine.Split(';', 2);
        var curSect = sect.First().Split(':').Select(i => i.Trim()[1..^1]).ToArray();
        var curDesc = sect.ElementAtOrDefault(1);

        if (!doc.Contains(curSect[0], out IniSection? self))
        {
            if (curSect.Length > 1)
            {
                _ = doc.Contains(curSect[1], out super);
                super ??= new(curSect[1]);
            }

            self = new(curSect[0], super, curDesc);
            doc.Add(self);
        }

        return self!;
    }

    private static void ParseEntry(this IniDoc doc, IniSection self, string entryLine)
    {
        var pair = entryLine.Split('=', 2);
        pair[0] = pair[0].Trim();
        pair[1] = pair[1].Trim();

        if (pair[0].Contains(';'))
            return;

        var val = new StringBuilder(pair[1]);
        var value = pair[1].Split(';', 2)[0];

        self.Add(
            pair[0] == "+" ? $"+{doc.Diff++}" : pair[0],
            value = value.Trim(),
            string.IsNullOrEmpty(value)
                ? val.ToString()
                : val.Replace(value, string.Empty, 0, value.Length).ToString()
        );
    }

    public static void Serialize(this IniDoc doc, FileInfo iniFile, string encoding = "utf-8", string pairing = "=", int blanklines = 1)
    {
        using var fs = iniFile.Open(FileMode.Create, FileAccess.Write, FileShare.Read);
        using StreamWriter sw = new(fs, Encoding.GetEncoding(encoding)); // DON'T forget gb2312, esp. MAP file from FA2.

        Serialize(doc, sw, pairing, blanklines);
    }

    public static void Serialize(IniDoc doc, TextWriter tw, string pairing = "=", int blanklines = 1)
    {
        foreach (var i in doc.Default)
            tw.WriteLine(i.Value.Comment);

        foreach (var i in doc)
        {
            tw.WriteLine(i.ToString());
            foreach (var j in i)
            {
                if (!j.Key.StartsWith(';'))
                    tw.Write($"{j.Key}{pairing}{j.Value.Value}");
                tw.WriteLine(j.Value.Comment);
            }
            tw.Write(new string('\n', blanklines));
        }
    }
}
