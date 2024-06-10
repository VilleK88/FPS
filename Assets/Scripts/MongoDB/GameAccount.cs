using UnityEngine;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
[System.Serializable]
public class GameAccount
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonIgnoreIfDefault]
    public string _id { get; set; }
    public int adminFlag { get; set; }
    public string username { get; set; }
    public string password { get; set; }
    public int kills { get; set; }
}