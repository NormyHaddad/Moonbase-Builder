using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HabController : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject addon;
    public List<GameObject> connectorWalls;

    public List<GameObject> lights;
    public Material on;
    public Material off;

    public int housingCapacity;

    float distanceThreshold;

    // Start is called before the first frame update
    void Start()
    {
        distanceThreshold = 0.1f;
        CheckConnections();
    }

    // Update is called once per frame
    void Update()
    {
        addon = gameObject.GetComponent<BuildableObj>().addon;

        // If the hab is powered, indicate it.
        if (addon != null && addon.GetComponent<BuildableObj>().addonType == "PowerGen")
        {
            foreach (GameObject light in lights)
            {
                MeshRenderer renderer = light.GetComponent<Lightbulb>().bulb.GetComponent<MeshRenderer>();
                Material[] materials = renderer.materials;

                for (int i = 0; i < materials.Length; i++)
                {
                    Debug.Log(materials[i].name);
                    if (materials[i].name == "Glass (Instance)")
                    {
                        materials[i] = on;
                        break;
                    }
                }
                renderer.materials = materials;
                light.GetComponent<Lightbulb>().light.SetActive(true);
            }
        }
    }

    public void CheckConnections()
    {
        // Use raycasting to find and disable any doors that are blocking connected tunnels
        foreach (GameObject wall in connectorWalls)
        {
            Ray ray = new Ray(wall.transform.position, wall.transform.forward);
            Debug.DrawRay(wall.transform.position, wall.transform.forward * 0.25f, Color.green, 1000f);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 0.5f)) // if it hits another object
            {
                if (hit.transform.GetComponent<HabController>() != null) // if the other object is a hab
                {
                    // look through all that hab's own walls
                    foreach (GameObject wall2 in hit.transform.GetComponent<HabController>().connectorWalls)
                    {
                        // find one with a distance short enough
                        if (Vector3.Distance(wall.transform.position, wall2.transform.position) <= distanceThreshold)
                        {
                            // disable both walls
                            wall.SetActive(false);
                            wall2.SetActive(false);
                        }
                    }
                }
            }
        }
    }
}
