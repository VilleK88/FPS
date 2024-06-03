using UnityEngine;
public class AuthCanvas : MonoBehaviour
{
    #region
    public static AuthCanvas Instance;
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
}