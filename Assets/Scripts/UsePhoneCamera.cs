using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpenCamera : MonoBehaviour
{
    public  RawImage      cameraImage;
    private WebCamTexture camTexture;
    private void Start()
    {
        StartCoroutine(OpenPhoneCamera());
    }
    /// <summary>
    /// 打开手机摄像头
    /// </summary>
    /// <returns></returns>
    public IEnumerator OpenPhoneCamera()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam); //请求打开手机摄像头权限,手机端会弹出一个选择弹窗
        WebCamDevice[] devices = WebCamTexture.devices;
        camTexture = new WebCamTexture(devices[0].name,Screen.width,Screen.height);
        camTexture.Play();
        cameraImage.texture = camTexture;

    }
}