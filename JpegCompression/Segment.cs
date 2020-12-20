namespace JpegCompression
{
    public class Segment
    {
        public double Left { get; set; }
        public double Right { get; set; }

        public Segment(double left, double right)
        {
            this.Left = left;
            this.Right = right;
        }

        public override string ToString()
        {
            return $"{this.Left} {this.Right}";
        }
    }
}
