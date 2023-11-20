using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum hotbarStates
{
    pickaxe = 0,
    waterProbe = 1
}
public class PlayerInteractions : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject ironOreCount;
    public GameObject quartzCount;
    public GameObject iceCount;
    public Camera cam;
    public GameObject lights;
    public GameObject progressBar;
    public int mineSpeed;
    public AudioSource mine;
    public ParticleSystem mineFX;
    public GameObject pickaxe;
    public GameObject probe;
    public hotbarStates chosenTool;
    bool lightsOn;
    Ray ray;
    int oreInventory;
    float timeMouseHeld = 0f;

    // Start is called before the first frame update
    void Start()
    {
        chosenTool = hotbarStates.pickaxe;
        gameManager.GetComponent<GameUiManager>().UpdateHotbarIndicator("pickaxe");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { chosenTool = hotbarStates.pickaxe;
            gameManager.GetComponent<GameUiManager>().UpdateHotbarIndicator("pickaxe"); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { chosenTool = hotbarStates.waterProbe;
            gameManager.GetComponent<GameUiManager>().UpdateHotbarIndicator("probe"); }

        if (Input.GetMouseButtonDown(0))
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 5))
            {
                if (chosenTool == hotbarStates.pickaxe)
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
                if (chosenTool == hotbarStates.waterProbe)
                {
                    probe.GetComponent<ProbeAnimController>().PlayAnimation();
                    if (hit.transform.CompareTag("Ore") && hit.transform.GetComponent<Ore>().oreType == "Ice")
                    {
                        gameManager.GetComponent<GameManager>().DoErrorMessage("Found water", 3f);
                    }
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
            if (chosenTool == hotbarStates.pickaxe)
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
                    string oreType = hit.transform.GetComponent<Ore>().oreType;
                    gameManager.GetComponent<GameManager>().inventory[oreType]++;
                    mine.pitch = Random.Range(0.8f, 1.2f);
                    mineFX.transform.position = hit.point;
                    mineFX.Play();
                    mine.Play();
                    if (ironOreCount != null) {
                        ironOreCount.GetComponent<TextMeshProUGUI>().text = "Iron Ore: " +
                            gameManager.GetComponent<GameManager>().inventory["Iron Ore"]; }
                    if (quartzCount != null) {
                        quartzCount.GetComponent<TextMeshProUGUI>().text = "Quartz: " +
                            gameManager.GetComponent<GameManager>().inventory["Quartz"]; }
                    if (iceCount != null) {
                        iceCount.GetComponent<TextMeshProUGUI>().text = "Ice: " +
                            gameManager.GetComponent<GameManager>().inventory["Ice"]; }
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
