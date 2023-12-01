using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirlockController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject door1;
    public GameObject door2;

    bool playerInRange = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                SwitchDoors();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void SwitchDoors()
    {
        if (door1.activeSelf == true)
        { door1.SetActive(false); }
        else { door1.SetActive(true); }
        
        if (door2.activeSelf == true)
        { door2.SetActive(false); }
        else { door2.SetActive(true); }
    }
}
