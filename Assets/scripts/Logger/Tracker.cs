using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Tracker : MonoBehaviour
{
    [SerializeField]
    public string filename = "";
    private Menu menuData;
    string fullFilename;
    [SerializeField]
    public float frequency = 10.0f;
    [SerializeField]
    public bool3 track_position = new bool3(true, true, true);
    [SerializeField]
    public bool4 track_orientation = new bool4();

    bool tracking = false;

    float time, onePerFreq, timeOffset;
    [SerializeField]
    public bool saved = false;
    bool stop = false;
    TrackingController controller;
    // Start is called before the first frame update
    void Start()
    {
        //Get the controller instance and add yourself as a tracker. Also calculate time between tracking points from frequency
        controller = TrackingController.instance;
        controller.AddTracker(this);
        onePerFreq = 1/frequency;
        menuData = Menu.instance;
        fullFilename = System.IO.Path.Combine(TrackingController.instance.basePath + "/" + filename);    
    }

    // Update is called once per frame
    void Update()
    {
        //Update the time
        time = Time.time;
    }

    public void startTracking(float timeO)
    {
        //If we aren't already tracking then start tracking and set the time offset
        if (!tracking)
        {
            tracking = true;
            time = Time.time-timeO;
            timeOffset = timeO;
            stop = false;
            saved = false;
            StartCoroutine("TrackIE");
        }
    }

    public void stopTracking()
    {
        //If we are tracking then stop trakcing
        if(tracking)
        {
            tracking = false;
            saved = false;
            stop = true;
        }
    }

    public IEnumerator TrackIE()
    {
        //Start tracking
        WaitForSeconds wait = new WaitForSeconds(onePerFreq);

        //Add starting point
        Vector3 position = new Vector3(track_position.x ? transform.position.x : 0, 
                                    track_position.y ? transform.position.y : 0,
                                    track_position.z ? transform.position.z : 0);
        Quaternion rotation = new Quaternion(track_orientation.x ? transform.rotation.x : 0,
                                            track_orientation.y ? transform.rotation.y : 0,
                                            track_orientation.z ? transform.rotation.z : 0,
                                            track_orientation.w ? transform.rotation.w : 0);
        TrackingPoint point = new TrackingPoint(position, rotation.normalized, time);

        
        yield return wait;

        using (StreamWriter writer = new StreamWriter(fullFilename))
        {
            writer.Write(point.position.x + ", " + point.position.y + ", " + point.position.z + ", " + point.orientation.x + ", " + point.orientation.y + ", " + point.orientation.z + ", " + point.orientation.w + ", " + point.time + "\n");
            writer.Flush();
            while (!stop)
            {
                //Create new point
                position = new Vector3(track_position.x ? transform.position.x : 0,
                                            track_position.y ? transform.position.y : 0,
                                            track_position.z ? transform.position.z : 0);
                rotation = new Quaternion(track_orientation.x ? transform.rotation.x : 0,
                                                    track_orientation.y ? transform.rotation.y : 0,
                                                    track_orientation.z ? transform.rotation.z : 0,
                                                    track_orientation.w ? transform.rotation.w : 0);
                point.position = position;
                point.orientation = rotation.normalized;
                point.time = time;

                writer.Write(point.position.x + ", " + point.position.y + ", " + point.position.z + ", " + point.orientation.x + ", " + point.orientation.y + ", " + point.orientation.z + ", " + point.orientation.w + ", " + point.time + "\n");
                writer.Flush();
                
                //Wait for the time calculated from frequency            
                yield return wait;
            }

        }
        saved = true;
    }
}
