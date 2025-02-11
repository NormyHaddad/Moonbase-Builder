using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GreenhouseController : MonoBehaviour
{
    bool isGenerating = false;
    bool enoughResources = false;
    public GameObject gameManager;

    public float productionCooldown; // In seconds
    public float resourceUseTime;

    float productionRate; // How much is produced per minute

    // Start is called before the first frame update
    void Start()
    {
        productionRate += 1 / productionCooldown * 60;
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

        if (gameObject.GetComponent<BuildableObj>().isPowered && !isGenerating && enoughResources) // Make sure to only begin the coroutine once
        {
            StartCoroutine(UseResources());
            StartCoroutine(GenerateFood());
            isGenerating = true;
            gameManager.GetComponent<PopulationManager>().foodProductionRate += productionRate;
        }
    }

    IEnumerator GenerateFood()
    {
        Debug.Log("Food generating");
        while (enoughResources)
        {
            yield return new WaitForSeconds(productionCooldown);
            if (enoughResources)
            {
                gameManager.GetComponent<GameManager>().inventory["Food"] += 1;
                gameManager.GetComponent<GameManager>().foodCount.GetComponent<TextMeshProUGUI>().text = "Food: " + gameManager.GetComponent<GameManager>().inventory["Food"];
            }
        }
    }

    IEnumerator UseResources()
    {
        Debug.Log("Resources used");
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

            if (!enoughResources) // Make sure to stop the coroutines only once
            {
                isGenerating = false;
                StopCoroutine(GenerateFood());
                StopCoroutine(UseResources());
                gameManager.GetComponent<PopulationManager>().foodProductionRate -= productionRate;
            }
        }
    }
}
