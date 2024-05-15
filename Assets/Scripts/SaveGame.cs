using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SaveGame : MonoBehaviour
{
    // Classes
    [System.Serializable]
    public class ObjectIndex
    {
        public string Name;
        public GameObject Object;
    }

    public List<ObjectIndex> objs;

    [System.Serializable]
    public class WorldObjProperties
    {
        public string obj;
        public string addon;
        public JsonVector pos;
        public float rot;
    }

    [System.Serializable]
    public class PlayerData
    {
        public JsonVector position;
        public Dictionary<string, int> inventory;
        public PlayerData(Vector3 pos, Dictionary<string, int> inv)
        {
            position = new JsonVector(pos.x, pos.y, pos.z);
            inventory = inv;
        }
    }

    [System.Serializable]
    public class WorldObjects
    {
        public List<WorldObjProperties> list;
    }

    [System.Serializable]
    public class JsonVector
    {
        public float x;
        public float y;
        public float z;
        public JsonVector(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
        }
    }

    [System.Serializable]
    public class SaveData
    {
        public PlayerData playerData;
        public WorldObjects worldObjects;
        public SaveData(PlayerData data, WorldObjects objects)
        {
            playerData = data;
            worldObjects = objects;
        }
    }

    // Variables
    public GameObject gameManager;

    public List<WorldObjProperties> loadedObjects;
    public WorldObjects worldObjects;

    private void Start()
    {
        worldObjects = new WorldObjects();
        worldObjects.list = new List<WorldObjProperties>();

        if(PlayerPrefs.GetString("Load Data") == "yes")
        {
            LoadGame();
        }
    }

    public GameObject FindObj(string name)
    {
        foreach (ObjectIndex i in objs)
        {
            if (name == i.Name)
            {
                return i.Object;
            }
        }
        Debug.Log("No match found");
        return null;
    }

    public void AddObjToList(GameObject obj)
    {
        Debug.Log("Adding " + obj.name + " to list");
        WorldObjProperties temp = new WorldObjProperties();
        Debug.Log("Created temp");
        temp.obj = obj.GetComponent<BuildableObj>().objName;
        Debug.Log("Assigned obj");
        temp.pos = new JsonVector(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);
        Debug.Log("Assigned pos " + obj.transform.position);
        temp.rot = obj.transform.rotation.y;
        Debug.Log("Assigned rot");
        if (obj.GetComponent<BuildableObj>().addon != null)
        {
            temp.addon = obj.GetComponent<BuildableObj>().addon.GetComponent<BuildableObj>().objName;
            Debug.Log("Assigned addon");
        }
        else
        {
            temp.addon = "none";
        }
        if (worldObjects.list != null)
            worldObjects.list.Add(temp);
        else
            Debug.Log("list is null");
        Debug.Log("Added temp to list");
    }

    public void SaveGameState(Vector3 playerPos, Dictionary<string, int> inventory)
    {
        // Here, all the data needed to save is compiled into one class for serialization.
        Debug.Log(playerPos);
        Debug.Log(inventory);
        Debug.Log(worldObjects.list);

        // Create the PlayerData subclass, the WorldObjects subclass already exists
        PlayerData pData = new PlayerData(playerPos, inventory);
        WorldObjects objectList = worldObjects;

        // Compile both into the main class, then pass for saving
        SaveData data = new SaveData(pData, worldObjects);
        SaveDataToFile(data, "world_objects.json");
    }

    SaveData LoadDataFromFile(string filePath)
    {
        Debug.Log($"Checking file existence: {filePath}");
        if (File.Exists(filePath))
        {
            Debug.Log("File found");
            try
            {
                JObject jsonData;
                using (StreamReader file = File.OpenText("world_objects.json"))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    jsonData = (JObject)JToken.ReadFrom(reader);
                }
                SaveData saveData = jsonData.ToObject<SaveData>();
                return saveData;
            }
            catch (System.Exception e)
            {
                Debug.Log($"Error loading data from file: {e.Message}");
                return null;
            }
        }
        else
        { Debug.Log($"File not found: {filePath}"); return null; }
    }

    public void LoadGame()
    {
        Debug.Log("LoadGame() called");
        if (PlayerPrefs.GetString("Loaded") != "yes")
        {
            PlayerPrefs.SetString("Loaded", "yes");
            SaveData loadedData = LoadDataFromFile("world_objects.json");

            // Check if the loaded data is not null load it into the scene
            if (loadedData != null)
            {
                foreach (WorldObjProperties i in loadedData.worldObjects.list)
                {
                    GameObject clone = Instantiate(FindObj(i.obj), new Vector3(i.pos.x, i.pos.y, i.pos.z), Quaternion.identity);
                    clone.transform.Rotate(0f, i.rot, 0f);
                    if (clone.GetComponent<HabController>() != null)
                    {
                        clone.GetComponent<HabController>().gameManager = gameManager;
                        clone.GetComponent<HabController>().CheckConnections();
                    }

                    // lf the building has an airlock
                    if (clone.GetComponent<AirlockController>() != null)
                    { clone.GetComponent<AirlockController>().gameManager = gameManager; }

                    // If the building is a fuel tank
                    if (clone.GetComponent<BuildableObj>().objType == "Fuel storage")
                    {
                        gameManager.GetComponent<GameManager>().fuelStorage += clone.GetComponent<BuildableObj>().storage;
                    }

                    // If the object has an addon
                    if (i.addon != "none")
                    {
                        GameObject addon = Instantiate(FindObj(i.addon), clone.GetComponent<BuildableObj>().addonPos.position, Quaternion.identity);
                        addon.transform.SetParent(clone.transform);
                        clone.GetComponent<BuildableObj>().addon = addon;
                    }
                    AddObjToList(clone);
                }
                gameManager.GetComponent<GameManager>().inventory = loadedData.playerData.inventory;
                JsonVector pos = loadedData.playerData.position;
                gameManager.GetComponent<GameManager>().player.transform.position = new Vector3(pos.x, pos.y, pos.z);
                gameManager.GetComponent<GameManager>().UpdateGUI();
            }
            else
            {
                Debug.Log("Failed to load data from file.");
            }
        }
        else { Debug.Log("Data previously loaded"); }
    }

    void SaveDataToFile(SaveData data, string filePath)
    {
        if (data != null)
        {
            string jsonData = JsonConvert.SerializeObject(data);
            File.WriteAllText(filePath, jsonData);
            Debug.Log(jsonData);
            Debug.Log("Data saved to file: " + filePath);
        }
        else
        {
            Debug.Log("data is null");
            Debug.Log(data);
        }
    }
}
