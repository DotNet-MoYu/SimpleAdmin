// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

global using MoYu;
global using MoYu.DataValidation;
global using MoYu.DependencyInjection;
global using MoYu.FriendlyException;
global using MoYu.Logging;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using System;
global using System.Collections.Generic;
global using System.ComponentModel;
global using System.ComponentModel.DataAnnotations;
global using System.IO;
global using System.Linq;
global using System.Text;
global using System.Threading.Tasks;
global using Yitter.IdGenerator;
global using Microsoft.AspNetCore.ResponseCompression;
global using System.IO.Compression;
global using Newtonsoft.Json;
global using Newtonsoft.Json.Linq;
global using MoYu.UnifyResult;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Lazy.Captcha.Core;
global using Lazy.Captcha.Core.Generator.Image;
global using Lazy.Captcha.Core.Generator.Image.Option;
global using Org.BouncyCastle.Utilities.Encoders;
global using Org.BouncyCastle.Crypto.Digests;
global using Org.BouncyCastle.Crypto.Generators;
global using Org.BouncyCastle.Crypto.Parameters;
global using Org.BouncyCastle.Math;
global using Org.BouncyCastle.Math.EC;
global using Org.BouncyCastle.Security;
global using SkiaSharp;
global using System.Drawing;
global using System.Drawing.Drawing2D;
global using ICSharpCode.SharpZipLib.Checksum;
global using ICSharpCode.SharpZipLib.Zip;
global using System.Net;
