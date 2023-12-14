using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public static Menu instance;

    private Rect windowRect = new Rect(20, 20, 250, 390);
    public static string participantID = "";
    public int selectedOption = -1;
    public string[] optionNames = new string[]{ "V1_UN", "V1_N_UN", "V2_UN", "V2_N_UN" };
    private int oldOption = -1;

    [SerializeField]
    public string basePath;

    void Awake(){
        if(instance!=null){
            Debug.LogWarning("Multiple instances of Menu");
        }
        instance = this;
    }

    private void OnGUI(){
        windowRect = GUI.Window(0, windowRect, windowFunction, "Controls");
    }

    void windowFunction(int windowID){
        participantID = GUI.TextField(new Rect(5, 20, 240, 45), participantID);
        selectedOption = GUI.SelectionGrid(new Rect(5, 70, 240, 240), selectedOption, optionNames, 1);
        if(selectedOption != oldOption){ 
            switch(selectedOption){
                case 0:
                    Debug.Log("Selected option V1_UN (0)");
                    break;
                case 1:
                    Debug.Log("Selected option V1_N_UN (1)");
                    break;
                case 2:
                    Debug.Log("Selected option V2_UN (2)");
                    break;
                case 3:
                    Debug.Log("Selected option V2_N_UN (3)");
                    break;
                default:
                    break;
            }
            oldOption = selectedOption;
        }
        if (GUI.Button(new Rect(5, 330, 240, 40), "Start Tracking")){
            Debug.Log("Participants id: " + participantID);
            basePath = basePath + "/ParticipantsData/" + participantID + "_" + selectedOption;
            System.IO.Directory.CreateDirectory(basePath);
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive);
        }
    }
}
