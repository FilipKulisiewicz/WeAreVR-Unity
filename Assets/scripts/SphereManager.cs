using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using System; 
using UnityEngine.Video;

public class SphereManager : MonoBehaviour{
    public static event Func<float, float, bool, PositionRotation> TransformRequestFrame;
    public static event Func<float, bool, PositionRotation> TransformRequest;
    //public static event Func<double> DiffRequest; 
    [SerializeField]
    private GameObject sphere;  
    [SerializeField]
    private float delay = 0.15f; //
    [SerializeField]
    private bool interpolate = true;
    private const float frametime = 1.0f / 29.97f;
    int vidVariant = 0;
    

    private void OnEnable(){
        WebCam.newFrame += Unwind;
        tick.newFrame += Unwind;
    }
    private void OnDisable(){
        WebCam.newFrame -= Unwind;
        tick.newFrame -= Unwind;
    }
    
    private float requestedTime = 0.0f;
    private float lastFrame = 0.0f;
    private float timeDiff;
    private PositionRotation fromBuffer;
    
    // private double diff;
    // private double diff2;
    // private Quaternion rot1;
    Quaternion correctionRot = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
    [SerializeField]
    bool IMUVertical = true;
    [SerializeField]
    bool framerateBasedUnwinding = true; 
    [SerializeField]
    bool detectLostFrames = true; //false for video

    private Menu menuData;
    
    void Awake(){
        menuData = Menu.instance;
        changeVariant(menuData.selectedOption);
        if (IMUVertical == true){
            correctionRot = new Quaternion(0.0f, 0.0f, 0.71f, 0.71f);
        }
    }

    int i = 0;
    public void Unwind(){
        if(Time.time < 2.0f || framerateBasedUnwinding == false){ //old unwinding
            fromBuffer = (PositionRotation)TransformRequest?.Invoke(delay, interpolate);
            requestedTime = Time.time;
        }
        else{ //framerate based unwinding plus lost frames detection
            requestedTime = lastFrame + frametime; 
            if(detectLostFrames == true){ //0.047 and -0.025
                if(Time.time - requestedTime > 0.047f  || Time.time - requestedTime < -0.025f){  //0.033 
                    Debug.Log("pre: " + (Time.time - requestedTime));
                    if(Time.time - requestedTime > 0){ // 1 frame behind
                    //    Debug.Log("+f");
                        requestedTime = requestedTime + frametime; 
                        i = i + 1;
                    }
                    else{ // 1 frame ahead
                    //    Debug.Log("-f");
                        requestedTime = requestedTime - frametime;
                        i = i - 1;
                    }
                }
            }
            fromBuffer = (PositionRotation)TransformRequestFrame?.Invoke(requestedTime, delay, interpolate);            
        }
        sphere.transform.rotation = correctionRot * fromBuffer.rotation; 
        i = i + 1;

        if(vidVariant == 2 && delay < -11.0f){
            delay = delay + 0.00031f;
        }

        /* Visual check for the skipped frame detection - idea dropped due to too big computational overhead during early tests
        Quaternion rot1 = fromBuffer.rotation; 
        diff = DiffRequest.Invoke(); //

        if(false){
            Debug.Log("Check " + diff);
            requestedTime = requestedTime + frametime;
            fromBuffer = (PositionRotation)TransformRequestFrame?.Invoke(requestedTime, delay, interpolate);
            sphere.transform.rotation = fromBuffer.rotation;
            diff2 = DiffRequest.Invoke();
            Debug.Log("nextframe " + diff2);
            if(diff2 * 1.1f > diff){ //reverse if needed
                Debug.Log("False");
                requestedTime = requestedTime - frametime;
                sphere.transform.rotation = rot1;
                diff = DiffRequest.Invoke();
            }
            Debug.Log("Check " + diff);
            requestedTime = requestedTime - frametime;
            fromBuffer = (PositionRotation)TransformRequestFrame?.Invoke(requestedTime, delay, interpolate);
            sphere.transform.rotation = fromBuffer.rotation;
            diff2 = DiffRequest.Invoke();
            Debug.Log("prevframe " + diff2);
            if(diff2 * 1.1f > diff){ //reverse if needed
                Debug.Log("False");
                requestedTime = requestedTime + frametime;
                sphere.transform.rotation = rot1;
                diff = DiffRequest.Invoke();
            }
        }        
        */

        /* Debug options
        //Debug.Log("Time.time:" + Time.time);
        //Debug.Log("requestedTime:" + requestedTime);
        //Debug.Log((Time.time - lastFrameUnityTime));
        //Debug.Log((Time.time - requestedTime)); 
        */

        lastFrame = requestedTime;
    }

    public void changeVariant(int variant){
        vidVariant = variant;
        VideoPlayer[] videoPlayer = this.GetComponents<VideoPlayer>();
        if(variant == 0 || variant == 1){
            delay = -13.365f;
            videoPlayer[0].enabled = true;
            videoPlayer[1].enabled = false;
        }
        else if(variant == 2 || variant == 3){
            delay = -13.7f;
            videoPlayer[0].enabled = false;
            videoPlayer[1].enabled = true;
        }
        if(variant == 1 || variant == 3){
            delay = 9999999999.9f;
        }
    }
}

