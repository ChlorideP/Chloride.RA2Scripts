# Chloride.RA2Scripts
写些`pyalert2yr`里不方便集成的工具脚本。当然那边也有个`docs/samples.md`，总之仅供参考。

> `Shimakaze.Sdk`貌似并没有作为库嵌入其他项目的设计，那我就只好自己搓一个了。

> 如需食用脚本，还请部署`.net 6.0 SDK`**自行编译**，并自行在项目目录重建`config.ini`。

## Chloride.RA2.IniExt
现已移入`Chloride.RA2Scripts.Formats`，提供对 INI 框架的扩展解析。

[原 IniExt 库简介](IniExt.ReadMe.md)  
[原 IniExt 库框架](IniExt.Framework.md)

> 注意：  
> 现在`master`分支上的 INI 实现与旧版不完全一致，**如只需 IniExt 请以`v3.0.1.3`为准。**  
