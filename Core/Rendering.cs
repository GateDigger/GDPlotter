//#define LIMIT_ITERATIONS

using System;
using System.Threading.Tasks;

using static GDPlotter.Utility;

namespace GDPlotter
{
    public delegate void Dotter(double x, double y);

    public delegate void Liner(double x1, double y1, double x2, double y2);

    public delegate void DoubleWriter(double value, double x1, double y1, double width, double height);

    public static class Rendering
    {
        #region FX

#if LIMIT_ITERATIONS
        const int MAX_PT_RENDER_ITERATIONS = 1 << 18;
#endif

        public static void RenderFx(Func<double, double> f, Dotter dotter, RectangleD controlAreaRect, RectangleD plotAreaRect, int ptCount)
        {
#if LIMIT_ITERATIONS
            ptCount = Min(ptCount, MAX_PT_RENDER_ITERATIONS);
#endif
            double x,
                y,
                dx = plotAreaRect.SignedWidth / ptCount,
                scaleX = controlAreaRect.SignedWidth / plotAreaRect.SignedWidth,
                scaleY = controlAreaRect.SignedHeight / plotAreaRect.SignedHeight;

            if (plotAreaRect.Y1 < plotAreaRect.Y2)
            {
                for (x = plotAreaRect.X1; ptCount > -1; x += dx, ptCount--)
                    if (plotAreaRect.Y1 < (y = f(x)) && y < plotAreaRect.Y2)
                        dotter((controlAreaRect.X1 + scaleX * (x - plotAreaRect.X1)), (controlAreaRect.Y1 + scaleY * (plotAreaRect.Y2 - y)));
            }
            else
            {
                for (x = plotAreaRect.X1; ptCount > -1; x += dx, ptCount--)
                    if (plotAreaRect.Y2 < (y = f(x)) && y < plotAreaRect.Y1)
                        dotter((controlAreaRect.X1 + scaleX * (x - plotAreaRect.X1)), (controlAreaRect.Y1 + scaleY * (plotAreaRect.Y2 - y)));
            }
        }

        public static void RenderFx_Parallel(Func<double, double> f, Dotter dotter, RectangleD controlAreaRect, RectangleD plotAreaRect, int ptCount)
        {
#if LIMIT_ITERATIONS
            ptCount = Min(ptCount, MAX_PT_RENDER_ITERATIONS);
#endif
            double dx = plotAreaRect.SignedWidth / ptCount,
                scaleX = controlAreaRect.SignedWidth / plotAreaRect.SignedWidth,
                scaleY = controlAreaRect.SignedHeight / plotAreaRect.SignedHeight;

            if (plotAreaRect.Y1 < plotAreaRect.Y2)
            {
                Parallel.For(0, ptCount, step =>
                {
                    double x,
                        y;
                    if (plotAreaRect.Y1 < (y = f(x = plotAreaRect.X1 + step * dx)) && y < plotAreaRect.Y2)
                        dotter((controlAreaRect.X1 + scaleX * (x - plotAreaRect.X1)), (controlAreaRect.Y1 + scaleY * (plotAreaRect.Y2 - y)));
                });
            }
            else
            {
                Parallel.For(0, ptCount, step =>
                {
                    double x,
                        y;
                    if (plotAreaRect.Y2 < (y = f(x = plotAreaRect.X1 + step * dx)) && y < plotAreaRect.Y1)
                        dotter((controlAreaRect.X1 + scaleX * (x - plotAreaRect.X1)), (controlAreaRect.Y1 + scaleY * (plotAreaRect.Y2 - y)));
                });
            }
        }

#endregion

        #region AXES

        public static void RenderAxes(Liner liner, RectangleD controlAreaRect, RectangleD plotAreaRect)
        {
            double chi0 = controlAreaRect.X1 - plotAreaRect.X1 * controlAreaRect.SignedWidth / plotAreaRect.SignedWidth,
                gamma0 = controlAreaRect.Y1 + plotAreaRect.Y2 * controlAreaRect.SignedHeight / plotAreaRect.SignedHeight;

            if (chi0.IsBetween(controlAreaRect.X1, controlAreaRect.X2))
                liner((chi0), (controlAreaRect.Y1), (chi0), (controlAreaRect.Y2));

            if (gamma0.IsBetween(controlAreaRect.Y1, controlAreaRect.Y2))
                liner((controlAreaRect.X1), (gamma0), (controlAreaRect.X2), (gamma0));
        }

