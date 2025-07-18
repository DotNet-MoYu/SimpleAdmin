﻿// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

global using MoYu;
global using MoYu.DataEncryption;
global using MoYu.DependencyInjection;
global using MoYu.EventBus;
global using MoYu.FriendlyException;
global using MoYu.ViewEngine;
global using Magicodes.ExporterAndImporter.Core;
global using Magicodes.ExporterAndImporter.Core.Models;
global using Magicodes.ExporterAndImporter.Excel;
global using Mapster;
global using Masuit.Tools;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using NewLife.Serialization;
global using OfficeOpenXml.Table;
global using SimpleAdmin.Cache;
global using SimpleAdmin.Core;
global using SimpleAdmin.Core.Utils;
global using SimpleAdmin.SqlSugar;
global using SimpleTool;
global using SqlSugar;
global using System.ComponentModel.DataAnnotations;
global using System.Reflection;
global using System.Text;
global using System.Web;
global using SkiaSharp;
global using System.Runtime.InteropServices;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Newtonsoft.Json.Linq;
global using NewLife;
