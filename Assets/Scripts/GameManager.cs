using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Camera cam;
    public float gravity;
    public GameObject player;

    // UI
    public GameObject buildScreen;
    public GameObject pauseScreen;
    public GameObject gameUI;
    public Vector3 tooltipOffset;

    // Inventory UI
    public GameObject regolithCount;
    public GameObject ironOreCount;
    public GameObject quartzCount;
    public GameObject concreteCount;
    public GameObject metalCount;
    public GameObject glassCount;
    public GameObject waterCount;
    public GameObject fuelCount;
    public GameObject fertilizerCount;
    public GameObject errorMessage;
    public GameObject buildInfo;

    // Other UI
    public GameObject foodCount;
        
    // Saving
    public List<GameObject> builtObjs;
    public GameObject worldSaver;

    // Build mode
    GameObject clone;
    public bool buildMode = false;
    public bool building = false;

    // Gameplay
    public int fuelStorage;
    public bool isPaused = false;
    public bool playerMovementLock;
    public int maxPopulation;
    public int population;

    public bool mouseOverButton = false;

    public Dictionary<string, int> inventory = new Dictionary<string, int>();

    public AudioSource thud;

    bool errorIsActive = false;
    float errorCounter;
    Vector3 pos;
    Ray ray;

    private void Start()
    {
        Time.timeScale = 1;
        population = 0;

        // New inventory system
        inventory.Add("Regolith", 0);
        inventory.Add("Iron Ore", 0);
        inventory.Add("Concrete", 0);
        inventory.Add("Metal", 10);
        inventory.Add("Quartz", 0);
        inventory.Add("Glass", 0);
        inventory.Add("Water", 0);
        inventory.Add("Fuel", 0);
        inventory.Add("Fertilizer", 25);

        // These are essential, separate from the inventory resources
        inventory.Add("Food", 10);

        // Game screens
        gameUI.SetActive(true);
        buildScreen.SetActive(false);
        pauseScreen.SetActive(false);

        regolithCount.GetComponent<TextMeshProUGUI>().text = "Regolith: " + inventory["Regolith"];
        ironOreCount.GetComponent<TextMeshProUGUI>().text = "Iron Ore: " + inventory["Iron Ore"];
        concreteCount.GetComponent<TextMeshProUGUI>().text = "Concrete: " + inventory["Concrete"];
        metalCount.GetComponent<TextMeshProUGUI>().text = "Metal: " + inventory["Metal"];
        quartzCount.GetComponent<TextMeshProUGUI>().text = "Quartz: " + inventory["Quartz"];
        glassCount.GetComponent<TextMeshProUGUI>().text = "Glass: " + inventory["Glass"];
        waterCount.GetComponent<TextMeshProUGUI>().text = "Water: " + inventory["Water"];
        fuelCount.GetComponent<TextMeshProUGUI>().text = "Fuel: " + inventory["Fuel"] + "/" + fuelStorage;
        fertilizerCount.GetComponent<TextMeshProUGUI>().text = "Fertilizer: " + inventory["Fertilizer"];

        foodCount.GetComponent<TextMeshProUGUI>().text = "Food: " + inventory["Food"];

        // Limit FPS to prevent excess GPU/CPU usage
        Application.targetFrameRate = 60;

        playerMovementLock = false;
    }

    private void Update()
    {
        // If the tooltip is active, make it follow the mouse
        if (buildInfo.activeSelf)
        {
            buildInfo.transform.position = Input.mousePosition + tooltipOffset;
        }

        // If the error message is visible
        if(errorIsActive)
        {
            errorCounter -= Time.deltaTime;
            if (errorCounter <= 0)
            {
                errorIsActive = false;
                errorMessage.SetActive(false);
            }
        }

        // Key Inputs
        if (Input.GetKeyDown(KeyCode.B) && !isPaused)
            EnterBuildMode();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (buildMode)
                ExitBuildMode();

            else if (!buildMode && !isPaused && !playerMovementLock)
                PauseGame();

            else if (isPaused)
                UnpauseGame();
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.S))
            { SaveGame(); }
        }

        if (building && buildMode)
        {
            LayerMask mask = LayerMask.GetMask("Ground");
            ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (clone.GetComponent<BuildableObj>().isAddon) // If the object is an addon
            {
                if (Physics.Raycast(ray, out hit, 100))
                {
                    // If the ray hits an object
                    if (hit.transform.GetComponent<BuildableObj>() != null)
                    {
                        // If the object has an addon position
                        if (hit.transform.GetComponent<BuildableObj>().addonPos != null)
                        {
                            pos = hit.transform.GetComponent<BuildableObj>().addonPos.position;
                        }

                        // If it does not
                        else 
                        {
                            pos = new Vector3(Mathf.Round(4 * hit.point.x) / 4, hit.point.y, Mathf.Round(4 * hit.point.z) / 4);
                        }
                    }

                    //If the ray does not hit a building
                    if (hit.transform.GetComponent<BuildableObj>() == null) 
                    { 
                        pos = new Vector3(Mathf.Round(4 * hit.point.x) / 4, hit.point.y, Mathf.Round(4 * hit.point.z) / 4); 
                    } 
                }
            }
            else // If the object is not an addon
            {
                if (Physics.Raycast(ray, out hit, 100, mask))
                {
                    pos = new Vector3(
                        Mathf.Round(2 * hit.point.x) / 2, 
                        Mathf.Round(hit.point.y) / 2, 
                        Mathf.Round(2 * hit.point.z) / 2
                        );
                }
            }


            if (clone != null) // null checking
                clone.transform.position = pos;
            if (Input.GetKeyDown(KeyCode.R))
                clone.transform.Rotate(new Vector3(0, 90, 0));

            if (Input.GetMouseButtonDown(0)) // Place obj
            {
                // If the object is NOT an addon, ie, a standalone object
                if (!clone.GetComponent<BuildableObj>().isAddon)
                {
                    // If the building IS clipping into other buildings
                    if (clone.GetComponent<BuildableObj>().isColliding)
                    { DoErrorMessage("Object cannot be placed inside other buildings"); }

                    // If the building is NOT clipping into other buildings, and if the click isn't the player choosing a new object
                    if (!clone.GetComponent<BuildableObj>().isColliding && !mouseOverButton)
                    { 
                        // Now the building is successfully placed
                        clone.GetComponent<BuildableObj>().isBuilt = true;
                        building = false;
                        thud.Play();
                        builtObjs.Add(clone);
                        //worldSaver.GetComponent<SaveGame>().AddObjToList(clone);

                        // Assign GameManager to the scrips on certain objects when built, and do other actions
                        // if the building is a hab
                        if (clone.GetComponent<HabController>() != null)
                        {
                            clone.GetComponent<HabController>().gameManager = gameObject;
                            clone.GetComponent<HabController>().CheckConnections();
                            maxPopulation += clone.GetComponent<HabController>().housingCapacity;
                        }

                        // if the building has an airlock
                        if (clone.GetComponent<AirlockController>() != null)
                        {
                            clone.GetComponent<AirlockController>().gameManager = gameObject;
                        }

                        // if the object is a refinery
                        if (clone.GetComponent<FuelRefinerController>() != null)
                        {
                            clone.GetComponent<FuelRefinerController>().gameManager = gameObject;
                        }

                        // if the object is a greenhouse
                        if (clone.GetComponent<GreenhouseController>() != null)
                        {
                            clone.GetComponent<GreenhouseController>().gameManager = gameObject;
                        }

                        UpdateInventory(clone.GetComponent<BuildableObj>().materials, clone.GetComponent<BuildableObj>().amount);

                        UpdateGUI();

                        // If the built object is a fuel tank
                        if (clone.GetComponent<BuildableObj>().objType == "Fuel storage")
                        {
                            fuelStorage += clone.GetComponent<BuildableObj>().storage;
                            fuelCount.GetComponent<TextMeshProUGUI>().text = "Fuel: " + inventory["Fuel"] + "/" + fuelStorage;
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

                    else if (hit.transform.GetComponent<BuildableObj>() != null) // If it is placed on an object
                    {
                        if (hit.transform.GetComponent<BuildableObj>().addon != null) // If it already has an addon
                        {
                            DoErrorMessage("Object already has an addon");
                        }
                        else
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
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Quit game");
        SaveGame();
        PlayerPrefs.SetString("Loaded", "no");
        PlayerPrefs.SetString("Load Data", "no");
    }

    public void LoadScene(string sceneName)
    { SceneManager.LoadScene(sceneName); }

    public void QuitGame()
    { Application.Quit(); }

    public void SaveGame()
    {
        if (builtObjs != null)
        {
            foreach (GameObject obj in builtObjs)
            {
                Debug.Log(obj);
                if (worldSaver.GetComponent<SaveGame>() != null && obj != null)
                    worldSaver.GetComponent<SaveGame>().AddObjToList(obj);
                Debug.Log("Added world objects to list");
                
            }
        }
        Debug.Log("Attempting to save");
        worldSaver.GetComponent<SaveGame>().SaveGameState(player.transform.position, inventory);
        Debug.Log("Saving complete");
    }

    public void UpdateGUI()
    {
        regolithCount.GetComponent<TextMeshProUGUI>().text = "Regolith: " + inventory["Regolith"];
        ironOreCount.GetComponent<TextMeshProUGUI>().text = "Iron Ore: " + inventory["Iron Ore"];
        concreteCount.GetComponent<TextMeshProUGUI>().text = "Concrete: " + inventory["Concrete"];
        glassCount.GetComponent<TextMeshProUGUI>().text = "Glass: " + inventory["Glass"];
        metalCount.GetComponent<TextMeshProUGUI>().text = "Metal: " + inventory["Metal"];
        quartzCount.GetComponent<TextMeshProUGUI>().text = "Quartz: " + inventory["Quartz"];
        fertilizerCount.GetComponent<TextMeshProUGUI>().text = "Fertilizer: " + inventory["Fertilizer"];
    }

    // A function to get this big block of code outside of the main update loop and reduce clutter
    public void UpdateInventory(List<string> materials, List<int> amount)
    {
        foreach (string item in materials) // Update the player inventory
        {
            if (item == "Regolith")
            { inventory["Regolith"] -= amount[materials.IndexOf("Regolith")]; }
            if (item == "Iron Ore")
            { inventory["Iron Ore"] -= amount[materials.IndexOf("Iron Ore")]; }
            if (item == "Concrete")
            { inventory["Concrete"] -= amount[materials.IndexOf("Concrete")]; }
            if (item == "Glass")
            { inventory["Glass"] -= amount[materials.IndexOf("Glass")]; }
            if (item == "Metal")
            { inventory["Metal"] -= amount[materials.IndexOf("Metal")]; }
            if (item == "Quartz")
            { inventory["Quartz"] -= amount[materials.IndexOf("Quartz")]; }
            if (item == "Fertilizer")
            { inventory["Fertilizer"] -= amount[materials.IndexOf("Fertilizer")]; }
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

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
        gameUI.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
        gameUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowBuildTooltip(string name, List<string> materials, List<int> amount)
    {
        buildInfo.SetActive(true);
        GameObject nameUI = buildInfo.transform.Find("Name").gameObject;
        GameObject materialsUI = buildInfo.transform.Find("Materials").gameObject;
        nameUI.GetComponent<TextMeshProUGUI>().text = name;
        string mats = "";

        // Iterate through the materials and form them into a string
        foreach (string mat in materials)
        {
            //mats += materials[amount.IndexOf(num)];
            //mats += ": ";
            //mats += num.ToString();
            //mats += "\n";
            mats += mat;
            mats += ": ";
            mats += amount[materials.IndexOf(mat)];
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
