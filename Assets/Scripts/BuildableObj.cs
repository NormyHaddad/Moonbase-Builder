using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableObj : MonoBehaviour
{
    public Transform addonPos;

    public GameObject addon;
    public enum AddonType
    {
        none = 0,
        powerGen = 1
    };

    public int addonType;

    public int buildCost;
    public bool isBuilt = false;
    public bool isAddon = false;

    public List<string> materials;
    public List<int> amount;

}
