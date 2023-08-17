using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleUnwind : MonoBehaviour
{

    public float delay;

    CustomDelayedBuffer<PositionRotation> buffer;
    public WebCamTexture webcam; // YOU NEED TO GET THIS FROM THE WEBCAM SCRIPT!!!
    public float frametime = 0.03333333f; //This should be the 1/fps of the camera

    public Transform tracker;
    // Start is called before the first frame update
    void Start()
    {
        buffer = new CustomDelayedBuffer<PositionRotation>(delay);
        //webcam = webcamscript.webcamtexture !!!!!!!
    }

    // Update is called once per frame
    void Update()
    {
        //Every frame save the position and rotation
        buffer.Add(new PositionRotation(tracker.position, tracker.rotation));

        //Only update the position and rotation when the image updates
        if(webcam.didUpdateThisFrame)
        {
            PositionRotation posrot = buffer.addTime(frametime, true, interpolate);

            transform.position = posrot.position;//Change these lines to match the original unwinding
            transform.rotation = posrot.rotation;//Change these lines to match the original unwinding
        }
    }

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

    public PositionRotation interpolate(PositionRotation pr1, PositionRotation pr2, float t)
    {
        return new PositionRotation(Vector3.Lerp(pr1.position, pr2.position, t), Quaternion.Lerp(pr1.rotation, pr2.rotation, t));
    }
}
