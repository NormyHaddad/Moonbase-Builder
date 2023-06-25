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
    public GameObject progressBar;
    public int mineSpeed;
    public AudioSource mine;
    public ParticleSystem mineFX;
    public GameObject pickaxe;

    bool lightsOn;
    Ray ray;
    int oreInventory;
    float timeMouseHeld = 0f;

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
                    // Activate the progress bar
                    progressBar.SetActive(true);
                    progressBar.GetComponent<ProgressBar>().max = mineSpeed;

                    // Start the pickaxe animation
                    pickaxe.GetComponent<PickaxeController>().isMining = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) // Deactivate the progress bar
        {
            progressBar.SetActive(false);
            timeMouseHeld = 0f;
            pickaxe.GetComponent<PickaxeController>().isMining = false;
        }

        if (Input.GetMouseButton(0))
        {
            // Mine the ore
            ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 5))
            {
                if (hit.transform.CompareTag("Ore"))
                {
                    progressBar.GetComponent<ProgressBar>().current = timeMouseHeld;
                    timeMouseHeld += Time.deltaTime;
                }
            }

            // If the ore is mined for x seconds
            if (timeMouseHeld >= progressBar.GetComponent<ProgressBar>().max)
            {
                timeMouseHeld = 0f;
                gameManager.GetComponent<GameManager>().inventory["Ore"]++;
                mine.pitch = Random.Range(0.8f, 1.2f);
                mineFX.transform.position = hit.point;
                mineFX.Play();
                mine.Play();
                if (oreCount != null)
                {
                    oreCount.GetComponent<TextMeshProUGUI>().text = "Ore: " + 
                        gameManager.GetComponent<GameManager>().inventory["Ore"];
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
