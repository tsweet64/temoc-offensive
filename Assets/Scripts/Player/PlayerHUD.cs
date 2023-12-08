using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviourPun, IPunObservable
{
    string playerTag;
    [SerializeField] Image healthBar;
    [SerializeField] TMP_Text playerName;
    [SerializeField] Target player;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsReading)
        {
            playerTag = (string) stream.ReceiveNext();
            playerName.text = playerTag;
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
            // gameObject.SetActive(false);
            foreach(Renderer r in gameObject.GetComponentsInChildren<Renderer>())
            {
                r.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(FindObjectOfType<Camera>().transform);
        //look x rotation
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);

        float health = player.health;
        healthBar.rectTransform.localScale = new Vector3(health / 100, healthBar.rectTransform.localScale.y, healthBar.rectTransform.localScale.z);
        healthBar.rectTransform.localPosition = new Vector3(health / 100 - 1, healthBar.rectTransform.localPosition.y, 0);

    }
}
