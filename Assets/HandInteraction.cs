using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteraction : MonoBehaviour
{
    public SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
    public float throwForce = 1.5f;

    //Swipe
    public float swipeSum;
    public float touchLast;
    public float touchCurrent;
    public float distance;
    public bool hasSwipedLeft;
    public bool hasSwipedRight;
    public ObjectMenuManager objectMenuManager;

    // Use this for initialization
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update()
    {
        device = SteamVR_Controller.Input((int)trackedObj.index);
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            touchLast = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
        }
        if (device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
        {
            touchCurrent = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
            distance = touchCurrent - touchLast;
            touchLast = touchCurrent;
            swipeSum += distance;
            if (swipeSum > 0.5f & !hasSwipedRight)
            {
                swipeSum = 0;
                SwipeRight();
                hasSwipedRight = true;
                hasSwipedLeft = false;
            }
            if (swipeSum < -0.5f & !hasSwipedLeft)
            {
                swipeSum = 0;
                SwipeLeft();
                hasSwipedRight = false;
                hasSwipedLeft = true;
            }
        }
        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            swipeSum = 0;
            touchCurrent = 0;
            touchLast = 0;
            hasSwipedLeft = false;
            hasSwipedRight = false;

        }
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            //Spawn object currently selected by menu
            SpawnObject();
        }
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {

        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.CompareTag("Throwable"))
        {
            if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                ThrowObject(col);
            }
            else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                GrabObject(col);
            }
        }
    }
    void GrabObject(Collider coli)
    {
        //make controller its parent
        coli.transform.SetParent(gameObject.transform);
        //turn off physics
        coli.GetComponent<Rigidbody>().isKinematic = true;
        //vibrate controller
        device.TriggerHapticPulse(2000);
        Debug.Log("Object grabbed!");
    }
    void ThrowObject(Collider coli)
    {
        //unparent controller
        coli.transform.SetParent(null);
        Rigidbody rb = coli.GetComponent<Rigidbody>();
        //turn on physics
        rb.isKinematic = false;
        //set velocity based on controller movement
        rb.velocity = device.velocity * throwForce;
        rb.angularVelocity = device.angularVelocity;
        Debug.Log("Object thrown!");
    }

    public void SwipeLeft()
    {
        objectMenuManager.MenuLeft();
        Debug.Log("Swiped left!");
    }
    public void SwipeRight()
    {
        objectMenuManager.MenuRight();
        Debug.Log("Swiped right!");
    }

    void SpawnObject()
    {
        objectMenuManager.SpawnCurrentObject();
    }
    void nextLevel()
    {
        objectMenuManager.SwitchLevel();
    }
}
    
