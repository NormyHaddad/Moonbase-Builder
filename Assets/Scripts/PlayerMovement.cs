using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float jumpStrength;
    public float rcsStrength;
    public Transform orientation;
    public AudioSource rcsHiss;

    public LayerMask playerLayer;

    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    bool isJumping;
    bool canJump;

    public GameObject gameManager;
    float gravity;

    void Start()
    {
        isJumping = false;
        canJump = true;
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
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping && canJump)
        {
            rb.AddForce(Vector3.up * jumpStrength);
            isJumping = true;
            StartCoroutine(JumpCooldown());
        }

        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
        {
            if(!gameManager.GetComponent<GameManager>().buildMode)
            {
                rcsHiss.Play();
            }
        }
        if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.E))
        {
            if(!gameManager.GetComponent<GameManager>().buildMode)
            {
                rcsHiss.Stop();
            }
        }

        // Check if the player is standing on something
        Ray ray = new Ray(transform.position + new Vector3(0f, 0.2f, 0f), Vector3.down);
        Debug.DrawRay(transform.position + new Vector3(0f, 0.2f, 0f), Vector3.down * 0.2f, Color.magenta, 0.1f);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0.2f, ~playerLayer)) // Make sure the raycast never hits the player itself
        {
            isJumping = false;
        }
        else
        {
            isJumping = true;
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    IEnumerator JumpCooldown()
    {
        canJump = false;
        isJumping = true;
        yield return new WaitForSeconds(0.5f);
        canJump = true;
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
