using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    // Continue button
    public void WinContinueScreen()
    {
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    // Main menu button
    public void WinMainMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    // Exit game button
    public void WinExitGame()
    {
        Application.Quit();
    }
}
