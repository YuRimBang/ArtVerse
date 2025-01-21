using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGMEvent : MonoBehaviour
{
    public GameObject BGM;
    public Sprite onBGM; 
    public Sprite offBGM;

    public Slider volumeSlider;
    private bool BGMCheck = true;

    public GameObject btn;

    private AudioSource BGMSource; 

    void Start()
    {
        if (BGM != null)
        {
            BGMSource = BGM.GetComponent<AudioSource>();
            if (BGMSource != null)
            {
                volumeSlider.value = BGMSource.volume;
                volumeSlider.onValueChanged.AddListener(SetVolume);
            }
        }
    }

    public void SetVolume(float volume)
    {
        Image buttonImage = btn.GetComponent<Image>();
        if (BGMSource != null)
        {
            BGMSource.volume = volume;

            if (volume == 0)
            {
                BGMCheck = false;
                buttonImage.sprite = BGMCheck ? onBGM : offBGM;
                BGM.SetActive(false);
            }
            else if (!BGM.activeSelf)
            {
                BGMCheck = true;
                buttonImage.sprite = BGMCheck ? onBGM : offBGM;
                BGM.SetActive(true);
            }
        }
    }

    public void ToggleBGM(Button clickedButton)
    {
        BGMCheck = !BGMCheck;

        // BGM 활성화/비활성화
        BGM.SetActive(BGMCheck);

        clickedButton.image.sprite = BGMCheck ? onBGM : offBGM;
    }
}
