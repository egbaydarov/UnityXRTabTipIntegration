using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;


public class ExternTextField : MonoBehaviour
{
    [SerializeField]
    public string ExternTextFieldData = "";

    System.Diagnostics.Process TFProc;
    System.Diagnostics.Process TabTipProc;

    EventWaitHandle TextChangedEvent;
    EventWaitHandle TextFieldClearEvent;
    MemoryMappedFile mmf;

    const int MMF_MAX_SIZE = 1024;
    const int MMF_VIEW_SIZE = 1024;
    byte[] buffer = new byte[MMF_VIEW_SIZE];

    MemoryMappedViewStream TextFieldDataStream = null;
    public static bool IsSharedMomeryReachable = true;

    private void Update()
    {
        if(TFProc == null || TFProc.HasExited)
        {
            RestartApp();
        }

    }

    public void ClearExterTextField()
    {
        TextFieldClearEvent.Set();
    }

    void IPCsetup()
    {
        TextChangedEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "ewhTabTipKeyboard");
        TextFieldClearEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "ewhTabTipClear");
        mmf = MemoryMappedFile.CreateOrOpen("hookTabTip", MMF_MAX_SIZE, MemoryMappedFileAccess.ReadWrite);
        TextFieldDataStream = mmf.CreateViewStream(0, MMF_VIEW_SIZE);

        var thread = new Thread(() =>
        {
            while (true)
            {
                TextChangedEvent.WaitOne();
                TextReader textReader = new StreamReader(TextFieldDataStream);
                ExternTextFieldData = textReader.ReadLine();
                TextFieldDataStream.Seek(0, SeekOrigin.Begin);
            }
        });
        thread.IsBackground = true;
        thread.Start();
    }

    private void Start()
    {
        IPCsetup();

        RestartApp();
        ShowKeyboard();
    }

    private void OnApplicationQuit()
    {
        TFProc.Kill();
        IsSharedMomeryReachable = false;
        //closeOnscreenKeyboard(); //TODO FIX COM EXCEPTION
    }

    public void RestartApp()
    {
        System.Diagnostics.Process[] pname = System.Diagnostics.Process.GetProcessesByName("TextFieldWidget");
        if (pname.Length == 0)
        {
            TFProc = System.Diagnostics.Process.Start(Application.dataPath + "\\StreamingAssets\\TabTipKeyboard\\TextFieldWidget.exe");
        }

        enabled = true;
    }

    public void ShowKeyboard()
    {
        System.Diagnostics.Process[] pname = System.Diagnostics.Process.GetProcessesByName("tabtip");
        if (pname.Length == 0)
        {
            string tabtipId = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) + @"\microsoft shared\ink\tabtip.exe";
            TabTipProc = System.Diagnostics.Process.Start(tabtipId);
        }
        else
        {
            TabTipProc = pname[0];
        }
        if (!IsKeyboardVisible())
        {
            var uiHostNoLaunch = new UIHostNoLaunch();
            var tipInvocation = (ITipInvocation)uiHostNoLaunch;
            tipInvocation.Toggle(GetDesktopWindow());
            Marshal.ReleaseComObject(uiHostNoLaunch);
        }
    }



    #region Show keyboard

    [ComImport, Guid("4ce576fa-83dc-4F88-951c-9d0782b4e376")]
    class UIHostNoLaunch
    {
    }

    [ComImport, Guid("37c994e7-432b-4834-a2f7-dce1f13b834b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ITipInvocation
    {
        void Toggle(IntPtr hwnd);
    }

    [DllImport("user32.dll", SetLastError = false)]
    static extern IntPtr GetDesktopWindow();
    #endregion

    #region check is keyboard visible
    /// <summary>
    /// The window is initially visible. See http://msdn.microsoft.com/en-gb/library/windows/desktop/ms632600(v=vs.85).aspx.
    /// </summary>
    public const UInt32 WS_VISIBLE = 0X94000000;
    /// <summary>
    /// Specifies we wish to retrieve window styles.
    /// </summary>
    public const int GWL_STYLE = -16;

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(String sClassName, String sAppName);

    [DllImport("user32.dll", SetLastError = true)]
    static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);


    /// <summary>
    /// Checks to see if the virtual keyboard is visible.
    /// </summary>
    /// <returns>True if visible.</returns>
    public static bool IsKeyboardVisible()
    {
        IntPtr keyboardHandle = GetKeyboardWindowHandle();

        bool visible = false;

        if (keyboardHandle != IntPtr.Zero)
        {
            UInt32 style = GetWindowLong(keyboardHandle, GWL_STYLE);
            visible = (style == WS_VISIBLE);
        }

        return visible;
    }

    public static IntPtr GetKeyboardWindowHandle()
    {
        return FindWindow("IPTip_Main_Window", null);
    }
    #endregion

    #region hide keyboard
    [DllImport("user32.dll")]
    public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);


    public const int WM_SYSCOMMAND = 0x0112;
    public const int SC_CLOSE = 0xF060;

    private void closeOnscreenKeyboard()
    {
        // retrieve the handler of the window  
        int iHandle = (int)FindWindow("IPTIP_Main_Window", "");
        if (iHandle > 0)
        {
            // close the window using API        
            SendMessage(iHandle, WM_SYSCOMMAND, SC_CLOSE, 0);
        }
    }
    #endregion


}
