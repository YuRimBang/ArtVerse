using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARUIManager : MonoBehaviour
{
    public GameObject uiCanvas;

    private bool check = false;

    // Trigger 버튼을 눌렀을 때 호출
    public void OnActivated()
    {
        if (uiCanvas != null && !uiCanvas.activeSelf)
        {
            check = true;
            uiCanvas.SetActive(check);
        }
    }

    // Trigger 버튼을 놓았을 때 호출
    public void OnDeactivated()
    {
        if (uiCanvas != null && uiCanvas.activeSelf)
        {
            check = false;
            uiCanvas.SetActive(check);
        }
    }

}
