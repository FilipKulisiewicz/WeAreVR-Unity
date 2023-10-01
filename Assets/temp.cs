using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp : MonoBehaviour
{
    [SerializeField]
    private GameObject sphere;  
    // Start is called before the first frame update
    public Quaternion rot;
    void Start()
    {
        rot = new Quaternion(0.71f, 0.0f, 0.0f, 0.71f);// new Quaternion(0.71f, 0f ,0f, 0.71f);
        sphere.transform.rotation = rot * new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        sphere.transform.rotation = rot * new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
    }
}
