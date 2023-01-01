namespace Chloride.RA2.IniExt.Scripts
{
    public class ArrayValueSort : IComparer<string>
    {
        private readonly List<string> order;

        public ArrayValueSort(IniSection order)
        {
            this.order = order.Values.ToList();
        }

        /* config.ini 示例：
         * [Settings]
         * target = ini.FullName
         * IterTypeList =
         * KeyToSort =
         * 
         * [Order]
         * lesser = ssks
         * greater = ddtms
         * 
         * 如果是类似csv那种单行有序 Value[]，
         * 可以先用 relertpy.ccini 结合 Python 的 enumerator 处理成字典。
         * 反正以行号顺序为准，靠前排上面。
         */

        public void Run(INI config)
        {
            var target = new INI(config["Settings", "Target"]);
            var type = config["Settings", "IterTypeList"];
            var key = config["Settings", "KeyToSort"];

            foreach (var i in target[type].Values)
            {
                if (!target.ini.Contains(i, out IniSection? isect))
                    continue;

                try
                {
                    var gamemodes = isect![key]!.Split(',').ToList();
                    gamemodes.Sort(this);
                    isect![key] = string.Join(',', gamemodes);
                }
                catch (FormatException) { continue; }
                catch { throw; }

            }

            target.Save();
        }

        public int Compare(string? x, string? y) =>
            order.IndexOf(x ?? string.Empty) - order.IndexOf(y ?? string.Empty);
    }
}
