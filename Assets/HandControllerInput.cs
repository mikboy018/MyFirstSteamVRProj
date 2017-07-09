using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControllerInput : MonoBehaviour {
    public SteamVR_TrackedObject trackedObj;
    public SteamVR_Controller.Device device;
    //Teleporter
    private LineRenderer laser;
    public GameObject teleportAimerObject;
    public Vector3 teleportLocation;
    public GameObject player;
    public LayerMask laserMask; //helps to determine WHERE you can teleport
    public float yNudgeAmount = 1f; //specific to teleportAimerObject height

    //Dash
    public float dashSpeed = 0.1f;
    private bool isDashing;
    private float lerpTime;
    private Vector3 dashStartPosition;

    //
    public Transform playerCam;
    public float moveSpeed = 4f;
    private Vector3 movementDirection;

    // Use this for initialization
    void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        laser = GetComponentInChildren<LineRenderer>();
    
	}
	
	// Update is called once per frame
	void Update () {
        device = SteamVR_Controller.Input((int)trackedObj.index);
        if (device.GetPress(SteamVR_Controller.ButtonMask.Grip))
        {
            movementDirection = playerCam.transform.forward;
            movementDirection = new Vector3(movementDirection.x, 0, movementDirection.z);
            movementDirection = movementDirection * moveSpeed * Time.deltaTime;
            player.transform.position += movementDirection;
        }
        if (isDashing)
        {
            //Lerp takes 2 values and smoothes them over time based on a float variable
            lerpTime = Time.deltaTime * dashSpeed;
            player.transform.position = Vector3.Lerp(dashStartPosition, teleportLocation, lerpTime);
            if(lerpTime >-1)
            {
                isDashing = false;
                lerpTime = 0;
            }
            else
            {
                //if holding button down -- can replace with touchpad
                if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
                {
                    laser.gameObject.SetActive(true);
                    teleportAimerObject.SetActive(true);
                    laser.SetPosition(0, gameObject.transform.position);
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, transform.forward, out hit, 15, laserMask))
                    {
                        teleportLocation = hit.point;
                        laser.SetPosition(1, teleportLocation);
                        //aimer position
                        teleportAimerObject.transform.position = new Vector3(teleportLocation.x, teleportLocation.y + yNudgeAmount, teleportLocation.z);
                    }
                    else
                    {
                        teleportLocation = new Vector3(transform.forward.x * 15 + transform.position.x, transform.forward.y * 15 + transform.position.y, transform.forward.z * 15 + transform.position.z);
                        RaycastHit groundRay;
                        if (Physics.Raycast(teleportLocation, -Vector3.up, out groundRay, 17, laserMask))
                        {
                            teleportLocation = new Vector3(transform.forward.x * 15 + transform.position.x, groundRay.point.y, transform.forward.z * 15 + transform.position.z);
                        }
                        laser.SetPosition(1, transform.forward * 15 + transform.position);
                        teleportAimerObject.transform.position = teleportLocation + new Vector3(0, yNudgeAmount, 0);
                    }
                }
                //if releasing button
                if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
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
}
