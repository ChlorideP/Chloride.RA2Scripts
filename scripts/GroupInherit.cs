namespace Chloride.RA2.IniExt.Scripts;
public class GroupInherit : IScript<GroupInherit.Capturer>
{
    // 原初的例子就是。。把换皮改写成继承
    // 毕竟以前换皮都是复制了再改嘛。
    // 知道了这个功能或许就想给自己写过的x山简并一下。

    // PS: 继承得基于 Ares 藕。再往前的平台（包括原版）是不得行的。

    public Capturer Config { get; set; }
    public IEnumerable<Capturer> Configs;
    public IniDoc GlobalRules;
    public IniDoc GlobalArt;

    public class Capturer : IConfig
    {
        /* config.ini
        ; 全局ini，对照组
        [Globals]
        rules =
        art =

        ; 待操作ini
        [CaptureList]
        0 = subRules ;subArt
        ssks = subRB ;subAB
        
        ; 表中项“0”待替换内容
        ; 比如有一个换皮建筑 A 打算搞成继承原建筑 B
        ; 就 A = B
        [Arguments0]
        ToReplace = ToMerge
        
        ; Arguments 后面跟着 CaptureList 中的左值
        [Argumentsssks]
        ToReplace = ToMerge
        */

        internal string rules;
        internal string? art;
        public IniDoc Rules => Common.InitIni(rules);
        public IniDoc? Art => art == null ? null : Common.InitIni(art);
        /// <summary>
        /// Key为待合并项，Value为对应母本
        /// </summary>
        public Dictionary<string, IniValue> CaptureSheet;

        public Capturer(string pRules, Dictionary<string, IniValue> sheet, string? pArt = null)
        {
            rules = pRules;
            art = pArt;
            CaptureSheet = sheet;
        }

        public static IniSection Capture(IniSection dst, IniSection src)
        {
            IniSection ret = new(dst.Name, src, dst.Summary);
            ret.AddRange(dst.Where(i => !i.IsPair || !src.Contains(i.Key, out IniEntry isrc) || i.Value != isrc.Value));
            ret.AddRange(src.Where(i => i.IsPair && !dst.Contains(i.Key, out _)).Select(i =>
            {
                i.Value = string.Empty;
                return i;
            }));
            return ret;
        }
    }

    public GroupInherit(IniDoc config)
    {
        GlobalRules = Common.InitIni(config["Globals", "rules"]!.Replace('\"', ' ').Trim());
        GlobalArt = Common.InitIni(config["Globals", "art"]!.Replace('\"', ' ').Trim());
        Configs = config["CaptureList"].Where(i => i.IsPair).Select(i => new Capturer(
            i.Value.Replace('\"', ' ').Trim(),
            config[$"Arguments{i.Key}"].Items,
            string.IsNullOrEmpty(i.Comment) || string.IsNullOrWhiteSpace(i.Comment)
                ? null : new(i.Comment[(i.Comment.IndexOf(';') + 1)..].Replace('\"', ' ').Trim())));
        Config = Configs.First();
    }

    public void Run()
    {
        var iter = Configs.GetEnumerator();

        while (iter.MoveNext())
        {
            Config = iter.Current;
            Console.WriteLine($"Processing {Config.rules} {Config.art}");

            var iRules = Config.Rules;
            var iArt = Config.Art;

            foreach (var j in Config.CaptureSheet)
            {
                if (!(iRules.Contains(j.Key, out IniSection? jDst) && GlobalRules.Contains(j.Value.ToString(), out IniSection? jSrc)))
                    continue;
                Console.WriteLine($"{j.Key} -> {j.Value}");

                iRules[j.Key] = Capturer.Capture(jDst!, jSrc!);
                if (iArt != null)
                {
                    var dstimg = jDst!.GetValue("Image") ?? j.Key;
                    var srcimg = jSrc!.GetValue("Image") ?? j.Value.ToString();
                    if (dstimg != srcimg && iArt.Contains(dstimg, out IniSection? aDst) && GlobalArt.Contains(srcimg, out IniSection? aSrc))
                        iArt[j.Key] = Capturer.Capture(aDst!, aSrc!);
                }
            }
            iRules.Serialize(new FileInfo(Config.rules));
            iArt?.Serialize(new FileInfo(Config.art!));
            Console.WriteLine("Done.\n");
        }
    }
}
