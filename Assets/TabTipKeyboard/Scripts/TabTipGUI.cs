using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabTipGUI : MonoBehaviour
{
    [SerializeField] ExternTextField ExternTextField;
    [SerializeField] MonitorToKeyboard mTK;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 45, 110, 30), new GUIContent("Show Keyboard")))
            ExternTextField.ShowKeyboard();
        if (GUI.Button(new Rect(10, 80, 175, 30), new GUIContent("Calibrate Keyboard Texture")))
            mTK.RunCalibrate();
        //if (GUI.Button(new Rect(10, 115, 70, 30), new GUIContent("Add Cap")))
        //    mTK.AddCap(); //TODO
        if (GUI.Button(new Rect(10, 115, 110, 30), new GUIContent("Clear")))
            ExternTextField.ClearExterTextField();
        //if (GUI.Button(new Rect(10, 150, 110, 30), new GUIContent("Remove All Caps")))
        //    mTK.RemoveAllCaps();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