        #endregion

        #region GRID

        public static void RenderGrid(Liner liner, double dx, double dy, RectangleD controlAreaRect, RectangleD plotAreaRect)
        {
            dx = Abs(dx);
            dy = Abs(dy);
            double scaleX = controlAreaRect.SignedWidth / plotAreaRect.SignedWidth,
                scaleY = controlAreaRect.SignedHeight / plotAreaRect.SignedHeight,
                x0 = plotAreaRect.X1 - (plotAreaRect.X1 % dx),
                y0 = plotAreaRect.Y1 - (plotAreaRect.Y1 % dy),
                current,
                gdx = plotAreaRect.X1 * plotAreaRect.SignedWidth > 0.0 ?
                        dx :
                        0.0,
                gdy = plotAreaRect.Y1 * plotAreaRect.SignedHeight > 0.0 ?
                        dy :
                        0.0;

            int steps;

            if (plotAreaRect.X1 < plotAreaRect.X2)
            {
                steps = (int)((plotAreaRect.X2 - x0 - gdx) / dx);
                dx *= scaleX;
                for (current = (controlAreaRect.X1 + scaleX * (x0 + gdx - plotAreaRect.X1)); steps > -1; current += dx, steps--)
                    liner((current), (controlAreaRect.Y1), (current), (controlAreaRect.Y2));
            }
            else
            {
                steps = (int)((x0 - gdx - plotAreaRect.X2) / dx);
                dx *= scaleX;
                for (current = (controlAreaRect.X1 + scaleX * (x0 - gdx - plotAreaRect.X1)); steps > -1; current -= dx, steps--)
                    liner((current), (controlAreaRect.Y1), (current), (controlAreaRect.Y2));
            }

            if (plotAreaRect.Y1 < plotAreaRect.Y2)
            {
                steps = (int)((plotAreaRect.Y2 - y0 - gdy) / dy);
                dy *= scaleY;
                for (current = (controlAreaRect.Y1 + scaleY * (plotAreaRect.Y2 - y0 - gdy)); steps > -1; current -= dy, steps--)
                    liner((controlAreaRect.X1), (current), (controlAreaRect.X2), (current));

            }
            else
            {
                steps = (int)((y0 - gdy - plotAreaRect.Y2) / dy);
                dy *= scaleY;
                for (current = (controlAreaRect.Y1 + scaleY * (plotAreaRect.Y2 - y0 + gdy)); steps > -1; current += dy, steps--)
                    liner((controlAreaRect.X1), (current), (controlAreaRect.X2), (current));
            }
        }

        #endregion

        #region RECT

        public static void RenderRect(Liner liner, RectangleD rect, RectangleD controlAreaRect, RectangleD plotAreaRect)
        {
            double scaleX = controlAreaRect.SignedWidth / plotAreaRect.SignedWidth,
                scaleY = controlAreaRect.SignedHeight / plotAreaRect.SignedHeight,
                offsetX = controlAreaRect.X1 - plotAreaRect.X1 * scaleX,
                offsetY = controlAreaRect.Y1 + plotAreaRect.Y2 * scaleY;


            double x1 = (offsetX + rect.X1 * scaleX),
                x1c = x1.Clamp(controlAreaRect.X1, controlAreaRect.X2),
                x2 = offsetX + rect.X2 * scaleX,
                x2c = x2.Clamp(controlAreaRect.X1, controlAreaRect.X2),
                y1 = offsetY - rect.Y1 * scaleY,
                y1c = y1.Clamp(controlAreaRect.Y1, controlAreaRect.Y2),
                y2 = offsetY - rect.Y2 * scaleY,
                y2c = y2.Clamp(controlAreaRect.Y1, controlAreaRect.Y2);


            if (y1.IsBetween(controlAreaRect.Y1, controlAreaRect.Y2))
                liner((x1c), (y1), (x2c), (y1));

            if (x2.IsBetween(controlAreaRect.X1, controlAreaRect.X2))
                liner((x2), (y1c), (x2), (y2c));

            if (y2.IsBetween(controlAreaRect.Y1, controlAreaRect.Y2))
                liner((x2c), (y2), (x1c), (y2));

            if (x1.IsBetween(controlAreaRect.X1, controlAreaRect.X2))
                liner((x1), (y2c), (x1), (y1c));
        }

