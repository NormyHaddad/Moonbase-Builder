using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAnimationController : MonoBehaviour
{
    public float rotateRate; // Degrees per second
    void Update()
    {
        transform.Rotate(0, rotateRate * Time.deltaTime, 0);
    }
}
