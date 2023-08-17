using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomDelayedBuffer<T>
{
        
    struct ValueTimePair
    {
        public T value;
        public float time;

        public ValueTimePair(T value, float time)
        {
            this.value = value;
            this.time = time;
        }
    }

    Queue<ValueTimePair> queue;
    public float delay;
    ValueTimePair currentValue;
    ValueTimePair next;
    float timeAdded;
    float latestTime;
    float requestedTime;

    public CustomDelayedBuffer()
    {
        queue = new Queue<ValueTimePair>();
        delay = 1f;
        currentValue = new ValueTimePair(default(T),-9999999999);
        timeAdded = 0f;
        latestTime = 0f;
        requestedTime = -delay;
    }

    public CustomDelayedBuffer(float delay)
    {
        queue = new Queue<ValueTimePair>();
        this.delay = delay;
        currentValue = new ValueTimePair(default(T), -9999999999);
        timeAdded = 0f;
        latestTime = 0f;
        requestedTime = -delay;
    }

    public void Add(T value)
    {
        latestTime = latestTime + Time.time - timeAdded;
        timeAdded = Time.time; 
        queue.Enqueue(new ValueTimePair(value, latestTime));
        if (queue.Count == 1)
            currentValue = queue.Peek();
    }

    public T addTime(float t, bool interpolate = false, System.Func<T, T, float, T> interpolator = null)
    {
        requestedTime += t;
        return getCurrentValue(interpolate, interpolator);
    }

    public T getCurrentValue(bool interpolate = false, System.Func<T, T, float, T> interpolator = null)
    {
        float delayedTime = requestedTime;
        for(int i = 0; i < queue.Count; i++)
        {
            next = queue.Peek();
            if (next.time > delayedTime)
            {
                if(!interpolate)
                    return currentValue.value;
                float t = Mathf.Clamp01((delayedTime - currentValue.time) / (next.time - currentValue.time));
                return interpolator(currentValue.value,next.value,t);
            }
            else
                currentValue = queue.Dequeue();
        }
        return currentValue.value;
    }
}

