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
    public bool isAddon = false;

    public int storage;

    public bool isColliding;

    public List<string> materials;
    public List<int> amount;

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
}
