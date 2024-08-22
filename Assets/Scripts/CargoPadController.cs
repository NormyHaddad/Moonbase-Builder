using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoPadController : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject player;
    public GameObject plumeLight;
    public GameObject ship;

    public GameObject orderScreen;

    public AudioSource rumble;
    public Animation animations;
    public ParticleSystem plume;

   
    public enum states
    {
        LAUNCHED,
        COOLDOWN,
        LANDED
    }
    states state;


    bool playerInRange = false;


    // Start is called before the first frame update
    void Start()
    {
        state = states.LANDED;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.F)) 
            {
                
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            gameManager.GetComponent<GameUiManager>().ShowInteractTooltip("F", "Interact");
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            gameManager.GetComponent<GameUiManager>().HideInteractTooltip();
            playerInRange = false;
            player = collision.gameObject;
        }
    }

    IEnumerator Takeoff()
    {
        state = states.LAUNCHED;
        StartCoroutine(Cooldown(states.COOLDOWN));
        rumble.Play();
        animations.Play("CargoshipTakeoff");
        plume.Play();
        plumeLight.SetActive(true);
        yield return new WaitForSeconds(20);
        rumble.Stop();
        ship.SetActive(false);
        StopCoroutine(Takeoff());
    }

    IEnumerator Land()
    {
        ship.SetActive(true);
        rumble.Play();
        animations.Play("CargoshipLanding");
        plume.Play();
        plumeLight.SetActive(true);
        yield return new WaitForSeconds(20);
        state = states.LANDED;
        rumble.Stop();
        plume.Stop();
        plumeLight.SetActive(false);
        StopCoroutine(Takeoff());
    }

    IEnumerator Cooldown(states st)
    {
        yield return new WaitForSeconds(21);
        state = st;
    }
}
