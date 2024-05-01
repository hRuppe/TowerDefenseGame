using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyBindingMenu : MonoBehaviour
{
    // Exit game button
    public void ExitGame()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);

    }
}
