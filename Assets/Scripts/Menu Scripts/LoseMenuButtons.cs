using UnityEngine;
using UnityEngine.SceneManagement;

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