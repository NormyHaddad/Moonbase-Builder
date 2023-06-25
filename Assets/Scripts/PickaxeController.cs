using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeController : MonoBehaviour
{
    public bool isMining;
    public Animation swing;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMining && !swing.isPlaying)
        {
            swing.Play();
        }

        if (!isMining)
        {
            swing.Stop();
            swing.Rewind();
        }
    }
}
