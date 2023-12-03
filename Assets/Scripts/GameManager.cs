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
    public GameObject ironOreCount;
    public GameObject quartzCount;
    public GameObject metalCount;
    public GameObject glassCount;
    public GameObject iceCount;
    public GameObject errorMessage;
    public GameObject buildInfo;
    public Vector3 tooltipOffset;

    public AudioSource thud;

    GameObject clone;
    public bool buildMode = false;
    public bool building = false;

    public bool mouseOverButton = false;

    public Dictionary<string, int> inventory = new Dictionary<string, int>();

    bool errorIsActive = false;
    float errorCounter;
    Vector3 pos;
    Ray ray;

    private void Start()
    {
        // New inventory system
        inventory.Add("Iron Ore", 100);
        inventory.Add("Metal", 100);
        inventory.Add("Quartz", 100);
        inventory.Add("Glass", 0);
        inventory.Add("Ice", 0);

        // Game screens
        gameUI.SetActive(true);
        buildScreen.SetActive(false);

        // Inventory stuff
        //inventory["Ore"] = 0;
        //inventory["Metal"] = 0;

        ironOreCount.GetComponent<TextMeshProUGUI>().text = "Iron Ore: " + inventory["Iron Ore"];
        metalCount.GetComponent<TextMeshProUGUI>().text = "Metal: " + inventory["Metal"];
        quartzCount.GetComponent<TextMeshProUGUI>().text = "Quartz: " + inventory["Quartz"];
        glassCount.GetComponent<TextMeshProUGUI>().text = "Glass: " + inventory["Glass"];
        iceCount.GetComponent<TextMeshProUGUI>().text = "Ice: " + inventory["Ice"];
    }

    private void Update()
    {
        // If the tooltip is active, make it follow the mouse
        if (buildInfo.activeSelf)
        {
            buildInfo.transform.position = Input.mousePosition + tooltipOffset;
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
                if (!clone.GetComponent<BuildableObj>().isAddon)
                {
                    // If the building IS clipping into other buildings
                    if (clone.GetComponent<BuildableObj>().isColliding)
                    {
                        DoErrorMessage("Object cannot be placed inside other buildings");
                    }

                    // If the building is NOT clipping into other buildings, and if the click isn't the player choosing a new object
                    if (!clone.GetComponent<BuildableObj>().isColliding && !mouseOverButton)
                    { 
                        // Now the building is successfully placed
                        clone.GetComponent<BuildableObj>().isBuilt = true;
                        building = false;
                        thud.Play();

                        // if the building is a hab
                        if (clone.GetComponent<HabController>() != null)
                        {
                            clone.GetComponent<HabController>().CheckConnections();
                        }

                        // Update the GUI
                        List<string> placedMaterials = clone.GetComponent<BuildableObj>().materials;
                        List<int> placedAmounts = clone.GetComponent<BuildableObj>().amount;

                        foreach (string item in clone.GetComponent<BuildableObj>().materials) // Update the player inventory
                        {
                            if (item == "Iron Ore")
                            {
                                inventory["Iron Ore"] -= placedAmounts[placedMaterials.IndexOf("Iron Ore")];
                                ironOreCount.GetComponent<TextMeshProUGUI>().text = "Iron Ore: " + inventory["Iron Ore"];
                            }

                            if (item == "Glass")
                            {
                                inventory["Glass"] -= placedAmounts[placedMaterials.IndexOf("Glass")];
                                glassCount.GetComponent<TextMeshProUGUI>().text = "Glass: " + inventory["Glass"];
                            }

                            if (item == "Metal")
                            {
                                inventory["Metal"] -= placedAmounts[placedMaterials.IndexOf("Metal")];
                                metalCount.GetComponent<TextMeshProUGUI>().text = "Metal: " + inventory["Metal"];
                            }

                            if (item == "Quartz")
                            {
                                inventory["Quartz"] -= placedAmounts[placedMaterials.IndexOf("Quartz")];
                                quartzCount.GetComponent<TextMeshProUGUI>().text = "Quartz: " + inventory["Quartz"];
                            }
                        }

                        if (ironOreCount != null)
                            ironOreCount.GetComponent<TextMeshProUGUI>().text = "Iron Ore: " + inventory["Iron Ore"];
                    }
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
                            if (item == "Iron Ore")
                            {
                                inventory["Iron Ore"] -= placedAmounts[placedMaterials.IndexOf("Iron Ore")];
                                ironOreCount.GetComponent<TextMeshProUGUI>().text = "Iron Ore: " + inventory["Iron Ore"];
                            }

                            if (item == "Glass")
                            {
                                inventory["Glass"] -= placedAmounts[placedMaterials.IndexOf("Glass")];
                                glassCount.GetComponent<TextMeshProUGUI>().text = "Glass: " + inventory["Glass"];
                            }

                            if (item == "Metal")
                            {
                                inventory["Metal"] -= placedAmounts[placedMaterials.IndexOf("Metal")];
                                metalCount.GetComponent<TextMeshProUGUI>().text = "Metal: " + inventory["Metal"];
                            }

                            if (item == "Quartz")
                            {
                                inventory["Quartz"] -= placedAmounts[placedMaterials.IndexOf("Quartz")];
                                quartzCount.GetComponent<TextMeshProUGUI>().text = "Quartz: " + inventory["Quartz"];
                            }
                        }
                    }
                }
            }
        }
    }

    public void DoErrorMessage(string message, float timeToDecay = 5f)
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
        GameObject nameUI = buildInfo.transform.Find("Name").gameObject;
        GameObject materialsUI = buildInfo.transform.Find("Materials").gameObject;
        nameUI.GetComponent<TextMeshProUGUI>().text = name;
        string mats = "";

        // Iterate through the materials and form them into a string
        foreach (int num in amount)
        {
            mats += materials[amount.IndexOf(num)];
            mats += ": ";
            mats += num.ToString();
            mats += "\n";
        }
        mats.Remove(mats.Length - 1); // Remove the last unneeded new line
        materialsUI.GetComponent<TextMeshProUGUI>().text = mats;
    }

    public void HideBuildTooltip()
    {
        buildInfo.SetActive(false);
    }

    public void BuildObj(GameObject objToBuild)
    {
        // Check if you have enough materials to build it
        bool enoughMaterials = CanBuildObj(objToBuild);

        if (enoughMaterials == true) // If you have enough to build it
        {
            if (clone != null && !clone.GetComponent<BuildableObj>().isBuilt)
            { Destroy(clone); }
            // Initially spawn it here so it doesn't spawn a new one every frame
            ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("Ground"))
            {
                pos = hit.transform.position;
            }
            buildMode = true;
            clone = Instantiate(objToBuild, pos, Quaternion.identity);
            clone.GetComponent<BuildableObj>().gameManager = gameObject;
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
