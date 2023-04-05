using System.Text;

namespace Chloride.RA2.IniExt;
public static class IniSerializer
{
    static IniSerializer() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

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

            if (string.IsNullOrEmpty(strip) || strip.StartsWith(';'))
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

        if (pair[0].Contains(';'))
            return;

        var val = new StringBuilder(pair[1]);
        var value = pair[1].Split(';', 2)[0];

        self.Add(
            pair[0] == "+" ? $"+{doc.Diff++}" : pair[0],
            value.Trim(),
            string.IsNullOrEmpty(value)
                ? val.ToString()
                : val.Replace(value.Trim(), string.Empty, 0, value.Length).ToString()
        );
    }

    public static void Serialize(this IniDoc doc, FileInfo iniFile, string encoding = "utf-8", string pairing = "=")
    {
        using var fs = iniFile.Open(FileMode.Create, FileAccess.Write, FileShare.Read);
        using StreamWriter sw = new(fs, Encoding.GetEncoding(encoding)); // DON'T forget gb2312, esp. MAP file from FA2.

        Serialize(doc, sw, pairing);
    }

    public static void Serialize(IniDoc doc, TextWriter tw, string pairing = "=")
    {
        foreach (var i in doc.Default)
            tw.WriteLine(i.ToString());

        foreach (var i in doc)
        {
            tw.WriteLine(i.ToString());
            foreach (var j in i)
            {
                if (!j.Key.StartsWith(';'))
                    tw.Write($"{j.Key}{pairing}{j.Value.Value}");
                tw.WriteLine(j.Value.Comment);
            }
        }
    }
}
