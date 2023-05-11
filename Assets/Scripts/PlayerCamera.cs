using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameManager gameManager;

    public float sensX;
    public float sensY;

    public Transform orientation;
    public Transform cameraPos;

    float xRotation;
    float yRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager != null && !gameManager.GetComponent<GameManager>().buildMode)
        {
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX;
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY;

            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(0, yRotation, 0);
            orientation.rotation = transform.rotation;
            cameraPos.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        }
    }
}
