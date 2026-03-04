# PuppergeistAccess

Puppergeist 的无障碍辅助模组，添加了全面的屏幕阅读器支持，让盲人和视障玩家能够完整游玩游戏。

## 功能特性

- **对话朗读**：自动朗读所有对话文本和角色名称
- **菜单导航**：完整的键盘导航支持，屏幕阅读器会朗读所有菜单元素
- **图像描述**：为重要的视觉元素提供可配置的描述
- **对话选项**：使用方向键或数字键导航和选择对话选项
- **交互提示**：接近可交互物体时会发出通知
- **小游戏结果**：朗读小游戏后的分数和排名
- **重复功能**：按 R 键重新朗读当前对话

## 安装方法

### 系统要求
- Puppergeist（试玩版或完整版）
- Windows 10/11 及屏幕阅读器（NVDA、JAWS 或讲述人）

### 安装步骤
1. 从 [Releases](../../releases) 页面下载 `PuppergeistAccess.zip`
2. 将压缩包解压到游戏目录（与 `Puppergeist.exe` 同级的文件夹）
3. 启动游戏 - 你应该会听到"PuppergeistAccess mod loaded"

就这么简单！便携版本已包含 MelonLoader 和所有必需的依赖项。

## 使用说明

### 键盘控制
- **F1**：显示帮助信息
- **F12**：切换调试模式
- **R**：重复当前对话（从屏幕重新读取）
- **方向键 / W/S**：导航对话选项
- **数字键（1-9）**：快速选择对话选项
- **Tab / 方向键**：导航菜单

### 自定义图像描述
编辑 Mods 文件夹中的 `ImageDescriptions.json` 来为任何精灵图添加描述：

```json
{
  "descriptions": {
    "sprite_name": "你的描述内容"
  }
}
```

未知的精灵图会在调试模式（F12）中记录，方便识别。

## 从源代码构建

### 要求
- .NET SDK 4.7.2 或更高版本
- Visual Studio 2019+ 或带 C# 扩展的 VS Code

### 构建步骤
```bash
dotnet build PuppergeistAccess.csproj -c Release
```

编译后的 DLL 文件位于 `bin/Release/net472/PuppergeistAccess.dll`

## 技术细节

- **模组加载器**：MelonLoader v0.7.2
- **目标框架**：.NET Framework 4.7.2
- **屏幕阅读器库**：Tolk（支持 NVDA、JAWS、SAPI、System Access、Window-Eyes）
- **补丁技术**：Harmony 运行时方法补丁

## 文档

查看 `docs/` 文件夹获取详细指南：
- `ACCESSIBILITY_MODDING_GUIDE.md`：模式和最佳实践
- `technical-reference.md`：MelonLoader、Harmony 和 Tolk 参考
- `game-api.md`：已记录的游戏方法和键位绑定
- 更多关于本地化、状态管理等的指南

## 贡献

欢迎贡献！请随时提交问题或拉取请求。

## 许可证

本模组按原样提供，用于无障碍访问目的。Puppergeist 版权归其各自所有者所有。

## 致谢

- 由 Claude (Anthropic) 协助开发
- Tolk 库由 Davy Kager 开发
- MelonLoader 由 LavaGang 开发
- Harmony 由 pardeike 开发

## 支持

如果遇到问题或有建议，请在 GitHub 上提交 issue。
