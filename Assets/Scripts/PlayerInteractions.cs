using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteractions : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject oreCount;
    public Camera cam;
    public GameObject lights;

    bool lightsOn;
    Ray ray;
    int oreInventory;

    // Start is called before the first frame update
    void Start()
    {
        //gameManager.GetComponent<GameManager>().inventory["Ore"] = 0;
        //gameManager.GetComponent<GameManager>().inventory["Metal"] = 0;
        //if (oreCount != null)
        //    oreCount.GetComponent<TextMeshProUGUI>().text = "Ore: 0";
        //oreCount.text = "Ore: " + oreInventory;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 5))
            {

                if (hit.transform.CompareTag("Ore"))
                {
                    gameManager.GetComponent<GameManager>().inventory["Ore"]++;
                    if (oreCount != null)
                    {
                        oreCount.GetComponent<TextMeshProUGUI>().text = "Ore: " + gameManager.GetComponent<GameManager>().inventory["Ore"];
                        //gameManager.GetComponent<GameManager>().inventory["Ore"] += 1;
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (lights.activeSelf == false)
            {
                lights.SetActive(true);
            }
            else if (lights.activeSelf == true)
            {
                lights.SetActive(false);
            }
        }
    }
}
