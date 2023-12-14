using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WebCam : MonoBehaviour{
    public static event Action newFrame;
    [SerializeField]
    private string camName = "RICOH THETA V/Z1 4K";
    private static WebCamTexture mycam;
    private int imWidth;
    private int imHeight;
    
    void OnEnable(){
        Application.onBeforeRender += OnBeforeRenderCallback;
    }

    void OnDestroy(){
        Application.onBeforeRender -= OnBeforeRenderCallback;
    }    

    void Start(){
        WebCamDevice[] devices = WebCamTexture.devices;
        Debug.Log("Number of web cams connected: " + devices.Length);

        for (int i=0; i < devices.Length; i++)
        {
            Debug.Log(i + " " + devices[i].name);
        }

        Renderer rend = this.GetComponentInChildren<Renderer>();
        mycam = new WebCamTexture();
        WebCamTexture.allowThreadedTextureCreation = true;
        Debug.Log("The webcam name is " + camName);
        mycam.deviceName = camName;
        rend.material.mainTexture = mycam;
        mycam.Play();

        imWidth = mycam.width;
        imHeight = mycam.height;
        // Debug.Log(imWidth);
        // Debug.Log(imHeight);
    }

    bool Test = false;
    void OnBeforeRenderCallback(){
        if(mycam.didUpdateThisFrame || Test == true){
            newFrame?.Invoke();           
        } 
    } 

    /* hardware support - binary clock 'embedding time' on the video stream - idea dropped due to acceptable performence of hardwereless methods  
    const float scaleResolution = 0.5f; //8f;  
    const int textHeight = (int)(50.0f * scaleResolution);
    const int textWidth = (int)(580.0f * scaleResolution);   
    void ClockFromTexture(WebCamTexture _webcamTexture, ref uint binClock)
    {
        binClock = 0;
        var checkedWidth = 0;
        var expectedWidthf = 2f;
        var checkedHeight = textHeight / 2;
        expectedWidthf = 2f;
        for (var idx = 0; idx < 32; idx++)
        {
            expectedWidthf +=  16.666666f * scaleResolution;    
            checkedWidth = (int)expectedWidthf; 
            var x = _webcamTexture.GetPixel(checkedWidth, checkedHeight)[0]; 
            if( x > 0.5f){
                binClock = (binClock << 1) + 1; 
            }
            else{
                binClock = (binClock << 1) + 0;
            }
        }            
    }
    */
}
