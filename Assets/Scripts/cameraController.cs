using UnityEngine;

public class cameraController : MonoBehaviour
{

    [SerializeField] float sensHorizontal;
    [SerializeField] float sensVertical;

    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;

    [SerializeField] bool inverty;

    float xRotation;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //get input
        float mouseX = Input.GetAxis("Mouse X") * sensHorizontal;
        float mouseY = Input.GetAxis("Mouse Y") * sensVertical * (inverty ? 1 : -1);

        xRotation += mouseY;

        //clamp camera rotation
        xRotation = Mathf.Clamp(xRotation, lockVertMin, lockVertMax);

        //rotate the camera on x axis
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        //rotate player
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}