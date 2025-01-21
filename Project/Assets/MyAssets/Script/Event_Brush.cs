using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class Event_Brush : MonoBehaviour
{
    public AudioSource brushSound;

    public XRBaseController left;
    public XRBaseController right;

    private InputDevice leftDevice;
    private InputDevice rightDevice;

    private GameObject leftObj;
    private GameObject rightObj;

    private string brushTag = "Brush";

    void Start()
    {
        leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    protected void Update()
    {
        if(leftObj != null && leftObj.CompareTag(brushTag) && isTrigger(leftDevice))
        {
            handleSprayEvent(left);
        }
        else if (rightObj != null && rightObj.CompareTag(brushTag) && isTrigger(rightDevice))
        {
            handleSprayEvent(right);
        }
        
        //스프레이 트리거 버튼 해제했을 때 소리 멈추도록
        else 
        {
            if (brushSound.isPlaying)
            {
                brushSound.Stop();
            }
        }
    }

    private void handleSprayEvent(XRBaseController controller)
    {
        controller.SendHapticImpulse(0.1f, 0.1f); //진동

        if(!brushSound.isPlaying)
        {
            brushSound.Play(); //소리
        }
    }

    public void setLeftObj(GameObject brush)
    {
        if(brush != null)
        {
            leftObj = brush;
        }
        else
        {
            leftObj = null;
        }
    }

    public void setRightObj(GameObject brush)
    {
        if(brush != null)
        {
            rightObj = brush;
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
