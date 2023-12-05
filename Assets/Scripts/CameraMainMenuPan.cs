using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMainMenuPan : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += Vector3.up * 0.5f * Time.deltaTime;
    }
}
