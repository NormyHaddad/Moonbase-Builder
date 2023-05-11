using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float jumpStrength;
    public float rcsStrength;
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    bool isJumping;

    public GameObject gameManager;
    float gravity;

    void Start()
    {
        isJumping = false;
        gravity = gameManager.GetComponent<GameManager>().gravity;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        rb.AddForce(0, -gravity, 0);
        MovePlayer();
    }

    void Update()
    {
        MyInput();
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            rb.AddForce(Vector3.up * jumpStrength);
            isJumping = true;
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void MovePlayer()
    {
        if (!gameManager.GetComponent<GameManager>().buildMode)
        {
            // Calculate movement direction
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
            //rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            if (horizontalInput == 0 && verticalInput == 0)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
            
            if (Input.GetKey(KeyCode.Q))
            {
                rb.AddForce(0, rcsStrength, 0);
                isJumping = true;
            }
            if (Input.GetKey(KeyCode.E))
            {
                rb.AddForce(0, -rcsStrength, 0);
            }
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }
}
