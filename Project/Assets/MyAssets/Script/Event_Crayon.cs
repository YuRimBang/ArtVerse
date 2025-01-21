using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class Event_Crayon : MonoBehaviour
{
    public AudioSource crayonSound;

    public XRBaseController left;
    public XRBaseController right;

    private InputDevice leftDevice;
    private InputDevice rightDevice;

    private GameObject leftObj;
    private GameObject rightObj;

    private string crayonTag = "Crayon";

    void Start()
    {
        leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    protected void Update()
    {
        if(leftObj != null && leftObj.CompareTag(crayonTag) && isTrigger(leftDevice))
        {
            handleSprayEvent(left);
        }
        else if (rightObj != null && rightObj.CompareTag(crayonTag) && isTrigger(rightDevice))
        {
            handleSprayEvent(right);
        }
        
        //스프레이 트리거 버튼 해제했을 때 소리 멈추도록
        else 
        {
            if (crayonSound.isPlaying)
            {
                crayonSound.Stop();
            }
        }
    }

    private void handleSprayEvent(XRBaseController controller)
    {
        controller.SendHapticImpulse(0.1f, 0.1f); //진동

        if(!crayonSound.isPlaying)
        {
            crayonSound.Play(); //소리
        }
    }

    public void setLeftObj(GameObject crayon)
    {
        if(crayon != null)
        {
            leftObj = crayon;
        }
        else
        {
            leftObj = null;
        }
    }

    public void setRightObj(GameObject crayon)
    {
        if(crayon != null)
        {
            rightObj = crayon;
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
