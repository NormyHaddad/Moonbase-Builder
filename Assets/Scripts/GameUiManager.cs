using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUiManager : MonoBehaviour
{
    public List<GameObject> HotbarIconsObj;
    public List<GameObject> hotbarTools;
    public List<string> HotbarIconsKey;
    public GameObject highlighter;

    public GameObject tooltipParent;
    public GameObject tooltipButton;
    public GameObject tooltipText;

    public GameObject orderScreen;


    public void UpdateHotbarIndicator(string key)
    {
        highlighter.transform.position = HotbarIconsObj[HotbarIconsKey.IndexOf(key)].transform.position;
        foreach (GameObject tool in hotbarTools)
        {
            tool.SetActive(false);
        }
        hotbarTools[HotbarIconsKey.IndexOf(key)].SetActive(true);
    }

    public void ShowInteractTooltip(string button, string message)
    {
        tooltipParent.SetActive(true);
        tooltipButton.GetComponent<TextMeshProUGUI>().text = button;
        tooltipText.GetComponent<TextMeshProUGUI>().text = message;
    }
    public void HideInteractTooltip()
    {
        tooltipParent.SetActive(false);
    }

    public void ChangeScreenResolution(int val)
    {
        if (val == 0)
            Screen.SetResolution(2560, 1440, true);
        if (val == 1)
            Screen.SetResolution(1920, 1080, true);
        if (val == 2)
            Screen.SetResolution(1280, 720, true);
    }

    public void ToggleFullscreen(bool val)
    {
        Screen.fullScreen = val;
    }

    public void ShowMiniscreen(GameObject screen)
    {
        screen.SetActive(true);
    }

    public void HideMiniscreen(GameObject screen)
    {
        screen.SetActive(false);
    }
}

