namespace ConsoleRender
{
    public struct Pixel
    {
        public char CharKey { get; set; }
        public ConsoleColor ColorKey { get; set; }
        public Pixel(char CharKey = ' ', ConsoleColor ColorKey = ConsoleColor.Gray)
        {
            this.CharKey = CharKey;
            this.ColorKey = ColorKey;
        }

    }
    public sealed class Render
    {
        static long t0;
        static long t1;
        static public int width { get; private set; }
        static public int height { get; private set; }
        static public float DeltaTime { get; private set; }
        static Render? currentRender = null;
        static public void Initialize(int Width, int Height)
        {
            Console.SetWindowSize(Width, Height);
            width = Console.WindowWidth;
            height = Console.WindowHeight;
            Console.SetBufferSize(width + 1, height + 1);
            Console.CursorVisible = false;
            t0 = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            t1 = t0;
        }
        static public long CalculateRunTime(Action action)
        {
            long tf = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            action.Invoke();
            long ts = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return ts - tf;
        }
        static public void CalculateDeltaTime()
        {
            t0 = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            DeltaTime = t0 - t1;
            DeltaTime /= 1000f;
            t1 = t0;
        }
        static public Render CreateRender(int Width, int Height)
        {
            if (currentRender == null)
            {
                Initialize(Width, Height);
                currentRender = new Render();
            }
            return currentRender;
        }
        static public Render GetRender()
        {
            if (currentRender == null) throw new Exception("Use CreateRender method to create it");
            return currentRender;
        }
        private Render()
        {
            currentCharScene = new char[width, height];
            pastCharScene = new char[width, height];
            currentColorScene = new ConsoleColor[width, height];
            pastColorScene = new ConsoleColor[width, height];
        }
        char[,] currentCharScene;
        char[,] pastCharScene;
        ConsoleColor[,] currentColorScene;
        ConsoleColor[,] pastColorScene;
        public void SetSymbol(int X, int Y, char Symbol, ConsoleColor Color = ConsoleColor.Gray)
        {
            if (!ValidCord(X, Y)) return;
            currentCharScene[X, Y] = Symbol;
            currentColorScene[X, Y] = Color;
        }
        public void SetSymbol(int X, int Y, Pixel pixel)
        {
            if (!ValidCord(X, Y)) return;
            currentCharScene[X, Y] = pixel.CharKey;
            currentColorScene[X, Y] = pixel.ColorKey;
        }
        bool ValidCord(int X, int Y)
        {
            if (X < 0 || X >= width || Y < 0 || Y >= height) return false;
            return true;
        }
        public void DumbRender()
        {
            Walk((X, Y) =>
            {
                Console.SetCursorPosition(X, Y);
                Console.Write(currentCharScene[X, Y]);
                currentCharScene[X, Y] = ' ';
            });
        }
        public void DumbColorRender()
        {
            Walk((X, Y) =>
            {
                Console.SetCursorPosition(X, Y);
                Console.ForegroundColor = currentColorScene[X, Y];
                Console.Write(currentCharScene[X, Y]);
                currentCharScene[X, Y] = ' ';
                currentColorScene[X, Y] = ConsoleColor.Gray;
            });
        }
        public void SmartRender()
        {
            Walk((X, Y) =>
            {
                if (currentCharScene[X, Y] != pastCharScene[X, Y])
                {
                    Console.SetCursorPosition(X, Y);
                    Console.Write(currentCharScene[X, Y]);
                }
                pastCharScene[X, Y] = currentCharScene[X, Y];
                currentCharScene[X, Y] = ' ';
            });
        }
        public void SmartColorRender()
        {
            Walk((X, Y) =>
            {
                if (currentCharScene[X, Y] != pastCharScene[X, Y] || currentColorScene[X, Y] != pastColorScene[X, Y])
                {
                    Console.SetCursorPosition(X, Y);
                    Console.ForegroundColor = currentColorScene[X, Y];
                    Console.Write(currentCharScene[X, Y]);
                }
                pastCharScene[X, Y] = currentCharScene[X, Y];
                pastColorScene[X, Y] = currentColorScene[X, Y];
                currentCharScene[X, Y] = ' ';
                currentColorScene[X, Y] = ConsoleColor.Gray;
            });
        }
        void Walk(Action<int, int> action)
        {
            for(int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    action.Invoke(x, y);
                }
            }
        }
    }
}