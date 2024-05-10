using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Options button
    public void OptionsScreen()
    {
        SceneManager.LoadScene(3, LoadSceneMode.Single);
    }

    // Settings button
    public void SettingsScreen()
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
