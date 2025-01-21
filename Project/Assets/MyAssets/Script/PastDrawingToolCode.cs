using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class DrawingTool_Crayon_L : MonoBehaviour
{
    public GameObject drawingSurface;   // 그림을 그릴 표면
    public GameObject brushMeshPrefab;  // 브러시 메시 프리팹
    public Material currentMaterial;    // 현재 선택된 도구의 재질
    public float drawingSpeed = 0.1f;   // 그림 그릴 속도
    public float currentBrushSize = 0.05f;  // 현재 브러시 두께

    private bool isDrawing = false;
    private Vector3 lastPosition;

    private XRBaseController controller;

    public Transform drawingParent;

    void Start()
    {
        controller = GetComponent<XRBaseController>();
    }

    // Trigger 버튼을 눌렀을 때 호출
    public void OnActivated()
    {
        if (!isDrawing)
        {
            StartDrawing();
        }
    }

    // Trigger 버튼을 놓았을 때 호출
    public void OnDeactivated()
    {
        if (isDrawing)
        {
            StopDrawing();
        }
    }

    // 팔레트에서 선택한 색상과 두께를 받아오는 변수
    public void SetDrawingColor(Material newMaterial)
    {
        currentMaterial = newMaterial;
    }

    public void SetBrushSize(float newSize)
    {
        currentBrushSize = newSize;
    }

    private void StartDrawing()
    {
        isDrawing = true;
        lastPosition = controller.transform.position;
    }

    private void Update()
    {
        if (isDrawing)
        {
            Draw();
        }
    }

    private void Draw()
    {
        Vector3 currentPosition = controller.transform.position;

        // 현재 위치와 이전 위치 사이에 메시를 이용한 그림 그리기
        if (currentPosition != lastPosition)
        {
            DrawMesh(lastPosition, currentPosition);
            lastPosition = currentPosition;
        }
    }

    private void DrawMesh(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        GameObject brushMesh = Instantiate(brushMeshPrefab, start, Quaternion.identity);
        brushMesh.transform.LookAt(end);

        if (drawingParent != null)
        {
            brushMesh.transform.SetParent(drawingParent);
        }

        // 브러시 크기 설정
        brushMesh.transform.localScale = new Vector3(currentBrushSize, distance / 2, currentBrushSize);
        brushMesh.transform.position = (start + end) / 2;

        // 메시의 재질을 현재 선택된 색상으로 설정
        MeshRenderer meshRenderer = brushMesh.GetComponent<MeshRenderer>();
        meshRenderer.material = currentMaterial;
    }

    private void StopDrawing()
    {
        isDrawing = false;
    }
}
