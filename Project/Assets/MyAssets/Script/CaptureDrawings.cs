using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.Android;

public class CaptureDrawings : MonoBehaviour
{
    public Camera camera; // 사용자 시야를 렌더링하는 XR 카메라
    public int imageSize = 1024; // 캡처 이미지 크기 (정사각형)
    public AudioSource cameraSound;

    private CameraClearFlags originalClearFlags;
    private Color originalBackgroundColor; 

    void Start()
    {
        // XR 카메라가 설정되지 않은 경우, 메인 카메라를 기본값으로 사용
        if (camera == null)
        {
            camera = Camera.main;
        }

        originalClearFlags = camera.clearFlags;
        originalBackgroundColor = camera.backgroundColor;
    }

    public void capture()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
        StartCoroutine(CaptureScene());
    }

    private IEnumerator CaptureScene()
    {
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.white;

        yield return new WaitForEndOfFrame();

        // RenderTexture 생성
        RenderTexture renderTexture = new RenderTexture(imageSize, imageSize, 24);
        camera.targetTexture = renderTexture;

        // 카메라 렌더링
        Texture2D screenshot = new Texture2D(imageSize, imageSize, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = renderTexture;

        // 캡처한 내용을 Texture2D에 복사
        screenshot.ReadPixels(new Rect(0, 0, imageSize, imageSize), 0, 0);
        screenshot.Apply();

        // 파일 저장
        string path = Application.persistentDataPath + "/Pictures/";
        string fileName = System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        File.WriteAllBytes(path + fileName, screenshot.EncodeToPNG());
        cameraSound.Play(); //사진 저장이 완료되면 카메라 소리 내주기

        // RenderTexture 및 카메라 복구
        camera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        camera.clearFlags = originalClearFlags;
        camera.backgroundColor = originalBackgroundColor;


        // 메모리 해제
        Destroy(screenshot);
    }
}
