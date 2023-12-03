using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableObj : MonoBehaviour
{
    public Transform addonPos;

    public GameObject addon;

    public GameObject gameManager;

    public string addonType;

    public int buildCost;
    public bool isBuilt = false;
    public bool isAddon = false;

    public bool isColliding;

    public List<string> materials;
    public List<int> amount;

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
