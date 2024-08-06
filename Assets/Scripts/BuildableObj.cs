using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableObj : MonoBehaviour
{
    public Transform addonPos;

    public GameObject addon;

    public GameObject gameManager;

    public string addonType;

    public string objType;
    public string objName;
    public int buildCost;
    public bool isBuilt = false;
    public bool isPowered = false;
    public bool isAddon = false;

    public bool generatesResources;
    public string resourceGenerated;
    public float generationCooldown;

    public int storage;

    public bool isColliding;

    public List<string> materials;
    public List<int> amount;

    void Update()
    {
        // If the object is powered
        if (addon != null && addon.GetComponent<BuildableObj>().addonType == "PowerGen")
        { isPowered = true; }

        if (isPowered == true ) { StartCoroutine(GenerateResource()); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Building"))
        {
            isColliding = true;
            Debug.Log("col");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Building"))
        {
            isColliding = false;
            Debug.Log("no col");
        }
    }

    // For more generic resource generators without specific conditions
    IEnumerator GenerateResource()
    {
        yield return new WaitForSeconds(generationCooldown);
        gameManager.GetComponent<GameManager>().inventory.Add(resourceGenerated, 1);
    }
}
