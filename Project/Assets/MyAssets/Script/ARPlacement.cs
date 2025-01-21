using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacement : MonoBehaviour
{
    public GameObject Pencil;  // 연필 오브젝트
    public GameObject Crayon;  // 크레용 오브젝트
    public GameObject Spray;   // 스프레이 오브젝트
    public GameObject Brush;   // 붓 오브젝트
    public GameObject Palette; // 팔레트 오브젝트

    private ARRaycastManager arRaycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool isObjectPlaced = false;

    void Start()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        // 물체가 배치되지 않은 경우에만 수행
        // if (!isObjectPlaced && Input.touchCount > 0)
        // {
             Touch touch = Input.GetTouch(0);

        //     // 터치가 시작되었을 때 평면 감지 시도
        //     if (touch.phase == TouchPhase.Began)
        //     {
                
        //     }
        // }
        // 화면에서 터치한 위치로 Raycast 수행
                if (arRaycastManager.Raycast(touch.position, hits, TrackableType.Planes))
                {
                    // 가장 가까운 평면에 히트된 위치 가져오기
                    Pose hitPose = hits[0].pose;

                    // 물체 배치
                    Instantiate(Palette, hitPose.position - new Vector3(0, 0, 0.2f), Quaternion.identity);
                    Instantiate(Pencil, hitPose.position - new Vector3(0, 0, 0.1f), Quaternion.identity);
                    Instantiate(Crayon, hitPose.position, Quaternion.identity);
                    Instantiate(Spray, hitPose.position + new Vector3(0, 0, 0.1f), Quaternion.identity);
                    Instantiate(Brush, hitPose.position + new Vector3(0, 0, 0.2f), Quaternion.identity);

                    isObjectPlaced = true; // 한 번만 배치되도록 설정
                }
    }
}
