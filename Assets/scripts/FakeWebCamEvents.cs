using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;   
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class tick : MonoBehaviour
{
    public static event Action newFrame;
    private float fireTimestamp; 
    private bool timestamp;
    private VideoPlayer[] videoPlayersList;
    private VideoPlayer videoPlayer;
    private long frame = 0;

    void Start(){
        videoPlayersList = GetComponents<VideoPlayer>();
        foreach (VideoPlayer vp in videoPlayersList){
            if(vp.enabled){
                videoPlayer = vp;
            }
        }
        frame = videoPlayer.frame;
    }

    void Update(){        
        if(frame < videoPlayer.frame){
            newFrame?.Invoke(); 
            frame = videoPlayer.frame;
        }
        else if(videoPlayer.frame == Convert.ToInt64(videoPlayer.clip.frameCount)){
            SceneManager.UnloadScene("SampleScene");
        } 
        // if (Input.GetKeyDown(KeyCode.Space)){ //Stopping mechanism - disabled during study
        //     Debug.Log("space key was pressed");
        //     if(videoPlayer.isPaused){
        //         videoPlayer.Play();
        //     }
        //     else{
        //         videoPlayer.Pause();
        //     }
        // }
    }
}
