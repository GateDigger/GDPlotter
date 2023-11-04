//#define BOUNDS_CHECK
#define CLAMP

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace GDPlotter
{
    public unsafe class Pixelmap32
    {
        Bitmap _bitmap;

        BitmapData _lockData;
        ARGB* _pixel0;
        int _stride;

        public Pixelmap32(int width, int height)
        {
            _bitmap = new Bitmap(
                Width = width,
                Height = height,
                PixelFormat.Format32bppArgb
                );
        }

        public Pixelmap32(bool wrap, Bitmap bitmap)
        {
            if (wrap)
            {
                if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                    throw new BadImageFormatException();

                _bitmap = bitmap;
                Width = bitmap.Width;
                Height = bitmap.Height;
            }
            else
            {
                _bitmap = new Bitmap(
                    Width = bitmap.Width,
                    Height = bitmap.Height,
                    PixelFormat.Format32bppArgb
                    );

                using (Graphics g = Graphics.FromImage(_bitmap))
                    g.DrawImage(bitmap, 0, 0);
            }
        }

        public int Width
        {
            get;
            private set;
        }
        public int Height
        {
            get;
            private set;
        }

        /// <summary>
        /// Locks the bitmap for random access to individual pixels
        /// </summary>
        public void Lock()
        {
            _lockData = _bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            _pixel0 = (ARGB*)_lockData.Scan0;
            _stride = _lockData.Stride / sizeof(ARGB);
        }

#if BOUNDS_CHECK

        public ARGB this[int x, int y]
        {
            get
            {
                if (!BoundsCheck(x, Width, y, Height))
                    return ARGB.Transparent;
                return *(_pixel0 + y * _stride + x);
            }
            set
            {
                if (BoundsCheck(x, Width, y, Height))
                    *(_pixel0 + y * _stride + x) = value;
            }
        }

        static bool BoundsCheck(int x, int width, int y, int height)
        {
            return (x | (width - x - 1) | y | (height - y - 1)) > -1;
        }

#elif CLAMP

        public ARGB this[int x, int y]
        {
            get
            {
                x = Clamp(x, 0, Width);
                y = Clamp(y, 0, Height);
                return *(_pixel0 + y * _stride + x);
            }
            set
            {
                x = Clamp(x, 0, Width);
                y = Clamp(y, 0, Height);
                *(_pixel0 + y * _stride + x) = value;
            }
        }

        static int Clamp(int x, int low, int high)
        {
            if (x < low)
                return low;

            if (x > --high)
                return high;

            return x;
        }

#else

        //Rendering is not built for this, it will throw exceptions if ControlArea goes out of bounds.
        public ARGB this[int x, int y]
        {
            get
            {
                return *(_pixel0 + y * _stride + x);
            }
            set
            {
                *(_pixel0 + y * _stride + x) = value;
            }
        }

#endif

        /// <summary>
        /// Unlocks the bitmap
        /// </summary>
        public void Unlock()
        {
            _bitmap.UnlockBits(_lockData);
            _lockData = null;
            _pixel0 = null;
            _stride = 0;
        }

        public Graphics GetGraphics()
        {
            return Graphics.FromImage(_bitmap);
        }
    }
}