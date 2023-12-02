using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using System;
//takes care of
public class PlayerController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

    //laser damage per second
    [SerializeField] float laserDamage = 30;

    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;
    Rigidbody rb;
    LineRenderer laser;

    //synced properties
    bool laserEnabled = false;
    Vector3 laserPosition0;
    Vector3 laserPosition1;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        laser = GetComponentInChildren<LineRenderer>();
        laser.enabled = false;
        if(!photonView.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        //on other clients, we have to update the position of the laser!
        if (!photonView.IsMine)
        {
            laser.enabled = laserEnabled;
            laser.SetPosition(0, laserPosition0);
            laser.SetPosition(1, laserPosition1);
            return;
        }
        Look();
        Move();
        Jump();
        Shoot();

       
    }

    void Shoot()
    {
        laser.enabled = false;

        //basic laser - ported from my other repo
        Camera playerCam = cameraHolder.GetComponentInChildren<Camera>();

        Ray ray = playerCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        // Debug.DrawLine(gunOrigin.position, Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 2)));
        if(Input.GetButton("Fire1"))
        {
            laserPosition1 = ray.origin + ray.direction * 4;
            laserPosition0 = playerCam.ScreenToWorldPoint(new Vector3(Screen.width / 8, Screen.height / 8, 0.1f));
            //if hit target
            //showing laser in debugging
            laserEnabled = Physics.Raycast(ray, out hit);
            if(laserEnabled)
            {
                if(hit.transform.GetComponent<Target>())
                {
                    //EXPLANATION:
                    //If a function is called on a networked object (IE, an object that has a PhotonView), it will only be called on the client that invoked it
                    //However, Photon has a concept of "ownership".
                    //updates to class members will only be sent to the rest of the server if they are made in the client that "owns" the object
                    //PhotonView.RPC is a way around that - it sends a network message to the client owning that particular PhotonView, and instructs it to invoke some function itself.
                    float damage = laserDamage * Time.deltaTime;
                    hit.transform.GetComponent<PhotonView>().RPC("registerHit", RpcTarget.All, damage);
                }
                laserPosition1 = hit.point;

            }
            laser.SetPosition(0, laserPosition0);
            laser.SetPosition(1, laserPosition1);
            laser.enabled = true;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(laserEnabled);
            stream.SendNext(laserPosition0);
            stream.SendNext(laserPosition1);
        }
        if(stream.IsReading)
        {
            this.laserEnabled = (bool)stream.ReceiveNext();
            this.laserPosition0 = (Vector3)stream.ReceiveNext();
            this.laserPosition1 = (Vector3)stream.ReceiveNext();
        }
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized; // normalize stops 2 ey faster

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime); // this is what google said but who i dont know whats happening but it works 
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }
    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    void FixedUpdate() // fixed interval where physics calcs should be so speed isnt tied to fps
    {
        
        if (!photonView.IsMine)
            return;

        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }
}
