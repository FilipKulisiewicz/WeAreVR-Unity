using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using System; 

public class SphereManager : MonoBehaviour
{
    public static event Func<float, float, bool, PositionRotation> TransformRequestFrame;
    public static event Func<float, bool, PositionRotation> TransformRequest;
    public static event Func<double> DiffRequest; 
    [SerializeField]
    private GameObject sphere;  
    [SerializeField]
    private float delay = 0.15f; //
    [SerializeField]
    private bool interpolate = true;
    private const float frametime = 1.0f / 29.97f;
    
    private void OnEnable(){
        WebCamDetect.newFrame += Unwind;
    }
    private void OnDisable(){
        WebCamDetect.newFrame -= Unwind;
    }
    
    private float requestedTime = 0.0f;
    private float lastFrame = 0.0f;
    private float timeDiff;
    private PositionRotation fromBuffer;
    
    // private double diff;
    // private double diff2;
    // private Quaternion rot1;
    Quaternion correctionRot = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
    bool IMUVertical = true;
    bool oldUnwinding = false;

    public void Start(){
        if (IMUVertical == true){
            correctionRot = new Quaternion(0.0f, 0.0f, 0.71f, 0.71f);
        }
    }

    public void Unwind(){
        if(Time.time < 2.0f || oldUnwinding == true){ //old unwinding
            fromBuffer = (PositionRotation)TransformRequest?.Invoke(delay, interpolate);
            requestedTime = Time.time;
        }
        else{ //framerate based unwinding
            requestedTime = lastFrame + frametime; //0.047 and -0.025
            if(Time.time - requestedTime > 0.047f  || Time.time - requestedTime < -0.025f){ //or while
                Debug.Log("pre: " + (Time.time - requestedTime));
                if(Time.time - requestedTime > 0){
                    Debug.Log("+f");
                    requestedTime = requestedTime + frametime; 
                }
                else{
                    Debug.Log("-f");
                    requestedTime = requestedTime - frametime;
                }
            }
            fromBuffer = (PositionRotation)TransformRequestFrame?.Invoke(requestedTime, delay, interpolate);            
        }
        sphere.transform.rotation = correctionRot * fromBuffer.rotation; 
        
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
}

