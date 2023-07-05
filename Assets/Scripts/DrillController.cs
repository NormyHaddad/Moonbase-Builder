using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrillController : MonoBehaviour
{
    public GameObject text;
    public GameObject gameManager;
    public LayerMask oreLayer;
    public int storageLimit;
    public ParticleSystem drillFX;

    GameObject player;
    public GameObject addon;
    public GameObject powerIndicator;
    public Material on;
    int oreStorage;
    bool playerInRange;
    bool canMine;
    bool isMining;
    bool isPowered = false;

    void Start()
    {
        playerInRange = false;  
        oreStorage = 0;
        canMine = false;
        isMining = false;
        text.GetComponent<TextMeshPro>().text = "Press C to collect " + oreStorage + " ore"; // Why not?
    }

    // Update is called once per frame
    void Update()
    {
        addon = gameObject.GetComponent<BuildableObj>().addon;

        // If the hab is powered, indicate it.
        if (addon != null && addon.GetComponent<BuildableObj>().addonType == "PowerGen")
        {
            powerIndicator.GetComponent<MeshRenderer>().material = on;
            isPowered = true;
        }

        Ray newRay = new Ray(transform.position + new Vector3(0, 1, 0), new Vector3(0, -1, 0));
        Debug.DrawRay(transform.position + new Vector3(0, 1, 0), new Vector3(0, -1.5f, 0), Color.magenta, 1f);
        RaycastHit hit;
        if (Physics.Raycast(newRay, out hit, 1, oreLayer) && !isMining && oreStorage < storageLimit && isPowered)
        {
            canMine = true;
            StartCoroutine(CollectOre());
            isMining = true;
            drillFX.Play();
        }

        if (oreStorage >= storageLimit)
        {
            canMine = false;
            StopCoroutine(CollectOre());
            isMining = false;
            drillFX.Stop();
        }

        if (Input.GetKeyDown(KeyCode.C) && playerInRange)
        {
            gameManager.GetComponent<GameManager>().inventory["Ore"] += oreStorage;
            gameManager.GetComponent<GameManager>().ironOreCount.GetComponent<TextMeshProUGUI>().text =
                "Ore: " + gameManager.GetComponent<GameManager>().inventory["Ore"];
            oreStorage = 0;
            text.GetComponent<TextMeshPro>().text = "Press C to collect " + oreStorage + " ore";
        }
    }

    IEnumerator CollectOre()
    {
        while (oreStorage < storageLimit)
        {
            yield return new WaitForSeconds(1f);
            oreStorage += 1;
            text.GetComponent<TextMeshPro>().text = "Press C to collect " + oreStorage + " ore";
            Debug.Log(oreStorage);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            text.SetActive(true);
            playerInRange = true;
            //player = collision.gameObject;
            if (gameManager == null)
            {   
                Debug.Log("no game man");
                gameManager = collision.gameObject.GetComponent<PlayerInteractions>().gameManager;
            }
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            text.SetActive(false);
            playerInRange = false;
            //player = null;
        }
    }
}
