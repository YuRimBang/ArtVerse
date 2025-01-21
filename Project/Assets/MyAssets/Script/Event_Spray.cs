using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class Event_Spray : MonoBehaviour
{
    public AudioSource spraySound;

    public XRBaseController left;
    public XRBaseController right;

    private InputDevice leftDevice;
    private InputDevice rightDevice;

    private GameObject leftObj;
    private GameObject rightObj;

    private string sprayTag = "Spray";

    void Start()
    {
        leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    protected void Update()
    {
        if(leftObj != null && leftObj.CompareTag(sprayTag) && isTrigger(leftDevice))
        {
            handleSprayEvent(left);
        }
        else if (rightObj != null && rightObj.CompareTag(sprayTag) && isTrigger(rightDevice))
        {
            handleSprayEvent(right);
        }
        
        //스프레이 트리거 버튼 해제했을 때 소리 멈추도록
        else 
        {
            if (spraySound.isPlaying)
            {
                spraySound.Stop();
            }
        }
    }

    private void handleSprayEvent(XRBaseController controller)
    {
        controller.SendHapticImpulse(0.7f, 0.1f); //진동

        if(!spraySound.isPlaying)
        {
            spraySound.Play(); //소리
        }
    }

    public void setLeftObj(GameObject spray)
    {
        if(spray != null)
        {
            leftObj = spray;
        }
        else
        {
            leftObj = null;
        }
    }

    public void setRightObj(GameObject spray)
    {
        if(spray != null)
        {
            rightObj = spray;
        }
        else
        {
            rightObj = null;
        }
    }
    
    private bool isTrigger(InputDevice device) 
    {

        if (device.isValid && device.TryGetFeatureValue(CommonUsages.triggerButton, out bool isTriggerPressed))
        {
            return isTriggerPressed;
        }
        return false;
    }
}
