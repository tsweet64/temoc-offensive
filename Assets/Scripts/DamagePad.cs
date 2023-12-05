using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

public class DamagePad : MonoBehaviour
{
    void OnTriggerStay(Collider col)
    {
        Debug.Log("Hit pad");
        if(col.gameObject.GetComponent<Target>())
        {
            col.gameObject.GetComponent<Target>().registerHit(1000);
        }
    }
}
