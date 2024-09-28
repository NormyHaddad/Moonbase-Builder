using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public int growthAmount; // The max amount of people that can arrive at once
    public float cooldownPeriod; // The cycle duration of usage of food/water new arrivals
    public float foodUseRate; // The amount of food and water used per minute
    public float foodProductionRate; // How much food is produced per minute
    public float waterProductionRate; // How much water is produced per minute

    public GameObject foodProductionRateUI;
    public GameObject populationUI;
    public int targetPopulation;
    bool populationGrowing;
    //public GameObject waterProductionRateUI;

    // Start is called before the first frame update
    void Start()
    {
        populationUI.GetComponent<TextMeshProUGUI>().text = "Population: " + GetComponent<GameManager>().population;
    }

    // Update is called once per frame
    void Update()
    {
        foodProductionRateUI.GetComponent<TextMeshProUGUI>().text = foodProductionRate.ToString();
        //waterProductionRateUI.GetComponent<TextMeshProUGUI>().text = waterProductionRate.ToString();

        // Calculate population growth
        // Take the food production rate, divide by food use rate to get target population (rounding down for leeway)
        targetPopulation = Mathf.FloorToInt(foodProductionRate / foodUseRate);
        // Slowly add more population until it reaches the target or maximum
        if (!populationGrowing)
        {
            populationGrowing = true;
            StartCoroutine(GrowPopulation());
        }
    }

    IEnumerator GrowPopulation()
    {
        Debug.Log("Starting population calculator");
        while (populationGrowing)
        {
            yield return new WaitForSeconds(cooldownPeriod);
            // If adding more people would exceed the population cap or target
            if (growthAmount + GetComponent<GameManager>().population > Mathf.Min(GetComponent<GameManager>().maxPopulation, targetPopulation))
            {
                // If the capacity is the lower of the 2
                if (GetComponent<GameManager>().maxPopulation < targetPopulation) 
                {
                    Debug.Log("Population would exceed capacity, adding" + (GetComponent<GameManager>().maxPopulation - GetComponent<GameManager>().population));
                    GetComponent<GameManager>().population += (Mathf.Clamp(GetComponent<GameManager>().maxPopulation - GetComponent<GameManager>().population, 0, growthAmount));
                }

                // If the target is the lower of the 2
                if (GetComponent<GameManager>().maxPopulation > targetPopulation)
                {
                    Debug.Log("Population would exceed target, adding" + (targetPopulation - GetComponent<GameManager>().population));
                    GetComponent<GameManager>().population += (Mathf.Clamp(targetPopulation - GetComponent<GameManager>().population, 0, growthAmount));
                }
                
                // If the target and cap are equal (here just for debugging)
                if (GetComponent<GameManager>().maxPopulation == targetPopulation) 
                {
                    Debug.Log("Population would exceed capacity and population, adding" + (GetComponent<GameManager>().maxPopulation - GetComponent<GameManager>().population));
                    GetComponent<GameManager>().population += (Mathf.Clamp(GetComponent<GameManager>().maxPopulation - GetComponent<GameManager>().population, 0, growthAmount));
                }
            }

            // If previous condition is not met
            else
            {
                Debug.Log("Adding " + growthAmount);
                GetComponent<GameManager>().population += growthAmount;
            }

            Debug.Log("Max Population: " + GetComponent<GameManager>().maxPopulation);
            Debug.Log("Target Population: " + targetPopulation);
            Debug.Log("Current Population: " + GetComponent<GameManager>().population);
            Debug.Log("________");

            populationUI.GetComponent<TextMeshProUGUI>().text = "Population: " + GetComponent<GameManager>().population;
        }
    }
}
