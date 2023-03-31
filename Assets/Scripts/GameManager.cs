using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool IsGameOver;

    private void Update()
    {
        if (IsGameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                IsGameOver = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                Debug.Log("JoJo");
            }
        }
    }
}
