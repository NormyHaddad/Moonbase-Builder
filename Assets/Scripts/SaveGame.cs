using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class SaveGame : MonoBehaviour
{
    [System.Serializable]
    public class WorldObjProperties
    {
        public string obj;
        public string addon;
        public JsonVector pos;
        public float rot;
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
    }

    public List<WorldObjProperties> loadedObjects;

    public WorldObjects worldObjects;
    private void Start()
    {
        worldObjects = new WorldObjects();
        worldObjects.list = new List<WorldObjProperties>();

    }

    public void AddObjToList(GameObject obj)
    {
        Debug.Log("Adding " + obj.name + " to list");
        WorldObjProperties temp = new WorldObjProperties();
        Debug.Log("Created temp");
        temp.obj = obj.GetComponent<BuildableObj>().objName;
        Debug.Log("Assigned obj");
        if (temp.pos != null)
        {
            temp.pos = new JsonVector();
            temp.pos.x = obj.transform.position.x;
            temp.pos.y = obj.transform.position.y;
            temp.pos.z = obj.transform.position.z;
            Debug.Log("Assigned pos");
        }
        else
        {
            Debug.Log("temp.pos is null");
        }
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

    public void SaveGameState()
    {
        Debug.Log(worldObjects.list);
        SaveDataToFile(worldObjects.list, "world_objects.json");
    }

    List<WorldObjProperties> LoadDataFromFile(string filePath)
    {
        Debug.Log($"Checking file existence: {filePath}");
        if (File.Exists(filePath))
        {
            Debug.Log("File found");
            try
            {
                //using (FileStream stream = new FileStream(filePath, FileMode.Open))
                //{
                //    BinaryFormatter formatter = new BinaryFormatter();
                //    List<WorldObjProperties> loadedList = (List<WorldObjProperties>)formatter.Deserialize(stream);
                //    return loadedList;
                //}
                string jsonData = File.ReadAllText(filePath);
                List<WorldObjProperties> loadedList = JsonUtility.FromJson<List<WorldObjProperties>>(jsonData);
                return loadedList;
            }
            catch (System.Exception e)
            {
                Debug.Log($"Error loading data from file: {e.Message}");
                return null;
            }
        }
        else
        {
            Debug.Log($"File not found: {filePath}");
            return null;
        }
    }

    public void LoadGame()
    {
        Debug.Log("LoadGame() called");

        List<WorldObjProperties> loadedList = LoadDataFromFile("world_objects.json");

        // Check if the loaded list is not null and print the data
        if (loadedList != null)
        {
            Debug.Log($"Loaded List Count: {loadedList.Count}");

            foreach (WorldObjProperties i in loadedList)
            {
                Debug.Log($"Object: {i.obj}, Position: {i.pos}, Position: {i.pos}");
            }
        }
        else
        {
            Debug.Log("Failed to load data from file.");
        }
    }

    void SaveDataToFile(List<WorldObjProperties> data, string filePath)
    {
        if (data != null)
        {
            foreach (WorldObjProperties i in data)
            {

                Debug.Log($"Object: {i.obj}, Position: {i.pos}, Position: {i.pos} saved");
            }
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
