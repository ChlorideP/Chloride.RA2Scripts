# Chloride.RA2.IniExt
Ares INI 结构的 C# 实现。

```Ini
[Section]:[Super];Summary
+= NewItem
Key=Value;Comment
```
- IniEntry  
    ini 中非节声明行由键、值、注释三个组件构成。
    - (string) Key  
        Key是直接决定非节行是否为键值对的重要部分，原则上只有空不空串的区分，并不可null。  
        传统的 ini 键不许重复（要么就读最后一个重复键的值），Ares 引入的`+=`是个例外。
    - (IniValue | string) Value  
        直观地说，红红ini里可以有`string` `int` `float(double)` `bool` `Array`五种类型值。  
        而从格式和读取的角度，其实ini并没有类型。我们读的一直是`string`。

        因此，在**小节的键值访问**中使用`IniValue`以便于转型；其余情形访问 Value 则为`string(?)`。  
        以 `doc[section, key] => string?` 为例：查无此人为 null，找到空的则为 string.Empty。

        在注册表中，值重复的取其中第一个。
    - (string?) Comment  
        原则上`;`起头的部分就算是注释，分为无注释(null)、空注释(string.Empty)、普通注释三种情形。
        
        考虑到`boy_next_door         ; deep♂ dark♂ fantasy`这种间隔特别长的注释需要在 Git 里维持间隔、减少diff，  
        实际存的注释是挖掉 Value 后的剩余字符串。

    三个组件均空，该行就空；Key为空Comment不空，就是纯注释；Key不空就是标准键值对了。

- IEnumerable[IniEntry] -> IniSection  
    节是`IniEntry`的容器，节声明`[Section]`底下的`IniEntry`均属于该节。  
    节的实例不应与某个文档强绑定，赋值给其他 ini 理应能够正常食用。  
    除此之外节自身也有属性：
    - (string) Name  
    节的名称
    - （可选）(IniSection?) Parent  
    Ares 引入的，类似子类继承自基类，子节自然有基节。  
    由于 C# 的强类型约束，Parent 的情形不再像 PyMapRA2 那样由类型决定。
        - `null`表示空节；
        - `IniSection.Count == 0`表示确实有继承，但是当前文档找不到；
        - 非空 IniSection 表示有继承也找到了。
    - (string?) Summary  
    节的注释

- IEnumerable[IniSection] -> IniDoc  
    把上述数据汇集起来，就是完整的 ini 了。

    考虑到一个完整的 ini 实例可能不止需要读一个 ini 文件（Ares 的 ini 拆分功能），  
    `IniDoc`只实现必要的 API，不再记录文件信息（编码、路径等等）。
