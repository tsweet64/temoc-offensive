using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviourPun, IPunObservable
{
    string playerTag;


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        TMP_Text innerText = GetComponentInChildren<TMP_Text>();
        if(stream.IsReading)
        {
            playerTag = (string) stream.ReceiveNext();
            innerText.text = playerTag;
        }
        if(stream.IsWriting)
            stream.SendNext(playerTag);
    }

    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine)
        {
            playerTag = PhotonNetwork.NickName;
            GetComponentInChildren<TMP_Text>().text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(FindObjectOfType<Camera>().transform);
    }
}
