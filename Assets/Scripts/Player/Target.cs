using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//handles objects that can take damage and die, such as the player
public class Target : MonoBehaviourPun, IPunObservable
{
    // public static Material damageMaterial;
    float health = 100;
    float overlayTimer = 0;
    HitTargetAnim hitOverlay;

    void Start()
    {
        hitOverlay = GetComponentInChildren<HitTargetAnim>();
    }

    void Update()
    {
        // GetComponent<Renderer>().material.color =  overlayTimer > 0 ? Color.red : originalColor;
        hitOverlay.setOverlayTime(overlayTimer);
        overlayTimer = Mathf.Max(overlayTimer - Time.deltaTime, 0);
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsReading)
        {
            this.health = (float)stream.ReceiveNext();
            this.overlayTimer = (float)stream.ReceiveNext();
        }
        if(stream.IsWriting)
        {
            stream.SendNext(health);
            stream.SendNext(overlayTimer);
        }
    }

    [PunRPC]
    public void registerHit(float damage)
    {
        //hit by laser case
        overlayTimer += Time.deltaTime;
        hitOverlay.setOverlayTime(overlayTimer);


        health -= damage;

        if(health <= 0)
            Die();
        
    }

    public void Die()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
