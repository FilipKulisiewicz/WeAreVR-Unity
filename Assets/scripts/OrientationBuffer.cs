using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using System;
using System.IO;

public struct PositionRotation{
    public Vector3 position;
    public Quaternion rotation;

    public PositionRotation(Vector3 position, Quaternion rotation){
        this.position = position;
        this.rotation = rotation;
    }
}

public class OrientationBuffer : MonoBehaviour{
    [SerializeField]
    private string rosTopic = "tool0";
    [SerializeField]
    private bool useFile = false;
    [SerializeField]
    private string fileName;
    [SerializeField]
    private static CustomBuffer<PositionRotation> buffer;
    private TransformStampedMsg TransformMsg;
    private Vector3 rosTranslation;
    private Quaternion rosQuaternion;
    private double rosTime;

    private void OnEnable(){
        SphereManager.TransformRequest += GetDelayedValue;
        SphereManager.TransformRequestFrame += GetNextFrameDelayedValue;
    }
    
    private void OnDisable(){
        SphereManager.TransformRequest -= GetDelayedValue;
        SphereManager.TransformRequestFrame -= GetNextFrameDelayedValue;
    }

    void Awake(){
        buffer = new CustomBuffer<PositionRotation>();
        if(useFile == true){
            TextAsset csvFile = Resources.Load<TextAsset>(fileName); // Load the CSV file from the Resources folder
            ParseCSV(csvFile, buffer);
        }
        else{
            ROSConnection.GetOrCreateInstance().Subscribe<TransformStampedMsg>(rosTopic, AddToBuffer);
        }
    }    

    public void AddToBuffer(TransformStampedMsg transformMessage){
        //translation //potentially TODO
        rosTranslation = new Vector3((float)transformMessage.transform.translation.x, (float)transformMessage.transform.translation.y, (float)transformMessage.transform.translation.z);
        //rotations
        if(string.Equals(rosTopic, "tool0")){
            rosQuaternion = ConvertFromRobotCoordinatesToUnityCoordinatesUnwinded(transformMessage.transform.rotation);
        }
        else if (string.Equals(rosTopic, "IMU")){
            rosQuaternion = ConvertFromIMUCoordinatesToUnityCoordinatesUnwinded(transformMessage.transform.rotation);
        }
        //time
        rosTime = (double)transformMessage.header.stamp.sec + ((double)transformMessage.header.stamp.nanosec / 1e+09);
        buffer.Add(new PositionRotation(rosTranslation, rosQuaternion), rosTime);
    }

    private Quaternion ConvertFromRobotCoordinatesToUnityCoordinatesUnwinded(RosMessageTypes.Geometry.QuaternionMsg robotQuaternion){   
        rosQuaternion = new Quaternion((float)robotQuaternion.y, (float)robotQuaternion.z, -(float)robotQuaternion.x, (float)robotQuaternion.w); //Quaternion(x, y, z, w) 
        return rosQuaternion;
    } 
    
    private Quaternion ConvertFromIMUCoordinatesToUnityCoordinatesUnwinded(RosMessageTypes.Geometry.QuaternionMsg imuQuaternion){
        rosQuaternion = new Quaternion(-(float)imuQuaternion.z, -(float)imuQuaternion.x, (float)imuQuaternion.y, (float)imuQuaternion.w); //Quaternion(x, y, z, w)
        return rosQuaternion;
    }
    
    private static PositionRotation GetNextFrameDelayedValue(float requestedTime, float delay, bool interpolate){
        return buffer.GetNextFrameDelayedValue(requestedTime, delay, interpolate, Interpolate);
    }

    private static PositionRotation GetDelayedValue(float delay, bool interpolate){
        return buffer.GetDelayedValue(delay, interpolate, Interpolate);
    }
    
    public static PositionRotation Interpolate(PositionRotation pr1, PositionRotation pr2, float t){
        return new PositionRotation(Vector3.Lerp(pr1.position, pr2.position, t), Quaternion.Lerp(pr1.rotation, pr2.rotation, t));
    }

    void ParseCSV(TextAsset csv, CustomBuffer<PositionRotation> buffer)
    {
        string[] lines = csv.text.Split('\n');
        foreach (string line in lines)
        {
            string[] cells = line.Split(',');
            if (cells.Length >= 11) // Ensure there are enough columns
            {
                int timeSeconds = int.Parse(cells[0]);
                int timeNanoseconds = int.Parse(cells[1]);
                double rosTime = (double)timeSeconds + ((double)timeNanoseconds / 1e+9f);
                Vector3 position = new Vector3(float.Parse(cells[4]), float.Parse(cells[5]), float.Parse(cells[6]));
                Quaternion rotation = new Quaternion(float.Parse(cells[7]), float.Parse(cells[8]), float.Parse(cells[9]), float.Parse(cells[10]));
                rotation = new Quaternion(-(float)rotation.z, -(float)rotation.x, (float)rotation.y, (float)rotation.w); //Quaternion(x, y, z, w) //ConvertFromIMUCoordinatesToUnityCoordinatesUnwinded
                buffer.Add(new PositionRotation(position, rotation), rosTime, false);
            }
        }
    }
}