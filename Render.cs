namespace ConsoleRender
{
    public sealed class Render
    {
        static long t0;
        static long t1;
        static public Render CurrentRender { get; set; }
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
        char[,] currentScene = new char[width, height];
        char[,] pastScene = new char[width, height];
        public bool SetSymbol(int X, int Y, char Symbol)
        {
            if (!ValidCord(X, Y)) return false;
            currentScene[X, Y] = Symbol;
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
                Console.Write(currentScene[X, Y]);
            });
        }
        public void SmartRender()
        {
            Walk((X, Y) =>
            {
                if (currentScene[X, Y] != pastScene[X, Y])
                {
                    Console.SetCursorPosition(X, Y);
                    Console.Write(currentScene[X, Y]);
                }
                pastScene[X, Y] = currentScene[X, Y];
                currentScene[X, Y] = ' ';
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