using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class DayNightCycle : MonoBehaviour
{
    public float rotation;

    // Based on the Y rotations of the sun when it rises/sets
    public float setThreshold;
    public float riseThreshold;
    public float fadeTime;
    public float transitRot; // Y angle at which it reaches its highest point

    public GameObject nightLight;

    bool aboveHorizon;
    bool hasSet;

    float earthDefaultIntensity;
    float sunDefaultIntensity;
    float baseShadowStrength;

    // Start is called before the first frame update
    void Start()
    {
        earthDefaultIntensity = nightLight.GetComponent<Light>().intensity;
        sunDefaultIntensity = gameObject.GetComponent<Light>().intensity;
        baseShadowStrength = gameObject.GetComponent<Light>().shadowStrength;

        // Before any of the cycle begins, determine where the sun is
        if (transform.rotation.eulerAngles.y < setThreshold && transform.rotation.eulerAngles.y > riseThreshold) // Sun is below
        { hasSet = true; }
        else
        { hasSet = false; }
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the shadow strength based off the sun's elevation angle
        gameObject.GetComponent<Light>().shadowStrength = Mathf.Lerp(baseShadowStrength, 1f, Mathf.Abs(Mathf.Clamp( (transitRot - transform.rotation.eulerAngles.y + 70), -70, 70)) / 70f);

        transform.Rotate(new Vector3(0f, 1, 0f), rotation * Time.deltaTime);
        // If the sun is below the horizon
        if (transform.rotation.eulerAngles.y < setThreshold && transform.rotation.eulerAngles.y > riseThreshold)
        {
            aboveHorizon = false;
            if (hasSet)
            {
                StartCoroutine(FadeOutSun());
                hasSet = false;
            }
        }
        else
        {
            aboveHorizon = true;
            if (!hasSet)
            {
                StartCoroutine(FadeInSun());
                hasSet = true;
            }
        }

        if (aboveHorizon) // Daytime
        {
            gameObject.GetComponent<Light>().shadowNormalBias = 0f;
            gameObject.GetComponent<LensFlareComponentSRP>().enabled = true;
            //nightLight.SetActive(false);
        }
        else if (!aboveHorizon) // Nighttime
        {
            gameObject.GetComponent<Light>().shadowNormalBias = 1.6f;
            gameObject.GetComponent<LensFlareComponentSRP>().enabled = false;
            //nightLight.SetActive(true);
        }
    }

    // Fade out the sun, and fade in the earthshine
    IEnumerator FadeOutSun()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            gameObject.GetComponent<Light>().shadowStrength = Mathf.Lerp(baseShadowStrength, 1f, Mathf.Clamp(elapsedTime / fadeTime, 0, 1));
            gameObject.GetComponent<Light>().intensity = Mathf.Lerp(sunDefaultIntensity, 0f, Mathf.Clamp(elapsedTime / fadeTime, 0, 1));
            nightLight.GetComponent<Light>().intensity = Mathf.Lerp(0f, earthDefaultIntensity, Mathf.Clamp(elapsedTime / fadeTime, 0, 1));
            yield return null;
        }
        gameObject.GetComponent<Light>().shadowStrength = 1;
        gameObject.GetComponent<Light>().intensity = 0f;
        nightLight.GetComponent<Light>().intensity = earthDefaultIntensity;
        Debug.Log(gameObject.GetComponent<Light>().intensity);
    }
    IEnumerator FadeInSun()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            gameObject.GetComponent<Light>().shadowStrength = Mathf.Lerp(1f, baseShadowStrength, Mathf.Clamp(elapsedTime / fadeTime, 0, 1));
            gameObject.GetComponent<Light>().intensity = Mathf.Lerp(0f, sunDefaultIntensity, Mathf.Clamp(elapsedTime / fadeTime, 0, 1));
            nightLight.GetComponent<Light>().intensity = Mathf.Lerp(earthDefaultIntensity, 0f, Mathf.Clamp(elapsedTime / fadeTime, 0, 1));
            yield return null;
        }
        gameObject.GetComponent<Light>().shadowStrength = baseShadowStrength;
        gameObject.GetComponent<Light>().intensity = sunDefaultIntensity;
        nightLight.GetComponent<Light>().intensity = 0f;
        Debug.Log(gameObject.GetComponent<Light>().intensity);
    }
}
