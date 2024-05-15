using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FuelRefinerController : MonoBehaviour
{
    public GameObject player;
    public GameObject gameManager;
    public GameObject addon;

    public string collectMsg;

    public GameObject powerLight;

    public Material on;

    public GameObject smeltLight;

    int refineryInv = 0;

    bool playerInRange = false;

    bool isRefining = false;

    private void Update()
    {
        addon = gameObject.GetComponent<BuildableObj>().addon;

        // If the furnace is powered, indicate it.
        if (addon != null && addon.GetComponent<BuildableObj>().addonType == "PowerGen")
            powerLight.GetComponent<MeshRenderer>().material = on;

        // If the player presses F while in range of the refinery and not in build mode
        if (Input.GetKeyDown(KeyCode.F) && playerInRange && gameManager.GetComponent<GameManager>().buildMode == false)
        {
            // If there is no power generator
            if (addon == null || addon.GetComponent<BuildableObj>().addonType != "PowerGen")
                gameManager.GetComponent<GameManager>().DoErrorMessage("Refinery is not powered", 3f);

            // Check if there is enough storage to store produced fuel, and if the player has ice to refine
            if (gameManager.GetComponent<GameManager>().inventory["Ice"] > 0)
            {
                if (gameManager.GetComponent<GameManager>().inventory["Fuel"] + refineryInv < gameManager.GetComponent<GameManager>().fuelStorage)
                {
                    gameManager.GetComponent<GameManager>().inventory["Ice"] -= 1;
                    gameManager.GetComponent<GameManager>().iceCount.GetComponent<TextMeshProUGUI>().text = "Ice: " + gameManager.GetComponent<GameManager>().inventory["Ice"];
                    refineryInv += 1;
                }
                else
                    gameManager.GetComponent<GameManager>().DoErrorMessage("Not enough fuel storage", 4f);

            }
            else
                gameManager.GetComponent<GameManager>().DoErrorMessage("Not enough ice", 4f);
        }

        // Only wanna start the coroutine once, only when there's ore in it, and only when its powered
        if (GetComponent<BuildableObj>() != null)
        {
            if (refineryInv >= 1 && !isRefining && GetComponent<BuildableObj>().addon.transform.GetComponent<BuildableObj>().addonType == "PowerGen")
            {
                StartCoroutine(RefineFuel());
                isRefining = true;
                smeltLight.SetActive(true);
            }
        }

    }

    IEnumerator RefineFuel()
    {
        while (refineryInv >= 1)
        {
            yield return new WaitForSeconds(1f);
            refineryInv -= 1;
            gameManager.GetComponent<GameManager>().inventory["Fuel"] += 1;
            gameManager.GetComponent<GameManager>().fuelCount.GetComponent<TextMeshProUGUI>().text = "Fuel: " + gameManager.GetComponent<GameManager>().inventory["Fuel"] + "/" + gameManager.GetComponent<GameManager>().fuelStorage;
        }
        StopCoroutine(RefineFuel());
        isRefining = false;
        smeltLight.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            gameManager.GetComponent<GameUiManager>().ShowInteractTooltip("F", "Refine fuel"); 
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            gameManager.GetComponent<GameUiManager>().HideInteractTooltip();
            playerInRange = false;
            player = collision.gameObject;
        }
    }
}