        public static void RenderRect(Liner liner, RectangleD rect, RectangleD controlAreaRect)
        {
            RectangleD rectC = RectangleD.Clamp(rect, controlAreaRect);

            if (rect.Y1.IsBetween(controlAreaRect.Y1, controlAreaRect.Y2))
                liner((rectC.X1), (rect.Y1), (rect.X2), (rect.Y1));

            if (rect.X2.IsBetween(controlAreaRect.X1, controlAreaRect.X2))
                liner((rect.X2), (rectC.Y1), (rect.X2), (rect.Y2));

            if (rect.Y2.IsBetween(controlAreaRect.Y1, controlAreaRect.Y2))
                liner((rect.X2), (rect.Y2), (rectC.X1), (rect.Y2));

            if (rect.X1.IsBetween(controlAreaRect.X1, controlAreaRect.X2))
                liner((rect.X1), (rectC.Y2), (rect.X1), (rectC.Y1));
        }

        public static void RenderRect(Liner liner, RectangleD rect)
        {
            liner((rect.X1), (rect.Y1), (rect.X2), (rect.Y1));
            liner((rect.X2), (rect.Y1), (rect.X2), (rect.Y2));
            liner((rect.X2), (rect.Y2), (rect.X1), (rect.Y2));
            liner((rect.X1), (rect.Y2), (rect.X1), (rect.Y1));
        }

        #endregion

        #region TEXT

        public static void RenderCoordText_Int(DoubleWriter horizontalWriter, DoubleWriter verticalWriter, double textboxHeight, RectangleD rectangle, RectangleD controlAreaRect)
        {
            // :)
            if (controlAreaRect.X1 < controlAreaRect.X2)
            {
                if (controlAreaRect.Y1 < controlAreaRect.Y2)
                {
                    horizontalWriter((rectangle.Y1), controlAreaRect.X1, controlAreaRect.Y2 - textboxHeight, controlAreaRect.X2 - controlAreaRect.X1, textboxHeight);
                    horizontalWriter((rectangle.Y2), controlAreaRect.X1, controlAreaRect.Y1, controlAreaRect.X2 - controlAreaRect.X1, textboxHeight);

                    verticalWriter((rectangle.X1), controlAreaRect.X1, controlAreaRect.Y1, textboxHeight, controlAreaRect.Y2 - controlAreaRect.Y1);
                    verticalWriter((rectangle.X2), controlAreaRect.X2 - textboxHeight, controlAreaRect.Y1, textboxHeight, controlAreaRect.Y2 - controlAreaRect.Y1);
                }
                else
                {
                    horizontalWriter((rectangle.Y1), controlAreaRect.X1, controlAreaRect.Y2, controlAreaRect.X2 - controlAreaRect.X1, textboxHeight);
                    horizontalWriter((rectangle.Y2), controlAreaRect.X1, controlAreaRect.Y1 - textboxHeight, controlAreaRect.X2 - controlAreaRect.X1, textboxHeight);

                    verticalWriter((rectangle.X1), controlAreaRect.X1, controlAreaRect.Y2, textboxHeight, controlAreaRect.Y1 - controlAreaRect.Y2);
                    verticalWriter((rectangle.X2), controlAreaRect.X2 - textboxHeight, controlAreaRect.Y2, textboxHeight, controlAreaRect.Y1 - controlAreaRect.Y2);
                }
            }
            else
            {
                if (controlAreaRect.Y1 < controlAreaRect.Y2)
                {
                    horizontalWriter((rectangle.Y1), controlAreaRect.X2, controlAreaRect.Y2 - textboxHeight, controlAreaRect.X1 - controlAreaRect.X2, textboxHeight);
                    horizontalWriter((rectangle.Y2), controlAreaRect.X2, controlAreaRect.Y1, controlAreaRect.X1 - controlAreaRect.X2, textboxHeight);

                    verticalWriter((rectangle.X1), controlAreaRect.X1 - textboxHeight, controlAreaRect.Y1, textboxHeight, controlAreaRect.Y2 - controlAreaRect.Y1);
                    verticalWriter((rectangle.X2), controlAreaRect.X2, controlAreaRect.Y1, textboxHeight, controlAreaRect.Y2 - controlAreaRect.Y1);
                }
                else
                {
                    horizontalWriter((rectangle.Y1), controlAreaRect.X2, controlAreaRect.Y2, controlAreaRect.X1 - controlAreaRect.X2, textboxHeight);
                    horizontalWriter((rectangle.Y2), controlAreaRect.X2, controlAreaRect.Y1 - textboxHeight, controlAreaRect.X1 - controlAreaRect.X2, textboxHeight);

                    verticalWriter((rectangle.X1), controlAreaRect.X1 - textboxHeight, controlAreaRect.Y2, textboxHeight, controlAreaRect.Y1 - controlAreaRect.Y2);
                    verticalWriter((rectangle.X2), controlAreaRect.X2, controlAreaRect.Y2, textboxHeight, controlAreaRect.Y1 - controlAreaRect.Y2);
                }
            }
        }

