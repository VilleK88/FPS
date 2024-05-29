using System;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Driver;
public class MongoDBHandler : MonoBehaviour
{
    // MongoDB Client
    private MongoClient client;
    private IMongoDatabase database;
    private IMongoCollection<BsonDocument> collection;
    // MongoDB yhteysosoite (URI) ja tietokannan nimi
    private string connectionString = "mongodb+srv://villekarppinen88:mongo@cluster0.y1p6lhv.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
    private string databaseName = "pelidb";
    private string collectionName = "players";
    private void Start()
    {
        ConnectToMongoDB();
        InsertData();
    }
    private void ConnectToMongoDB()
    {
        try
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase(databaseName);
            collection = database.GetCollection<BsonDocument>(collectionName);
            Debug.Log("Connected to MongoDB");
        }
        catch(Exception ex)
        {
            Debug.LogError($"Connection to MongoDB failed: {ex.Message}");
        }
    }
    private void InsertData()
    {
        try
        {
            BsonDocument newDocument = new BsonDocument
            {
                {"playerName", "John Doe" },
                {"score", 100 },
                {"level", 5 },
                {"timestamp", DateTime.Now }
            };
            collection.InsertOne(newDocument);
            Debug.Log("Data inserted to MongoDB");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Inserting data failed: {ex.Message}");
        }
    }
}