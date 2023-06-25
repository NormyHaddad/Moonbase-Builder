using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Camera cam;
    public float gravity;
    public GameObject hab;
    public GameObject buildScreen;
    public GameObject gameUI;
    public GameObject oreCount;
    public GameObject metalCount;
    public GameObject errorMessage;
    public GameObject buildInfo;

    GameObject clone;
    public bool buildMode = false;
    public bool building = false;
    //public int oreInventory;
    public Dictionary<string, int> inventory = new Dictionary<string, int>();

    bool errorIsActive = false;
    float errorCounter;
    Vector3 pos;
    Ray ray;

    private void Start()
    {
        // New inventory system
        inventory.Add("Ore", 0);
        inventory.Add("Metal", 0);

        // Game screens
        gameUI.SetActive(true);
        buildScreen.SetActive(false);

        // Inventory stuff
        inventory["Ore"] = 0;
        inventory["Metal"] = 0;

        oreCount.GetComponent<TextMeshProUGUI>().text = "Ore: 0";
        metalCount.GetComponent<TextMeshProUGUI>().text = "Metal: 0";
    }

    private void Update()
    {
        if (buildInfo.activeSelf)
        {
            buildInfo.transform.position = Input.mousePosition;
        }

        if(errorIsActive)
        {
            errorCounter -= Time.deltaTime;
            if (errorCounter <= 0)
            {
                errorIsActive = false;
                errorMessage.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
            EnterBuildMode();

        if (Input.GetKeyDown(KeyCode.Escape))
            ExitBuildMode();

        if (building && buildMode)
        {
            LayerMask mask = LayerMask.GetMask("Ground");
            ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (clone.GetComponent<BuildableObj>().isAddon) // If the object is an addon
            {
                if (Physics.Raycast(ray, out hit, 100))
                {
                    // More sensitive y point rounding
                    pos = new Vector3(Mathf.Round(2 * hit.point.x) / 2, 
                        Mathf.Round(4 * hit.point.y) / 4, 
                        Mathf.Round(2 * hit.point.z) / 2);
                }
            }
            else // If the object is not an addon
            {
                if (Physics.Raycast(ray, out hit, 100, mask))
                {
                    pos = new Vector3(Mathf.Round(2 * hit.point.x) / 2, 
                        Mathf.Round(hit.point.y) / 2, 
                        Mathf.Round(2 * hit.point.z) / 2);
                }
            }


            if (clone != null) // null checking
                clone.transform.position = pos;
            if (Input.GetKeyDown(KeyCode.R))
                clone.transform.Rotate(new Vector3(0, 90, 0));

            if (Input.GetMouseButtonDown(0)) // Place obj
            {
                // If the object is NOT an addon
                if (!clone.GetComponent<BuildableObj>().isAddon) // If its a regular object
                {
                    clone.GetComponent<BuildableObj>().isBuilt = true;
                    building = false;

                    // Update the GUI
                    List<string> placedMaterials = clone.GetComponent<BuildableObj>().materials;
                    List<int> placedAmounts = clone.GetComponent<BuildableObj>().amount;

                    foreach (string item in clone.GetComponent<BuildableObj>().materials) // Update the player inventory
                    {
                        if (item == "Ore")
                        {
                            inventory["Ore"] -= placedAmounts[placedMaterials.IndexOf("Ore")];
                            oreCount.GetComponent<TextMeshProUGUI>().text = "Ore: " + inventory["Ore"];
                        }

                        if (item == "Metal")
                        {
                            inventory["Metal"] -= placedAmounts[placedMaterials.IndexOf("Metal")];
                            metalCount.GetComponent<TextMeshProUGUI>().text = "Metal: " + inventory["Metal"];
                        }
                    }

                    if (oreCount != null)
                        oreCount.GetComponent<TextMeshProUGUI>().text = "Ore: " + inventory["Ore"];
                }

                // If the object is an addon
                if (clone.GetComponent<BuildableObj>().isAddon)
                {
                    // If the addon is not placed on another object
                    if (hit.transform.GetComponent<BuildableObj>() == null)
                    {
                        DoErrorMessage("Object must be placed on another object", 6f);
                    }

                    else if (hit.transform.GetComponent<BuildableObj>() != null)
                    {
                        clone.GetComponent<BuildableObj>().isBuilt = true;
                        clone.transform.position = hit.transform.GetComponent<BuildableObj>().addonPos.transform.position;
                        hit.transform.GetComponent<BuildableObj>().addon = clone;
                        building = false;

                        // Update the GUI
                        List<string> placedMaterials = clone.GetComponent<BuildableObj>().materials;
                        List<int> placedAmounts = clone.GetComponent<BuildableObj>().amount;

                        // Update the player inventory
                        foreach (string item in clone.GetComponent<BuildableObj>().materials)
                        {
                            if (item == "Ore")
                            {
                                inventory["Ore"] -= placedAmounts[placedMaterials.IndexOf("Ore")];
                                oreCount.GetComponent<TextMeshProUGUI>().text = "Ore: " + inventory["Ore"];
                            }

                            if (item == "Metal")
                            {
                                inventory["Metal"] -= placedAmounts[placedMaterials.IndexOf("Metal")];
                                metalCount.GetComponent<TextMeshProUGUI>().text = "Metal: " + inventory["Metal"];
                            }
                        }
                    }
                }
            }
        }
    }

    public void DoErrorMessage(string message, float timeToDecay)
    {
        errorMessage.SetActive(true);
        errorMessage.GetComponent<TextMeshProUGUI>().text = message;
        errorIsActive = true;
        errorCounter = timeToDecay;
    }

    public void EnterBuildMode()
    {
        buildMode = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameUI.SetActive(false);
        buildScreen.SetActive(true);
    }

    public void ExitBuildMode()
    {
        buildMode = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameUI.SetActive(true);
        buildScreen.SetActive(false);
        if (clone != null && clone.GetComponent<BuildableObj>().isBuilt == false)
            Destroy(clone);
    }

    public void ShowBuildTooltip(string name, List<string> materials, List<int> amount)
    {
        buildInfo.SetActive(true);
    }

    public void HideBuildTooltip()
    {
        buildInfo?.SetActive(false);
    }

    public void BuildObj(GameObject objToBuild)
    {
        // Check if you have enough materials to build it
        bool enoughMaterials = CanBuildObj(objToBuild);

        if (enoughMaterials == true) // If you have enough to build it
        {
            // Initially spawn it here so it doesn't spawn a new one every frame
            ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("Ground"))
            {
                pos = hit.transform.position;
            }
            buildMode = true;
            clone = Instantiate(objToBuild, pos, Quaternion.identity);
            building = true;
        }
    }

    bool CanBuildObj(GameObject obj)
    {
        bool canBuild = true;
        int index = 0;
        int tempNum;

        string invItem;
        int itemCount;

        string missingItems = "";

        List<string> materialsNeeded = obj.GetComponent<BuildableObj>().materials;
        List<int> amountNeeded = obj.GetComponent<BuildableObj>().amount;

        foreach (string itemName in materialsNeeded) // Look through the materials needed
        {
            // Get each of the same item from the inventory and check if it contains enough of each
            if (inventory[itemName] < amountNeeded[materialsNeeded.IndexOf(itemName)])// If the inventory does not contain enough
            {
                canBuild = false;
                missingItems += itemName + ", ";
            }
            index++;
        }

        if (!canBuild)
            DoErrorMessage("Not enough " + missingItems, 5f);

        return canBuild;
    }
}
