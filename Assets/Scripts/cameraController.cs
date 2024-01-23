using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{

    [SerializeField] int sensHorizontal;
    [SerializeField] int sensVertical;

    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;

    [SerializeField] bool inverty;

    float xRotation;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //get input
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensHorizontal;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensVertical;

        if (inverty)
        {
            xRotation += mouseY;
        }
        else
        {
            xRotation -= mouseY;
        }


        //clamp camera roatation

        xRotation = Mathf.Clamp(xRotation, lockVertMin, lockVertMax);

        //rotate the camera on x axis

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // rotate player

        transform.parent.Rotate(Vector3.up * mouseX);
    }
}