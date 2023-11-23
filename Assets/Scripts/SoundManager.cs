using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public List<AudioSource> soundtrack;
    public List<string> soundNames;
    AudioSource currentSound;
    public float maxdowntime;
    float downtime;
    bool isWaiting;
    int prevIndex;

    // Start is called before the first frame update
    void Start()
    {
        isWaiting = false;
        int randIndex = Random.Range(0, soundtrack.Count - 1);
        prevIndex = randIndex;
        currentSound = soundtrack[randIndex];
        currentSound.Play();
        Debug.Log("Now playing " + soundNames[randIndex]);
    }

    // Update is called once per frame
    void Update()
    {
        if (!currentSound.isPlaying & !isWaiting)
        {
            StartCoroutine(PlayNextSoundtrack());
            isWaiting = true;
        }
        // For testing purposes
        if (Input.GetKeyDown(KeyCode.Y))
        {
            currentSound.Stop();
            StartCoroutine(PlayNextSoundtrack());
            isWaiting = true;
        }
    }

    IEnumerator PlayNextSoundtrack()
    {
        yield return new WaitForSeconds(Random.Range(0, maxdowntime));
        int randIndex = Random.Range(0, soundtrack.Count - 1);
        while (randIndex == prevIndex)
            randIndex = Random.Range(0, soundtrack.Count - 1);
        prevIndex = randIndex;
        currentSound = soundtrack[randIndex];
        currentSound.Play();
        Debug.Log("Now playing " + soundNames[randIndex]);
        isWaiting = false;
        yield return null;
    }

}
