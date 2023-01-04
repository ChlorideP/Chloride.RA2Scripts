# Chloride.CCiniExt.Scripts
ini 批处理脚本范例。

如果真的需要食用这里的轮子，请准备好`config.ini`，并根据注释的样例自拟。  
当然自己写脚本或者 C# 控制台也不戳，反正 ini 的框架也就那样。

### 造好的轮子
先叠个甲（）我接到的需求基本都比较 specific，就怎么方便怎么来了。  
如果觉得我写的东西能够缓解低血压，还请不要吝惜您的 PR:pray:

- `GroupInherit.cs` 将换皮组件改成继承写法。
- `ArrayValueSort.cs` 根据已知顺序，将特定注册表`IterTypeList`中成员的特定键`KeyToSort`重新排序。  

### 关于 null 警告
值为 null 的情况只可能是 IniDoc\[sect, key\] 访问的时候找不到。正常照写的 config.ini 不可能 null。