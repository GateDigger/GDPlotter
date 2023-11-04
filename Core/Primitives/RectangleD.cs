using System.Drawing;
using static GDPlotter.Utility;

namespace GDPlotter
{
    public struct RectangleD
    {
        public static RectangleD NaR
        {
            get
            {
                return new RectangleD(double.NaN, double.NaN, double.NaN, double.NaN);
            }
        }

        public static RectangleD Unit
        {
            get
            {
                return new RectangleD(0.0, 0.0, 1.0, 1.0);
            }
        }

        #region CTOR

        public RectangleD(double x1, double y1, double x2, double y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public RectangleD(PointD p1, PointD p2)
        {
            X1 = p1.X;
            Y1 = p1.Y;
            X2 = p2.X;
            Y2 = p2.Y;
        }

        #endregion

        #region PROPS

        public double X1
        {
            get;
            private set;
        }

        public double Y1
        {
            get;
            private set;
        }

        public double X2
        {
            get;
            private set;
        }

        public double Y2
        {
            get;
            private set;
        }

        public double SignedWidth
        {
            get
            {
                return X2 - X1;
            }
        }

        public double SignedHeight
        {
            get
            {
                return Y2 - Y1;
            }
        }

        #endregion


        #region COORDINATE_CONVERSIONS

        public static PointD ConvertPoint(PointD p, RectangleD origin, RectangleD target)
        {
            return new PointD(
                Chi(origin.X1, origin.X2, target.X1, target.X2, p.X),
                Gamma(origin.Y1, origin.Y2, target.Y1, target.Y2, p.Y)
                );
        }
        static double Chi(double ox1, double ox2, double tx1, double tx2, double x)
        {
            return tx1 + (x - ox1) * (tx2 - tx1) / (ox2 - ox1);
        }
        static double Gamma(double oy1, double oy2, double ty1, double ty2, double y)
        {
            return ty1 + (oy2 - y) * (ty2 - ty1) / (oy2 - oy1);
        }

        #endregion

        #region ZOOM

        public static RectangleD Zoom(RectangleD rectangle, PointD center, double scale)
        {
            return PointD.IsNaP(center) || double.IsNaN(scale) ? 
                        rectangle :
                        new RectangleD(
                            center.X + scale * (rectangle.X1 - center.X),
                            center.Y + scale * (rectangle.Y1 - center.Y),
                            center.X + scale * (rectangle.X2 - center.X),
                            center.Y + scale * (rectangle.Y2 - center.Y)
                            );
        }

        public static RectangleD ScaleX(RectangleD rectangle, double centerX, double scaleX)
        {
            return double.IsNaN(centerX) || double.IsNaN(scaleX) ?
                        rectangle :
                        new RectangleD(
                            centerX + scaleX * (rectangle.X1 - centerX),
                            rectangle.Y1,
                            centerX + scaleX * (rectangle.X2 - centerX),
                            rectangle.Y2
                            );
        }

        public static RectangleD ScaleY(RectangleD rectangle, double centerY, double scaleY)
        {
            return double.IsNaN(centerY) || double.IsNaN(scaleY) ?
                        rectangle :
                        new RectangleD(
                            rectangle.X1,
                            centerY + scaleY * (rectangle.Y1 - centerY),
                            rectangle.X2,
                            centerY + scaleY * (rectangle.Y2 - centerY)
                            );
        }

        public static RectangleD ScaleXY(RectangleD rectangle, double centerX, double scaleX, double centerY, double scaleY)
        {
            return double.IsNaN(centerX) || double.IsNaN(scaleX) ?
                        double.IsNaN(centerY) || double.IsNaN(scaleY) ?
                            rectangle :
                            new RectangleD(
                                rectangle.X1,
                                centerY + scaleY * (rectangle.Y1 - centerY),
                                rectangle.X2,
                                centerY + scaleY * (rectangle.Y2 - centerY)
                                ) :
                        double.IsNaN(centerY) || double.IsNaN(scaleY) ?
                            new RectangleD(
                                centerX + scaleX * (rectangle.X1 - centerX),
                                rectangle.Y1,
                                centerX + scaleX * (rectangle.X2 - centerX),
                                rectangle.Y2
                                ) :
                            new RectangleD(
                                centerX + scaleX * (rectangle.X1 - centerX),
                                centerY + scaleY * (rectangle.Y1 - centerY),
                                centerX + scaleX * (rectangle.X2 - centerX),
                                centerY + scaleY * (rectangle.Y2 - centerY)
                                );
        }

        public static bool CheckScale(RectangleD rectangle, double low, double high)
        {
            return Abs(rectangle.SignedWidth).IsBetween(low, high) &&
                Abs(rectangle.SignedHeight).IsBetween(low, high);
        }

        #endregion

        public static RectangleD Shift(RectangleD rectangle, PointD offset)
        {
            return new RectangleD(rectangle.X1 + offset.X, rectangle.Y1 + offset.Y, rectangle.X2 + offset.X, rectangle.Y2 + offset.Y);
        }

        public static RectangleD Pad(RectangleD rectangle, double x1, double y1, double x2, double y2)
        {
            double xScale = rectangle.X1 < rectangle.X2 ? 1.0 : -1.0,
                yScale = rectangle.Y1 < rectangle.Y2 ? 1.0 : -1.0;

            return new RectangleD(rectangle.X1 + xScale * x1, rectangle.Y1 + yScale * y1, rectangle.X2 - xScale * x2, rectangle.Y2 - yScale * y2);
        }

        public static RectangleD MakeRectangle(PointD pt1, PointD pt2, bool xOrientation, bool yOrientation)
        {
            return xOrientation == pt1.X < pt2.X ?
                        yOrientation == pt1.Y < pt2.Y ?
                            new RectangleD(pt1.X, pt1.Y, pt2.X, pt2.Y) :
                            new RectangleD(pt1.X, pt2.Y, pt2.X, pt1.Y) :
                        yOrientation == pt1.Y < pt2.Y ?
                            new RectangleD(pt2.X, pt1.Y, pt1.X, pt2.Y) :
                            new RectangleD(pt2.X, pt2.Y, pt1.X, pt1.Y);
        }

        public static RectangleD Clamp(RectangleD child, RectangleD parent)
        {
            return new RectangleD(
                child.X1.Clamp(parent.X1, parent.X2),
                child.Y1.Clamp(parent.Y1, parent.Y2),
                child.X2.Clamp(parent.X1, parent.X2),
                child.Y2.Clamp(parent.Y1, parent.Y2)
                );
        }

        public static implicit operator RectangleD(Rectangle r)
        {
            return new RectangleD(r.X, r.Y, r.X + r.Width, r.Y + r.Height);
        }

        public static bool IsNaR(RectangleD rectangle)
        {
            return double.IsNaN(rectangle.X1) ||
                double.IsNaN(rectangle.X2) ||
                double.IsNaN(rectangle.Y1) ||
                double.IsNaN(rectangle.Y2);
        }

        public override string ToString()
        {
            return "Rectangle{x1 = " + X1.ToString("0.00") + "; y1 = " + Y1.ToString("0.00") + "; x2 = " + X2.ToString("0.00") + "; y2 = " + Y2.ToString("0.00") + "}";
        }
    }
}