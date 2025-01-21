using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Android;
using TMPro;
using System.IO;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ResponseData
{
    public int count { get; set; }                    
    public List<float> labels { get; set; }         
    public List<float> ratios { get; set; }        

    public ResponseData()
    {
        labels = new List<float>();
        ratios = new List<float>();
        count = 0;
    }
}

public class House_YOLOInference : MonoBehaviour
{
    public GameObject resultPanel;
    public TMP_Text resultText;
    public GameObject audioObject;
    public GameObject BGM;
    public GameObject btn;

    public Camera passthroughCamera; // 패스스루 카메라 (OVR 카메라)
    private RenderTexture passthroughTexture; // 카메라 출력 텍스처
    private Texture2D inputTexture; // 모델 입력용 텍스처

    private string interpretation = "";

    private readonly string[] classNames = new string[] {
        "굴뚝", "길", "꽃", "나무", "문", "산", "연기", "연못", "울타리", "잔디", "지붕", "집벽", "집전체", "창문", "태양"
    };

    public void capture()
    {
        BGM.SetActive(true);
        btn.SetActive(false);

        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }

        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = Color.white;

        // 캡처 및 추론 시작
        StartCoroutine(CaptureAndRunInference());
    }

    private IEnumerator CaptureScene()
    {
        yield return new WaitForEndOfFrame();

        // RenderTexture 생성
        RenderTexture renderTexture = new RenderTexture(640, 640, 24);
        Camera.main.targetTexture = renderTexture;

        // 카메라 렌더링
        Texture2D screenshot = new Texture2D(640, 640, TextureFormat.RGB24, false);
        Camera.main.Render();
        RenderTexture.active = renderTexture;

        // 캡처한 내용을 Texture2D에 복사
        screenshot.ReadPixels(new Rect(0, 0, 640, 640), 0, 0);
        screenshot.Apply();

        // RenderTexture 및 카메라 복구
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        inputTexture = screenshot;

        // 메모리 해제
        Destroy(screenshot);
    }

    private IEnumerator CaptureAndRunInference()
    {
        // CaptureScene 코루틴 호출
        yield return StartCoroutine(CaptureScene());

        // 카메라 상태 복구
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = new Color(0, 0, 0, 0);

        if (inputTexture != null)
        {
            SendImageToFlaskServer(inputTexture);
        }
    }

    private async void SendImageToFlaskServer(Texture2D image)
    {
        using (var client = new HttpClient())
        {
            try
            {
                byte[] imageBytes = image.EncodeToPNG();

                using (var content = new MultipartFormDataContent())
                {
                    // 바이트 배열을 컨텐트로 추가
                    ByteArrayContent byteArrayContent = new ByteArrayContent(imageBytes);
                    byteArrayContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
                    content.Add(byteArrayContent, "file", "image.png");

                    // HTTP POST 요청
                    var response = await client.PostAsync("http://pettopia.iptime.org:8001/artversAIHouse", content);

                    string responseData = await response.Content.ReadAsStringAsync();
                 
                    JObject jsonResponse = JObject.Parse(responseData);

                    int cnt = jsonResponse["counts"]?.ToObject<int>() ?? 0;
                    var labels = jsonResponse["labels"]?.ToObject<List<float>>() ?? new List<float>();
                    var ratios = jsonResponse["ratios"]?.ToObject<List<float>>() ?? new List<float>();

                    if (cnt > 0 && labels.Count > 0 && ratios.Count > 0)
                    {
                        MapResultsToPsychologicalState(cnt, labels, ratios);
                    }
                    else
                    {
                        Console.WriteLine("Invalid data received.");
                    }

                    ShowResult(interpretation);

                }
            }
            catch (HttpRequestException e)
            {
                ShowResult("Error: " + e.Message);
            }
            catch (Exception e)
            {
                ShowResult("Unexpected error: " + e.Message);
            }
        }
    }

    private Texture2D ScaleTexture(Texture2D source, int width, int height)
    {
        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(source, rt);
        RenderTexture.active = rt;
        Texture2D result = new Texture2D(width, height);
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();
        RenderTexture.ReleaseTemporary(rt);
        return result;
    }

    private void MapResultsToPsychologicalState(int cnt, List<float> labels, List<float> ratios)
    {
        int[] box = new int[classNames.Length];

        for (int i = 0; i < cnt; i++)
        {
            int label = (int)labels[i];
            float ratio = ratios[i];
            box[label]++;
        }

        for (int i = 0; i < cnt; i++)
        {
            int label = (int)labels[i];
            float ratio = ratios[i];

            switch (classNames[label])
            {
                case "집전체":
                    interpretation += InterpretFamily(ratio);
                    break;
                case "지붕":
                    interpretation += InterpretRoof(ratio);
                    break;
                case "집벽":
                    interpretation += InterpretWall(ratio);
                    break;
                case "문":
                    interpretation += InterpretDoor(ratio);
                    break;
                case "창문":
                    interpretation += InterpretWindow(ratio);
                    break;
                case "굴뚝":
                    interpretation += InterpretChimney(box[label]);
                    break;
                case "연기":
                    interpretation += InterpretSmoke(ratio);
                    break;
                case "울타리":
                    interpretation += InterpretFence(ratio);
                    break;
                case "길":
                    interpretation += InterpretRoad(ratio);
                    break;
                case "연못":
                    interpretation += InterpretPond(ratio);
                    break;
                case "산":
                    interpretation += InterpretMountain(box[label]);
                    break;
                case "나무":
                    interpretation += InterpretTree(ratio);
                    break;
                case "꽃":
                    interpretation += InterpretFlower(box[label]);
                    break;
                case "잔디":
                    interpretation += InterpretGrass(ratio);
                    break;
                case "태양":
                    interpretation += InterpretSun(ratio);
                    break;
            }
        }
    }

    private string InterpretFamily(float ratio)
    {
        if (ratio > 0.4)
            interpretation += "가정 환경에서 안정감이 높음\n";
        else if (ratio < 0.2)
            interpretation += "가정 환경에 대한 위축감이 있을 수 있음\n";

        return interpretation;
    }

       private string InterpretRoof(float ratio)
    {
        return ratio > 0.1 ? "보호받고 싶은 욕구가 큼\n" : "보호받는 느낌이 적음\n";
    }

    private string InterpretWall(float ratio)
    {
        return ratio > 0.3 ? "안정적인 가정 환경을 느끼고 있음\n" : "가정 내에서의 불안정감을 느낄 수 있음\n";
    }

    private string InterpretDoor(float ratio)
    {
        return ratio > 0.1 ? "개방적이고 외부와의 소통을 중요하게 여김\n" : "외부와의 소통에 소극적일 수 있음\n";
    }

    private string InterpretWindow(float ratio)
    {
        return ratio > 0.05 ? "외부 세계에 대한 관심이 큼\n" : "외부 세계에 대한 관심이 적음\n";
    }

    private string InterpretChimney(float count)
    {
        return count >= 1 ? "따뜻한 가족 분위기를 느끼고 싶어함\n" : "가족 분위기에 대한 관심이 적음\n";
    }

    private string InterpretSmoke(float ratio)
    {
        return ratio > 0.02 ? "가족의 긴장감이나 스트레스가 높을 수 있음\n" : "가정 내에서의 긴장감이 적음\n";
    }

    private string InterpretFence(float ratio)
    {
        return ratio > 0.05 ? "방어적이며 보호받기를 원함\n" : "방어적 태도가 적고 개방적인 성향\n";
    }

    private string InterpretRoad(float ratio)
    {
        return ratio > 0.1 ? "미래에 대한 명확한 목표가 있음\n" : "미래에 대한 방향이 불확실할 수 있음\n";
    }

    private string InterpretPond(float ratio)
    {
        return ratio > 0.03 ? "감정적으로 안정된 상태를 추구\n" : "감정적 안정에 대한 욕구가 낮음\n";
    }

    private string InterpretMountain(float count)
    {
        return count >= 1 ? "도전적인 성향이 강하고 목표가 많음\n" : "도전에 대한 관심이 적음\n";
    }

    private string InterpretTree(float ratio)
    {
        return ratio > 0.2 ? "가족 간의 연결이 강하고 안정적인 관계\n" : "가족 간의 유대가 약할 수 있음\n";
    }

    private string InterpretFlower(float count)
    {
        return count >= 5 ? "감정적으로 풍부하고 표현이 다양함\n" : "감정 표현이 단조로울 수 있음\n";
    }

    private string InterpretGrass(float ratio)
    {
        return ratio > 0.15 ? "자연과의 연결을 중요시함\n" : "자연에 대한 관심이 적음\n";
    }

    private string InterpretSun(float ratio)
    {
        return ratio > 0.1 ? "긍정적인 에너지가 많고 낙관적인 성향\n" : "긍정적 에너지가 부족할 수 있음\n";
    }

    private void ShowResult(string message)
    {
        resultPanel.SetActive(true);
        BGM.SetActive(false);
        audioObject.SetActive(true);
        resultText.text = message;
    }

}
