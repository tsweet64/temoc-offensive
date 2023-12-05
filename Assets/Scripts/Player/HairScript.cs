using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(FindObjectOfType<Camera>().transform);
        //look x rotation
        transform.localEulerAngles = new Vector3(90, transform.localEulerAngles.y, transform.localEulerAngles.z);       
    }
}
