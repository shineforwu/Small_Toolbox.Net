/**
 * @Author: Shine Wu
 * @Date:   2025-02-21 20:54:00
 * @Last Modified by:   Shine Wu
 * @Last Modified time: 2025-02-21 22:08:19
 */

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace SmallToolBox
{
    public class MyHook
    {

        // 引入 Windows API 函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        // 引入 Windows API 函数用于消息循环
        [DllImport("user32.dll")]
        private static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        private static extern bool TranslateMessage(ref MSG lpMsg);

        [DllImport("user32.dll")]
        private static extern IntPtr DispatchMessage(ref MSG lpMsg);

        [DllImport("user32.dll")]
        private static extern void PostQuitMessage(int nExitCode);

        // 引入 Windows API 函数用于窗口操作
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);


        // 消息结构体
        public struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public Point pt;
        }

        public struct Point
        {
            public int X;
            public int Y;

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        // 定义常量
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int VK_CONTROL = 0x11;
        private const int VK_MENU = 0x12; // Alt 键的虚拟键码

        private const int VK_F1 = 0x70;
        private const int VK_F2 = 0x71;
        private const int VK_F3 = 0x72;
        private const int VK_F4 = 0x73;
        private const int VK_F5 = 0x74;
        private const int VK_F6 = 0x75;
        private const int VK_F9 = 0x78;
        private const int VK_F10 = 0x79;
        private const int VK_F11 = 0x7A;
        private const int VK_F12 = 0x7B;
        private const int VK_ESCAPE = 0x1B; // Esc 键的虚拟键码

        private const uint KEYEVENTF_KEYDOWN = 0x0000;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        public bool flag = true;



        /// 定义委托
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        // 钩子句柄
        public IntPtr _hookId = IntPtr.Zero;

        public Action<int> press_key_action;

        // 钩子回调函数
        public IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                //MyLogger.Instance.Debug("MyListening", $"in HookCallback");
                if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_KEYUP))
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    if (wParam == (IntPtr)WM_KEYDOWN && vkCode == VK_ESCAPE)
                    {
                        // 按下 Esc 键，设置 flag 为 false 以退出消息循环
                        flag = false;
                        MyLogger.Instance.Debug("MyListening", $"VK_ESCAPE is pressed, exiting message loop");
                    }
                    if (wParam == (IntPtr)WM_KEYDOWN && vkCode == VK_F9)
                    {

                        // 检查 Ctrl 键是否被按下
                        //if ((GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0 && (GetAsyncKeyState(VK_MENU) & 0x8000) != 0)
                        if ((GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0)
                        {
                            //Console.WriteLine("Ctrl + F1 is pressed");
                            FocusWindowAndSendKeys("Footman (DEBUG)", VK_F1);
                            //press_key_action?.Invoke(0);
                            MyLogger.Instance.Debug("MyListening", $"VK_F9      Ctrl +Alt+ F1 is pressed");
                        }
                    }
                    if (wParam == (IntPtr)WM_KEYDOWN && vkCode == VK_F10)
                    {

                        // 检查 Ctrl 键是否被按下
                        //if ((GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0 && (GetAsyncKeyState(VK_MENU) & 0x8000) != 0)
                        if ((GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0)
                        {
                            //Console.WriteLine("Ctrl + F1 is pressed");
                            FocusWindowAndSendKeys("Footman (DEBUG)", VK_F2);
                            //press_key_action?.Invoke(0);
                            MyLogger.Instance.Debug("MyListening", $"VK_F10      Ctrl +Alt+ F1 is pressed");
                        }
                    }
                    if (wParam == (IntPtr)WM_KEYDOWN && vkCode == VK_F11)
                    {

                        // 检查 Ctrl 键是否被按下
                        //if ((GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0 && (GetAsyncKeyState(VK_MENU) & 0x8000) != 0)
                        if ((GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0)
                        {
                            //Console.WriteLine("Ctrl + F1 is pressed");
                            FocusWindowAndSendKeys("Footman (DEBUG)", VK_F3);
                            //press_key_action?.Invoke(0);
                            MyLogger.Instance.Debug("MyListening", $"VK_F11      Ctrl +Alt+ F2 is pressed");
                        }
                    }
                    if (wParam == (IntPtr)WM_KEYDOWN && vkCode == VK_F12)
                    {
                        // 检查 Ctrl 键是否被按下
                        //if ((GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0 && (GetAsyncKeyState(VK_MENU) & 0x8000) != 0)
                        if ((GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0)
                        {
                            MyLogger.Instance.Debug("MyListening", $"VK_F12      Ctrl +alt + F3 is pressed");
                            //press_key_action?.Invoke(0);
                            FocusWindowAndSendKeys("Footman (DEBUG)", VK_F4);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MyLogger.Instance.Error("MyListening", $"Exception in HookCallback: {ex.Message}");
            }


            return CallNextHookEx(_hookId, nCode, wParam, lParam);


        }

        // 设置钩子
        public void SetHook()
        {
            try
            {
                //MyLogger.Instance.Debug("MyListening", $"in SetHook");
                using (System.Diagnostics.Process curProcess = System.Diagnostics.Process.GetCurrentProcess())
                using (System.Diagnostics.ProcessModule curModule = curProcess.MainModule)
                {
                    LowLevelKeyboardProc proc = HookCallback;
                    _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                    if (_hookId == IntPtr.Zero)
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        MyLogger.Instance.Error("MyListening", $"SetWindowsHookEx failed with error code: {errorCode}");
                    }
                    //MyLogger.Instance.Debug("MyListening", $"end SetHook");
                }
            }
            catch (Exception ex)
            {
                MyLogger.Instance.Error("MyListening", $"Exception in SetHook: {ex.Message}");
            }
        }

        // 卸载钩子
        public void Unhook()
        {
            try
            {
                if (_hookId != IntPtr.Zero)
                {
                    UnhookWindowsHookEx(_hookId);
                    _hookId = IntPtr.Zero;
                }
                MyLogger.Instance.Debug("MyListening", $"Unhooked successfully");
            }
            catch (Exception ex)
            {
                MyLogger.Instance.Error("MyListening", $"Exception in Unhook: {ex.Message}");
            }
        }

        // 运行消息循环
        public void RunMessageLoop()
        {
            try
            {
                SetHook();

                // 引入消息循环
                MSG msg;
                while (flag)
                {
                    try
                    {
                        if(GetMessage(out msg, IntPtr.Zero, 0, 0))
                        {
                            TranslateMessage(ref msg);
                            DispatchMessage(ref msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        MyLogger.Instance.Error("MyListening", $"Exception in message loop: {ex.Message}");
                    }
                    
                }

            }
            catch (Exception ex)
            {
                MyLogger.Instance.Error("MyListening", $"Exception in message loop: {ex.Message}");
            }
            finally
            {
                Unhook();
            }
        }

        // 获取窗口句柄并使其聚焦
        public void FocusWindowAndSendKeys(string windowTitle, int keyCode)
        {
            try
            {
                // 获取窗口句柄
                IntPtr hWnd = FindWindow(null, windowTitle);
                if (hWnd == IntPtr.Zero)
                {
                    MyLogger.Instance.Error("MyListening", $"Window with title '{windowTitle}' not found");
                    return;
                }

                //使窗口聚焦
                if (!SetForegroundWindow(hWnd))
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    MyLogger.Instance.Error("MyListening", $"SetForegroundWindow failed with error code: {errorCode}");
                    return;
                }
                Thread.Sleep(50);
                // 发送 Ctrl + Alt + F1 组合键
                keybd_event((byte)VK_CONTROL, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                // keybd_event((byte)VK_MENU, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                keybd_event((byte)keyCode, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                Thread.Sleep(50);
                keybd_event((byte)keyCode, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                //keybd_event((byte)VK_MENU, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                keybd_event((byte)VK_CONTROL, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

                //MyLogger.Instance.Debug("MyListening", $"Sent Ctrl + Alt + F1 to window '{windowTitle}'");
            }
            catch (Exception ex)
            {
                MyLogger.Instance.Error("MyListening", $"Exception in FocusWindowAndSendKeys: {ex.Message}");
            }
        }



    }
}