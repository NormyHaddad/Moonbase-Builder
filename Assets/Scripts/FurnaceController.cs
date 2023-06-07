using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FurnaceController : MonoBehaviour
{
    public GameObject interactText;
    public GameObject collectText;
    public GameObject player;
    public GameObject gameManager;
    //public GameObject addon;

    public GameObject smeltLight;

    int furnaceInv = 0;
    int furnaceOut = 0;
    bool playerInRange = false;

    bool isSmelting = false;

    private void Update()
    {
        // If the player presses F while in range of the furnace and not in build mode
        if (Input.GetKeyDown(KeyCode.F) && playerInRange && gameManager.GetComponent<GameManager>().buildMode == false)
        {
            if (gameManager.GetComponent<GameManager>().inventory["Ore"] > 0) // Won't work if you have no ore to put in
            {
                gameManager.GetComponent<GameManager>().inventory["Ore"] -= 1;
                gameManager.GetComponent<GameManager>().oreCount.GetComponent<TextMeshProUGUI>().text =
                    "Ore: " + gameManager.GetComponent<GameManager>().inventory["Ore"];

                furnaceInv += 1;
            }
            else
                gameManager.GetComponent<GameManager>().DoErrorMessage("Not enough ore", 4f);
        }

        if (collectText.GetComponent<TextMeshProUGUI>() != null)
            collectText.GetComponent<TextMeshProUGUI>().text = "'C' Collect " + furnaceOut.ToString() + " metal";

        // Press C to collect smelted ore
        if (Input.GetKeyDown(KeyCode.C) && playerInRange && gameManager.GetComponent<GameManager>().buildMode == false)
        {
            gameManager.GetComponent<GameManager>().inventory["Metal"] += furnaceOut;
            gameManager.GetComponent<GameManager>().metalCount.GetComponent<TextMeshProUGUI>().text =
                "Metal: " + gameManager.GetComponent<GameManager>().inventory["Metal"];
            furnaceOut = 0;
            if (collectText.GetComponent<TextMeshPro>() != null)
                collectText.GetComponent<TextMeshPro>().text = "'C' Collect " + furnaceOut.ToString() + " metal";
        }

        // Only wanna start the coroutine once, and only when there's ore in it, and only when its powered
        if (furnaceInv >= 1 && !isSmelting
            && GetComponent<BuildableObj>().addon.transform.GetComponent<BuildableObj>().addonType == 1)
        {
            Debug.Log(1);
            StartCoroutine(SmeltOre());
            isSmelting = true;
            smeltLight.SetActive(true);
        }

        if (furnaceInv <= 0 && isSmelting)
        {
            //Debug.Log(2);
            //StopCoroutine(SmeltOre());
            //Debug.Log("why?");
            //isSmelting = false;
            //smeltLight.SetActive(false);
        }
    }

    IEnumerator SmeltOre()
    {
        while (furnaceInv >= 1)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log(collectText.GetComponent<TextMeshPro>());
            furnaceInv -= 1;
            furnaceOut += 1;
            if (collectText.GetComponent<TextMeshPro>() != null)
                collectText.GetComponent<TextMeshPro>().text = "'C' Collect " + furnaceOut.ToString() + " metal";
        }
        StopCoroutine(SmeltOre());
        isSmelting = false;
        smeltLight.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            interactText.SetActive(true);
            collectText.SetActive(true);
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            interactText.SetActive(false);
            collectText.SetActive(false);
            playerInRange = false;
            player = collision.gameObject;
        }
    }
}
