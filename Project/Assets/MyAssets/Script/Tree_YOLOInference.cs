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

public class Tree_YOLOInference : MonoBehaviour
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
        "가지", "구름", "그네", "기둥", "꽃", "나무전체", "나뭇잎", "다람쥐", "달", "별", "뿌리", "새", "수관", "열매"
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
                    var response = await client.PostAsync("http://pettopia.iptime.org:8001/artversAITree", content);

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
                    case "나무전체":
                        interpretation += InterpretTree(ratio);
                        break;
                    case "기둥":
                        interpretation += InterpretPillar(ratio);
                        break;
                    case "수관":
                        interpretation += InterpretPipe(ratio);
                        break;
                    case "가지":
                        interpretation += InterpretBranch(box[label]);
                        break;
                    case "뿌리":
                        interpretation += InterpretRoot(ratio);
                        break;
                    case "나뭇잎":
                        interpretation += InterpretLeaf(box[label]);
                        break;
                    case "꽃":
                        interpretation += InterpretFlower(box[label]);
                        break;
                    case "열매":
                        interpretation += InterpretFruit(ratio);
                        break;
                    case "그네":
                        interpretation += InterpretSwing(ratio);
                        break;
                    case "새":
                        interpretation += InterpretBird(box[label]);
                        break;
                    case "다람쥐":
                        interpretation += InterpretSquirrel(box[label]);
                        break;
                    case "구름":
                        interpretation += InterpretCloud(box[label]);
                        break;
                    case "달":
                        interpretation += InterpretMoon(ratio);
                        break;
                    case "별":
                        interpretation += InterpretStar(box[label]);
                        break;
                
            }
        }

    }



    private string InterpretTree(float ratio)
    {
        string interpretation = "";
        if (ratio > 0.3)
            interpretation += "자아존중감이 높고 자신감이 있음\n";
        else if (ratio < 0.2)
            interpretation += "위축된 상태, 자신감 부족\n";

        return interpretation;
    }

    private string InterpretPillar(float ratio)
    {
        return ratio > 0.15 ? "내적 강인함과 안정감이 높음\n" : "불안정한 상태, 자아의 기반이 약함\n";
    }

    private string InterpretPipe(float ratio)
    {
        return ratio > 0.25 ? "감정적 풍부함과 사회적 관심이 큼\n" : "내향적이며 감정 표현이 제한적일 수 있음\n";
    }

    private string  InterpretBranch(float count)
    {
        return count >= 5 ? "사회적 연결망이 넓고, 감정적으로 풍부함\n" : "사회적 관계에 대한 관심이 적음\n";
    }

    private string InterpretRoot(float ratio)
    {
        return ratio > 0.05 ? "정서적 안정감이 있음\n" : "불안정한 상태, 정서적 기초가 약함\n";
    }

    private string InterpretLeaf(float count)
    {
        return count >= 10 ? "정서적 풍요로움과 감정의 다양성이 큼\n" : "감정적 단조로움이 있을 수 있음\n";
    }

    private string InterpretFlower(float count)
    {
        return count >= 5 ? "다소 낙관적이지 않거나 혼자 있는 느낌을 나타낼 수 있음\n" : "안정적이며 보통의 감정을 느끼는 상태를 나타낼 수 있음\n";
    }

    private string InterpretFruit(float ratio)
    {
        return ratio > 0.02 ? "성취와 보상에 대한 기대가 큼\n" : "성취에 대한 관심이 낮음\n";
    }

    private string InterpretSwing(float ratio)
    {
        return ratio > 0.03 ? "놀이와 자유를 즐기는 성향\n" : "놀이와 자유에 대한 관심이 낮음\n";
    }

    private string InterpretBird(float count)
    {
        return count >= 2 ? "상상력이 풍부하고 자유로운 성향\n" : "상상력에 대한 표현이 제한적일 수 있음\n";
    }

    private string InterpretSquirrel(float count)
    {
        return count >= 1 ? "호기심이 많고 활동적임\n" : "호기심이 제한적일 수 있음\n";
    }

    private string InterpretCloud(float count)
    {
        return count > 3 ? "불안감이나 스트레스를 경험하고 있을 수 있음\n" : "가벼운 걱정이나 불안감을 느끼고 있을 수 있음\n";
    }

    private string InterpretMoon(float ratio)
    {
        return ratio > 0.05 ? "감정적으로 민감하며 꿈꾸는 성향\n" : "현실적이고 감정 표현이 적음\n";
    }

    private string InterpretStar(float count)
    {
        return count > 5 ? "희망과 상상력이 풍부함\n" : "희망이나 상상력에 대한 표현이 제한적일 수 있음\n";
    }

    private void ShowResult(string message)
    {
        resultPanel.SetActive(true);
        BGM.SetActive(false);
        audioObject.SetActive(true);
        resultText.text = message;
    }

}

