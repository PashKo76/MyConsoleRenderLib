namespace ConsoleRender
{
    public struct Pixel
    {
        public char CharKey { get; set; }
        public ConsoleColor ColorKey { get; set; }
        public Pixel()
        {
            CharKey = ' ';
            ColorKey = ConsoleColor.Gray;
        }
        public Pixel(char CharKey)
        {
            this.CharKey = CharKey;
            ColorKey = ConsoleColor.Gray;
        }
        public Pixel(char CharKey, ConsoleColor ColorKey)
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
        char[,] currentCharScene = new char[width, height];
        char[,] pastCharScene = new char[width, height];
        ConsoleColor[,] currentColorScene = new ConsoleColor[width, height];
        ConsoleColor[,] pastColorScene = new ConsoleColor[width, height];
        public bool SetSymbol(int X, int Y, char Symbol)
        {
            if (!ValidCord(X, Y)) return false;
            currentCharScene[X, Y] = Symbol;
            return true;
        }
        public bool SetColorSymbol(int X, int Y, char Symbol, ConsoleColor Color)
        {
            if (!ValidCord(X, Y)) return false;
            currentCharScene[X, Y] = Symbol;
            currentColorScene[X, Y] = Color;
            return true;
        }
        public bool SetPixel(int X, int Y, Pixel pixel)
        {
            if (!ValidCord(X, Y)) return false;
            currentCharScene[X, Y] = pixel.CharKey;
            currentColorScene[X, Y] = pixel.ColorKey;
            return true;
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
        delegate void Walker(int X, int Y);
        void Walk(Walker walker)
        {
            for(int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    walker.Invoke(x, y);
                }
            }
        }
    }
}