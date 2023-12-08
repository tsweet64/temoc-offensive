using UnityEngine;
using Photon.Pun;
using System.IO;
using System;
//Player manager's job is to instantiate each player gameobject
public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        
        Debug.Log("instantiate player");
        GameObject player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), Vector3.zero, Quaternion.identity);

        //fun idea: give each player their own cool color
        player.GetComponent<PlayerController>().color = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 0.75f, 1);

        player.GetComponent<PlayerController>().Respawn();
    }
}
