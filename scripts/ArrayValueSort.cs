namespace Chloride.RA2.IniExt.Scripts;
public class ArrayValueSort : IScript<ArrayValueSort.Sorter>
{
    // 原初的例子就是 XNA 客户端的 MPMaps。
    // 里面的地图适用于不同的模式（序列已知），
    // 为了便于管理需要把模式按序排列。

    public class Sorter : IConfig, IComparer<string>
    {
        /* config.ini
        [Settings]
        Target = ;ini路径
        IterTypeList =
        KeyToSort =
        SequenceOrder = ;单行有序值表，v1,v2这种不用再预处理之后写在下边了。
        
        [Order]
        lesser = ssks
        greater = ddtms
        ;反正以行号顺序为准，靠前排上面。
        */
        public string? Target;
        public string? TypeList;
        public string? Key;

        private readonly List<string> _order;

        public Sorter(IEnumerable<string> order)
        {
            _order = order.ToList();
        }

        public int Compare(string? x, string? y) =>
            _order.IndexOf(x ?? string.Empty) - _order.IndexOf(y ?? string.Empty);
    }

    public Sorter Config { get; }

    public ArrayValueSort(IniDoc config)
    {
        Config = new(
            config["Settings", "SequenceOrder"] is null
            ? config.GetTypeList("Order")
            : config["Settings"]["SequenceOrder"].Split())
        {
            Target = config["Settings", "Target"],
            TypeList = config["Settings", "IterTypeList"],
            Key = config["Settings", "KeyToSort"]
        };
    }

    public void Run()
    {
        var docTarget = Common.InitWithInis(Config.Target!);

        foreach (var i in docTarget[Config.TypeList!].Values)
        {
            if (!docTarget.Contains(i, out IniSection? isect))
                continue;
            try
            {
                var gamemodes = isect![Config.Key!].Split().ToList();
                gamemodes.Sort(Config);
                isect![Config.Key!] = IniValue.Join(gamemodes);
            }
            catch (FormatException) { continue; }
            catch { throw; }
        }

        docTarget.Serialize(new(Config.Target!));
    }
}
