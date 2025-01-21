using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouTubeUI : MonoBehaviour
{
    public string url = "https://www.youtube.com/";

    public void OpenWebPage()
    {
        Application.OpenURL(url);
    }
}


