namespace GDPlotter
{
    public struct IntervalD
    {
        public IntervalD(double floor, double ceiling)
        {
            Floor = floor;
            Ceiling = ceiling;
        }

        public double Floor
        {
            get;
            private set;
        }

        public double Ceiling
        {
            get;
            private set;
        }

        public static bool IsNaI(IntervalD interval)
        {
            return double.IsNaN(interval.Floor) || double.IsNaN(interval.Ceiling);
        }

        public override string ToString()
        {
            return "Interval[" + Floor.ToString("0.00") + "; " + Ceiling.ToString("0.00") + ']';
        }
    }
}