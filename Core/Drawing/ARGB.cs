using System.Drawing;

namespace GDPlotter
{
    public struct ARGB
    {
        byte b, g, r, a;
        public ARGB(byte red, byte green, byte blue)
        {
            a = 0xFF;
            r = red;
            g = green;
            b = blue;
        }

        public ARGB(byte alpha, byte red, byte green, byte blue)
        {
            a = alpha;
            r = red;
            g = green;
            b = blue;
        }

        public byte A
        {
            get
            {
                return a;
            }
        }
        public byte R
        {
            get
            {
                return r;
            }
        }
        public byte G
        {
            get
            {
                return g;
            }
        }
        public byte B
        {
            get
            {
                return b;
            }
        }

        public static implicit operator Color(ARGB color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static implicit operator ARGB(Color color)
        {
            return new ARGB(color.A, color.R, color.G, color.B);
        }

        public static bool operator ==(ARGB left, ARGB right)
        {
            return left.B == right.B &&
                left.G == right.G &&
                left.R == right.R &&
                left.A == right.A;
        }

        public static bool operator !=(ARGB left, ARGB right)
        {
            return left.B != right.B ||
                left.G != right.G ||
                left.R != right.R ||
                left.A != right.A;
        }

        public override bool Equals(object obj)
        {
            return (obj is ARGB c) && c == this;
        }

        public override int GetHashCode()
        {
            return A << 24 | R << 16 | G << 8 | B;
        }

        public override string ToString()
        {
            return '(' + A.ToString() + ';' + R.ToString() + ';' + G.ToString() + ';' + B.ToString() + ')';
        }
    }
}