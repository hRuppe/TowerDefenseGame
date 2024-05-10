using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoseMenuButtons : MonoBehaviour
{
    // Continue button
    public void LoseContinueScreen()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }


    // Exit game button
    public void LoseExitGame()
    {
        Application.Quit();
    }
}