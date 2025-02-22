/**
 * @Author: Shine Wu
 * @Date:   2025-02-22 16:13:20
 * @Last Modified by:   Shine Wu
 * @Last Modified time: 2025-02-22 20:12:12
 */
using SmallToolBox;
using System;
using System.Threading;

namespace MyConsoleApp
{
    internal class Program
    {
        private static Thread _listeningThread;
        private static MyHook myHook;

        static void Main(string[] args)
        {
            // 注册进程退出事件
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            // 注册控制台取消按键事件（例如 Ctrl+C 或关闭按钮）
            Console.CancelKeyPress += Console_CancelKeyPress;

            MyLogger.Instance.Log_Acation += Console.WriteLine;
            MyLogger.Instance.SetWriteValue(0);
            myHook = new MyHook();
            myHook.press_key_action += (int i) => { Console.WriteLine($"{i}"); };
            _listeningThread = new Thread(() => { myHook.RunMessageLoop2("FootMan"); });
            _listeningThread.Start();
            Console.WriteLine("Press esc key to exit...");
            while (myHook.flag)
            {
                Thread.Sleep(1000); // 使用非阻塞方式等待
            }
            myHook.Unhook();
            Console.WriteLine("Exiting...");

            // 等待 _listeningThread 结束
            _listeningThread.Join();

            Console.ReadKey();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Cleanup();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true; // 取消默认行为，以便执行清理操作
            Cleanup();
        }

        private static void Cleanup()
        {
            if (myHook != null)
            {
                myHook.flag = false;
                myHook.Unhook();
            }

            if (_listeningThread != null && _listeningThread.IsAlive)
            {
                _listeningThread.Join();
            }
        }
    }
}