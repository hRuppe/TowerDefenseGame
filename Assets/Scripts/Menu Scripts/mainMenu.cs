using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainMenu : MonoBehaviour
{
    [SerializeField] Button newGameBtn;
    [SerializeField] TMP_InputField nameInput;

    private void Awake()
    {
        newGameBtn.enabled = false;
        newGameBtn.interactable = false;
    }

    // New game button
    public void newGame()
    {
        // Save player name in playerprefs
        PlayerPrefs.SetString("PlayerName", nameInput.text);

        // Load first level
        SceneManager.LoadScene(1,LoadSceneMode.Single);
    }

    // Settings button
    public void settingsScreen()
    {
        SceneManager.LoadScene(2,LoadSceneMode.Single);
    }

    // Exits game button
    public void exitGame()
    {
        Application.Quit();
    }

    // Checks that there is something in the name input to enable/disable new game btn
    public void EnableNewGameBtn()
    {
        if (nameInput.text.Length < 1)
        {
            newGameBtn.enabled = false;
            newGameBtn.interactable = false;
        }
        else
        {
            newGameBtn.enabled = true;
            newGameBtn.interactable = true;
        }
    }
}
