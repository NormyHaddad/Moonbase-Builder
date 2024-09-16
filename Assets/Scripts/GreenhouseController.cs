using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GreenhouseController : MonoBehaviour
{
    bool isGenerating = false;
    bool enoughResources = false;
    public GameObject gameManager;

    public float productionSpeed;
    public float resourceUseTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // If there is enough water and fertilizer
        if (gameManager.GetComponent<GameManager>().inventory["Water"] > 0 && gameManager.GetComponent<GameManager>().inventory["Fertilizer"] > 0)
            enoughResources = true;

        // If either water or fertilizer has run out
        if (gameManager.GetComponent<GameManager>().inventory["Water"] <= 0 || gameManager.GetComponent<GameManager>().inventory["Fertilizer"] <= 0)
            enoughResources = false;

        if (gameObject.GetComponent<BuildableObj>().isPowered && !isGenerating && enoughResources)
        {
            StartCoroutine(UseResources());
            StartCoroutine(GenerateFood());
            isGenerating = true;
        }
    }

    IEnumerator GenerateFood()
    {
        while (enoughResources)
        {
            yield return new WaitForSeconds(productionSpeed);
            if (enoughResources)
            {
                gameManager.GetComponent<GameManager>().inventory["Food"] += 1;
                gameManager.GetComponent<GameManager>().foodCount.GetComponent<TextMeshProUGUI>().text = "Food: " + gameManager.GetComponent<GameManager>().inventory["Food"];
                Debug.Log("Food: " + gameManager.GetComponent<GameManager>().inventory["Food"]);
            }
        }
    }

    IEnumerator UseResources()
    {
        while (enoughResources)
        {
            yield return new WaitForSeconds(resourceUseTime);
            if (enoughResources)
            {
                gameManager.GetComponent<GameManager>().inventory["Water"] -= 1;
                gameManager.GetComponent<GameManager>().waterCount.GetComponent<TextMeshProUGUI>().text = "Water: " + gameManager.GetComponent<GameManager>().inventory["Water"];
                gameManager.GetComponent<GameManager>().inventory["Fertilizer"] -= 1;
                gameManager.GetComponent<GameManager>().fertilizerCount.GetComponent<TextMeshProUGUI>().text = "Fertilizer: " + gameManager.GetComponent<GameManager>().inventory["Fertilizer"];
            }

            if (!enoughResources)
            {
                Debug.Log("not enough resources");
                isGenerating = false;
                StopCoroutine(GenerateFood());
                StopCoroutine(UseResources());
            }
        }
    }
}
