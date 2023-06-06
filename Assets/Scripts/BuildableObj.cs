using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableObj : MonoBehaviour
{
    public int buildCost;
    public bool isBuilt = false;
    public bool isAddon = false;

    public List<string> materials;
    public List<int> amount;

}
