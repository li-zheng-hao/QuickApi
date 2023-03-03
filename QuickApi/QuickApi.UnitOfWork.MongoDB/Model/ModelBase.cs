using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;

namespace QuickApi.UnitOfWork.MongoDB;

/// <summary>
/// 数据库集合基类
/// </summary>
public class ModelBase:IEntity
{
    /// <summary>
    /// 未映射上的字段全都记录在此对象内
    /// </summary>
    [JsonIgnore]
    [BsonExtraElements]
    public BsonDocument CatchAll { get; set; }

    public string GenerateNewID()
    {
        return ObjectId.GenerateNewId().ToString();
    }

    [JsonPropertyName("Id")] // System.Text.Json库
    [BsonId, ObjectId]
    public string ID { get; set; } = ObjectId.GenerateNewId().ToString();
    
    /// <summary>
    /// use this to specify an exclusion projection
    /// </summary>
    [Ignore]
    [JsonIgnore]
    public bool _dummyProp { get; set; } 
}