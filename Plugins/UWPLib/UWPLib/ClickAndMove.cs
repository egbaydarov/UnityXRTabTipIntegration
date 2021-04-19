using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.UI.Input.Preview.Injection;

namespace UWPLib
{
    public class ClickAndMove
    {
        [DllExport(CallingConvention.StdCall)]
        public static string GetCursorXY(out double[] XY)
        {
            try
            {
                XY = new double[2];
                var pointerPosition = Windows.UI.Core.CoreWindow.GetForCurrentThread().PointerPosition;
                XY[0] = pointerPosition.X;
                XY[1] = pointerPosition.Y;   
            }
            catch (Exception ex)
            {
                XY = new double[2] { 0,0};
                return "Exception:" + ex.ToString() + "exMessage:" + ex.Message;
            }
            return "Ok";
        }


        [DllExport(CallingConvention.StdCall)]
        public static string Move(int x, int y, int MouseOption)
        {
            try
            {
                InputInjector ij = InputInjector.TryCreate();

                List<InjectedInputMouseInfo> input = new List<InjectedInputMouseInfo>();

                input.Add(new InjectedInputMouseInfo()
                {
                    MouseOptions = (InjectedInputMouseOptions)MouseOption,
                    DeltaX = x,
                    DeltaY = y
                });

                ij.InjectMouseInput(input);
            }
            catch (Exception ex)
            {
                return "Exception:" + ex.ToString() + "exMessage:" + ex.Message;
            }
            return "Ok";
        }

        [DllExport(CallingConvention.StdCall)]
        public static string ClickDown()
        {
            try
            {
                InputInjector ij = InputInjector.TryCreate();

                List<InjectedInputMouseInfo> input = new List<InjectedInputMouseInfo>();

                input.Add(new InjectedInputMouseInfo() { MouseOptions = InjectedInputMouseOptions.LeftDown });

                ij.InjectMouseInput(input);
            }
            catch (Exception ex)
            {
                return "Exception:" + ex.ToString() + "exMessage:" + ex.Message;
            }
            return "Ok";
        }

        [DllExport(CallingConvention.StdCall)]
        public static string ClickUp()
        {
            try
            {
                InputInjector ij = InputInjector.TryCreate();

                List<InjectedInputMouseInfo> input = new List<InjectedInputMouseInfo>();

                input.Add(new InjectedInputMouseInfo() { MouseOptions = InjectedInputMouseOptions.LeftUp });

                ij.InjectMouseInput(input);
            }
            catch (Exception ex)
            {
                return "Exception:" + ex.ToString() + "exMessage:" + ex.Message;
            }
            return "Ok";
        }
    }
}
