using UnityEngine;
using UnityEngine.UI;

public class PaletteManager : MonoBehaviour
{
    public Material pencilMat;
    public Material crayonMat;
    public Material sprayMat;
    public Material brushMat;

    public Sprite clickPencilSprite;
    public Sprite clickCrayonSprite;
    public Sprite clickSpraySprite;
    public Sprite clickBrushSprite;

    public Sprite pencilSprite;
    public Sprite crayonSprite;
    public Sprite spraySprite;
    public Sprite brushSprite;

    private bool penCheck;
    private bool crayonCheck;
    private bool sprayCheck;
    private bool brushCheck;

    private string objectName;
    private GameObject myTarget;

    public Button[] colorButtons;

    void Start()
    {

        penCheck = false;
        crayonCheck = false;
        sprayCheck = false;
        brushCheck = false;
        

        // 모든 버튼에 클릭 이벤트 추가
        foreach (Button button in colorButtons)
        {
            button.onClick.AddListener(() => ChangeColor(button));
        }

    }

    // 버튼 클릭 시 색상 변경 처리
    public void ChangeColor(Button clickedButton)
    {
        // 버튼의 이미지 컴포넌트에서 색상 가져오기
        Color buttonColor = clickedButton.image.color;

        // 현재 도구에 맞는 재질의 색상 변경
        if (objectName == "Pencil")
        {
            pencilMat.color = buttonColor;
        }
        else if (objectName == "Crayon")
        {
            crayonMat.color = buttonColor;
        }
        else if (objectName == "Spray")
        {
            sprayMat.color = buttonColor;
        }
        else if (objectName == "Brush")
        {
            brushMat.color = buttonColor;
        }
    }

    // 버튼 클릭 시 도구 변경

    public void ChangeTool_Pen(Button clickedButton)
    {
        if(penCheck == false)
        {
            objectName = "Pencil";
            penCheck = true;
            clickedButton.image.sprite = clickPencilSprite;
        }else
        {
            objectName = null;
            penCheck = false;
            clickedButton.image.sprite = pencilSprite;
        }
    }

    public void ChangeTool_Crayon(Button clickedButton)
    {
        if(crayonCheck == false)
        {
            objectName = "Crayon";
            crayonCheck = true;
            clickedButton.image.sprite = clickCrayonSprite;
        }else
        {
            objectName = null;
            crayonCheck = false;
            clickedButton.image.sprite = crayonSprite;
        }
        
    }

    public void ChangeTool_Spray(Button clickedButton)
    {
        if(sprayCheck == false)
        {
            objectName = "Spray";
            sprayCheck = true;
            clickedButton.image.sprite = clickSpraySprite; 
        }else
        {
            objectName = null;
            sprayCheck = false;
            clickedButton.image.sprite = spraySprite; 
        }

    }

    public void ChangeTool_Brush(Button clickedButton)
    {
        if(brushCheck == false)
        {
            objectName = "Brush";
            brushCheck = true;
            clickedButton.image.sprite = clickBrushSprite;
        }else
        {
            objectName = null;
            brushCheck = false;     
            clickedButton.image.sprite = brushSprite; 
        } 
    }
}
