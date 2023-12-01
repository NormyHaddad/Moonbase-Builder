using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject gameManager;
    public GameObject buildableObj;

    public AudioSource hover;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        gameManager.GetComponent<GameManager>().mouseOverButton = true;
        hover.Play();
        if (buildableObj != null)
        {
            string name = buildableObj.GetComponent<BuildableObj>().name;
            List<string> materials = buildableObj.GetComponent<BuildableObj>().materials;
            List<int> amount = buildableObj.GetComponent<BuildableObj>().amount;
            gameManager.GetComponent<GameManager>().ShowBuildTooltip(name, materials, amount);
        }
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (gameManager != null)
        { gameManager.GetComponent<GameManager>().HideBuildTooltip(); gameManager.GetComponent<GameManager>().mouseOverButton = false; }
    }
}
