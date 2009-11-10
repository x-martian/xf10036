using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Render2DCS
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll")]
        static extern int QueryPerformanceCounter(out Int64 count);
        [DllImport("kernel32.dll")]
        static extern int QueryPerformanceFrequency(out Int64 frequency);
        [DllImport("interpolator.dll")]
        static extern int fninterpolator(IntPtr imageArray, int w, int h, int l, int cnt, IntPtr outImage, IntPtr lut);
        [DllImport("interpolator.dll")]
        static extern void fnalloc(out IntPtr imageArray, int w, int h, int l, out IntPtr outArray, out IntPtr lut);

        private IntPtr imageArray;
        private IntPtr lut;
        private IntPtr dispArray;
        private int currentImage;
        private Boolean go;
        private Int64 tmr;
        private Bitmap bm;
        private static int IMAGEWIDTH = 512;
        private static int IMAGEHEIGHT = 512;
        private static int IMAGELENGTH = 512;

        public unsafe Form1()
        {
            InitializeComponent();

            //fnalloc(out imageArray, IMAGEWIDTH, IMAGEHEIGHT, IMAGELENGTH, out dispArray, out lut);
            imageArray = Marshal.AllocHGlobal((int)(IMAGEWIDTH * IMAGEHEIGHT * IMAGELENGTH*2)); // two byte integers
            short* tmp = (short*)imageArray;
            Random rand = new Random(1);
            for (int k = 0; k < IMAGELENGTH; ++k)
                for (int j = 0; j < IMAGEHEIGHT; ++j)
                    for (int i = 0; i < IMAGEWIDTH; ++i)
                    {
                        //short v = rand->Next(2048);
                        short v = (short)(i + j + 2*k);
                        if (v > 2047) v = 2047;
                        *tmp = v;
                        ++tmp;
                    }

            lut = Marshal.AllocHGlobal(2048);
            Byte* pLut = (Byte*)lut;
            for (short b = 0; b < 2048; ++b)
            {
                *pLut = (Byte)(b * 256 / 2048);
                ++pLut;
            }
            dispArray = IntPtr.Zero;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
			bm = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, CreateGraphics());
			ClientSize = new System.Drawing.Size(300, 300);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
			//BufferedGraphicsContext context = BufferedGraphicsManager.Current;
			//BufferedGraphics graphics = context.Allocate(CreateGraphics(), DisplayRectangle);
			Rectangle rect = ClientRectangle;
            if (currentImage == IMAGELENGTH)
                currentImage = 0;
            unsafe
            {
                interpolate((short*)(imageArray), IMAGEWIDTH, IMAGEHEIGHT, currentImage, 1, (byte*)(dispArray), (byte*)(lut));
                BitmapData bd = bm.LockBits(rect, ImageLockMode.WriteOnly, bm.PixelFormat);
                IntPtr ptr = bd.Scan0;
                Byte* p = (Byte*)(void*)ptr;
                Byte* pv = (Byte*)dispArray; // +300 * 300 * currentImage;
                for (int i = 0; i < 300 * 300; ++i)
                {
                    Byte v = *pv++;
                    *p = v; ++p; *p = v; ++p; *p = v; ++p; *p = 255; ++p;
                }
                bm.UnlockBits(bd);
            }

            
            /*
            // do the interplation in an unsafe context
        	int w = rect.Width;
            int h = rect.Height;
            unsafe {
                short* pData = (short*)imageArray;
                Byte* pLut = (Byte*)lut;
                short* p0 = pData+IMAGEWIDTH*IMAGEHEIGHT*currentImage;
                IntPtr interp = Marshal.AllocHGlobal((IMAGEWIDTH+1)*8);


                    // set value at each pixel, use linear interplation
	                short* lo;
	                short* hi;
                    Byte v;
        			BitmapData bd = bm.LockBits(rect, ImageLockMode.WriteOnly, bm.PixelFormat);
		        	IntPtr ptr = bd.Scan0;
        			Byte* p = (Byte*)(void*)ptr;
                	for (int j=0; j<h; ++j) {
                		double x = (double)((2*j+1)*IMAGEHEIGHT-h)/(double)(2*h);
                		if (x>0.0) {
			                int index = (int)(x);
			                x -= index;
			                lo = p0+index*IMAGEWIDTH;
                			hi = lo+IMAGEWIDTH;
                		} else {
                			x += 1.0;
			                hi = p0;
                			if (hi>pData)
            		    		lo = hi - IMAGEWIDTH;
			                else
                				lo = pData + IMAGEWIDTH*(IMAGEHEIGHT*IMAGELENGTH-1);
                        }
                   		double y = 1.0-x;
                        double* mid = (double*)interp;
                            double* tmp = mid;
    		                for (int m=0; m<IMAGEWIDTH; ++m) {
	    		                *tmp = *lo*y+*hi*x;
		    	                ++tmp; ++lo; ++hi;
		                    }
                            *tmp = *mid;

                    		for (int i=0; i<w; ++i) {
                		    	x = (double)((2*i+1)*IMAGEWIDTH-w)/(double)(2*w);
                                if (x > 0.0)
                                {
                                    int index = (int)x;
                                    x -= index;
                                    double s = *(mid + index) * (1 - x) + *(mid + index + 1) * x;
                                    v = *(pLut+(int)s);
                                    *p = v; ++p; *p = v; ++p; *p = v; ++p; *p = 255; ++p;
                                }
                                else
                                {
                                    p += 4;
                                }
                            }
        			}
        			bm.UnlockBits(bd);
        	}   // unsafe
*/
			e.Graphics.DrawImage(bm, 0, 0);

			++currentImage;
			if (go==true) {
				if (currentImage < IMAGELENGTH-1)
					Invalidate(rect);
				else {
					Int64 now, freq;
					QueryPerformanceCounter(out now);
					QueryPerformanceFrequency(out freq);
					Int64 span = now-tmr;
					Int64 fact = freq;
					while (span > 10000) {
						span = span >> 1;
						fact = fact >> 1;
					}

					double fps = (double)(currentImage)*fact/(double)(span);
					StringBuilder sb = new StringBuilder();
					sb.AppendFormat("fps: {0}", fps);
					MessageBox.Show(sb.ToString());
					currentImage = 0;
					go = false;
					Invalidate();
				}
			} else if (currentImage == IMAGELENGTH-1)
				currentImage = 0;

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
			if (ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0)
				return;

			bm = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, CreateGraphics());
            
            if (dispArray != IntPtr.Zero)
                Marshal.FreeHGlobal(dispArray);
            dispArray = Marshal.AllocHGlobal(ClientRectangle.Width * ClientRectangle.Height * 512);
            Marshal.WriteByte(dispArray, 300 * 300 * 512 - 1, 1);
            
			Invalidate();
        }

        private void benchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            go = true;
            currentImage = 0;
            QueryPerformanceCounter(out tmr);
            Invalidate();
        }

        private void benchToolStripMenuItem_MouseMove(object sender, MouseEventArgs e)
        {
            Invalidate();
        }

        private void mathToolStripMenuItem_Click(object sender, EventArgs e)
        {
		    System.IO.FileStream fs = new System.IO.FileStream("..\\..\\..\\Release\\clrBench\\CSharpRelease.txt", System.IO.FileMode.Create);
		    System.IO.TextWriter sw = new System.IO.StreamWriter(fs);
            Int64 now, delta;
	    	QueryPerformanceCounter(out tmr);
            QueryPerformanceCounter(out now);
	    	delta = now-tmr;	// find the p/invoke penalty
            Int64 fact0;
            QueryPerformanceFrequency(out fact0);
	    	for (int length = 10; length < 512; length+=10)
		    {
			    QueryPerformanceCounter(out tmr);
    		    QueryPerformanceCounter(out now);
    			delta = now-tmr;	// find the p/invoke penalty
	    		QueryPerformanceCounter(out tmr);
                unsafe
                {
                    interpolate((short*)(imageArray), IMAGEWIDTH, IMAGEHEIGHT, 0, length, (byte*)(dispArray), (byte*)(lut));
                }
    			QueryPerformanceCounter(out now);
    	        Int64 span = now - tmr - delta;
	    		Int64 fact = fact0;
		        while (span > 1000000 && fact > 1000)
			    {
				    span = span >> 1;
			        fact = fact >> 1;
			    }
    			span = span*1000/fact;
	    		sw.WriteLine("{0} {1} {2}", length, span, delta);
		    	sw.Flush();
		    }
    		sw.Close();
	    	fs.Close();
        }

        private unsafe static void interpolate(short* imageArray, int iw, int ih, int il, int cnt, byte* disp, byte* lut)
        {
            // do the interplation in an unsafe context
            int w = 300;
            int h = 300;
            int ie = il + cnt;
            double* interp = (double*)(Marshal.AllocHGlobal((iw + 1) * 8));
            do
            {
                Byte* pv = (Byte*)disp;
                short* p0 = imageArray + iw * ih * il;
                for (int j = 0; j < h; ++j)
                {
                    short* lo;
                    short* hi;
                    double x = (double)((2 * j + 1) * ih - h) / (double)(2 * h);
                    if (x > 0.0)
                    {
                        int index = (int)x;
                        x -= index;
                        lo = p0 + index * iw;
                        hi = lo + iw;
                    }
                    else
                    {
                        x += 1.0;
                        hi = p0;
                        if (hi > imageArray)
                            lo = hi - iw;
                        else
                            lo = imageArray + iw * (ih * ie);
                    }
                    double y = 1.0 - x;
                    double* mid = (double*)interp;
                    for (int m = 0; m < iw; ++m)
                    {
                        *mid = *lo * y + *hi * x;
                        ++mid; ++lo; ++hi;
                    }
                    *mid = *interp;

                    for (int i = 0; i < w; ++i)
                    {
                        x = (double)((2 * i + 1) * iw - w) / (double)(2 * w);
                        if (x > 0.0)
                        {
                            int index = (int)x;
                            x -= index;
                            double s = *(interp + index) * (1 - x) + *(interp + index + 1) * x;
                            *pv = *(lut + (int)s);
                            ++pv;
                        }
                    }
                }
            } while (++il < ie);
            Marshal.FreeHGlobal((IntPtr)(interp));
        }

        private void mathDLLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.IO.FileStream fs = new System.IO.FileStream("..\\..\\..\\Release\\clrBench\\DLL_CSharpHost.txt", System.IO.FileMode.Create);
            System.IO.TextWriter sw = new System.IO.StreamWriter(fs);
            Int64 now, delta;
            QueryPerformanceCounter(out tmr);
            QueryPerformanceCounter(out now);
            delta = now - tmr;	// find the p/invoke penalty
            Int64 fact0;
            QueryPerformanceFrequency(out fact0);
            for (int length = 10; length < 512; length += 10)
            {
                QueryPerformanceCounter(out tmr);
                QueryPerformanceCounter(out now);
                delta = now - tmr;	// find the p/invoke penalty
                QueryPerformanceCounter(out tmr);
                unsafe
                {
                    fninterpolator(imageArray, IMAGEWIDTH, IMAGEHEIGHT, 0, length, dispArray, lut);
                }
                QueryPerformanceCounter(out now);
                Int64 span = now - tmr - delta;
                Int64 fact = fact0;
                while (span > 1000000 && fact > 1000)
                {
                    span = span >> 1;
                    fact = fact >> 1;
                }
                span = span * 1000 / fact;
                sw.WriteLine("{0} {1} {2}", length, span, delta);
                sw.Flush();
            }
            sw.Close();
            fs.Close();
        }
    }
}
