using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public string sceneToLoadName = "RealisticStyleScene"; 

    public void OnStartButtonClick()
    {
        // Loads the scene whent the 'START' button is pressed
        SceneManager.LoadScene(sceneToLoadName);
    }
}
