using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ArtExhibition1 : MonoBehaviour
{
    public Canvas exhibitionCanvas;
    public Canvas selectArtCanvas;

    public Image art1, art2, art3, art4, art5, art6, art7, 
                art8, art9, art10, art11, art12;

    public Image selectArt; 

    public Button prevButton;
    public Button nextButton;

    public TMP_Text pageNum;
    public string mainScene;

    private int currentPage = 1;
    private int maxFileForPage = 12;
    private int totalPage = 0;

    private string path = "/storage/emulated/0/Android/data/com.DefaultCompany.ArtVerse/files/Pictures";
    private List<string> fileList = new List<string>();
    private Dictionary<Image, string> slotToFileMap = new Dictionary<Image, string>();

    void Start()
    {
        selectArtCanvas.gameObject.SetActive(false);
        currentPage = 1;

        // 슬롯 초기화
        foreach (var art in new List<Image> { art1, art2, art3, art4, art5, art6, art7, art8, art9, art10, art11, art12 })
        {
            art.gameObject.SetActive(false);
            art.sprite = null;

            art.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        loadFiles();
        calTotalPageNum();
        UpdateArtListUI();

        //파일 선택 UI 활성화
        exhibitionCanvas.gameObject.SetActive(true);
    }

    private void loadFiles()
    {
        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path, "*.png");
            fileList = new List<string>(files);
        }
    }

    private void setButtonListeners()
    {
        prevButton.onClick.RemoveAllListeners();
        nextButton.onClick.RemoveAllListeners();

        prevButton.onClick.AddListener(onPrevButton);
        nextButton.onClick.AddListener(OnNextButton);
    }

    private void onPrevButton()
    {
        if (currentPage > 1)
        {
            currentPage--;
            UpdateArtListUI();
        }
    }

    private void OnNextButton()
    {
        if (currentPage < totalPage)
        {
            currentPage++;
            UpdateArtListUI();
        }
    }

    public void OnCloseButton()
    {
        SceneManager.LoadScene(mainScene, LoadSceneMode.Single);
    }

    public void onBackButton()
    {
        selectArtCanvas.gameObject.SetActive(false);
        exhibitionCanvas.gameObject.SetActive(true);
    }

    private void calTotalPageNum()
    {
        totalPage = Mathf.CeilToInt((float)fileList.Count / maxFileForPage);
    }

    private void UpdateArtListUI()
    {
        setButtonListeners();

        List<Image> artSlots = new List<Image> { art1, art2, art3, art4, art5, art6, art7, art8, art9, art10, art11, art12 };

        //페이지넘버 텍스트
        pageNum.text = $"{currentPage} / {totalPage}";

        // UI 오브젝트 비활성화
        foreach (var art in artSlots)
        {
            art.gameObject.SetActive(false);
            art.sprite = null;
            slotToFileMap.Remove(art);
            art.GetComponent<Button>().onClick.RemoveAllListeners();
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
                Image art = artSlots[i - startFileIndex];
                art.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.1f, 0.1f));
                art.gameObject.SetActive(true);

                slotToFileMap[art] = filePath; // 슬롯과 파일 매핑

                // 슬롯에 클릭 이벤트 추가
                art.GetComponent<Button>().onClick.AddListener(() => OnFileClicked(art));
            }
        }

        // 페이지 버튼 활성화 여부
        prevButton.interactable = currentPage > 1;
        nextButton.interactable = currentPage < totalPage;
    }

    private void OnFileClicked(Image art)
    {
        if (slotToFileMap.TryGetValue(art, out string filePath))
        {
            Texture2D texture = LoadPNG(filePath);

            if (texture != null)
            {
                selectArt.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.1f, 0.1f));
                selectArt.gameObject.SetActive(true);
            }
        }
        exhibitionCanvas.gameObject.SetActive(false);
        selectArtCanvas.gameObject.SetActive(true);
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
