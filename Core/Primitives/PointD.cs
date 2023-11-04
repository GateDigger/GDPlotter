namespace GDPlotter
{
    public struct PointD
    {
        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X
        {
            get;
            private set;
        }

        public double Y
        {
            get;
            private set;
        }

        public static PointD Zero
        {
            get
            {
                return new PointD(0.0, 0.0);
            }
        }

        public static PointD NaP
        {
            get
            {
                return new PointD(double.NaN, double.NaN);
            }
        }

        public static implicit operator PointD(System.Drawing.Point p)
        {
            return new PointD(p.X, p.Y);
        }

        public static PointD operator +(PointD left, PointD right)
        {
            return new PointD(left.X + right.X, left.Y + right.Y);
        }

        public static PointD operator -(PointD p)
        {
            return new PointD(-p.X, -p.Y);
        }

        public static PointD operator -(PointD left, PointD right)
        {
            return new PointD(left.X - right.X, left.Y - right.Y);
        }

        public static bool IsNaP(PointD point)
        {
            return double.IsNaN(point.X) || double.IsNaN(point.Y);
        }

        public override string ToString()
        {
            return "Point(" + X.ToString("0.00") + "; " + Y.ToString("0.00") + ')';
        }
    }
}