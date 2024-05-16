using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenuButtons : MonoBehaviour
{
    // Continue button
    public void ContinueScreen()
    {
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    // Main menu button
    public void MainMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    // Exit game button
    public void ExitGame()
    {
        Application.Quit();
    }
}
