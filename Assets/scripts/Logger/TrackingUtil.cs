
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class TrackingPoint
{
    //Class containing tracked position, orientation and time
    public Vector3 position;
    public Quaternion orientation;
    public float time;

    public TrackingPoint()
    {
        position = new Vector3();
        orientation = new Quaternion(0,0,0,1);
        time = 0.0f;
    }

    public TrackingPoint(Vector3 position, Quaternion orientation, float time)
    {
        this.position = position;
        this.orientation = orientation;
        this.time = time;
    }

    public override bool Equals(object obj)
    {
        //Equals only checks location and position, not the time. This is for optimization
        if(obj==null)
            return false;
        TrackingPoint p = obj as TrackingPoint;
        if((System.Object)p==null)
            return false;
        return (p.position.Equals(this.position) && p.orientation.Equals(this.orientation));
    }

    public bool Equals(TrackingPoint p)
    {
        if((object)p==null)
            return false;
        return (p.position.Equals(this.position) && p.orientation.Equals(this.orientation));
    }

    public static void Clone(TrackingPoint f, TrackingPoint s)
    {
        //Copy the variables over to another object
        s.position.x = f.position.x;
        s.position.y = f.position.y;
        s.position.z = f.position.z;

        s.orientation.x = f.orientation.x;
        s.orientation.y = f.orientation.y;
        s.orientation.z = f.orientation.z;
        s.orientation.w = f.orientation.w;
        s.time = f.time;
    }
}

[System.Serializable]
public class Path
{
    //Path class containing list of tracking points
    public List<TrackingPoint> points;

    public Path()
    {
        points = new List<TrackingPoint>();
    }

    public Path(string file)
    {
        //Load path from file
         if (File.Exists(file))
        {
            // Read the entire file and save its contents.
            string fileContents = File.ReadAllText(file);
            points = new List<TrackingPoint>();
            foreach(string line in fileContents.Split('\n'))
            {
                if(line.Length>0)
                {
                    //Parse file line into Tracking point using schema: position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, rotation.w, time
                    try
                    {
                        string[] parts = line.Split(',');
                        TrackingPoint p = new TrackingPoint();
                        p.position.x = float.Parse(parts[0]);
                        p.position.y = float.Parse(parts[1]);
                        p.position.z = float.Parse(parts[2]);
                        p.orientation.x = float.Parse(parts[3]);
                        p.orientation.y = float.Parse(parts[4]);
                        p.orientation.z = float.Parse(parts[5]);
                        p.orientation.w = float.Parse(parts[6]);
                        p.time = float.Parse(parts[7]);
                        points.Add(p);
                    }
                    catch
                    {
                        Debug.LogError("Error parsing line: "+line);
                    }
                    
                }
            }
        }
    }

    public void Save(string file)
    {
         // Serialize the object into string using schema:  position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, rotation.w, time
        string dataString = "";
        foreach(TrackingPoint p in points)
        {
            dataString += p.position.x + ", " + p.position.y + ", " + p.position.z + ", " + p.orientation.x + ", " + p.orientation.y + ", " + p.orientation.z + ", " + p.orientation.w + ", " + p.time + "\n";
        }

        // Write data to file.
        File.WriteAllText(file, dataString);
    }


}
[System.Serializable]
public class bool3
{
    //Vector3 of booleans
    public bool x,y,z;

    public bool3(bool x, bool y, bool z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public bool3()
    {
        this.x = false;
        this.y = false;
        this.z = false;
    }

    public override bool Equals(object obj)
    {
        if(obj == null)
            return false;
        
        bool3 b = obj as bool3;
        if((System.Object)b == null)
            return false;
        return this.x == b.x && this.y == b.y && this.z == b.z;
    }

    public bool Equals(bool3 b)
    {
        if((object)b==null)
            return false;
        return this.x == b.x && this.y == b.y && this.z == b.z;
    }

}

[System.Serializable]
public class bool4
{
    //Vector4 of booleans
    public bool x, y, z, w;

    public bool4(bool x, bool y, bool z, bool w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public bool4()
    {
        this.x = false;
        this.y = false;
        this.z = false;
    }
    public override bool Equals(object obj)
    {
        if(obj == null)
            return false;
        
        bool4 b = obj as bool4;
        if((System.Object)b == null)
            return false;
        return this.x == b.x && this.y == b.y && this.z == b.z && this.w == b.w;
    }

    public bool Equals(bool4 b)
    {
        if((object)b==null)
            return false;
        return this.x == b.x && this.y == b.y && this.z == b.z && this.w == b.w;
    }
}