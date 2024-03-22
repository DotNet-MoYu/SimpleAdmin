// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.System;

/// <summary>
/// 文件表
///</summary>
[SugarTable("sys_file", TableDescription = "文件表")]
[Tenant(SqlSugarConst.DB_DEFAULT)]
public class SysFile : DataEntityBase
{
    /// <summary>
    /// 存储引擎
    ///</summary>
    [SugarColumn(ColumnName = "Engine", ColumnDescription = "存储引擎", Length = 200)]
    public string Engine { get; set; }

    /// <summary>
    /// 存储桶
    ///</summary>
    [SugarColumn(ColumnName = "Bucket", ColumnDescription = "存储桶", Length = 200)]
    public string Bucket { get; set; }

    /// <summary>
    /// 文件名称
    ///</summary>
    [SugarColumn(ColumnName = "Name", ColumnDescription = "文件名称")]
    public string Name { get; set; }

    /// <summary>
    /// 文件后缀
    ///</summary>
    [SugarColumn(ColumnName = "Suffix", ColumnDescription = "文件后缀", Length = 200)]
    public string Suffix { get; set; }

    /// <summary>
    /// 文件大小kb
    ///</summary>
    [SugarColumn(ColumnName = "SizeKb", ColumnDescription = "文件大小kb")]
    public long SizeKb { get; set; }

    /// <summary>
    /// 文件大小（格式化后）
    ///</summary>
    [SugarColumn(ColumnName = "SizeInfo", ColumnDescription = "文件大小（格式化后）", Length = 200)]
    public string SizeInfo { get; set; }

    /// <summary>
    /// 文件的对象名（唯一名称）
    ///</summary>
    [SugarColumn(ColumnName = "ObjName", ColumnDescription = "文件的对象名（唯一名称）")]
    public string ObjName { get; set; }

    /// <summary>
    /// 文件存储路径
    ///</summary>
    [SugarColumn(ColumnName = "StoragePath", ColumnDescription = "文件存储路径")]
    public string StoragePath { get; set; }

    /// <summary>
    /// 文件下载路径
    ///</summary>
    [SugarColumn(ColumnName = "DownloadPath", ColumnDescription = "文件下载路径", IsNullable = true)]
    public string DownloadPath { get; set; }

    /// <summary>
    /// 图片缩略图
    ///</summary>
    [SugarColumn(ColumnName = "Thumbnail", ColumnDescription = "图片缩略图", IsNullable = true, ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string Thumbnail { get; set; }
}
