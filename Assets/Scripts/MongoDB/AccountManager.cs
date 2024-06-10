using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
public class AccountManager : MonoBehaviour
{
    #region Singleton
    public static AccountManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
            Destroy(gameObject);
    }
    #endregion
    private const string passwordRegex = "(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{8,24})";
    private const string connectionString = "mongodb+srv://villekarppinen88:mongo@cluster0.y1p6lhv.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
    private const string databaseName = "loginGameDatabase";
    private const string collectionName = "accounts";
    private IMongoDatabase database;
    private IMongoCollection<BsonDocument> accountCollection;
    [SerializeField] private TextMeshProUGUI alertText;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button createButton;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    public bool loggedIn;
    public GameAccount loggedInAccount;
    [SerializeField] private GameObject container;
    private void Start()
    {
        var client = new MongoClient(connectionString);
        database = client.GetDatabase(databaseName);
        accountCollection = database.GetCollection<BsonDocument>(collectionName);
    }
    public void ShowContainer()
    {
        container.active = true;
    }
    public void HideContainer()
    {
        container.active = false;
    }
    public GameAccount GetAccountData()
    {
        return loggedInAccount;
    }
    public void OnEnemyKilled()
    {
        StartCoroutine(IncrementKills());
    }
    private IEnumerator IncrementKills()
    {
        string username = usernameInputField.text;
        if(string.IsNullOrEmpty(username))
        {
            alertText.text = "Invalid username";
            yield break;
        }
        var filter = Builders<BsonDocument>.Filter.Eq("username", username);
        var update = Builders<BsonDocument>.Update.Inc("kills", 1);
        var options = new FindOneAndUpdateOptions<BsonDocument> { ReturnDocument = ReturnDocument.After };
        var result = accountCollection.FindOneAndUpdate(filter, update, options);
        if(result != null)
        {
            var updatedAccount = BsonSerializer.Deserialize<GameAccount>(result);
            int kills = updatedAccount.kills;
            Debug.Log("Kills updated: " + kills);
            loggedInAccount.kills = kills;
            if (Score.Instance != null)
            {
                Score.Instance.killsText.text = "Kills: " + kills;
                Score.Instance.accountData.kills = kills;
            }
            else
                Debug.LogError("Error updating kills.");
        }
        yield return null;
    }
    public void OnLoginClick()
    {
        alertText.text = "Signing in...";
        ActivateButtons(false);
        StartCoroutine(TryLogin());
    }
    public void OnCreateClick()
    {
        alertText.text = "Creating account...";
        ActivateButtons(false);
        StartCoroutine(TryCreate());
    }
    private IEnumerator TryLogin()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;
        if (username.Length < 3 || username.Length > 24)
        {
            alertText.text = "Invalid username";
            ActivateButtons(true);
            yield break;
        }
        if (!Regex.IsMatch(password, passwordRegex))
        {
            alertText.text = "Invalid credentials";
            ActivateButtons(true);
            yield break;
        }
        var filter = Builders<BsonDocument>.Filter.Eq("username", username);
        var accountDoc = accountCollection.Find(filter).FirstOrDefault();
        if(accountDoc != null)
        {
            var account = BsonSerializer.Deserialize<GameAccount>(accountDoc);
            if(account.password == password)
            {
                loggedInAccount = account;
                ActivateButtons(false);
                alertText.text = "Welcome " + username;
                loggedIn = true;
                Debug.Log("Kills: " + loggedInAccount.kills);
            }
            else
            {
                alertText.text = "Invalid credentials";
                ActivateButtons(true);
            }
        }
        else
        {
            alertText.text = "Invalid credentials";
            ActivateButtons(true);
        }
        yield return null;
    }
    private IEnumerator TryCreate()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;
        if (username.Length < 3 || username.Length > 24)
        {
            alertText.text = "Invalid username";
            ActivateButtons(true);
            yield break;
        }
        if (!Regex.IsMatch(password, passwordRegex))
        {
            alertText.text = "Invalid credentials";
            ActivateButtons(true);
            yield break;
        }
        var filter = Builders<BsonDocument>.Filter.Eq("username", username);
        var account = accountCollection.Find(filter).FirstOrDefault();
        if (account == null)
        {
            var newAccount = new GameAccount
            {
                username = username,
                password = password,
                kills = 0,
            };
            accountCollection.InsertOne(newAccount.ToBsonDocument());
            alertText.text = "Account has been created";
        }
        else
            alertText.text = "Username is already taken";
        ActivateButtons(true);
        yield return null;
    }
    private void ActivateButtons(bool toggle)
    {
        loginButton.interactable = toggle;
        createButton.interactable = toggle;
    }
}