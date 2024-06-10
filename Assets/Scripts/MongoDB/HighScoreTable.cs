using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
[BsonIgnoreExtraElements]
public class User
{
    public ObjectId Id { get; set; }
    public string username { get; set; }
    public int kills { get; set; }
}
public class HighScoreTable : MonoBehaviour
{
    [SerializeField] GameObject content;
    //[SerializeField] GameObject userInfoPrefab;
    [SerializeField] GameObject[] userInfoPrefabs;
    private async void Start()
    {
        await LoadAndDisplayLeaderBoard();
    }
    private async Task LoadAndDisplayLeaderBoard()
    {
        try
        {
            string connectionString = "mongodb+srv://villekarppinen88:mongo@cluster0.y1p6lhv.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
            if (string.IsNullOrEmpty(connectionString))
            {
                Debug.LogError("MongoDB connection string is not sen in environment variables.");
                return;
            }
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("loginGameDatabase");
            var collection = database.GetCollection<User>("accounts");
            var users = await collection.Find(new BsonDocument()).ToListAsync();
            var leaderboard = GenerateLeaderboard(users);
            DisplayLeaderboard(leaderboard);
        }
        catch(MongoAuthenticationException ex)
        {
            Debug.LogError($"MongoDB Authentication Exception: {ex.Message}");
        }
        catch(Exception ex)
        {
            Debug.LogError($"An error occurred: {ex.Message}");
        }
        }
    private List<User> GenerateLeaderboard(List<User> users)
    {
        users.Sort((x, y) => y.kills.CompareTo(x.kills));
        return users;
    }
    private void DisplayLeaderboard(List<User> leaderboard)
    {
        int count = Math.Min(leaderboard.Count, userInfoPrefabs.Length);
        for(int i = 0; i < count; i++)
        {
            var user = leaderboard[i];
            GameObject userInfo = userInfoPrefabs[i];
            TextMeshProUGUI[] textComponents = userInfo.GetComponentsInChildren<TextMeshProUGUI>();
            foreach(var textComponent in textComponents)
                textComponent.text = $"{i+1}. {user.username}, Kills: {user.kills}";
        }
    }
}