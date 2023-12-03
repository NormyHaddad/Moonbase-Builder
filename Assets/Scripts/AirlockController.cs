using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirlockController : MonoBehaviour
{
    GameObject gameManager;

    public GameObject door1;
    public GameObject door2;

    bool playerInRange = false;

    void Start()
    {
        gameManager = GetComponent<HabController>().gameManager;
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
            gameManager.GetComponent<GameUiManager>().ShowInteractTooltip("F", "Toggle airlock");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            gameManager.GetComponent<GameUiManager>().HideInteractTooltip();
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
