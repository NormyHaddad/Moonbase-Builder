using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HabController : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject addon;

    public GameObject lightBulb;
    public GameObject light;
    public Material on;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        addon = gameObject.GetComponent<BuildableObj>().addon;

        // If the hab is powered, indicate it.
        if (addon != null && addon.GetComponent<BuildableObj>().addonType == "PowerGen")
        {
            lightBulb.GetComponent<MeshRenderer>().material = on;
            light.SetActive(true);
        }
    }
}
