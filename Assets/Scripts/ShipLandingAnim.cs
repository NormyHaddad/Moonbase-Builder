using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ShipLandingAnim : MonoBehaviour
{
    public float reduceSpeedHeight;
    public float endHeight;
    public float descentSpeed;
    public float decel;

    public AudioSource engineRumble;

    public List<ParticleSystem> plumesToToggle;
    public List<GameObject> lightsToToggle;

    public GameObject mainMenu;
    public GameObject mainText;
    public List<GameObject> buttonsToFade;

    float speed;
    bool fadeStarted;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;

        fadeStarted = false;
        speed = descentSpeed;

        PlayerPrefs.SetString("Loaded", "no");

        foreach (GameObject button in buttonsToFade) 
        {
            button.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(0, speed, 0) * Time.deltaTime;
        if (transform.position.y <= reduceSpeedHeight && speed > 0.25f)
        {
            speed -= decel * Time.deltaTime;
        }

        if (transform.position.y <= endHeight)
        {
            transform.position = new Vector3(transform.position.x, endHeight, transform.position.z);
            foreach (GameObject light in lightsToToggle)
            {
                light.SetActive(false);
            }            
            foreach (ParticleSystem plume in plumesToToggle)
            {
                plume.Stop();
                engineRumble.Stop();
            }
            if (!fadeStarted)
            {
                StartCoroutine(FadeInMenu());
                fadeStarted = true;
            }
            foreach (GameObject button in buttonsToFade)
            {
                button.SetActive(true);
            }
        }
    }

    public void LoadScene(string loadData)
    {
        if (loadData == "yes")
        { PlayerPrefs.SetString("Load Data", "yes"); }
        else
        { PlayerPrefs.SetString("Load Data", "no"); }

        SceneManager.LoadScene("Outpost Artemis");
    }
    public void Quit()
    {
        Application.Quit();
    }

    IEnumerator FadeInMenu()
    {
        mainMenu.SetActive(true);

        Color mainTextColor = mainText.GetComponent<TextMeshProUGUI>().color;
        Color buttonColor = buttonsToFade[0].GetComponent<Button>().image.color;
        Color buttonTextColor = buttonsToFade[0].GetComponentInChildren<TextMeshProUGUI>().color;

        yield return new WaitForSeconds(1.5f);
        for (float i = 0; i <= 1; i += Time.deltaTime / 2)
        {
            // set color with i as alpha
            mainText.GetComponent<TextMeshProUGUI>().color = new Color(mainTextColor.r, mainTextColor.g, mainTextColor.b, i);

            foreach (GameObject button in buttonsToFade)
            {
                button.GetComponent<Button>().image.color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, i);
                button.GetComponentInChildren<TextMeshProUGUI>().color = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, i);
                yield return null;
            }
        }
    }
}