using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using OpenCvSharp;
using System.Threading.Tasks;

public class WebCamDetect : MonoBehaviour
{
    public static event Action newFrame;
    //-------------------------
    [SerializeField]
    private string camName = "RICOH THETA V/Z1 4K";
    private static WebCamTexture mycam;
    private int imWidth;
    private int imHeight;
    //private Color32[] tempdata;
    // private InputOutputArray flow;
    private Vec3b[] ImageData;
    private Mat Image;
    Vec3b[] ImageData10;
    Mat Image10;
    Mat Image10Prev;
    Mat Imagediff;
    Mat ImageGray10;
    Mat ImageGray10Prev;

    InputOutputArray diff;
    // private Mat prevImage;
    // private Mat ImageGray;
    // private Mat prevImageGray;
    // private Mat ImageGrayLow;
    // private Mat prevImageGrayLow;
    // private Size dim = new Size(600, 300);
    //float[] c;
    //--
    
    void Awake(){
        Application.onBeforeRender += OnBeforeRenderCallback;
    }

    void OnDestroy()
    {
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
        Debug.Log(imWidth);
        Debug.Log(imHeight);
    }

    void OnBeforeRenderCallback()
    {
        if(mycam.didUpdateThisFrame){
            newFrame?.Invoke();           
        } 
    } 

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

    void TextureToMat100(WebCamTexture _webcamTexture, Vec3b[] ImageData, Mat Image)
    {
        // Color32 array : r, g, b, a
        Color32[] c = _webcamTexture.GetPixels32();
        // Parallel for loop
        // convert Color32 object to Vec3b object
        // Vec3b is the representation of pixel for Mat
        int startHeidth = imHeight / 2;
        Parallel.For(startHeidth, startHeidth + 100, i =>
        {
            for (var j = 0; j < 100; j++)
            {
                var col = c[j + i * imWidth];
                var vec3 = new Vec3b
                {
                    Item0 = col.b,
                    Item1 = col.g,
                    Item2 = col.r
                };
                // set pixel to an array
                ImageData[j + (i - startHeidth) * 100] = vec3;
            }
        });
        // assign the Vec3b array to Mat
        Image.SetArray(0, 0, ImageData); //looooooooong 
    }

    void TextureToMat(WebCamTexture _webcamTexture, Vec3b[] ImageData, Mat Image)
    {
        // Color32 array : r, g, b, a
        
        Color32[] c = _webcamTexture.GetPixels32();
        // Parallel for loop
        // convert Color32 object to Vec3b object
        // Vec3b is the representation of pixel for Mat

        Parallel.For(0, imHeight, i =>
        {
            for (var j = 0; j < imWidth; j ++)
            {
                var col = c[j + i * imWidth];
                var vec3 = new Vec3b
                {
                    Item0 = col.b,
                    Item1 = col.g,
                    Item2 = col.r
                };
                // set pixel to an array
                //Debug.Log(j + i * width);
                ImageData[j + i * imWidth] = vec3;
            }
        });
        // assign the Vec3b array to Mat
        Image.SetArray(0, 0, ImageData); //looooooooong 
    }
}
