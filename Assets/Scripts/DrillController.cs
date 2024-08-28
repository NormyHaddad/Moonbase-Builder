using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrillController : MonoBehaviour
{
    public string collectMessage;
    public GameObject gameManager;
    public LayerMask oreLayer;
    public int storageLimit;
    public ParticleSystem drillFX;

    GameObject player;
    public GameObject addon;
    public GameObject powerIndicator;
    public Material on;
    string oreType;
    int oreStorage;
    bool playerInRange;
    bool canMine;
    bool isMining;
    bool isPowered = false;
    public bool isWaterExtractor;

    void Start()
    {
        gameManager = GetComponent<BuildableObj>().gameManager;
        playerInRange = false;  
        oreStorage = 0;
        canMine = false;
        isMining = false;
        collectMessage = "";
    }

    // Update is called once per frame
    void Update()
    {
        addon = gameObject.GetComponent<BuildableObj>().addon;

        // If the drill is powered, indicate it.
        if (addon != null && addon.GetComponent<BuildableObj>().addonType == "PowerGen")
        {
            powerIndicator.GetComponent<MeshRenderer>().material = on;
            isPowered = true;
        }

        // Check if drill is above ore
        Ray newRay = new Ray(transform.position + new Vector3(0, 1, 0), new Vector3(0, -1.5f, 0));
        Debug.DrawRay(transform.position + new Vector3(0, 1, 0), new Vector3(0, -1.5f, 0), Color.magenta, 1f);
        RaycastHit hit;

        // If the ray hits an ore, the drill is not currently mining, the drill is powered, and the drill has enough storage
        if (Physics.Raycast(newRay, out hit, 1, oreLayer) && !isMining && oreStorage < storageLimit && isPowered)
        {
            Debug.Log(hit.transform.tag);
            oreType = hit.transform.GetComponent<Ore>().oreType;

            // If the drill is a water extractor, and the targeted ore is water
            if (isWaterExtractor && hit.transform.GetComponent<Ore>().oreType == "Water")
            {
                canMine = true;
                StartCoroutine(CollectOre());
                isMining = true;
                if (drillFX != null) { drillFX.Play(); }
            }

            // If the drill is a normal drill, and the targeted ore is not water
            if (!isWaterExtractor && hit.transform.GetComponent<Ore>().oreType != "Water")
            {
                canMine = true;
                StartCoroutine(CollectOre());
                isMining = true;
                if (drillFX != null) { drillFX.Play(); }
            }
        }

        if (oreStorage >= storageLimit)
        {
            canMine = false;
            StopCoroutine(CollectOre());
            isMining = false;
            if (drillFX != null) { drillFX.Stop(); }
        }

        if (Input.GetKeyDown(KeyCode.C) && playerInRange)
        {
            gameManager.GetComponent<GameManager>().inventory[oreType] += oreStorage;
            if (oreType == "Iron Ore")
            { gameManager.GetComponent<GameManager>().ironOreCount.GetComponent<TextMeshProUGUI>().text =
                    "Iron Ore: " + gameManager.GetComponent<GameManager>().inventory["Iron Ore"]; }            
            if (oreType == "Quartz")
            { gameManager.GetComponent<GameManager>().quartzCount.GetComponent<TextMeshProUGUI>().text =
                    "Quartz: " + gameManager.GetComponent<GameManager>().inventory["Quartz"]; }
            if (oreType == "Water")
            { gameManager.GetComponent<GameManager>().waterCount.GetComponent<TextMeshProUGUI>().text =
                    "Water: " + gameManager.GetComponent<GameManager>().inventory["Water"]; }

            oreStorage = 0;
            collectMessage = "Press C to collect " + oreStorage + " " + oreType;
            gameManager.GetComponent<GameUiManager>().ShowInteractTooltip("C", collectMessage);
        }
    }

    IEnumerator CollectOre()
    {
        while (oreStorage < storageLimit)
        {
            yield return new WaitForSeconds(1f);
            oreStorage += 1;
            collectMessage = "Press C to collect " + oreStorage + " " + oreType;
            // Dont show the tooltip if the player isn't in range
            if (playerInRange) { gameManager.GetComponent<GameUiManager>().ShowInteractTooltip("C", collectMessage); }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            gameManager.GetComponent<GameUiManager>().ShowInteractTooltip("C", collectMessage);
            playerInRange = true;
            if (gameManager == null)
            { gameManager = collision.gameObject.GetComponent<PlayerInteractions>().gameManager; }
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            playerInRange = false;
            gameManager.GetComponent<GameUiManager>().HideInteractTooltip();
        }
    }
}
