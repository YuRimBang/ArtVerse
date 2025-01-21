using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getPermission : MonoBehaviour
{
    void Start()
    {
        // 외부 저장소 읽기 권한 요청
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageRead))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageRead);
        }

        // 외부 저장소 쓰기 권한 요청
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageWrite))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageWrite);
        }
    }
}
