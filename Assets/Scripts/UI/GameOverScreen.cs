using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOverScreen : MonoBehaviour
{
    #region Singleton
    public static GameOverScreen instance;
    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this);
        else instance = this;
    }
    #endregion
    [SerializeField] private Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public IEnumerator GameOver()
    {
        anim.SetTrigger("GameOver");
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("1 - Menu");
    }
}