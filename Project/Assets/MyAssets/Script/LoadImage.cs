using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadImage : MonoBehaviour
{
    public Transform photoContainer;

    private List<Texture2D> loadedTextures = new List<Texture2D>();

    public void load()
    {
        string path = "/storage/emulated/0/Android/data/com.DefaultCompany.ArtVerse/files/Pictures";
        //string path = "/storage/emulated/0/Pictures/.thumbnails"; //접근 안됨...

        Debug.Log($"사진을 로드할 폴더 경로: {path}");
        
        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path, "*.jpg"); // JPG 파일 가져오기
            if (files.Length > 0)
            {
                StartCoroutine(LoadAllTextures(files)); // 비동기적으로 사진 로드
            }
            else
            {
                Debug.Log("폴더에 사진이 없습니다.");
            }
        }
        else
        {
            Debug.LogError($"경로가 존재하지 않습니다: {path}"); // 경로가 없으면 오류 출력
        }
    }

    IEnumerator LoadAllTextures(string[] files)
    {
        foreach (var filePath in files)
        {
            WWW www = new WWW("file://" + filePath); // 로컬 파일 로드
            yield return www;

            if (www.texture != null)
            {
                loadedTextures.Add(www.texture); // 텍스처 리스트에 추가
                CreatePhotoObject(www.texture); // 오브젝트 생성
            }

        }
    }

    void CreatePhotoObject(Texture2D texture)
    {
        // Quad 생성
        GameObject photoObject = GameObject.CreatePrimitive(PrimitiveType.Quad);

        // 텍스처 적용
        photoObject.GetComponent<Renderer>().material = new Material(Shader.Find("Unlit/Texture"));
        photoObject.GetComponent<Renderer>().material.mainTexture = texture;

        // 부모 설정
        if (photoContainer != null)
        {
            photoObject.transform.SetParent(photoContainer);
        }

        // 사진을 적절히 배치 (예: 가로로 나열)
        int index = photoContainer != null ? photoContainer.childCount - 1 : 0;
        photoObject.transform.localPosition = new Vector3(index * 2.0f, 0, 5); // X축으로 2씩 간격 배치
        photoObject.transform.localRotation = Quaternion.identity;

        photoObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // 기본 크기 설정
    }
}
