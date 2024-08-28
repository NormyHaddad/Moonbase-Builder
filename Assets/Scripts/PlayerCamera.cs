using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameManager gameManager;

    public float sensitivity;
    public GameObject sensitivityText;

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
        if (gameManager != null && !gameManager.GetComponent<GameManager>().buildMode && !gameManager.GetComponent<GameManager>().playerMovementLock)
        {
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;

            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(0, yRotation, 0);
            orientation.rotation = transform.rotation;
            cameraPos.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        }
    }

    public void ChangeMouseSensitivity(float val)
    {
        sensitivity = val * 10;
        sensitivityText.GetComponent<TextMeshProUGUI>().text = val.ToString();
    }
}
