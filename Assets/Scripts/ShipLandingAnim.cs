using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipLandingAnim : MonoBehaviour
{
    public float reduceSpeedHeight;
    public float endHeight;
    public float descentSpeed;
    public float decel;

    public AudioSource engineRumble;

    public List<ParticleSystem> plumesToToggle;
    public List<GameObject> lightsToToggle;

    float speed;
    // Start is called before the first frame update
    void Start()
    {
        speed = descentSpeed;
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
            StartCoroutine(LoadScene());
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Main");
    }
}