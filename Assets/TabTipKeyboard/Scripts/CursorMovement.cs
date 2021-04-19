using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CursorMovement : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    public POINT pointInstance;

    [SerializeField]
    uDesktopDuplication.Texture uddTexture;

    [DllImport("UWPLib")]
    extern static string Move(int x, int y, int MouseOption);

    [DllImport("UWPLib")]
    extern static string ClickUp();

    [DllImport("UWPLib")]
    extern static string ClickDown();

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool SetCursorPos(int x, int y);

    public void PointerDown()
    {
        ClickDown();
    }

    public void PointerUp()
    {
        ClickUp();
    }

    public void PointerMove(Vector2Int destination)
    {
        Move((int)(65535.0 * destination.x / uddTexture.monitor.width), (int)(65535.0 * destination.y / uddTexture.monitor.height), 32768); //32768 InjectedInputMouseOptions Enum value for Absolute mode
    }

    private void Start()
    {

    }

    private void Update()
    {
        GetCursorPos(out pointInstance);
    }

}
