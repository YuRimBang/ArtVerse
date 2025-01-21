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

public class Man_YOLOInference : MonoBehaviour
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
        "귀", "남자구두", "눈", "다리", "단추", "머리", "머리카락", "목", "발", "사람전체", "상체", "손", "얼굴", "운동화", "입", "주머니", "코", "팔"
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
                    var response = await client.PostAsync("http://pettopia.iptime.org:8001/artversAIMan", content);

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
                    case "사람전체":
                        interpretation += InterpretHuman(ratio);
                        break;
                    case "머리":
                        interpretation += InterpretHead(ratio);
                        break;
                    case "얼굴":
                        interpretation += InterpretFace(ratio);
                        break;
                    case "눈":
                        interpretation += InterpretEye(box[label]);
                        break;
                    case "코":
                        interpretation += InterpretNose(ratio);
                        break;
                    case "입":
                        interpretation += InterpretMouse(box[label]);
                        break;
                    case "귀":
                        interpretation += InterpretEar(box[label]);
                        break;
                    case "머리카락":
                        interpretation += InterpretHair(ratio);
                        break;
                    case "목":
                        interpretation += InterpretNeck(ratio);
                        break;
                    case "상체":
                        interpretation += InterpretBody(box[label]);
                        break;
                    case "팔":
                        interpretation += InterpretArm(box[label]);
                        break;
                    case "손":
                        interpretation += InterpretHand(box[label]);
                        break;
                    case "다리":
                        interpretation += InterpretLeg(ratio);
                        break;
                    case "발":
                        interpretation += InterpretFoot(box[label]);
                        break;
                    case "단추":
                        interpretation += InterpretButton(box[label]);
                        break;
                    case "주머니":
                        interpretation += InterpretPocket(box[label]);
                        break;
                    case "운동화":
                        interpretation += InterpretSneakers(box[label]);
                        break;
                    case "남자구두":
                        interpretation += InterpretShoes(box[label]);
                        break;
            
                
            }
        }

    }



    private string InterpretHuman(float ratio)
    {
        string interpretation = "";
        if (ratio > 0.3)
            interpretation += "자아존중감이 높고 자신감이 있음\n";
        else if (ratio < 0.2)
            interpretation += "자신감이 부족하고 위축된 상태\n";

        return interpretation;
    }

    private string InterpretHead(float ratio)
    {
        return ratio > 0.1 ? "지능이나 사고에 대해 높은 자아 인식을 가짐\n" : "지능이나 사고에 대한 관심이 낮음\n";
    }

    private string InterpretFace(float ratio)
    {
        return ratio > 0.15 ? "자아에 대한 관심이 큼\n" : "자아에 대한 관심이 낮음\n";
    }

    private string  InterpretEye(float ratio)
    {
        return ratio > 0.05 ? "외부 세계에 대한 관심이 크고 주의 깊음\n" : "외부 세계에 대한 관심이 적음\n";
    }

    private string InterpretNose(float ratio)
    {
        return ratio > 0.03 ? "자신을 표현하려는 성향이 있음\n" : "자신을 드러내려는 경향이 적음\n";
    }

    private string InterpretMouse(float ratio)
    {
        return ratio > 0.05 ? "의사 표현이 강하고 감정 표현이 자유로움\n" : "감정 표현이 제한적일 수 있음\n";
    }

    private string InterpretEar(float ratio)
    {
        return ratio > 0.02 ? "타인의 의견에 민감하고 잘 듣는 성향\n" : "타인의 의견에 대한 관심이 적음\n";
    }

    private string InterpretHair(float cnt)
    {
        return cnt >= 5 ? "개성과 감정 표현이 풍부함\n" : "감정 표현이 단조로울 수 있음\n";
    }

    private string InterpretNeck(float ratio)
    {
        return ratio > 0.02 ? "자신을 연결하는 데에 대한 자각이 강함\n" : "자기 표현에 대한 인식이 낮음\n";
    }

    private string InterpretBody(float ratio)
    {
        return ratio > 0.2 ? "자신의 신체에 대한 자각이 높고 자신감 있음\n" : "자신의 신체에 대한 자각이 낮음\n";
    }

    private string InterpretArm(float count)
    {
        return count >= 2 ? "타인과의 관계에 대한 관심이 많음\n" : "타인과의 관계에 소극적일 수 있음\n";
    }

    private string InterpretHand(float ratio)
    {
        return ratio > 0.05 ? "사회적 관계를 중요하게 생각함\n" : "사회적 관계에 대한 관심이 낮음\n";
    }

    private string InterpretLeg(float ratio)
    {
        return ratio > 0.1 ? "안정감이 높고 현실에 대한 자각이 큼\n" : "안정감이 부족할 수 있음\n";
    }

    private string InterpretFoot(float ratio)
    {
        return ratio > 0.05 ? "활동적이고 에너지가 많음\n" : "활동성이 낮고 소극적일 수 있음\n";
    }

    private string InterpretButton(float cnt)
    {
        return cnt > 3 ? "세부적인 것에 관심이 많고 주의 깊음\n" : "세부 사항에 대한 관심이 적음\n";
    }

    private string InterpretPocket(float cnt)
    {
        return cnt > 2 ? "보호받고 싶어하는 욕구가 있음\n" : "보호받고 싶은 욕구가 적음\n";
    }

    private string InterpretSneakers(float ratio)
    {
        return ratio > 0.05 ? "활동적이고 스포츠에 대한 관심이 큼\n" : "활동성이 낮고 스포츠에 대한 관심이 적음\n";
    }

    private string InterpretShoes(float ratio)
    {
        return ratio > 0.05 ? "사회적 지위와 스타일에 대한 인식이 큼\n" : "사회적 지위에 대한 관심이 적음\n";
    }

    

    private void ShowResult(string message)
    {
        resultPanel.SetActive(true);
        BGM.SetActive(false);
        audioObject.SetActive(true);
        resultText.text = message;
    }

}

