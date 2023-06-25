using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonController : MonoBehaviour
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

    private void OnMouseOver()
    {
        Debug.Log(1);
        string name = buildableObj.GetComponent<BuildableObj>().name;
        List<string> materials = buildableObj.GetComponent<BuildableObj>().materials;
        List<int> amount = buildableObj.GetComponent<BuildableObj>().amount;
        gameManager.GetComponent<GameManager>().ShowBuildTooltip(name, materials, amount);
    }

    private void OnMouseExit()
    {
        Debug.Log(2);
        //gameManager.GetComponent<GameManager>().HideBuildTooltip();
    }
}
