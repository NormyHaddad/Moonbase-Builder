using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowObj : MonoBehaviour
{
    public Transform objToFollow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.LookAt(objToFollow.position);
    }
}