        public static void RenderCoordText_Ext(DoubleWriter horizontalWriter, DoubleWriter verticalWriter, double textboxHeight, RectangleD rectangle, RectangleD controlAreaRect)
        {
            // :)
            if (controlAreaRect.X1 < controlAreaRect.X2)
            {
                if (controlAreaRect.Y1 < controlAreaRect.Y2)
                {
                    horizontalWriter((rectangle.Y1), controlAreaRect.X1, controlAreaRect.Y2, controlAreaRect.X2 - controlAreaRect.X1, textboxHeight);
                    horizontalWriter((rectangle.Y2), controlAreaRect.X1, controlAreaRect.Y1 - textboxHeight, controlAreaRect.X2 - controlAreaRect.X1, textboxHeight);

                    verticalWriter((rectangle.X1), controlAreaRect.X1 - textboxHeight, controlAreaRect.Y1, textboxHeight, controlAreaRect.Y2 - controlAreaRect.Y1);
                    verticalWriter((rectangle.X2), controlAreaRect.X2, controlAreaRect.Y1, textboxHeight, controlAreaRect.Y2 - controlAreaRect.Y1);
                }
                else
                {
                    horizontalWriter((rectangle.Y1), controlAreaRect.X1, controlAreaRect.Y2 - textboxHeight, controlAreaRect.X2 - controlAreaRect.X1, textboxHeight);
                    horizontalWriter((rectangle.Y2), controlAreaRect.X1, controlAreaRect.Y1, controlAreaRect.X2 - controlAreaRect.X1, textboxHeight);

                    verticalWriter((rectangle.X1), controlAreaRect.X1 - textboxHeight, controlAreaRect.Y2, textboxHeight, controlAreaRect.Y1 - controlAreaRect.Y2);
                    verticalWriter((rectangle.X2), controlAreaRect.X2, controlAreaRect.Y2, textboxHeight, controlAreaRect.Y1 - controlAreaRect.Y2);
                }
            }
            else
            {
                if (controlAreaRect.Y1 < controlAreaRect.Y2)
                {
                    horizontalWriter((rectangle.Y1), controlAreaRect.X2, controlAreaRect.Y2, controlAreaRect.X1 - controlAreaRect.X2, textboxHeight);
                    horizontalWriter((rectangle.Y2), controlAreaRect.X2, controlAreaRect.Y1 - textboxHeight, controlAreaRect.X1 - controlAreaRect.X2, textboxHeight);

                    verticalWriter((rectangle.X1), controlAreaRect.X1, controlAreaRect.Y1, textboxHeight, controlAreaRect.Y2 - controlAreaRect.Y1);
                    verticalWriter((rectangle.X2), controlAreaRect.X2 - textboxHeight, controlAreaRect.Y1, textboxHeight, controlAreaRect.Y2 - controlAreaRect.Y1);
                }
                else
                {
                    horizontalWriter((rectangle.Y1), controlAreaRect.X2, controlAreaRect.Y2 - textboxHeight, controlAreaRect.X1 - controlAreaRect.X2, textboxHeight);
                    horizontalWriter((rectangle.Y2), controlAreaRect.X2, controlAreaRect.Y1, controlAreaRect.X1 - controlAreaRect.X2, textboxHeight);

                    verticalWriter((rectangle.X1), controlAreaRect.X1, controlAreaRect.Y2, textboxHeight, controlAreaRect.Y1 - controlAreaRect.Y2);
                    verticalWriter((rectangle.X2), controlAreaRect.X2 - textboxHeight, controlAreaRect.Y2, textboxHeight, controlAreaRect.Y1 - controlAreaRect.Y2);
                }
            }
        }

        #endregion
    }
}