using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUiManager : MonoBehaviour
{
    public List<GameObject> HotbarIconsObj;
    public List<GameObject> hotbarTools;
    public List<string> HotbarIconsKey;
    public GameObject highlighter;

    public void UpdateHotbarIndicator(string key)
    {
        highlighter.transform.position = HotbarIconsObj[HotbarIconsKey.IndexOf(key)].transform.position;
        foreach (GameObject tool in hotbarTools)
        {
            tool.SetActive(false);
        }
        hotbarTools[HotbarIconsKey.IndexOf(key)].SetActive(true);
    }
}

