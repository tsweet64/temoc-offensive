using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class Powerup : MonoBehaviour
{

    public enum PowerupType {
        HEALTH,
        ROCKETS
    }

    public float rotationSpeed = 0.5f;
    public float jumpSpeed = 1;
    public float heightDelta = 0.5f;

    public float healingAmount = 25f;

    Vector3 initialPosition;
    public PowerupType type = PowerupType.HEALTH;


    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        transform.position = initialPosition + (Vector3.up * (Mathf.Sin(Time.time * jumpSpeed) * heightDelta));
        transform.eulerAngles += Vector3.up * rotationSpeed;
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.GetComponent<PlayerController>())
        {
            ActivatePowerup(col.gameObject.GetComponent<PlayerController>());
        }
    }

    void ActivatePowerup(PlayerController player)
    {
        switch(type)
        {
            case PowerupType.HEALTH:
                player.GetComponent<Target>().health = Mathf.Min(100, player.GetComponent<Target>().health + healingAmount);
                break;
            case PowerupType.ROCKETS:
                player.GetComponent<PlayerController>().rocketCount += 2;
                break;
        }

        StartCoroutine(TempDisablePowerup());
    }

    IEnumerator TempDisablePowerup()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(10);
        GetComponent<Collider>().enabled = true;
        GetComponent<Renderer>().enabled = true;
    }
}
