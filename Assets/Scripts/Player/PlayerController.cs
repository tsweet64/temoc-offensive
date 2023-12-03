using UnityEngine;
using Photon.Pun;
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
    Camera playerCam;

    //synced properties
    bool laserEnabled = false;
    //the point in global space that the laser is hitting
    //I am doing it this way because it enables using PhotonTransformView for this property,
    //which is much faster than sending the position as a Vector3 manually.
    //otherwise the laser will look very choppy
    public Transform laserHitpoint;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        laser = GetComponentInChildren<LineRenderer>();
        playerCam = cameraHolder.GetComponentInChildren<Camera>();
        laser.enabled = false;
        if(!photonView.IsMine)
        {
            Destroy(playerCam.gameObject);
            Destroy(rb);
            Destroy(GetComponentsInChildren<Canvas>()[0].gameObject);
            return;
        }
        GetComponentInChildren<Canvas>().worldCamera = FindObjectOfType<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        //on other clients, we have to update the position of the laser!
        if (!photonView.IsMine)
        {
            laser.enabled = laserEnabled;
            laser.SetPosition(0, cameraHolder.transform.position);
            laser.SetPosition(1, laserHitpoint.position);
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
        laserEnabled = Input.GetButton("Fire1");
        if(laserEnabled)
        {
            laser.enabled = true;
            Vector3 laserPosition0 = playerCam.ScreenToWorldPoint(new Vector3(Screen.width / 8, Screen.height / 8, 0.1f));
            laser.SetPosition(0, laserPosition0);

            //initialize laserPosition1 with the value that will be used if it is shot into space (IE, not hitting any object)
            Vector3 laserPosition1 = ray.origin + ray.direction * 4;
            //if hit target
            if(Physics.Raycast(ray, out hit))
            {
                laserPosition1 = hit.point;
                if(hit.transform.GetComponent<Target>())
                {
                    //EXPLANATION:
                    //If a function is called on a networked object (IE, an object that has a PhotonView), it will only be called on the client that invoked it
                    //However, Photon has a concept of "ownership". Unless that client happens to own that particualr object, no other clients will receive the update!
                    //updates to class members will only be sent to the rest of the server if they are made in the client that "owns" the object
                    //PhotonView.RPC is a way around that - it sends a network message to the client owning that particular PhotonView, and instructs it to invoke the function itself.
                    float damage = laserDamage * Time.deltaTime;
                    hit.transform.GetComponent<PhotonView>().RPC("registerHit", RpcTarget.All, damage);
                }

            }
            laser.SetPosition(1, laserPosition1);
            laserHitpoint.position = laserPosition1;
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if(stream.IsWriting)
        {
            stream.SendNext(laserEnabled);
        }
        if(stream.IsReading)
        {
            this.laserEnabled = (bool)stream.ReceiveNext();
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

    public void Respawn()
    {
        SpawnPoint[] spawns = FindObjectsOfType<SpawnPoint>();
        int spawnpoint = (int)UnityEngine.Random.Range(0, spawns.Length - 0.01f);
        Debug.Log(spawns.Length + " " + spawnpoint);
        transform.position = spawns[spawnpoint].transform.position;
    }
}
