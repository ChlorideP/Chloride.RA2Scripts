namespace Chloride.RA2.IniExt.Scripts
{
    public static class Common
    {
        public static void Main()
        {
            // config 根据待跑脚本自拟。
            var config = new INI("config.ini");

            GroupInherit.Run(config);
        }

        public static (INI rules, INI art) LoadGlobals(INI config)
        {
            var rules = new INI(config["Globals", "rules"].Replace('\"', ' ').Trim());
            rules.Load(rules.ini.GetTypeList("#include").Select(i => new FileInfo(Path.Combine(rules.file.DirectoryName, i.Trim()))).ToArray());

            var art = new INI(config["Globals", "art"].ToString().Replace('\"', ' ').Trim());
            art.Load(art.ini.GetTypeList("#include").Select(i => new FileInfo(Path.Combine(art.file.DirectoryName, i.Trim()))).ToArray());

            return (rules, art);
        }
    }
}