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

    /*
    Without considering git, INI should be very easy though.

    However, sometimes we may want to batch something, with ini structure still kept.
    Otherwise every thing goes a mess, which is unable to realize what we have done.

    That's why I implement this library. The more to consider, the lower efficiency.
    */
    public static void Deserialize(IniDoc doc, TextReader tr)
    {
        IniSection? self = doc.Default;

        while (tr.Peek() > 0)
        {
            var line = tr.ReadLine();
            var strip = line?.Trim();

            // "; Comment"
            // "\n"
            // "                ; Comment"
            if (string.IsNullOrEmpty(strip) || strip.StartsWith(';'))
            {
                self!.Add(line);
                continue;
            }

            // [S]
            // [S]:[P]
            // [S]:[P]:[GP] // may be invalid in game, just consider Parent right here.
            // [S]      ;Comment
            // [S]:[P]  ;Comment
            // Hint: wouldn't consider the white-spaces in section declaration comments - they're not the most.
            if (strip.StartsWith('['))
            {
                IniSection? super = null;

                var sect = strip.Split(';', 2);
                var curSect = sect.First().Split(':').Select(i => i.Trim()[1..^1]).ToArray();
                var curDesc = sect.ElementAtOrDefault(1);

                if (doc.Contains(curSect[0], out self))
                    continue;

                if (curSect.Length > 1)
                {
                    _ = doc.Contains(curSect[1], out super);
                    super ??= new(curSect[1]);
                }

                self = new(curSect[0], curDesc, super);
                doc.Add(self);
            }

            // k=v
            // k2=v2   ; desc
            // k3;=v3   // invalid
            // k4=;v4
            // k5=     ; v5
            else if (strip.Contains('='))
            {
                var pair = strip.Split('=', 2);
                pair[0] = pair[0].Trim();

                if (pair[0].Contains(';'))
                    continue;

                var value = pair[1].Split(';', 2)[0];
                StringBuilder desc = new(pair[1]);

                IniEntry entry = new(
                    key: pair[0] == "+" ? $"+{doc.Diff++}" : pair[0],
                    val: value.Trim()
                );
                entry.Comment = string.IsNullOrEmpty(entry.Value)
                    ? desc.ToString()
                    : desc.Replace(entry.Value, string.Empty, 0, value.Length).ToString();

                self!.Add(entry);
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
