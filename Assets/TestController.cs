using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour {

    public SteamVR_TrackedObject trackedObj;
    public SteamVR_Controller.Device device;

    //Teleporter
    //The laser we see on the screen
    private LineRenderer laser;

    //where we are aiming at
    public GameObject teleportAimerObject;

    //location where we will teleport to
    public Vector3 teleportLocation;

    //Our gameobject in the game world
    public GameObject player;

    //layers with which the laser can collide
    public LayerMask laserMask;

    //Amount the cylinder is over the surface
    public float yNudgeAmount = 1f;

    //Dash
    public float dashSpeed = 0.1f;
    private bool isDashing;
    private float lerpTime;
    private Vector3 dashStartPosition;

    //Walking
    public Transform playerCam;
    public float moveSpeed = 4f;
    private Vector3 movementDirection;


    // Use this for initialization
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        laser = GetComponentInChildren<LineRenderer>();
        Debug.Log("Start works");

    }

    // Update is called once per frame
    void Update()
    {
        //Left Touch trigger- primary index trigger 
        //Right TOuch Trigger- Secondary index trigger
        //if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger)) 
        //{
        //	movementDirection = playerCam.transform.forward;
        //Keeping the y at zero because we dont want the player to fly
        //	movementDirection = new Vector3 (movementDirection.x, 0, movementDirection.z);
        //	movementDirection *= moveSpeed * Time.deltaTime;
        //Update position of player every frame
        //	player.transform.position += movementDirection;
        //	Debug.Log ("Button down detected");
        //}

        //For Dash and Transport system, this conditional is true when the player is
        //dashing and false when the player has reached its location
        if (isDashing)
        {
            lerpTime = Time.deltaTime * dashSpeed;
            player.transform.position = Vector3.Lerp(dashStartPosition, teleportLocation, 1);
            if (lerpTime >= 1)
            {
                isDashing = false;
                lerpTime = 0;
            }
        }
        else
        {
            //GetPress- as long the button is held down the conditional is returned true
            //GetPressDown- only the frame on which the button is pressed down
            if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            {
                Debug.Log("Press Detected");
                laser.gameObject.SetActive(true);
                teleportAimerObject.SetActive(true);

                //Sets the origin of the laser at the hand controller
                laser.SetPosition(0, gameObject.transform.position);

                //Determining the laser's end point and our teleport location using raycasting
                //RaycastHit creates a variable in which to store the ray information
                RaycastHit hit;

                //transform.position- origin transform of the ray, transform.forward- direction of ray,
                //out- outputs the hit object so that we can read information of it, 15- range
                //laserMask- variable, used to know which layers to collide with
                //Summary- This conditional will return true if the laser collides with an object 
                //in the forward direction, within 15 meters from the origin of the ray
                if (Physics.Raycast(transform.position, transform.forward, out hit, 15, laserMask))
                {
                    teleportLocation = hit.point;
                    laser.SetPosition(1, teleportLocation);
                    //AimerPosition
                    teleportAimerObject.transform.position = new Vector3(teleportLocation.x, teleportLocation.y + yNudgeAmount, teleportLocation.z);
                    Debug.Log("It hit something and teleported you there");
                }
                else
                {
                    teleportLocation = transform.position + (transform.forward * 15);
                    Debug.Log("Running the else statement");
                    RaycastHit groundRay;
                    if (Physics.Raycast(teleportLocation, -Vector3.up, out groundRay, 17, laserMask))
                    {
                        teleportLocation = new Vector3(transform.forward.x * 15 + transform.position.x, groundRay.point.y, transform.forward.z * 15 + transform.position.z);
                        Debug.Log("Running the if statement inside the else");
                    }
                    laser.SetPosition(1, transform.forward * 15 + transform.position);
                    //aimer position
                    teleportAimerObject.transform.position = teleportLocation + new Vector3(0, yNudgeAmount, 0);
                }
            }

            //GetPressUp- only the frame the button was released
            //GetPressDown- only the frame on which the button is pressed down
            if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            {
                laser.gameObject.SetActive(false);
                teleportAimerObject.SetActive(false);
                //player.transform.position = teleportLocation;
                dashStartPosition = player.transform.position;
                isDashing = true;
            }
        }
    }

}
