using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;
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
    [SerializeField] private string loginEndpoint = "http://127.0.0.1:13756/account/login";
    [SerializeField] private string createEndpoint = "http://127.0.0.1:13756/account/create";
    [SerializeField] private string scoreEndpoint = "http://127.0.0.1:13756/account/updateScore";
    [SerializeField] private TextMeshProUGUI alertText;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button createButton;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    public bool loggedIn;
    public GameAccount loggedInAccount;
    [SerializeField] private GameObject container;
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
        WWWForm form = new WWWForm();
        form.AddField("rUsername", username);
        UnityWebRequest request = UnityWebRequest.Post(scoreEndpoint, form);
        yield return request.SendWebRequest();
        if(request.result == UnityWebRequest.Result.Success)
        {
            CreateResponse response = JsonUtility.FromJson<CreateResponse>(request.downloadHandler.text);
            if(response.code == 0)
            {
                //alertText.text = "Kills updated: " + response.data.kills;
                Debug.Log("Kills updated: " + response.data.kills);
                loggedInAccount.kills = response.data.kills;
                if (Score.Instance != null)
                {
                    Score.Instance.killsText.text = "Kills: " + response.data.kills;
                    Score.Instance.accountData.kills = response.data.kills;
                }
            }
            else
            {
                //alertText.text = response.msg;
                Debug.LogError("Error: " + response.msg);
            }
        }
        else
        {
            //alertText.text = "Error connecting to the server...";
            Debug.LogError("Error connecting to the server...");
        }
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
        if(username.Length < 3 || username.Length > 24)
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
        WWWForm form = new WWWForm();
        form.AddField("rUsername", username);
        form.AddField("rPassword", password);
        UnityWebRequest request = UnityWebRequest.Post(loginEndpoint, form);
        var handler = request.SendWebRequest();
        float startTime = 0.0f;
        while(!handler.isDone)
        {
            startTime += Time.deltaTime;
            if (startTime > 10.0f)
                break;
            yield return null;
        }
        if (request.result == UnityWebRequest.Result.Success)
        {
            //Debug.Log(request.downloadHandler.text);
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
            if (response.code == 0) // login success?
            {
                ActivateButtons(false);
                alertText.text = "Welcome " + ((response.data.adminFlag == 1) ? " Admin" : "");
                loggedIn = true;
                Debug.Log("Kills: " + response.data.kills);
                loggedInAccount.kills = response.data.kills;
                loggedInAccount.username = response.data.username;
            }
            else
            {
                switch (response.code)
                {
                    case 1:
                        alertText.text = "Invalid credentials";
                        ActivateButtons(true);
                        break;
                    default:
                        alertText.text = "Corruption detected";
                        ActivateButtons(false);
                        break;
                }
            }
        }
        else
        {
            alertText.text = "Error connecting to the server...";
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
        WWWForm form = new WWWForm();
        form.AddField("rUsername", username);
        form.AddField("rPassword", password);
        UnityWebRequest request = UnityWebRequest.Post(createEndpoint, form);
        var handler = request.SendWebRequest();
        float startTime = 0.0f;
        while (!handler.isDone)
        {
            startTime += Time.deltaTime;
            if (startTime > 10.0f)
                break;
            yield return null;
        }
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(request.downloadHandler.text);
            CreateResponse response = JsonUtility.FromJson<CreateResponse>(request.downloadHandler.text);
            if (response.code == 0)
            {
                alertText.text = "Account has been created";
            }
            else
            {
                switch(response.code)
                {
                    case 1:
                        alertText.text = "Invalid credentials";
                        break;
                    case 2:
                        alertText.text = "Username is already taken";
                        break;
                    case 3:
                        alertText.text = "Password is unsafe";
                        break;
                    default:
                        alertText.text = "Corruption detected";
                        break;
                }
            }
        }
        else
        {
            alertText.text = "Error connecting to the server...";
        }
        ActivateButtons(true);
        yield return null;
    }
    private void ActivateButtons(bool toggle)
    {
        loginButton.interactable = toggle;
        createButton.interactable = toggle;
    }
}