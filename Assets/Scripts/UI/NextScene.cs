using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    public string sceneName;
    public float time;
    float timer = 0;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(sceneName);
        }

        if(time > timer)
        {
            timer += Time.deltaTime;
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
