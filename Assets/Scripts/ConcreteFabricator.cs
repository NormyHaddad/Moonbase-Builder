using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConcreteFabricator : MonoBehaviour
{
    public GameObject player;
    public GameObject gameManager;
    public GameObject addon;

    public string collectMsg;

    public GameObject powerLight;
    public GameObject cureLight;

    public Material on;

    int fabricatorInv = 0;
    int fabricatorOut = 0;
    bool playerInRange = false;

    bool isCuring = false;

    private void Update()
    {
        addon = gameObject.GetComponent<BuildableObj>().addon;

        // If the fabricator is powered, indicate it.
        if (addon != null && addon.GetComponent<BuildableObj>().addonType == "PowerGen")
            powerLight.GetComponent<MeshRenderer>().material = on;

        // If the player presses F while in range of the fabricator and not in build mode
        if (Input.GetKeyDown(KeyCode.F) && playerInRange && gameManager.GetComponent<GameManager>().buildMode == false)
        {
            // If there is no power generator
            if (addon == null || addon.GetComponent<BuildableObj>().addonType != "PowerGen")
                gameManager.GetComponent<GameManager>().DoErrorMessage("Fabricator is not powered", 3f);

            // Won't work if you have no resources to put in
            if (gameManager.GetComponent<GameManager>().inventory["Regolith"] > 0 && gameManager.GetComponent<GameManager>().inventory["Water"] > 0)
            {
                gameManager.GetComponent<GameManager>().inventory["Regolith"] -= 1;
                gameManager.GetComponent<GameManager>().inventory["Water"] -= 1;
                gameManager.GetComponent<GameManager>().regolithCount.GetComponent<TextMeshProUGUI>().text =
                    "Regolith: " + gameManager.GetComponent<GameManager>().inventory["Regolith"];
                gameManager.GetComponent<GameManager>().waterCount.GetComponent<TextMeshProUGUI>().text =
                    "Water: " + gameManager.GetComponent<GameManager>().inventory["Water"];

                fabricatorInv += 1;
            }
            else
            {
                string msg = "";
                if (gameManager.GetComponent<GameManager>().inventory["Regolith"] <= 0 && gameManager.GetComponent<GameManager>().inventory["Water"] > 0)
                {
                    msg = "Not enough regolith";
                }
                else if (gameManager.GetComponent<GameManager>().inventory["Water"] <= 0 && gameManager.GetComponent<GameManager>().inventory["Regolith"] > 0)
                {
                    msg = "Not enough water";
                }
                else if (gameManager.GetComponent<GameManager>().inventory["Water"] <= 0 && gameManager.GetComponent<GameManager>().inventory["Regolith"] <= 0)
                {
                    msg = "Not enough water, regolith";
                }
                gameManager.GetComponent<GameManager>().DoErrorMessage(msg, 5f);
            }
        }

        collectMsg = "Collect " + fabricatorOut.ToString() + " Concrete";


        // Press C to collect concrete
        if (Input.GetKeyDown(KeyCode.C) && playerInRange && gameManager.GetComponent<GameManager>().buildMode == false)
        {
            gameManager.GetComponent<GameManager>().inventory["Concrete"] += fabricatorOut;
            gameManager.GetComponent<GameManager>().concreteCount.GetComponent<TextMeshProUGUI>().text =
                "Concrete: " + gameManager.GetComponent<GameManager>().inventory["Concrete"];
            fabricatorOut = 0;
            gameManager.GetComponent<GameUiManager>().ShowInteractTooltip("F", "Mix Concrete");
            collectMsg = "Collect " + fabricatorOut.ToString() + " Concrete";
        }

        // Only want to start the coroutine once, and only when there's resources in it, and only when its powered
        if (fabricatorInv >= 1 && !isCuring
            && GetComponent<BuildableObj>().addon.transform.GetComponent<BuildableObj>().addonType == "PowerGen")
        {
            Debug.Log(1);
            StartCoroutine(CureConcrete());
            isCuring = true;
            cureLight.SetActive(true);
        }

    }

    IEnumerator CureConcrete()
    {
        while (fabricatorInv >= 1)
        {
            yield return new WaitForSeconds(1f);
            fabricatorInv -= 1;
            fabricatorOut += 1;
            collectMsg = "Collect " + fabricatorOut.ToString() + " Concrete";

            if (playerInRange)
            {
                if (fabricatorOut <= 0) { gameManager.GetComponent<GameUiManager>().ShowInteractTooltip("F", "Mix Concrete"); }
                else { gameManager.GetComponent<GameUiManager>().ShowInteractTooltip("F/C", "Mix Concrete/Collect " + fabricatorOut + " Concrete"); }
            }
        }
        StopCoroutine(CureConcrete());
        isCuring = false;
        cureLight.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (fabricatorOut <= 0) { gameManager.GetComponent<GameUiManager>().ShowInteractTooltip("F", "Mix Concrete"); }
            else { gameManager.GetComponent<GameUiManager>().ShowInteractTooltip("F/C", "Mix Concrete/" + collectMsg); }
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
