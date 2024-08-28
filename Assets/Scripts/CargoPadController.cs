using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CargoPadController : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject player;
    public GameObject plumeLight;
    public GameObject ship;
    public AudioSource rumble;
    public Animation animations;
    public ParticleSystem plume;

    // UI
    public GameObject orderScreen;
    public GameObject orderText;
    public GameObject selectedResourceText;
    public GameObject infoText;

    bool playerInScreenMode;

    // Gameplay values
    public int capacity;
    public int cooldownTime;

    public Dictionary<string, int> order = new Dictionary<string, int>();
    public Dictionary<string, int> cargo = new Dictionary<string, int>();

    string selectedResource;
   
    // LAUNCHED = when the order is first initially made
    // COOLDOWN = while the ship is collecting cargo
    // RETURNING = fully loaded and on its way back with cargo
    // LANDED = ready to collect cargo
    // READY = cargo collected and ready to get a new order
    public enum states
    {
        LAUNCHED,
        COOLDOWN,
        RETURNING,
        LANDED,
        READY
    }
    states state;


    bool playerInRange = false;


    // Start is called before the first frame update
    void Start()
    {
        state = states.READY;
        playerInScreenMode = false;
        orderScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            if (state == states.LANDED)
            { gameManager.GetComponent<GameUiManager>().ShowInteractTooltip("C", "Collect Resources"); }
            else { gameManager.GetComponent<GameUiManager>().ShowInteractTooltip("F", "Open Screen"); }

            if (Input.GetKeyDown(KeyCode.F) && !gameManager.GetComponent<GameManager>().buildMode && state != states.LANDED) // Dont enter this screen while in build mode or while the ship has cargo ready to collect
            {
                EnterCargoScreen();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitCargoScreen();
            }
            if (Input.GetKeyDown(KeyCode.C))
            { 
                if (state == states.LANDED)
                {
                    // Collect order
                    foreach (KeyValuePair<string, int> item in order)
                    {
                        gameManager.GetComponent<GameManager>().inventory[item.Key] += item.Value;
                    }
                    gameManager.GetComponent<GameManager>().UpdateGUI();
                    UpdateOrderGUI();
                   state = states.READY;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
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

    public void EnterCargoScreen()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerInScreenMode = true;
        gameManager.GetComponent<GameUiManager>().orderScreen.SetActive(true);
        gameManager.GetComponent<GameManager>().playerMovementLock = true;
        UpdateOrderGUI();
    }

    public void ExitCargoScreen()
    {
        Cursor.lockState= CursorLockMode.Locked;
        Cursor.visible = false;
        playerInScreenMode = false;
        gameManager.GetComponent<GameUiManager>().orderScreen.SetActive(false);
        gameManager.GetComponent<GameManager>().playerMovementLock = false;
    }

    public void UpdateOrderGUI()
    {
        string msg = "";
        if (order.Count > 0)
        {
            foreach (KeyValuePair<string, int> item in order)
            {
                msg += item.Key + ": " + item.Value.ToString() + "\n";
            }
            msg.Remove(msg.Length - 1);
        }
        orderText.GetComponent<TextMeshProUGUI>().text = msg;
    }

    public void ChooseResource(string resource)
    { 
        selectedResource = resource;
        selectedResourceText.GetComponent<TextMeshProUGUI>().text = selectedResource;
    }

    public void AddResource()
    {
        if (state == states.READY)
        {
            int total = 0;
            foreach (KeyValuePair<string, int> item in order)
            {
                total += item.Value;
            }

            if (total < capacity)
            {
                if (order.ContainsKey(selectedResource)) // If order does not yet contain the chosen resource
                { order[selectedResource] += 1; }
                else
                { order.Add(selectedResource, 1); }
                infoText.GetComponent<TextMeshProUGUI>().text = "";
            }
            else
            {
                infoText.GetComponent<TextMeshProUGUI>().text = "MAXIMUM NUMBER OF ITEMS REACHED";
            }
            UpdateOrderGUI();
        }
    }

    public void SubtractResource() 
    {
        if (state == states.READY)
        {
            if (order.ContainsKey(selectedResource))
            {
                infoText.GetComponent<TextMeshProUGUI>().text = "";
                if (order[selectedResource] > 0)
                {
                    order[selectedResource]--;
                }

                // If the resource chosen gets to 0, remove it from the order
                if (order[selectedResource] <= 0)
                {
                    order.Remove(selectedResource);
                }
            }
            UpdateOrderGUI();
        }
    }

    public void LaunchSequence()
    { StartCoroutine(Takeoff()); }

    IEnumerator Takeoff()
    {
        state = states.LAUNCHED;
        rumble.Play();
        animations.Play("CargoshipTakeoff");
        plume.Play();
        plumeLight.SetActive(true);
        yield return new WaitForSeconds(20);
        state = states.COOLDOWN;
        rumble.Stop();
        ship.SetActive(false);
        yield return new WaitForSeconds(cooldownTime);
        state = states.RETURNING;
        StartCoroutine(Land());
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
}
