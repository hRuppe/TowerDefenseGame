using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    // Exit game button
    public void ExitGame()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);

    }
}
