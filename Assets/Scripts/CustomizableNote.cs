using TMPro;
using UnityEngine;

public class CustomizableNote : MonoBehaviour
{
    [Multiline]
    [SerializeField] string noteText;

    bool noteOpen = false;
    bool inNoteRange = false;
    TMP_Text readNotePrompt;
    AudioSource audioSource;

    private void Start()
    {
        readNotePrompt = gameManager.instance.readNotePrompt;

        readNotePrompt.text = "Press 'r' to read note...";

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Read Note") && !noteOpen && inNoteRange)
        {
            gameManager.instance.playerScript.hasOpenedNote = true;
            gameManager.instance.tutorialUI.gameObject.SetActive(false);
            gameManager.instance.isPaused = false;
            OpenNote();
        }
        if (Input.GetButton("Close Note") && noteOpen && inNoteRange)
        {
            CloseNote();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        gameManager.instance.noteText.text = noteText;

        if (gameManager.instance.playerScript.hasOpenedNote == false)
        {
            gameManager.instance.tutorialUI.text = "Oh look a note! Open it to see what it says!";
            gameManager.instance.tutorialUI.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }

        if (other.CompareTag("Player"))
        {
            // Show open note prompt
            readNotePrompt.gameObject.SetActive(true);

            inNoteRange = true;
        }
    }

    private void OpenNote()
    {
        audioSource.Play();
        gameManager.instance.noteObject.SetActive(true);
        readNotePrompt.text = "Press 'c' to close note...";
        noteOpen = true;
    }

    private void CloseNote()
    {
        audioSource.Play();
        gameManager.instance.noteObject.SetActive(false);
        readNotePrompt.text = "Press 'r' to read note...";
        noteOpen = false;
    }

    private void OnTriggerExit(Collider other)
    {
        noteOpen = false;
        gameManager.instance.noteObject.SetActive(false);
        gameManager.instance.readNotePrompt.gameObject.SetActive(false);
        gameManager.instance.readNotePrompt.text = "Press 'r' to read note...";

        inNoteRange = true;
    }
}
