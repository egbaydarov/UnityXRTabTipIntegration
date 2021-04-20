using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;
using UnityEngine.UI;

public class MonitorToKeyboard : MonoBehaviour
{
    [SerializeField]
    int yUpSplit;
    [SerializeField]
    int yDownSplit;
    [SerializeField]
    int xLeftSplit;
    [SerializeField]
    int xRightSplit;

    List<Vector4> Caps = new List<Vector4>();
    List<Color> CapsColors = new List<Color>();
    List<GameObject> CapsGOs = new List<GameObject>();
    [SerializeField] GameObject CapsParent;


    UnityEvent OnSpaceUp = new UnityEvent();
    CursorMovement cursorData;
    TMP_Text TextTip;
    CopyTextureBuffer keyboardTexture;
    EnableDisableCalibrationTips tipsDisabler;

    string FileName = "/CalData.dat";

    public void RunCalibrate()
    {
        tipsDisabler.EnableDisableTips(true);

        int pointsCounter = 0;
        var vals = new Vector2Int[4];
        var tips = new string[]
        {
            "Put mouse pointer to upper border of Tab Tip Windows Keyboard then press Space button",
            "Put mouse pointer to down  border of Tab Tip Windows Keyboard then press Space button",
            "Put mouse pointer to left  border of Tab Tip Windows Keyboard then press Space button",
            "Put mouse pointer to right border of Tab Tip Windows Keyboard then press Space button",
            "Successful!"
        };
        TextTip.text = tips[pointsCounter];

        OnSpaceUp.RemoveAllListeners();
        OnSpaceUp.AddListener(() =>
        {
            vals[pointsCounter++] = new Vector2Int(cursorData.pointInstance.X, cursorData.pointInstance.Y);
            Debug.Log(new Vector2Int(cursorData.pointInstance.X, cursorData.pointInstance.Y));

            TextTip.text = tips[pointsCounter];


            if (pointsCounter == 4)
            {
                yUpSplit = vals[0].y;
                yDownSplit = vals[1].y;
                xLeftSplit = vals[2].x;
                xRightSplit = vals[3].x;

                keyboardTexture.UpdateTexutreWithBorders(yUpSplit, yDownSplit, xLeftSplit, xRightSplit);
                tipsDisabler.EnableDisableTips(false);
                OnSpaceUp.RemoveAllListeners();
                SaveCalibratedData();
            }
        });
    }

    //TODO

    //public void AddCap()
    //{
    //    tipsDisabler.EnableDisableTips(true);


    //    int pointsCounter = 0;
    //    var vals = new Vector2Int[4];
    //    var tips = new string[]
    //    {
    //        "Put mouse pointer to upper corner of place where cap will be placed then press Space button",
    //        "Put mouse pointer to down  corner of place where cap will be placed then press Space button",
    //        "Put mouse pointer to left  corner of place where cap will be placed then press Space button",
    //        "Put mouse pointer to right corner of place where cap will be placed then press Space button (Color of pixel under this point will be color of cap)",
    //        "Successful!"
    //    };
    //    TextTip.text = tips[pointsCounter];

    //    OnSpaceUp.RemoveAllListeners();
    //    OnSpaceUp.AddListener(() =>
    //    {
    //        vals[pointsCounter++] = new Vector2Int(cursorData.pointInstance.X, cursorData.pointInstance.Y);
    //        TextTip.text = tips[pointsCounter];


    //        if (pointsCounter == 4)
    //        {
    //            Caps.Add(new Vector4(vals[0].y, vals[1].y, vals[2].x, vals[3].x));
    //            CapsColors.Add(keyboardTexture.GetPixel(vals[3].x, vals[3].y));

    //            OnSpaceUp.RemoveAllListeners();
    //            tipsDisabler.EnableDisableTips(false);
    //            CapsGOs.Add(InitializeCap(Caps[Caps.Count - 1], CapsColors[CapsColors.Count - 1]));
    //            SaveCalibratedData();
    //            OnSpaceUp.RemoveAllListeners();
    //        }
    //    });
    //}

    //GameObject InitializeCap(Vector4 capCoords, Color color)
    //{
    //    GameObject cap = new GameObject();


    //    cap.transform.parent = CapsParent.transform;
    //    cap.AddComponent<CanvasRenderer>();
    //    RectTransform rTransform = cap.AddComponent<RectTransform>();
    //    rTransform.sizeDelta = new Vector2(capCoords.w - capCoords.z, capCoords.y - capCoords.x);
    //    rTransform.localScale = Vector3.one;

    //    Image capImage = cap.AddComponent<Image>();

