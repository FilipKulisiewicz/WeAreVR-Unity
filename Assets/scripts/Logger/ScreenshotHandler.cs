using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotHandler : MonoBehaviour
{
    private static ScreenshotHandler instance;
    [SerializeField]
    private Camera myCamera;
    private bool takeScreenshotOnNextFrame;

    private Menu menuData;
    private string basePath; //change in inspector inly if different that default (different that one set in GUI)

    void Awake()
    {
        if(instance!=null)
            Debug.LogWarning("Multiple instances of ScreenshotHandler");
        instance = this;
        menuData = Menu.instance;
        basePath = menuData.basePath;
        System.IO.Directory.CreateDirectory(basePath + "/Screenshots");
    }

    int idx = 0;
    private void OnPostRender() {
        if (takeScreenshotOnNextFrame) {
            idx = idx + 1;
            takeScreenshotOnNextFrame = false;
            RenderTexture renderTexture = myCamera.targetTexture;
            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false); 
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);
            byte[] byteArray = renderResult.EncodeToPNG();
            System.IO.File.WriteAllBytes(basePath + "/Screenshots/" + "/CameraScreenshot" + idx + ".png", byteArray);
            Debug.Log("Saved CameraScreenshot" + idx);
            RenderTexture.ReleaseTemporary(renderTexture);
            myCamera.targetTexture = null;
        }
    }
    private void TakeScreenshot (int width, int height) {
        myCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        takeScreenshotOnNextFrame = true;
    }
    
    public static void TakeScreenshot_Static(int width, int height) {
        instance.TakeScreenshot(width, height);
    }
}
