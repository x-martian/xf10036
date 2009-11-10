// interpolator.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "interpolator.h"


// This is an example of an exported variable
INTERPOLATOR_API int ninterpolator=0;

INTERPOLATOR_API void fnalloc(void*& imageArray, int iw, int ih, int il, void*& disp, void*&lut)
{/*
	imageArray = (void*)::HeapAlloc(LMEM_FIXED, iw*ih*il*2);
	lut = (void*)::HeapAlloc(LMEM_FIXED, 2048);
	disp = (void*)::HeapAlloc(LMEM_FIXED, 300*300); //new BYTE[300*300];
*/	imageArray = (void*)::HeapAlloc(GetProcessHeap(), 0, iw*ih*il*2);
	lut = (void*)::HeapAlloc(GetProcessHeap(), 0, 2048);
	disp = (void*)::HeapAlloc(GetProcessHeap(), 0, 300*300); //new BYTE[300*300];

}


// This is an example of an exported function.
//INTERPOLATOR_API void fninterpolator(void* imageArray, int iw, int ih, int il, void* disp, void* lut)
INTERPOLATOR_API int fninterpolator(short* imageArray, int iw, int ih, int il, int cnt, unsigned char* disp, unsigned char* lut)
{/*
	LARGE_INTEGER m_tmr, now, freq;
	::QueryPerformanceCounter(&m_tmr);
	::QueryPerformanceCounter(&now);
	LONGLONG delta = now.QuadPart - m_tmr.QuadPart;
	::QueryPerformanceFrequency(&freq);

	::QueryPerformanceCounter(&m_tmr);
	::QueryPerformanceCounter(&now);
	delta = now.QuadPart - m_tmr.QuadPart;

	::QueryPerformanceCounter(&m_tmr);
*/
	int w = 300;
	int h = 300;
    int ie = il + cnt;
	double* interp = new double[iw+1];
	// set value at each pixel, use linear interpolation
    BYTE* pv = disp + 300*300*il;
    do
    {
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
        }   // outer fixed
    } while (++il < ie);
/*
	::QueryPerformanceCounter(&now);

	LONGLONG span = now.QuadPart - m_tmr.QuadPart - delta;
	LONGLONG fact2 = freq.QuadPart;
    while (span > 1000000 && fact2 > 1000)
	{
		span = span >> 1;
	    fact2 = fact2 >> 1;
	}
	span = span*1000/fact2;

	return (int)span;
	*/return 0;
}

// This is the constructor of a class that has been exported.
// see interpolator.h for the class definition
Cinterpolator::Cinterpolator()
{
	return;
}
