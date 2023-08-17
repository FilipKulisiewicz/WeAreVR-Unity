using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getRicohStream : MonoBehaviour
{

    static WebCamTexture ricohStream;
    public string camName = "RICOH THETA V/Z1 FullHD"; // Name of your camera. 
    public Material camMaterial;  // Skybox material

    void Start()
    {
        ricohStream = new WebCamTexture(camName, 3840, 1920); // Resolution you want
        ricohStream.deviceName = camName;
        ricohStream.Play();

	    if (camMaterial != null)
            camMaterial.mainTexture = ricohStream;

    }

}