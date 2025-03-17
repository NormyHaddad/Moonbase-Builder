using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform cameraPosition;
    public GameManager gameManager;
    public Transform buildModePos;
    public Camera cam;

    public float camMoveSpeed;
    public int minCamHeight;
    public int maxCamHeight;

    //Transform moveOrientation;

    float horizontalInput;
    float verticalInput;

    bool buildInit = false;

    void Update()
    {
        if (!gameManager.GetComponent<GameManager>().buildMode && !gameManager.GetComponent<GameManager>().playerMovementLock) // If not in build mode or not locked
        {
            cam.transform.rotation = transform.rotation;
            buildInit = false;
            transform.position = cameraPosition.position;
            transform.rotation = cameraPosition.rotation;
        }
        if (gameManager.GetComponent<GameManager>().buildMode) // If in build mode
        {
            if (!buildInit) // Allow for separate camera movement
            {
                cam.transform.Rotate(30f, 0f, 0f);
                transform.position = buildModePos.position;
                transform.rotation = buildModePos.rotation;
                buildInit = true;
            }

            // Move camera
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            transform.position += (transform.forward * verticalInput + transform.right * horizontalInput) * camMoveSpeed * Time.deltaTime;

            if (Input.GetKey(KeyCode.LeftShift) && gameObject.transform.position.y < maxCamHeight) { transform.position += new Vector3(0, camMoveSpeed * Time.deltaTime, 0); }
            if (Input.GetKey(KeyCode.LeftControl) && gameObject.transform.position.y > minCamHeight) { transform.position -= new Vector3(0, camMoveSpeed * Time.deltaTime, 0); }

            // Rotate camera
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(0f, -camMoveSpeed * Time.deltaTime * 5f, 0f);
            }

            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(0f, camMoveSpeed * Time.deltaTime * 5f, 0f);
            }
        }
    }
}
