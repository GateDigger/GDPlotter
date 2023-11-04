namespace GDPlotter
{
    public static class Utility
    {
        //TODO
        public static int ToInt(double value)
        {
            return (int)(value + 0.5);
        }

        public static double Clamp(this double x, double left, double right)
        {
            return left < right ?
                        x < left ?
                            left :
                            x > right ?
                                right :
                                x :
                        x < right ?
                            right :
                            x > left ?
                                left :
                                x;
        }

        public static bool IsBetween(this double x, double left, double right)
        {
            return left < right ?
                    left < x && x < right :
                    right < x && x < left;
        }

        public static double Abs(double x)
        {
            return x < 0.0 ? -x : x;
        }

        public static int Min(int left, int right)
        {
            return left < right ? left : right;
        }
    }
}