    //    capImage.color = color;

    //    return cap;
    //}

    public void RemoveAllCaps()
    {
        Caps.Clear();
        CapsColors.Clear();
        foreach (var i in CapsGOs)
            Destroy(i);
        CapsGOs.Clear();

        SaveCalibratedData();
    }

    void Start()
    {
        LoadCalibratedData();
        keyboardTexture.UpdateTexutreWithBorders(yUpSplit, yDownSplit, xLeftSplit, xRightSplit);

    }

    private void Awake()
    {
        cursorData = FindObjectOfType<CursorMovement>();
        keyboardTexture = FindObjectOfType<CopyTextureBuffer>();
        tipsDisabler = FindObjectOfType<EnableDisableCalibrationTips>();
        var TempGO = GameObject.Find("TipTabTipUnity");
        if (TempGO == null)
        {
            enabled = false;
            Debug.Log("[MonitorToKeyboard] Can't find TipTabTibkeyboard GO.");
        }
        TextTip = TempGO.GetComponent<TextMeshProUGUI>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnSpaceUp.Invoke();
        }
    }

    void SaveCalibratedData()
    {
        BinaryFormatter bf = new BinaryFormatter();

        CalibrationData data = new CalibrationData(yUpSplit, xRightSplit, yDownSplit, xLeftSplit, Caps, CapsColors);

        using (FileStream stream = new FileStream(Application.persistentDataPath + FileName, FileMode.OpenOrCreate))
        {
            bf.Serialize(stream, data);
        }
        Debug.Log("Caibration data saved!");
    }

    void LoadCalibratedData()
    {

        if (File.Exists(Application.persistentDataPath + FileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            CalibrationData data;

            using (FileStream stream = new FileStream(Application.persistentDataPath + FileName, FileMode.Open))
            {
                data = (CalibrationData)bf.Deserialize(stream);
            }

            Caps = data.getUnityVectorsForCaps();
            yUpSplit = data.yUpSplit;
            xRightSplit = data.xRightSplit;
            yDownSplit = data.yDownSplit;
            xLeftSplit = data.xLeftSplit;
            CapsColors = data.getUnityVectorsForCapsColors();

            for (int i = 0; i < Caps.Count; ++i)
            {
                //CapsGOs.Add(InitializeCap(Caps[i], CapsColors[i]));//TODO
            }

            Debug.Log("Calibration data loaded!");
        }
        else
            Debug.LogWarning("There is no pre-saved calibration data data!");
    }
}

[Serializable]
class CalibrationData
{
    public int yUpSplit;
    public int yDownSplit;
    public int xLeftSplit;
    public int xRightSplit;

    public Vector4Serializsable[] Caps = null;
    public Vector4Serializsable[] CapsColors = null;

    public List<Vector4> getUnityVectorsForCaps()
    {
        List<Vector4> vectors = new List<Vector4>();
        if (Caps != null)
            foreach (var i in Caps)
                vectors.Add(new Vector4(i.x, i.y, i.z, i.w));
        return vectors;
    }

    public List<Color> getUnityVectorsForCapsColors()
    {
        List<Color> vectors = new List<Color>();
        if (CapsColors != null)
            foreach (var i in CapsColors)
                vectors.Add(new Color(i.x, i.y, i.z, i.w));
        return vectors;
    }

    public CalibrationData(int yUpSplit, int xRightSplit, int yDownSplit, int xLeftSplit, List<Vector4> caps, List<Color> capsColors)
    {
        this.yUpSplit = yUpSplit;
        this.yDownSplit = yDownSplit;
        this.xLeftSplit = xLeftSplit;
        this.xRightSplit = xRightSplit;
        Caps = new Vector4Serializsable[caps.Count];
        CapsColors = new Vector4Serializsable[capsColors.Count];
        for (int i = 0; i < Caps.Length; ++i)
        {
            Caps[i] = new Vector4Serializsable(caps[i]);
        }

        for (int i = 0; i < CapsColors.Length; ++i)
        {
            CapsColors[i] = new Vector4Serializsable(capsColors[i]);
        }
    }

    public CalibrationData()
    {
    }

    [Serializable]
    public class Vector4Serializsable
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Vector4Serializsable()
        {
        }

        public Vector4Serializsable(Vector4 cap)
        {
            this.x = (int)cap.x;
            this.y = (int)cap.y;
            this.z = (int)cap.z;
            this.w = (int)cap.w;
        }

        public Vector4Serializsable(Color capColor)
        {
            this.x = capColor.r;
            this.y = capColor.g;
            this.z = capColor.b;
            this.w = capColor.a;
        }
    }
}
