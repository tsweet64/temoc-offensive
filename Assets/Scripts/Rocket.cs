using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float velocity = 10;
    
    Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        direction = transform.TransformDirection(Vector3.forward);
        transform.GetComponent<Rigidbody>().velocity = direction * velocity;

       //auto-destory rocket after 10 seconds
        StartCoroutine(DestroyAfterTenSeconds());
    }


    IEnumerator DestroyAfterTenSeconds()
    {
        yield return new WaitForSeconds(10);
        PhotonNetwork.Destroy(transform.gameObject);
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider col) {
        if(col.GetComponent<Rocket>())
            return;

        if(col.GetComponent<Target>()) {
            col.GetComponent<PhotonView>().RPC("registerHit", RpcTarget.All, 50f);
        }
        PhotonNetwork.Destroy(transform.gameObject);
    }
}
