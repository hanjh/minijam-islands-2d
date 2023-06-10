using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void playGame()
    {
        Debug.Log("in SceneSwitcher.playGame");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void Update()
    {
        if (Input.anyKey)
        {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}