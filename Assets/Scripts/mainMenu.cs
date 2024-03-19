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

    public void newGame()
    {
        // Save player name in playerprefs
        PlayerPrefs.SetString("PlayerName", nameInput.text);

        // Load first level
        SceneManager.LoadScene(1,LoadSceneMode.Single);
    }

    public void EnableNewGameBtn()
    {
        newGameBtn.enabled = true;
        newGameBtn.interactable = true; 
    }
}
