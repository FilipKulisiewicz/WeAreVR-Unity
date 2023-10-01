using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using OpenCvSharp;
using System.Threading.Tasks;

public class CamScript : MonoBehaviour
{
    private int imWidth = 100;
    private int imHeight = 100;
    private bool firstFrame = true;
    private Camera mCamera;
    private Texture2D texture;

    private Vec3b[] ImageData10;
    private Mat Image10;
    private Mat Imagediff;
    private Mat ImageGray10;
    private Mat ImageGray10Prev;

    private InputOutputArray diff;

    private void OnEnable(){
        SphereManager.DiffRequest += Measure;
    }
    private void OnDisable(){
        SphereManager.DiffRequest -= Measure;
    }

    private void Start(){
        mCamera = gameObject.GetComponent<Camera>();
        ImageData10 = new Vec3b[100 * 100];
        Image10 = new Mat(100, 100, MatType.CV_8UC3);
        ImageGray10 = new Mat(100, 100, MatType.CV_8UC1);
        ImageGray10Prev = new Mat(100, 100, MatType.CV_8UC1);

        Imagediff = new Mat(100, 100, MatType.CV_8UC1);
        diff = new InputOutputArray(Imagediff);
    }
    private double a;

    private double Measure(){
        texture = RTImage();

        diff = new InputOutputArray(ImageGray10Prev);
        TextureToMat(texture, ImageData10, Image10);
        Cv2.CvtColor(Image10, ImageGray10, ColorConversionCodes.BGR2GRAY);
        if(firstFrame){
            firstFrame = false;
        }
        else{
            //newFrame?.Invoke();           
            Cv2.Subtract(ImageGray10, ImageGray10Prev, diff);
            a = diff.GetMat().Sum()[0];              
        }
        ImageGray10Prev = ImageGray10.Clone();
        diff.Dispose();
        return a;
    }

    private void TextureToMat(Texture2D _webcamTexture, Vec3b[] ImageData, Mat Image)
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
        Image.SetArray(0, 0, ImageData); //time consuming 
    }

    private Texture2D RTImage()
	{
		UnityEngine.Rect rect = new UnityEngine.Rect(0, 0, imWidth, imHeight);
		RenderTexture renderTexture = new RenderTexture(imWidth, imHeight, 24);
		Texture2D screenShot = new Texture2D(imWidth, imHeight, TextureFormat.RGBA32, false);

		mCamera.targetTexture = renderTexture;
		mCamera.Render();

		RenderTexture.active = renderTexture;
		screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();

		mCamera.targetTexture = null;
		RenderTexture.active = null;

		Destroy(renderTexture);
		renderTexture = null;
		return screenShot;
	}
}
