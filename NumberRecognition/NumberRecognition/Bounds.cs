namespace NumberRecognition
{
    public class Bounds
    {
        public bool WasModified { get; set; }

        public Bounds()
        {
            Bottom = int.MinValue;
            Left = int.MaxValue;
            Top = int.MaxValue;
            Right = int.MinValue;
        }

        public int Left { get; set; }

        public int Right { get; set; }

        public int Top { get; set; }

        public int Bottom { get; set; }

    }
}
