using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using System.IO;

public class ControllerInput : MonoBehaviour
{
    private static ControllerInput instance;

    [SerializeField]
    public XRBaseController controller;
    public InputActionProperty thumbstick;
    public InputActionProperty button;
    public InputActionProperty trigger;
    public InputActionProperty rotation;
    
    float thumbstickValue, buttonValue, triggerValue, prevThumbstickValue = 0, prevButtonValue = 0, prevTriggerValue = 0;
    
    private Menu menuData;
    private string basePath; //change in inspector inly if different that default (different that one set in GUI)
    
    void Awake(){
        if(instance!=null)
            Debug.LogWarning("Multiple instances of ScreenshotHandler");
        instance = this;
        menuData = Menu.instance;
        basePath = menuData.basePath;
    }

    void Update(){
        thumbstickValue = thumbstick.action.ReadValue<float>();
        buttonValue = button.action.ReadValue<float>();
        triggerValue = trigger.action.ReadValue<float>();
        if(thumbstickValue != prevThumbstickValue || buttonValue != prevButtonValue || prevTriggerValue != triggerValue){
            if(buttonValue > 0.9){
                SendHaptics(controller, 0.15f, 0.15f);
                Debug.Log("Yes");
                using (StreamWriter writer = new StreamWriter(basePath + "/buttons.txt", true)){
                    writer.WriteLine(Time.time + ": Yes-Button");
                    writer.Close();
                }
            }
            if(thumbstickValue > 0.9){
                SendHaptics(controller, 0.2f, 0.25f);
                Debug.Log("No");
                using (StreamWriter writer = new StreamWriter(basePath + "/buttons.txt", true)){
                    writer.WriteLine(Time.time + ": No-Button");
                    writer.Close();
                }
            }
            if(triggerValue > 0.9){
                SendHaptics(controller, 0.4f, 0.15f);
                ScreenshotHandler.TakeScreenshot_Static(500,500);
                Debug.Log("Trigger:" + rotation.action.ReadValue<Quaternion>());
                using (StreamWriter writer = new StreamWriter(basePath + "/buttons.txt", true)){
                    writer.WriteLine(Time.time + ": Trigger - q:" + rotation.action.ReadValue<Quaternion>());
                    writer.Close();
                }
            }
            prevThumbstickValue = thumbstickValue; 
            prevButtonValue = buttonValue;
            prevTriggerValue = triggerValue;
        }
    }

    public void SendHaptics (XRBaseController controller, float amplitude, float duration){
        controller.SendHapticImpulse(amplitude, duration);
    }
}

