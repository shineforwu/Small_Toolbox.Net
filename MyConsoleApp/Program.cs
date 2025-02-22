using SmallToolBox;

namespace MyConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MyLogger.Instance.Log_Acation += Console.WriteLine;
            MyLogger.Instance.SetWriteValue(0);
            MyHook myHook = new MyHook();
            myHook.press_key_action += (int i) => { Console.WriteLine($"{i}"); };
            Thread _listeningThread = new Thread(() => { myHook.RunMessageLoop(); });
            _listeningThread.Start();
            Console.WriteLine("Press esc key to exit...");
            while (myHook.flag)
            {
                Thread.Sleep(1000); // 使用非阻塞方式等待
            }
            myHook.Unhook();
            Console.WriteLine("Exiting...");
            Console.ReadKey();
        }
    }
}
