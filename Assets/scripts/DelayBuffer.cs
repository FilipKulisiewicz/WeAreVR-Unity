using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CustomBuffer<T>
{
    private Queue<ValueTimePair> queue;
    private ValueTimePair nextSample;
    private ValueTimePair currentSample;
    private bool firstFrame = true;
    public static double initialRosTime;
    private float unityTime;
    private float delayedTime;
    private float deltaTime;
    
    struct ValueTimePair
    {
        public T value;
        public float time;

        public ValueTimePair(T value, float unityTime)
        {
            this.value = value;
            this.time = unityTime;
        }
    }

    public CustomBuffer()
    {
        queue = new Queue<ValueTimePair>();
        currentSample = default(ValueTimePair);
    }

    private float syncDiffTime = 2.5f;
    private float syncScaleFactor = 1.1f;
    public void Add(T value, double rosTime)
    {
        if(firstFrame){
            initialRosTime = rosTime;
            firstFrame = false;
        }
        unityTime = covertToUnityTime(rosTime);
        queue.Enqueue(new ValueTimePair(value, unityTime));
        if(Math.Abs(unityTime - Time.time) > 0.05f){  //sync to Time.time //does not work (?) for lower frequencies 
            //Debug.Log("RosData sync correction");
            if(syncDiffTime > 0.001f){
                syncDiffTime = syncDiffTime / syncScaleFactor;
            }
            if(unityTime < Time.time - 0.05f){
                initialRosTime -= syncDiffTime;
            }
            else if(unityTime > Time.time + 0.05f){
                initialRosTime += syncDiffTime;
            }
        }
    }
    private float covertToUnityTime(double rosTime){
        return (float)(rosTime - initialRosTime);
    }

    public int Size()
    {
        return queue.Count;
    }
    

    public T GetDelayedValue(float delay, bool interpolate = false, System.Func<T, T, float, T> Interpolator = null)
    {
        delayedTime = Time.time - (Time.deltaTime / 10.0f ) - delay; // test
        //delayedTime = Time.time - delay;
        for(int i = 0; i < queue.Count; i++)
        {
            nextSample = queue.Peek();
            if (nextSample.time > delayedTime)
            {
                if(interpolate){
                    deltaTime = Mathf.Clamp01((delayedTime - currentSample.time) / (nextSample.time - currentSample.time));
                    return Interpolator(currentSample.value, currentSample.value, deltaTime); 
                }
                else{
                    return currentSample.value;
                }
            }
            else
            {
                currentSample = queue.Dequeue();
            }
        }
        //Debug.Log("Failed to match Orientation data for requested time");
        return currentSample.value;
    }
    
    public T GetNextFrameDelayedValue(float requestedTime, float delay, bool interpolate = false, System.Func<T, T, float, T> Interpolator = null)
    {
        delayedTime = requestedTime - delay; // Time.time - delay;
        for(int i = 0; i < queue.Count; i++)
        {
            //Debug.Log(queue.Peek().time);
            nextSample = queue.Peek();
            if (nextSample.time > delayedTime)
            {
                if(interpolate){
                    deltaTime = Mathf.Clamp01((delayedTime - currentSample.time) / (nextSample.time - currentSample.time));
                    return Interpolator(currentSample.value, nextSample.value, deltaTime); 
                }
                else{
                    return currentSample.value;
                }
            }
            else
            {
                currentSample = queue.Dequeue();
            }
        }
        //Debug.Log("Failed to match Orientation data for requested time");
        return currentSample.value;
    }
}
