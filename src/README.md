# Chloride.CCiniExt
Ares INI 结构的 C# 实现。

```Ini
[Section]:[Super];Summary
+= NewItem
Key=Value;Comment
```
- (string) Key  
    Key是直接决定非节行是否为键值对的重要部分，原则上只有空不空串的区分，并不可null。  
    传统的 ini 键不许重复（要么就读最后一个重复键的值），Ares 引入的`+=`是个例外。
- (IniValue) Value  
    红红ini里值可以有`string` `int` `float(double)` `bool` `Array`五种类型。  
    而 C# 事强类型的。因此首先需要实现类型统一，以及相应的转换器。  
    在注册表中，值重复的取其中第一个。
- (string?) Comment  
    原则上`;`起头的部分就算是注释，分为无注释`null`、空注释`string.Empty`、普通注释三种情形。
    
    考虑到`114514　　　; 好臭藕`这种间隔特别长的注释，需要在 Git 里维持间隔、减少diff，
    实际存的注释是挖掉 Value 后的剩余字符串。

* IniItem  
    ini 中非节声明行就由上面三个组件构成。  
    三个组件均空，该行就空；Key为空Comment不空，就是纯注释；Key不空就是标准键值对了。

- IEnumerable[IniItem] -> IniSection  
    节声明`[Section]`底下的`IniItem`均属于该节，即节是`IniItem`的容器。  
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

* IEnumerable[IniSection] -> IniDoc  
    把上述数据汇集起来，就是完整的 ini 了。

    考虑到一个完整的 ini 实例可能不止需要读一个 ini 文件（Ares 的 ini 拆分功能），  
    `IniDoc`只实现必要的 API，不再记录文件信息（编码、路径等等）。
