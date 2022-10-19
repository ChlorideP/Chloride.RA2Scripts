# Chloride.CCiniExt
此类库用于处理带注释 Ares INI，同时是[PyMapRA2](https://github.com/Chloride1/PyMapRA2)项目中`relertpy.ccini`模块的扩展（原本只读键值对嘛）。

### 为何是 Ares INI？

Ares 是《红色警戒2：尤里的复仇》的扩展平台，它在 ini 方面新增了一些约定：
- 允许[嵌套](https://ares-developers.github.io/Ares-docs/new/misc/include.html) `[#include]`  
	可以拆出许多子 ini，再通过`[#include]`整合到一起。  
	但与 C 不同的是，子 ini 会在父文件读完之后再读取。读取过程递归进行。

- 允许继承 `[Child]:[Parent]`  
	在 Ares 里实质上是做了内存拷贝，因此父小节必须出现在子节之前。  
	此外也不支持`[Child]:[Parent]:[GrandParent]`这样的多级继承。

- 允许丢弃追加 `+= NewItem`  
	在红警2里，某些对象类型会保存在 ini 里，并挂靠在 Type List（习惯上我们称之为注册表）中。  
	键在注册表里只作为占位符，游戏直接用值表初始化。Ares 则提供了`+=`的语法糖，使追加更简便。

	`+=`会在我这里转换为`+%d`，保存出来大概是`+0` `+1`之类的。  
	处理不同子ini时可能要注意，注册表的`+=`项有没有因此重复。

### 简单的用例
下面的示例使用了 C# 9.0 的船新特性“顶级语句”，作用是重排动画注册表。
```C#
using Chloride.CCiniExt;

IniDoc InitIni(FileInfo ini)
{
	IniDoc ret = new();
	ret.Deserialize(ini);
	return ret;
}

int idx = 0;

var rulesFile = new FileInfo(".\\rulesmd.ini");
var rules = InitIni(rulesFile);
rules["Animations"] = new(
	"Animations",
	rules.GetTypeList("Animations").Select(i => new IniItem(idx++, i))
	);
rules.Serialize(rulesFile, "gb2312"); // buxv "ansi".
```

### 复杂的用例
参见`Chloride.CCiniExt.Scripts.csproj`（位于`scripts`目录）。

### 其他用法
作为模块导入 PowerShell Core 也是可以的。**注意不是 Windows PowerShell！**  
只是读写文件可能有一些麻烦：
- PowerShell 不支持扩展方法，需要显式调用。
- PowerShell 不支持默认参数，方法里的默认参数必须全部（或全不）指定。
```PowerShell
using namespace Chloride.CCiniExt

$eg = [IniDoc]::new()
[IniSerializer]::Deserialize($eg, ".\ssks.ini")
# Serialize(this IniDoc doc,
#           FileInfo iniFile,
#           string encoding = "utf-8",
#           string pairing = "=")
[IniSerializer]::Serialize($eg, ".\ssks.ini")
[IniSerializer]::Serialize($eg, ".\ssks_ansi.ini", "gb2312", "=")
[IniSerializer]::Serialize($eg, ".\ssks_with_space.ini", "utf-8", " = ")
```