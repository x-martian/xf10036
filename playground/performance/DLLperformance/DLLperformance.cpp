// DLLperformance.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
//#include "..\staticlibrary\staticlibfunc.h"
#include "..\interpolator\interpolator.h"
#define IMAGEWIDTH 512
#define IMAGEHEIGHT 512
#define IMAGELENGTH 512

//typedef void (*PFUNC)(void* imageArray, int iw, int ih, int il, void* disp, void* lut);
typedef int (*PFUNC)(short* imageArray, int iw, int ih, int il, unsigned char* disp, unsigned char* lut);

extern "C" {
//	INTERPOLATOR_API void localinterpolator(void* imageArray, int iw, int ih, int il, void* disp, void* lut);
	void localinterpolator(short* imageArray, int iw, int ih, int il, unsigned char* disp, unsigned char* lut);
}

int _tmain(int argc, _TCHAR* argv[])
{
	short* m_pData;
	unsigned char* m_lut;
	unsigned char* m_interp;

	int m_currentImage;
	LARGE_INTEGER m_tmr;
	bool m_go;

	m_pData = (short*)::LocalAlloc(LMEM_FIXED, IMAGEWIDTH*IMAGEHEIGHT*IMAGELENGTH*2);
	m_lut = (BYTE*)::LocalAlloc(LMEM_FIXED, 2048);
	m_interp = (unsigned char*) ::LocalAlloc(LMEM_FIXED, 300*300); //new BYTE[300*300];
/*	void *tmp1, *tmp2, *tmp3;
	fnalloc(tmp1, 512, 512, 512, tmp2, tmp3);
	m_pData = (short*)tmp1;
	m_interp = (unsigned char*)tmp2;
	m_lut = (unsigned char*)tmp3;
*/
	srand(1);
	short* p = m_pData;
	for (unsigned k=0; k<IMAGELENGTH; ++k)
		for (unsigned j=0; j<IMAGEHEIGHT; ++j)
			for (unsigned i=0; i<IMAGEWIDTH; ++i) {
//				short v = rand()/((RAND_MAX+1)/(2048));
				short v = i+j;//+k/2;
				*p = v;
				++p;
			}

	for (short b=0; b<2048; ++b)
	{
		m_lut[b] = b*256/2048;
	}


#ifdef _DEBUG
	std::ofstream fs("D:\\clrBench\\NativeCppDebug.txt");
#else
	std::ofstream fs("D:\\clrBench\\NativeCppRelease.txt");
#endif
	LARGE_INTEGER now, freq;
	::QueryPerformanceCounter(&m_tmr);
	::QueryPerformanceCounter(&now);
	LONGLONG delta = now.QuadPart - m_tmr.QuadPart;
	::QueryPerformanceFrequency(&freq);

	for (int i=10; i<511; i+=10)
	{
		::QueryPerformanceCounter(&m_tmr);
		::QueryPerformanceCounter(&now);
		delta = now.QuadPart - m_tmr.QuadPart;

		::QueryPerformanceCounter(&m_tmr);
		::localinterpolator(m_pData, IMAGEWIDTH, IMAGEHEIGHT, i, m_interp, m_lut);
		::QueryPerformanceCounter(&now);

		LONGLONG span = now.QuadPart - m_tmr.QuadPart - delta;
		LONGLONG fact2 = freq.QuadPart;
	    while (span > 1000000 && fact2 > 1000)
		{
			span = span >> 1;
		    fact2 = fact2 >> 1;
		}
		span = span*1000/fact2;
		fs << i << ' ' << span << ' ' << delta << std::endl;
		fs.flush();
	}
	fs.close();

#ifdef _DEBUG
	fs.open("D:\\clrBench\\DLL_CppNativeHostDebug.txt");
#else
	fs.open("D:\\clrBench\\DLL_CppNativeHostRelease.txt");
#endif
	HMODULE hModule = ::LoadLibrary(L"interpolator.dll");
	PFUNC pfunc = (PFUNC)::GetProcAddress(hModule, "fninterpolator");

	for (int i=10; i<511; i+=10)
	{
		::QueryPerformanceCounter(&m_tmr);
		::QueryPerformanceCounter(&now);
		delta = now.QuadPart - m_tmr.QuadPart;

		::QueryPerformanceCounter(&m_tmr);
		int time = pfunc(m_pData, IMAGEWIDTH, IMAGEHEIGHT, i, m_interp, m_lut);
		//fninterpolator(m_pData, IMAGEWIDTH, IMAGEHEIGHT, i, m_interp, m_lut);
		QueryPerformanceCounter(&now);

		LONGLONG span = now.QuadPart - m_tmr.QuadPart - delta;
		LONGLONG fact2 = freq.QuadPart;
	    while (span > 1000000 && fact2 > 1000)
		{
			span = span >> 1;
		    fact2 = fact2 >> 1;
		}
		span = span*1000/fact2;
		fs << i << ' ' << span << ' ' << delta << ' ' << time << std::endl;
		fs.flush();
	}
	::FreeLibrary(hModule);

	return 0;
}

void localinterpolator(short* imageArray, int iw, int ih, int il, unsigned char* disp, unsigned char* lut)
{
	int w = 300;
	int h = 300;
	double* interp = new double[iw+1];
	// set value at each pixel, use linear interpolation
	short* lo;
	short* hi;
	unsigned char v = 0;
	int l = 0;
	do {
	unsigned char* pv = (unsigned char*)disp;
	for (int j=0; j<h; ++j) {
		short* p0 = (short*)imageArray+iw*ih*l;
		double* mid = interp;
		double x = double((2*j+1)*ih-h)/double(2*h);
		if (x>0.0) {
			int index = x;
			x -= index;
			lo = p0+index*iw;
			hi = lo+iw;
		} else {
			x += 1.0;
			hi = p0;
			if (hi>imageArray)
				lo = hi - iw;
			else
				lo = (short*)imageArray + iw*(ih*il);
		}
		double y = 1.0-x;
		for (int m=0; m<iw; ++m) {
			*mid = *lo*y+*hi*x;
			++mid; ++lo; ++hi;
		}
		*mid = *interp;	// repeat the first point at the end
		for (int i=0; i<w; ++i) {
			x = double((2*i+1)*iw-w)/double(2*w);
			if (x>0.0)
			{
				int index = x;
				x -= index;
				double s = *(interp+index)*(1-x)+*(interp+index+1)*x;
				v = *((unsigned char*)lut+(int)s);
				*pv = v;
				++pv;
			}
		}
	}
	} while (++l <= il);

	return;
}
