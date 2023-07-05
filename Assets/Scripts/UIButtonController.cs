using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject gameManager;
    public GameObject buildableObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        string name = buildableObj.GetComponent<BuildableObj>().name;
        List<string> materials = buildableObj.GetComponent<BuildableObj>().materials;
        foreach (string i in materials)
            Debug.Log(i);
        List<int> amount = buildableObj.GetComponent<BuildableObj>().amount;
        gameManager.GetComponent<GameManager>().ShowBuildTooltip(name, materials, amount);
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        gameManager.GetComponent<GameManager>().HideBuildTooltip();
    }
}
