// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using System.Text.RegularExpressions;

namespace SimpleAdmin.SqlSugar;

/// <summary>
/// 种子数据工具类
/// </summary>
public class SeedDataUtil
{
    public static List<T> GetSeedData<T>(string jsonName)
    {
        var seedData = new List<T>();//种子数据结果
        var basePath = AppContext.BaseDirectory;//获取项目目录
        var json = basePath.CombinePath("SeedData", "Json", jsonName);//获取文件路径
        var dataString = FileHelper.ReadFile(json);//读取文件
        if (!string.IsNullOrEmpty(dataString))//如果有内容
        {
            //字段没有数据的替换成null
            dataString = dataString.Replace("\"\"", "null");
            //将json字符串转为实体，这里ExtJson可以正常转换为字符串
            var seedDataRecord1 = dataString.ToJsonEntity<SeedDataRecords<T>>();

            //正则匹配"ConfigValue": "[{开头的字符串以]"结尾
            var matches = Regex.Matches(dataString, "\"ConfigValue\": \"\\[\\{.*?\\}\\]\"");
            foreach (Match match in matches)
            {
                //获取匹配的值
                var value = match.Value;
                //将匹配的值替换成"ConfigValue": "null"
                dataString = dataString.Replace(value, "\"ConfigValue\": \"null\"");
            }

            #region 针对导出的json字符串嵌套json字符串如 "DefaultDataScope": "{\"Level\":5,\"ScopeCategory\":\"SCOPE_ALL\",\"ScopeDefineOrgIdList\":[]}"

            //字符串是\"的替换成"
            dataString = dataString.Replace("\\\"", "\"");
            //字符串是\{替换成{
            dataString = dataString.Replace("\"{", "{");
            //字符串是}"的替换成}
            dataString = dataString.Replace("}\"", "}");

            //将json字符串转为实体,这里ExtJson会转为null，替换字符串把ExtJson值变为实体类型而实体类是string类型
            var seedDataRecord2 = dataString.ToJsonEntity<SeedDataRecords<T>>();

            #endregion 针对导出的json字符串嵌套json字符串如 "DefaultDataScope": "{\"Level\":5,\"ScopeCategory\":\"SCOPE_ALL\",\"ScopeDefineOrgIdList\":[]}"

            //遍历seedDataRecord2
            for (var i = 0; i < seedDataRecord2.Records.Count; i++)
            {
                #region 处理ExtJosn

                //获取ExtJson属性
                var propertyExtJson = typeof(T).GetProperty(nameof(PrimaryKeyEntity.ExtJson));
                if (propertyExtJson != null)
                {
                    //获取ExtJson的值
                    var extJson = propertyExtJson.GetValue(seedDataRecord2.Records[i])?.ToString();
                    // 如果ExtJson不为空并且包含NullableDictionary表示序列化失败了
                    if (!string.IsNullOrEmpty(extJson) && extJson.Contains("NullableDictionary"))
                    {
                        //设置ExtJson为seedDataRecord1对应的值
                        extJson = propertyExtJson.GetValue(seedDataRecord1.Records[i])?.ToString();
                        //seedDataRecord2赋值seedDataRecord1的ExtJson
                        propertyExtJson.SetValue(seedDataRecord2.Records[i], extJson);
                    }
                }

                #endregion 处理ExtJosn

                #region 处理ConfigValue

                //获取ExtJson属性
                var propertyConfigValue = typeof(T).GetProperty("ConfigValue");
                if (propertyConfigValue != null)
                {
                    //获取configValue的值
                    var configValue = propertyConfigValue.GetValue(seedDataRecord2.Records[i])?.ToString();
                    // 如果configValue不为空并且包含NullableDictionary表示序列化失败了
                    if (!string.IsNullOrEmpty(configValue) && (configValue.Contains("NullableDictionary") || configValue == "null"))
                    {
                        //设置ExtJson为seedDataRecord1对应的值
                        configValue = propertyConfigValue.GetValue(seedDataRecord1.Records[i])?.ToString();
                        //seedDataRecord2赋值seedDataRecord1的ExtJson
                        propertyConfigValue.SetValue(seedDataRecord2.Records[i], configValue);
                    }
                }

                #endregion 处理ConfigValue
            }
            //种子数据赋值
            seedData = seedDataRecord2.Records;
        }

        return seedData;
    }
}

/// <summary>
/// 种子数据格式实体类,遵循Navicat导出json格式
/// </summary>
/// <typeparam name="T"></typeparam>
public class SeedDataRecords<T>
{
    /// <summary>
    /// 数据
    /// </summary>
    public List<T> Records { get; set; }
}
