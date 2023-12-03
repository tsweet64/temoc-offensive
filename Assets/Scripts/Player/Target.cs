using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

//handles objects that can take damage and die, such as the player
public class Target : MonoBehaviourPun, IPunObservable
{
    // public static Material damageMaterial;
    float health = 100;
    float overlayTimer = 0;
    HitTargetAnim hitOverlay;
    TMP_Text healthText;


    void Start()
    {
        hitOverlay = GetComponentInChildren<HitTargetAnim>();
        if(photonView.IsMine && GetComponent<PlayerController>())
            healthText = GetComponentInChildren<TMP_Text>();
    }

    void Update()
    {
        // GetComponent<Renderer>().material.color =  overlayTimer > 0 ? Color.red : originalColor;
        hitOverlay.setOverlayTime(overlayTimer);
        overlayTimer = Mathf.Max(overlayTimer - Time.deltaTime, 0);

        if(photonView.IsMine && GetComponent<PlayerController>())
            healthText.text = string.Format("Health: {0:N2}/100", health);
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
        // PhotonNetwork.Destroy(gameObject);
        //TODO: figure out what to do upon death
        health = 100;
        transform.position = Vector3.zero;
    }
}
