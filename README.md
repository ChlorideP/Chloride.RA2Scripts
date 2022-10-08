# Chloride.CCiniExt
此类库用于处理含注释的 Ares INI，同时是[PyMapRA2](https://github.com/Chloride1/PyMapRA2)项目中`relertpy.ccini`模块的扩展（原本只读键值对嘛）。  
至于为什么是 C# 类库嘛，只能说 PowerShell 某种意义上比 Python 更自由一点咯。

### 为何是 Ares INI？

Ares 是红色警戒2的扩展平台，它在 ini 方面新增了一些约定：
- 允许嵌套。

你可以拆出许多子 ini，再通过`[#include]`整合到一起。  
但与 C 不同的是，子 ini 会在父文件读完之后再读取。读取过程递归进行。
```Ini
[#include]
0=cute_cat_white_cat.ini
1=SubDirectory\ssks.ini
```

- 允许继承

可以像`[Child]:[Parent]`这样继承一个 Section。

在 Ares 里实质上是做了内存拷贝，而我这边则是链表的思路。无论如何，父小节必须出现在子节之前。  
嗯？你说多继承？dbq，Ares 都不支持，我干嘛要做（

- 注册表 Key 允许丢弃

在红警2里，某些类型的实例名称会保存在 ini 里，称为`Type List`，modder习惯叫“注册表”。  
Ares 提供了类似丢弃`_`的“注册”方式（即`+= ABC`），以方便追加。

`+=`键值对会在我这里转换为`+%d`的形式，保存出来大概会看到`+0=` `+1=`之类的。  
处理不同子ini时可能要注意，注册表的`+=`项有没有因此重复。

### 类 & 方法声明
```c#
namespace Chloride.CCiniExt {
    // 父类有的我就不写了= =
    public class Ini : IEnumerable<IniSection> {
        // 初始化为空 ini
        public Ini();
        // 此法添加的 IniSection 会覆盖已有的
        public IniSection this[string sec] { get; set; }

        public bool Contains(string section, out IniSection? result);
        public bool ContainsKey(string section, string key);

        // 已有同名小节不再添加
        public void AddNew(string sect);
        public void Remove(string sect);
        // _old 不存在时不再试图更名
        public void Rename(string _old, string _new);

        public IniValue GetValue(string sect, string key);
        public string[] GetTypeList(string sect);
        public void SetValue(string sect, string key, IniValue value);

        public void Clear();
        // 默认按字母序重排小节顺序
        public void Sort();
        // 用自定的比较方式重排小节顺序
        public void Sort(IComparer<IniSection>? condExpr);

        /* [#include] 最好是自己实现读取。因为不同mod的情况不同。*/
        // 读 ini，自动扫描编码
        public void Load(FileInfo[] paths);
        // 读 ini，手动指定编码
        public void Load(FileInfo[] paths, string encoding = "utf-8");
        // 保存 ini 到指定路径，默认用 UTF-8 编码，键值间的等号不带空格。
        public virtual void Save(FileInfo dest, string codec = "utf-8", bool space = false);
    }

    public class CCIni : Ini {
        public readonly string Encoding; // 初始化时的编码，由encoding参数指定

        public CCIni(FileInfo ini, string encoding = "utf-8");

        // 默认路径为初始化读的 ini 路径，默认编码参见 Encoding。
        public override void Save(FileInfo? dest = null, string? codec = null, bool space = false);
    }

    public class IniSection : IEnumerable<IniItem>, IComparable<IniSection> {
        public string Name; // 节名称
        public IniSection? Parent; // 父小节，当前文件没有但是确实继承了用空实例，实在没有用null.
        public string? Description; // 节注释，就是挂在 [Section] 右边的注释。

        public IniSection(string section, IniSection? super = null, string? desc = null);
        public IniSection(string section, IDictionary<string, IniValue> source);
        public IniValue this[int line] { get; set; }
        public IniValue this[string key] { get; set; }

        public int Count { get; }

        public void Insert(int line, IniItem item);
        // 插入操作不会检查 Key 唯一性
        public void Insert(int line, string key, IniValue value);

        public bool Remove(IniItem item);
        // recurse: 子节不直接 Remove 父节的 Key，而是置为空值（懒得搞默认值表）
        public bool Remove(string key, bool recurse = false);
        public void RemoveAt(int line);

        public void Add(IniItem item);
        public void Add(string key, IniValue value, string? desc = null);
        public void AddRange(IEnumerable<IniItem> sequence);
        public void AddRange(IDictionary<string, IniValue> source);

        public void Clear();

        public bool Contains(string key, out IniItem item);

        // 找不到就返回指定的默认值
        public string GetValue(string key, string fallback = "");

        public IEnumerable<string> Keys { get; }
        public IEnumerable<IniValue> Values { get; }
        public Dictionary<string, IniValue> Items { get; }
    }

    public class IniItem {
        public string Key;
        public IniValue Value;
        public string? Comment;

        public bool IsEmptyLine { get; }
        public bool IsPair { get; }

        // 初始化小节里的空行或纯注释行（头部原则上不修改）
        public IniItem(string? desc = null);
        // 初始化小节里的键值对
        public IniItem(string key, IniValue value, string? desc = null);
        public IniItem(KeyValuePair<string, IniValue> pair);
        // 允许自定赋值号（其实保存的时候只有"A = B"和"A=B"两种）
        public string ToString(string delimiterPairing);
    }

    public struct IniValue {
        public IniValue();
        public IniValue(string? s);

        public string[] TrySplit();
        public bool IsNull { get; }
    }
}
```
