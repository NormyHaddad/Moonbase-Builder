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

    public int storage;

    public bool isColliding;

    public List<string> materials;
    public List<int> amount;

    void Update()
    {
        // If the object is powered
        if (addon != null && addon.GetComponent<BuildableObj>().addonType == "PowerGen")
        { isPowered = true; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Building"))
        {
            isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Building"))
        {
            isColliding = false;
        }
    }
}
