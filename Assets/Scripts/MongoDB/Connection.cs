using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;
public class Connection
{
    MongoClient mongoClient = new MongoClient("mongodb+srv://villekarppinen88:mongo@cluster0.y1p6lhv.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0");
    public IMongoDatabase db;
    IMongoCollection<BsonDocument> collection;
    public Connection(string dbName, string dbCollection)
    {
        db = mongoClient.GetDatabase(dbName);
        collection = db.GetCollection<BsonDocument>(dbCollection);
        Debug.Log($"Connection-olio luotiin tietokannalle {dbName}, collectionille {dbCollection}");
    }
}