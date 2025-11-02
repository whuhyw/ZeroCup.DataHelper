# ZeroCup.DataHelper

## 简介

这是一个用来帮助处理 **零杯** 网页设计比赛 **第一个主题** 图片和文案数据的工具。

## 功能

- 简化图片文件命名
- 读取目录结构，生成 json 配置文件
- 读取景点文案，写入 json 配置文件

## 数据结构
```json
[category: {
    Id: string;
    Name: string;
    CoverImage: string;
    Description: any;
    Attractions: {
        Id: string;
        Name: string;
        Zen: string;
        Details: {
            Summary: string;
            Images: string[];
            Coordinates: number[];
        };
    }
}]
```
这个程序会完成 Id, Name, Images, Summary 的填写。

## 使用方法

- clone 本仓库
- 使用安装了.NET 桌面开发的 Visual Studio 打开 .sln 文件
- 把 Data.cs 改成你自己需要的数据结构
- 运行