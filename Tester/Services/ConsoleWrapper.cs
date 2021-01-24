using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tester.Services
{
    //https://www.michalbialecki.com/2018/05/25/how-to-make-you-console-app-look-cool/
    public interface IConsoleWrapper
    {
        void Write(string text, ConsoleColor color = ConsoleColor.Yellow);
        string Read();
        string PromptAndRead(string prompt, ConsoleColor color = ConsoleColor.Yellow);
        Task<string> ReadAndWrite2(string prompt, ConsoleColor color = ConsoleColor.Yellow);
    }

    public class WrappedConsole : IConsoleWrapper
    {
        int writeLastPosLeft = 0;
        int writeLastPosTop = 0;
        int readLastPosLeft = 0;
        int readLastPosTop = 0;
        bool isReading = false;
        object ForLocking = new object();
        public string Read()
        {
            isReading = true;
            var ret = Console.ReadLine();
            isReading = false;
            return ret;
        }

        public string PromptAndRead(string prompt, ConsoleColor color = ConsoleColor.Yellow)
        {
            Console.Clear();
            Console.SetCursorPosition(1, 1);
            Console.Write(prompt);
            lock (ForLocking)
            {
                readLastPosLeft = Console.CursorLeft;
                readLastPosTop = Console.CursorTop;
            }
            //Write(prompt, color);
            return Read();
        }

        public void Write(string text, ConsoleColor color = ConsoleColor.Yellow)
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition(writeLastPosLeft, writeLastPosTop);
            Console.WriteLine(text);
            lock (ForLocking)
            {
                writeLastPosLeft = Console.CursorLeft;
                writeLastPosTop = Console.CursorTop;
            }
            if (writeLastPosTop >= Console.WindowHeight)
                writeLastPosTop = 2;
            if (isReading)
            {
                Console.SetCursorPosition(readLastPosLeft, readLastPosTop);
                Console.CursorVisible = true;
            }
        }

        public void Animate()
        {
            Console.CursorLeft = 20;
            Console.CursorTop = 20;
            var counter = 0;
            for (int i = 0; i < 50; i++)
            {
                switch (counter % 4)
                {
                    case 0: Console.Write("/"); break;
                    case 1: Console.Write("-"); break;
                    case 2: Console.Write("\\"); break;
                    case 3: Console.Write("|"); break;
                }
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                counter++;
                Thread.Sleep(100);
            }
        }

        public Task<string> ReadAndWrite2(string prompt, ConsoleColor color = ConsoleColor.Yellow)
        {
            var d = "";
            Task.Run(() => { d = PromptAndRead(prompt); });
            return Task.FromResult(d);
        }
    }
}
