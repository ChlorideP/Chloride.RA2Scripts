namespace Chloride.RA2.IniExt.Scripts
{
    public static class GroupInherit
    {
        /* 这里 config.ini 大致如下：
         * 
         * ; 待操作ini
         * [CaptureList]
         * 0 = subRules ;subArt
         * ssks = subRB ;subAB
         * 
         * ; 表中项“0”待替换内容
         * ; 比如有一个换皮建筑 A 打算搞成继承原建筑 B
         * ; 就 A = B
         * [Arguments0]
         * ToReplace = ToMerge
         * 
         * ; Arguments 后面跟着 CaptureList 中的左值
         * [Argumentsssks]
         * ToReplace = ToMerge
         */

        /// <summary>
        /// 用给定的 config、全局 rules 和 art 跑合并脚本。
        /// </summary>
        /// <param name="config"></param>

        public static void Run(INI config)
        {
            var (rules, art) = Common.LoadGlobals(config);

            foreach (var i in config["CaptureList"])
            {
                if (!i.IsPair)
                    continue;
                Console.WriteLine($"Processing {i.Key} - {i.Value} {i.Comment}");

                INI iRules = new(i.Value!.Replace('\"', ' ').Trim());
                INI? iArt = string.IsNullOrEmpty(i.Comment) || string.IsNullOrWhiteSpace(i.Comment)
                    ? null
                    : new(i.Comment[(i.Comment.IndexOf(';') + 1)..].Replace('\"', ' ').Trim());

                foreach (var j in config[$"Arguments{i.Key}"].Items)
                {
                    if (!(iRules.ini.Contains(j.Key, out IniSection? jDst) && rules.ini.Contains(j.Value!, out IniSection? jSrc)))
                        continue;
                    Console.WriteLine($"{j.Key} -> {j.Value}");

                    iRules[j.Key] = ExportUnique(jDst!, jSrc!);
                    if (iArt != null)
                    {
                        var dstimg = jDst!["Image"] ?? j.Key;
                        var srcimg = jSrc!["Image"] ?? (string)j.Value!;
                        if (dstimg != srcimg && iArt.ini.Contains(dstimg, out IniSection? aDst) && art.ini.Contains(srcimg, out IniSection? aSrc))
                            iArt[j.Key] = ExportUnique(aDst!, aSrc!);
                    }
                }
                iRules.Save();
                iArt?.Save();
                Console.WriteLine("Done.\n");
            }
        }

        public static IniSection ExportUnique(IniSection dst, IniSection src)
        {
            IniSection ret = new(dst.Name, dst.Summary, src);
            ret.AddRange(dst.Where(i => !i.IsPair || !src.Contains(i.Key, out IniEntry isrc) || i.Value != isrc.Value));
            ret.AddRange(src.Where(i => i.IsPair && !dst.Contains(i.Key, out _)).Select(i =>
            {
                i.Value = null;
                return i;
            }));
            return ret;
        }
    }
}
