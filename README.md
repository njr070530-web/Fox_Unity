# 👥 团队使用指南（给组员看的说明）

## 🐍 Python 后端环境配置

### 📦 需要安装的库
本项目使用了以下第三方依赖：

| 模块 | 安装命令 | 功能说明 |
|------|-----------|----------|
| `opencv-python` | `pip install opencv-python` | 摄像头图像捕获与处理 |
| `mediapipe` | `pip install mediapipe` | Google 的姿态识别库 |
| `pyaudio` | `pip install pyaudio` | 语音采集（麦克风输入） |
| `numpy` | `pip install numpy` | 数值运算与矩阵处理 |


### git基本使用指南
1. 🪄 一次性初始化（第一次使用）

    克隆仓库

    ``git clone https://github.com/njr070530-web/Fox_Unity.git
cd Fox_Unity``


  查看分支

  ``git branch -a``


==主分支是 main，不要直接在上面开发！==

2. 创建自己的分支

``git checkout -b feature/你的名字-功能名``


3. 示例：

``git checkout -b feature/jiarui-soundcontrol``

4. 🔄 每次更新代码前（重要！）

先从远程仓库获取最新版本：

``git pull origin main``


>这一步可以避免你在 push 时出现冲突。

5. 🧩 提交你的修改

添加改动：

``git add .``


6. 提交改动：

``git commit -m "描述一下修改内容"``


7. 推送到远程（你的分支）：

``git push origin feature/你的名字-功能名``

8. 🧠 合并回主分支（通过 GitHub 完成）

- 打开项目的 GitHub 页面

- 点击 “Compare & pull request”

- 说明你修改了什么，让负责人审核后合并到 main

==⚠️ 不要直接在 main 分支 push！
所有更改请先通过 PR（Pull Request）合并。==
