using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;
public class FindAllDocuments : MonoBehaviour
{
    Connection conn;
    public IMongoCollection<BsonDocument> collection;
    private void Awake()
    {
        conn = new Connection("sample_mflix", "movies");
        collection = conn.collection;
    }
    void Start()
    {
        //FindAll();
        FindAllDocumentsUsingFilter(500);
    }
    void FindAll()
    {
        var documents = collection.Find(new BsonDocument());
        foreach (BsonDocument doc in documents.ToList())
        {
            //print(doc);
            //print(doc["plot"]);
        }
    }
    void FindAllDocumentsUsingFilter(int score)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("score", score);
        var results = collection.Find(filter).ToList();
        Debug.Log(string.Join(",", results));
    }
}