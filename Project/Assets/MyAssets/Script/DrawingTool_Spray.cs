using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class DrawingToolSpary : MonoBehaviour
{
    public GameObject drawingSurface;   // 그림을 그릴 표면
    public GameObject brushMeshPrefab;  // 브러시 메시 프리팹
    public Material currentMaterial;    // 현재 선택된 도구의 재질
    private float drawingSpeed = 0.0005f;   // 그림 그릴 속도
    public float currentBrushSize = 0.05f;  // 기본 브러시 두께

    public XRBaseController leftController;
    public XRBaseController rightController;

    private InputDevice leftDevice;
    private InputDevice rightDevice;

    private GameObject leftObj;
    private GameObject rightObj;

    private bool isLeftDrawing = false;
    private bool isRightDrawing = false;

    private Vector3 lastLeftPosition;
    private Vector3 lastRightPosition;

    private string sprayTag = "Spray";

    void Start()
    {
        leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    void Update()
    {
        // 왼손 그림
        if (leftObj != null && leftObj.CompareTag(sprayTag) && IsTriggerPressed(leftDevice))
        {
            if (!isLeftDrawing)
            {
                StartDrawing(leftController, ref isLeftDrawing, ref lastLeftPosition);
            }
            UpdateDrawing(leftController, ref lastLeftPosition);
        }
        else if (isLeftDrawing)
        {
            isLeftDrawing = false;
        }

        // 오른손 그림
        if (rightObj != null && rightObj.CompareTag(sprayTag) && IsTriggerPressed(rightDevice))
        {
            if (!isRightDrawing)
            {
                StartDrawing(rightController, ref isRightDrawing, ref lastRightPosition);
            }
            UpdateDrawing(rightController, ref lastRightPosition);
        }
        else if (isRightDrawing)
        {
            isRightDrawing = false;
        }
        
    }

    public void SetLeftObj(GameObject spray)
    {
        leftObj = spray;
    }

    public void SetRightObj(GameObject spray)
    {
        rightObj = spray;
    }

    private bool IsTriggerPressed(InputDevice device)
    {
        return device.isValid && device.TryGetFeatureValue(CommonUsages.triggerButton, out bool isTriggerPressed) && isTriggerPressed;
    }

    private void StartDrawing(XRBaseController controller, ref bool isDrawing, ref Vector3 lastPosition)
    {
        isDrawing = true;
        lastPosition = controller.transform.position;
    }

    private void UpdateDrawing(XRBaseController controller, ref Vector3 lastPosition)
    {
        Vector3 currentPosition = controller.transform.position;
        if (Vector3.Distance(currentPosition, lastPosition) >= drawingSpeed)
        {
            DrawMesh(lastPosition, currentPosition);
            lastPosition = currentPosition;
        }
    }

    private void StopDrawing(ref bool isDrawing)
    {
        isDrawing = false;
    }

    private void DrawMesh(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        GameObject brushMesh = Instantiate(brushMeshPrefab, start, Quaternion.identity);
        brushMesh.transform.LookAt(end);
        brushMesh.transform.localScale = new Vector3(currentBrushSize, distance / 2, currentBrushSize);
        brushMesh.transform.position = (start + end) / 2;

        MeshRenderer meshRenderer = brushMesh.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            Material newMaterial = new Material(currentMaterial.shader)
            {
                color = currentMaterial.color
            };
            meshRenderer.material = newMaterial;
        }
    }

    public void SetDrawingColor(Material newMaterial)
    {
        currentMaterial = newMaterial;
    }

    public void SetBrushSize(float newSize)
    {
        currentBrushSize = newSize;
    }
}
