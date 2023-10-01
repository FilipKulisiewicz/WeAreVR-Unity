using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using System;

public struct PositionRotation
    {
        public Vector3 position;
        public Quaternion rotation;

        public PositionRotation(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

public class RosSubscriberRot : MonoBehaviour
{
    [SerializeField]
    private string rosTopic = "tool0";
    [SerializeField]
    private static CustomBuffer<PositionRotation> buffer;
    private TransformStampedMsg TransformMsg;
    private Vector3 rosTranslation;
    private Quaternion rosQuaternion;
    private double rosTime;
    // private static Quaternion rotRobotToUnity = new Quaternion(0.707f, 0.707f, 0.0f, 0.0f); //Quaternion(x, y, z, w) #To be removed
    // private Quaternion rotRobotToUnity_inv = Quaternion.Inverse(rotRobotToUnity);  
    
    private void OnEnable(){
        SphereManager.TransformRequest += GetDelayedValue;
        SphereManager.TransformRequestFrame += GetNextFrameDelayedValue;
    }
    
    private void OnDisable(){
        SphereManager.TransformRequest -= GetDelayedValue;
        SphereManager.TransformRequestFrame -= GetNextFrameDelayedValue;
    }

    void Start(){
        buffer = new CustomBuffer<PositionRotation>();
        ROSConnection.GetOrCreateInstance().Subscribe<TransformStampedMsg>(rosTopic, AddToBuffer);
    }    

    public void AddToBuffer(TransformStampedMsg transformMessage){
        //translation #TODO
        rosTranslation = new Vector3((float)transformMessage.transform.translation.x, (float)transformMessage.transform.translation.y, (float)transformMessage.transform.translation.z);
        //rotations
        if(string.Equals(rosTopic, "tool0"))
            rosQuaternion = ConvertFromRobotCoordinatesToUnityCoordinatesUnwinded(transformMessage.transform.rotation);
        else if (string.Equals(rosTopic, "IMU")){
            rosQuaternion = ConvertFromIMUCoordinatesToUnityCoordinatesUnwinded(transformMessage.transform.rotation);
        }
        //time
        rosTime = (double)transformMessage.header.stamp.sec + ((double)transformMessage.header.stamp.nanosec / 1e+09);
        buffer.Add(new PositionRotation(rosTranslation, rosQuaternion), rosTime);
    }

    private Quaternion ConvertFromRobotCoordinatesToUnityCoordinatesUnwinded(RosMessageTypes.Geometry.QuaternionMsg robotQuaternion){   
        rosQuaternion = new Quaternion((float)robotQuaternion.y, (float)robotQuaternion.z, -(float)robotQuaternion.x, (float)robotQuaternion.w); //Quaternion(x, y, z, w) // y & z flipped due to Unity Coordinate system convention 
        return rosQuaternion;
    } 
    
    private Quaternion ConvertFromIMUCoordinatesToUnityCoordinatesUnwinded(RosMessageTypes.Geometry.QuaternionMsg imuQuaternion)
    {
        //rosQuaternion = new Quaternion((float)imuQuaternion.x, -(float)imuQuaternion.z, (float)imuQuaternion.y, (float)imuQuaternion.w); //Quaternion(x, y, z, w)  // y & z flipped due to Unity Coordinate system convention 
        
        rosQuaternion = new Quaternion( -(float)imuQuaternion.z, -(float)imuQuaternion.x, (float)imuQuaternion.y, (float)imuQuaternion.w); //Quaternion(x, y, z, w)  // y & z flipped due to Unity Coordinate system convention 
        //rosQuaternion = new Quaternion( 0.0f, 0.71f, 0.0f, 0.71f) * rosQuaternion * Quaternion.Inverse(new Quaternion(  0.0f, 0.71f, 0.0f, 0.71f));
        return rosQuaternion;
    }
    
    private static PositionRotation GetNextFrameDelayedValue(float requestedTime, float delay, bool interpolate){
        return buffer.GetNextFrameDelayedValue(requestedTime, delay, interpolate, Interpolate);
    }

    private static PositionRotation GetDelayedValue(float delay, bool interpolate){
        return buffer.GetDelayedValue(delay, interpolate, Interpolate);
    }
    
    public static PositionRotation Interpolate(PositionRotation pr1, PositionRotation pr2, float t)
    {
        return new PositionRotation(Vector3.Lerp(pr1.position, pr2.position, t), Quaternion.Lerp(pr1.rotation, pr2.rotation, t));
    }
}