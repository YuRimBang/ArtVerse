using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectFile : MonoBehaviour
{
    public GameObject mainUI;
    public Canvas selectFileCanvas;
    public Canvas InputEmailCanvas;
    public Image slot1, slot2, slot3, slot4, slot5; // 5개의 이미지 슬롯
    public Button prevButton; // 이전 페이지 버튼
    public Button nextButton; // 다음 페이지 버튼

    public TMP_Text pageNumberText;

    private int currentPage = 1;
    private int maxFileForPage = 5;
    private int totalPage = 0;

    private string path = "/storage/emulated/0/Android/data/com.DefaultCompany.ArtVerse/files/Pictures";
    private List<string> fileList = new List<string>();

    private List<string> userSelectedFileList = new List<string>();
    private Dictionary<Image, string> slotToFileMap = new Dictionary<Image, string>();


    public List<string> getUserSelectedFiles()
    {
        return userSelectedFileList;
    }

    public void viewSelectFileUI()
    {
        currentPage = 1;
        userSelectedFileList.Clear();

        // 슬롯 초기화
        foreach (var slot in new List<Image> { slot1, slot2, slot3, slot4, slot5 })
        {
            slot.gameObject.SetActive(false);
            slot.sprite = null;

            slot.GetComponent<Outline>().enabled = false;
            slot.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        loadFiles();
        calTotalPageNum();
        UpdateFileListUI();

        mainUI.SetActive(false);
        selectFileCanvas.gameObject.SetActive(true);
    }

    private void loadFiles()
    {
        if (Directory.Exists(path))
        {
            //png 파일만 불러와 리스트에 저장
            string[] files = Directory.GetFiles(path, "*.png");
            fileList = new List<string>(files);
        }
    }

    public void OnCloseButton()
    {
        selectFileCanvas.gameObject.SetActive(false);
        mainUI.SetActive(true);
    }

    public void OnGoToNextButton() //다음으로 넘어가기
    {
        selectFileCanvas.gameObject.SetActive(false);
        InputEmailCanvas.gameObject.SetActive(true);
    }

    public void OnNextPageButton()
    {
        if (currentPage < totalPage)
        {
            currentPage++;
            UpdateFileListUI();
        }
    }

    public void OnPrevPageButton()
    {
        if (currentPage > 1)
        {
            currentPage--;
            UpdateFileListUI();
        }
    }

    private void calTotalPageNum()
    {
        totalPage = Mathf.CeilToInt((float)fileList.Count / maxFileForPage);
    }

    private void UpdateFileListUI()
    {
        List<Image> imageSlots = new List<Image> { slot1, slot2, slot3, slot4, slot5 };

        //페이지넘버 텍스트
        pageNumberText.text = $"{currentPage} / {totalPage}";

        // UI 오브젝트 비활성화
        foreach (var slot in imageSlots)
        {
            slot.gameObject.SetActive(false);
            slot.sprite = null;
            slotToFileMap.Remove(slot);
            slot.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        // 페이지에 맞는 파일 항목을 UI에 표시
        int startFileIndex = (currentPage - 1) * maxFileForPage;
        int endFileIndex = Mathf.Min(startFileIndex + maxFileForPage, fileList.Count);

        for (int i = startFileIndex; i < endFileIndex; i++)
        {
            string filePath = fileList[i];
            Texture2D texture = LoadPNG(filePath);

            if (texture != null)
            {
                Image slot = imageSlots[i - startFileIndex];
                slot.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.1f, 0.1f));
                slot.gameObject.SetActive(true);

                slotToFileMap[slot] = filePath; // 슬롯과 파일 매핑

                // 슬롯에 클릭 이벤트 추가
                slot.GetComponent<Button>().onClick.AddListener(() => OnFileClicked(slot));
                //외곽선 체크
                slot.GetComponent<Outline>().enabled = userSelectedFileList.Contains(filePath);
            }
        }

        // 페이지 버튼 활성화 여부
        prevButton.interactable = currentPage > 1;
        nextButton.interactable = currentPage < totalPage;
    }

    private void OnFileClicked(Image slot)
    {
        if (slotToFileMap.TryGetValue(slot, out string filePath))
        {
            Outline outline = slot.GetComponent<Outline>();

            if (userSelectedFileList.Contains(filePath))
            {
                // 이미 선택된 파일이라면 선택 해제
                userSelectedFileList.Remove(filePath);
                outline.enabled = false;
            }
            else
            {
                // 새로 선택
                userSelectedFileList.Add(filePath);
                outline.effectColor = Color.blue;
                outline.enabled = true;
            }
        }
    }

    // PNG -> Texture2D
    private Texture2D LoadPNG(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);

        if (texture.LoadImage(fileData)) // PNG 이미지 로드
        {
            return texture;
        }
        else
        {
            return null;
        }
    }
}
