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
            if (Physics.Raycast(ray, out hit, 100, mask))
            {
                pos = new Vector3(Mathf.Round(2 * hit.point.x) / 2, Mathf.Round(2 * hit.point.y) / 2, Mathf.Round(2 * hit.point.z) / 2);
            }
            if (clone != null) // Godamn null checking for no reason
                clone.transform.position = pos;
            if (Input.GetKeyDown(KeyCode.R))
                clone.transform.Rotate(new Vector3(0, 90, 0));

            if (Input.GetMouseButtonDown(0)) // Place obj
            {
                clone.GetComponent<BuildableObj>().isBuilt = true;
                building = false;
                //oreInventory -= clone.GetComponent<BuildableObj>().buildCost;

                // Update the GUI
                List<string> placedMaterials = clone.GetComponent<BuildableObj>().materials;
                List<int> placedAmounts = clone.GetComponent<BuildableObj>().amount;

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

                //inventory["Ore"] -= clone.GetComponent<BuildableObj>().buildCost;// Multi-item crafting, still in testing

                if (oreCount != null)
                    oreCount.GetComponent<TextMeshProUGUI>().text = "Ore: " + inventory["Ore"];
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

    public void BuildObj(GameObject objToBuild)
    {
        bool enoughMaterials = CanBuildObj(objToBuild);

        // Testing the new system
        if (enoughMaterials == true)
            print("Success");
        else
            print("Failure");

        // If you don't have enough to build it
        if (enoughMaterials == false) //inventory["Ore"] < objToBuild.GetComponent<BuildableObj>().buildCost) 
        {
            //DoErrorMessage("Not enough materials", 5f);
        }

        else if (enoughMaterials == true) // If you have enough to build it
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
