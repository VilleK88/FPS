using UnityEngine;
using MongoDB.Driver;
using System.Collections.Generic;
public class TestConnection : MonoBehaviour
{
    MongoClient mongoClient;
    IMongoDatabase db;
    private void Awake()
    {
        // yhteyden m‰‰rittely
        mongoClient = new MongoClient("mongodb+srv://villekarppinen88:mongo@cluster0.y1p6lhv.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0");

        List<string> collections = mongoClient.GetDatabase("scoreDB").ListCollectionNames().ToList();
        foreach (string col in collections)
            print(col);
    }
}