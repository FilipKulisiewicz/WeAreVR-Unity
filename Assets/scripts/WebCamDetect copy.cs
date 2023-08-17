// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System;
// using OpenCvSharp;
// using System.Threading.Tasks;


// public class WebCamDetect : MonoBehaviour
// {
//     public static event Action newFrame;
//     //-------------------------
//     [SerializeField]
//     private string camName = "RICOH THETA V/Z1 4K";
//     private static WebCamTexture mycam;
//     private int imWidth;
//     private int imHeight;
//     //private Color32[] tempdata;
//     private InputOutputArray flow;
//     private Vec3b[] ImageData;
//     private Mat Image;
//     private Mat prevImage;
//     private Mat ImageGray;
//     private Mat prevImageGray;
//     private Mat ImageGrayLow;
//     private Mat prevImageGrayLow;
//     private Size dim = new Size(600, 300);
//     float[] c;
//     const float scaleResolution = 0.8f;  
//     const int textHeight = (int)(50.0f * scaleResolution);
//     const int textWidth = (int)(580.0f * scaleResolution);
    
//     void Awake(){
//         Application.onBeforeRender += OnBeforeRenderCallback;
//     }

//     void Start(){
//         WebCamDevice[] devices = WebCamTexture.devices;
//         Debug.Log("Number of web cams connected: " + devices.Length);

//         for (int i=0; i < devices.Length; i++)
//         {
//             Debug.Log(i + " " + devices[i].name);
//         }

//         Renderer rend = this.GetComponentInChildren<Renderer>();
//         mycam = new WebCamTexture();
//         WebCamTexture.allowThreadedTextureCreation = true;
//         Debug.Log("The webcam name is " + camName);
//         mycam.deviceName = camName;
//         rend.material.mainTexture = mycam;
//         mycam.Play();

//         imWidth = mycam.width;
//         imHeight = mycam.height;
//         ImageData = new Vec3b[imWidth * imHeight];
//         Image = new Mat(imHeight, imWidth, MatType.CV_8UC3);
//         prevImage = new Mat(imHeight, imWidth, MatType.CV_8UC3);
//         ImageGray = new Mat(imHeight, imWidth, MatType.CV_8UC1);
//         prevImageGray = new Mat(imHeight, imWidth, MatType.CV_8UC1);
//         ImageGrayLow = new Mat(imHeight, imWidth, MatType.CV_8UC1);
//         prevImageGrayLow = new Mat(imHeight, imWidth, MatType.CV_8UC1);
//         flow = new InputOutputArray(prevImageGrayLow);
//         Debug.Log(imWidth);
//         Debug.Log(imHeight);
//         c = new float[imWidth * imHeight]; 
//         //flow = new InputOutputArray(dim);
//     }

//     void OnDestroy()
//     {
//         Application.onBeforeRender -= OnBeforeRenderCallback;
//     }    

//     bool firstFrame = true; 
//     uint binClock = 0;
//     // OpenCvSharp.Rect[] rects = new OpenCvSharp.Rect[20];
//     // string[] texts = new string[20];
//     // float[] confidences = new float[20]; 
//     // CvText.OCRLevel component_level = 0;
//     void OnBeforeRenderCallback()
//     {
//         if(mycam.didUpdateThisFrame){
//             if(firstFrame){
//                 firstFrame = false;
//             }
//             else{
//                 TexturePart(mycam, binClock);
//                 //new InputOutputArray(prev: height, next: width, CV_32FC2)
//                 //flow = new InputOutputArray(prevImageGrayLow);
//                 // Cv2.CvtColor(Image, ImageGray, ColorConversionCodes.BGR2GRAY);
//                 // Cv2.Resize(ImageGray, ImageGrayLow, dim, interpolation: InterpolationFlags.Cubic);
//                 // Cv2.CalcOpticalFlowFarneback(prevImageGrayLow, ImageGrayLow, flow, 
//                                         //  0.5, 3, 15, 3, 5, 1.2, 0);
//                 //Debug.Log(flow.GetMat().Sum());
//             } 
//             //prevImage = Image;
//             // prevImageGrayLow = ImageGrayLow;
//             //------------
        
//             newFrame?.Invoke();
//         } 
//     }

//     void TexturePart(WebCamTexture _webcamTexture, uint binClock)
//     {
//         // Color32 array : r, g, b, a
        
//         //Color32[] c = _webcamTexture.GetPixels32();
//         // Parallel for loop
//         // convert Color32 object to Vec3b object
//         // Vec3b is the representation of pixel for Mat
//         binClock = 0;
//         var j = 0;
//         var jf = 2f;
//         for (var i = 20 ; i < 21; i++)
//         {
//             jf = 2f;
//             for (var idx = 0; idx < 32; idx++)
//             {
//                 jf = jf  + 13.3333333f;    
//                 j = (int)jf; 
//                 var x = _webcamTexture.GetPixel(j, i)[0]; 
//                 if( x > 0.5f){
//                     binClock = (binClock << 1) + 1; 
//                 }
//                 else{
//                     binClock = (binClock << 1) + 0;
//                 }
//                 Debug.Log(x);
//             }
//         }
//         // assign the Vec3b array to Mat
//         //Image.SetArray(0, 0, ImageData); //looooooooong 
//     }

//     void TextureToMat(WebCamTexture _webcamTexture, Vec3b[] ImageData, Mat Image)
//     {
//         // Color32 array : r, g, b, a
        
//         Color32[] c = _webcamTexture.GetPixels32();
//         // Parallel for loop
//         // convert Color32 object to Vec3b object
//         // Vec3b is the representation of pixel for Mat

//         Parallel.For(0, imHeight, i =>
//         {
//             for (var j = 0; j < imWidth; j++)
//             {
//                 var col = c[j + i * imWidth];
//                 var vec3 = new Vec3b
//                 {
//                     Item0 = col.b,
//                     Item1 = col.g,
//                     Item2 = col.r
//                 };
//                 // set pixel to an array
//                 //Debug.Log(j + i * width);
//                 ImageData[j + i * imWidth] = vec3;
//             }
//         });
//         // assign the Vec3b array to Mat
//         //Image.SetArray(0, 0, ImageData); //looooooooong 
//     }


// }
