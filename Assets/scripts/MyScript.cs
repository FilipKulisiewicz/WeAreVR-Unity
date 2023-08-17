using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.WebRTC.Unity;
// using Microsoft.MixedReality.WebRTC;
using System.Threading.Tasks;

public class MyScript : MonoBehaviour
{
    public static System.Collections.Generic.IReadOnlyList<Microsoft.MixedReality.WebRTC.VideoCaptureDevice> deviceList;
    
    public VideoRenderer videoRenderer;
    public WebcamSource webcamsource;
 
    public async void startVideoStream()
    {
        
        await startMain();

        //webcamsource.WebcamDevice = deviceList[3]; //not changing anything
        //webcamsource;

        Debug.Log("Used webcam: " + webcamsource.WebcamDevice.name + " id: " + webcamsource.WebcamDevice.id);
        videoRenderer.StartRendering(webcamsource.Source);
    }
 
    public void stopVideoStream()
    {
        videoRenderer.StopRendering(webcamsource.Source);
    }

    public static async Task startMain()
    {
        try
        {
            // Asynchronously retrieve a list of available video capture devices (webcams).
            deviceList = await PeerConnection.GetVideoCaptureDevicesAsync();

            // For example, print them to the standard output
            foreach (var device in deviceList)
            {
                Debug.Log("Found webcam: " + device.name + " id: " + device.id);
            }
            // Debug.Log("2: Found webcam: " + VideoCaptureDevice.name + " id: " + deviceList[2].id);
            // VideoCaptureDevice = deviceList[2];
            // Debug.Log("2: Found webcam: " + VideoCaptureDevice.name + " id: " + deviceList[2].id);
            //Debug.Log("2: VideoCaptureDevice
        }
        catch
        {
            Debug.Log("Error");
        }
        
        //Found webcam: RICOH THETA V/Z1 FullHD id: foo:bar
    }

}
 