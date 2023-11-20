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
    public GameObject addon;

    public GameObject powerLight;
    public GameObject radiator;
    public GameObject shield;

    public Material on;
    public Material radiatorGlow;
    public Material radiatorNormal;
    public Material shieldGlow;
    public Material shieldNormal;

    public GameObject smeltLight;

    int furnaceInv = 0;
    int furnaceOut = 0;
    bool playerInRange = false;

    bool isSmelting = false;

    private void Update()
    {
        addon = gameObject.GetComponent<BuildableObj>().addon;

        // If the furnace is powered, indicate it.
        if (addon != null && addon.GetComponent<BuildableObj>().addonType == "PowerGen")
            powerLight.GetComponent<MeshRenderer>().material = on;

        // If the player presses F while in range of the furnace and not in build mode
        if (Input.GetKeyDown(KeyCode.F) && playerInRange && gameManager.GetComponent<GameManager>().buildMode == false)
        {
            // If there is no power generator
            if (addon == null || addon.GetComponent<BuildableObj>().addonType != "PowerGen")
                gameManager.GetComponent<GameManager>().DoErrorMessage("Furnace is not powered", 3f);

            // Won't work if you have no ore to put in
            if (gameManager.GetComponent<GameManager>().inventory["Iron Ore"] > 0)
            {
                gameManager.GetComponent<GameManager>().inventory["Iron Ore"] -= 1;
                gameManager.GetComponent<GameManager>().ironOreCount.GetComponent<TextMeshProUGUI>().text =
                    "Ore: " + gameManager.GetComponent<GameManager>().inventory["Iron Ore"];

                furnaceInv += 1;
            }
            else
                gameManager.GetComponent<GameManager>().DoErrorMessage("Not enough iron ore", 4f);
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
            && GetComponent<BuildableObj>().addon.transform.GetComponent<BuildableObj>().addonType == "PowerGen")
        {
            Debug.Log(1);
            StartCoroutine(SmeltOre());
            isSmelting = true;
            smeltLight.SetActive(true);
        }

    }

    IEnumerator SmeltOre()
    {
        while (furnaceInv >= 1)
        {
            shield.GetComponent<MeshRenderer>().material = shieldGlow;
            radiator.GetComponent<MeshRenderer>().material = radiatorGlow;
            yield return new WaitForSeconds(1f);
            Debug.Log(collectText.GetComponent<TextMeshPro>());
            furnaceInv -= 1;
            furnaceOut += 1;
            if (collectText.GetComponent<TextMeshPro>() != null)
                collectText.GetComponent<TextMeshPro>().text = "'C' Collect " + furnaceOut.ToString() + " metal";
        }
        StopCoroutine(SmeltOre());
        isSmelting = false;
        shield.GetComponent<MeshRenderer>().material = shieldNormal;
        radiator.GetComponent<MeshRenderer>().material = radiatorNormal;
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
