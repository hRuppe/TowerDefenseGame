using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseMenu : MonoBehaviour
{
    // Continue button
    public void ContinueScreen()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }


    // Exit game button
    public void ExitGame()
    {
        Application.Quit();
    }
}