using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingController : MonoBehaviour
{
    //Control all tracking points
    public static TrackingController instance;  //Quick Singleton pattern
    float startTime, stopTime;
    
    public List<Tracker> trackers = new List<Tracker>(); //List of points we are tracking
    [SerializeField]
    public bool showGUI = false;
    [SerializeField]
    public bool hasSaved = false;
    private bool startedSave = false;

    private Menu menuData;
    public string basePath;

    void Awake()
    {
        if(instance!=null)
            Debug.LogWarning("Multiple instances of TrackingController");
        instance = this;
        menuData = Menu.instance;
        basePath = menuData.basePath;
        startTime = -1;
    }


    void OnGUI()
    {
        if (!showGUI)
            return;
        //Buttons to control the tracking
        if (GUI.Button(new Rect(10, 10, 150, 30), "Start Tracking"))
            startTracking();
        if (GUI.Button(new Rect(10, 50, 150, 30), "Stop Tracking"))
            stopTracking();
    }

    public void startTracking()
    {
        //Starting the tracking
        //If startTime == 0 we are starting tracking for the first time. This means we should start from 0. Otherwise we start from where we paused tracking
        if(startTime == 0)
            startTime = Time.time;
        else
            startTime += (Time.time-stopTime);
        
        //Tell each tracker to start tracking
        foreach(Tracker tracker in trackers)
            tracker.startTracking(startTime);
    }

    public void stopTracking()
    {
        //Tell each tracker to stop tracking
        stopTime = Time.time;
        startedSave = true;
        hasSaved = false;
        foreach (Tracker tracker in trackers)
            tracker.stopTracking();
    }

    public void AddTracker(Tracker tracker){
        //Add new tracker to the list
        trackers.Add(tracker);
    }

    private void Update()
    {
        if(startTime < 0){
            startTracking();
        }
        if(startedSave)
        {
            if(!hasSaved)
            {
                bool finished = true;
                foreach(Tracker t in trackers)
                {
                    if (!t.saved)
                        finished = false;
                }
                if (finished)
                    hasSaved = true;
            }
        }
    }
}
