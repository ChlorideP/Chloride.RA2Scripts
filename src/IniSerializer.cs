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
        IniSection? self = doc.Default, super = null;

        while (tr.Peek() > 0)
        {
            var line = tr.ReadLine();
            var strip = line?.Trim();

            if (string.IsNullOrEmpty(strip))
            {
                self!.Add();
                continue;
            }

            switch (strip.First())
            {
                case '[':
                    var sect = strip.Split(';', 2);
                    var curSect = sect.First().Split(':').Select(i => i.Trim()[1..^1]).ToArray();
                    var curDesc = sect.ElementAtOrDefault(1);

                    if (!doc.Contains(curSect[0], out self))
                    {
                        if (curSect.Length > 1)
                        {
                            _ = doc.Contains(curSect[1], out super);
                            super = super ?? new(curSect[1]);
                        }
                        else super = null;

                        self = new(curSect[0], curDesc, super);
                        doc.Add(self);
                    }
                    break;
                case ';':
                    self!.Add(line);
                    break;
                default:
                    if (strip.Contains('='))
                    {
                        var pair = strip.Split('=', 2);
                        pair[0] = pair[0].Trim();

                        if (!pair[0].Contains(';'))
                        {
                            var key = pair[0] == "+" ? $"+{doc.Diff++}" : pair[0];
                            var val = pair[1].Split(';', 2)[0];
                            var value = val.Trim();
                            var desc = string.IsNullOrEmpty(value) ? pair[1] : new StringBuilder(pair[1])
                                .Replace(value.ToString(), string.Empty, 0, val.Length)
                                .ToString();

                            self!.Add(key, value, desc);
                        }
                    }
                    break;
            }
        }

        tr.Close();
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
                tw.WriteLine(j.ToString(pairing));
        }
    }
}
