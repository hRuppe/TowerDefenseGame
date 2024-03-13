using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CustomizableNote : MonoBehaviour
{
    [SerializeField] string noteText; 

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            gameManager.instance.noteText.text = noteText;
            gameManager.instance.noteObject.SetActive(true); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        gameManager.instance.noteObject.SetActive(false);
    }
}
