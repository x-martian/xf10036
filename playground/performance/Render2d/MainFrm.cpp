
// MainFrm.cpp : implementation of the CMainFrame class
//

#include "stdafx.h"
#include "Render2d.h"
#include "interpolator.h"

#include "MainFrm.h"
#include <assert.h>
#include <fstream>

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

typedef void (*PFUNC)(void* imageArray, int iw, int ih, int il, int cnt, void* disp, void* lut);

// CMainFrame

IMPLEMENT_DYNAMIC(CMainFrame, CFrameWnd)

BEGIN_MESSAGE_MAP(CMainFrame, CFrameWnd)
	ON_WM_CREATE()
	ON_WM_SETFOCUS()
	ON_WM_PAINT()
	ON_WM_SIZE()
	ON_WM_MOUSEWHEEL()
	ON_COMMAND(ID_ACTION_RACE, &CMainFrame::OnActionRace)
	ON_COMMAND(ID_ACTION_MATH, &CMainFrame::OnActionMath)
	ON_COMMAND(ID_ACTION_MATHDLL, &CMainFrame::OnActionMathdll)
END_MESSAGE_MAP()

static UINT indicators[] =
{
	ID_SEPARATOR,           // status line indicator
	ID_INDICATOR_CAPS,
	ID_INDICATOR_NUM,
	ID_INDICATOR_SCRL,
};

// CMainFrame construction/destruction

CMainFrame::CMainFrame() : m_currentImage(0), m_go(false)
{
//	m_pData = new short[IMAGEWIDTH*IMAGEHEIGHT*IMAGELENGTH];
	m_pData = (short*)::LocalAlloc(LMEM_FIXED, IMAGEWIDTH*IMAGEHEIGHT*IMAGELENGTH*2);
	m_lut = (BYTE*)::LocalAlloc(LMEM_FIXED, 2048);
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
}

CMainFrame::~CMainFrame()
{
	::LocalFree(m_pData);
//	delete [] m_pData;
}

int CMainFrame::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
	if (CFrameWnd::OnCreate(lpCreateStruct) == -1)
		return -1;

	// create a view to occupy the client area of the frame
/*	if (!m_wndView.Create(NULL, NULL, AFX_WS_DEFAULT_VIEW, CRect(0, 0, 0, 0), this, AFX_IDW_PANE_FIRST, NULL))
	{
		TRACE0("Failed to create view window\n");
		return -1;
	}

	if (!m_wndStatusBar.Create(this))
	{
		TRACE0("Failed to create status bar\n");
		return -1;      // fail to create
	}
	m_wndStatusBar.SetIndicators(indicators, sizeof(indicators)/sizeof(UINT));
*/
//	m_wndStatusBar

	m_memDC.CreateCompatibleDC(this->GetDC());
	m_bitmap.CreateCompatibleBitmap(GetDC(), 1, 1);
	m_memDC.SelectObject(&m_bitmap);
	m_interp = 0;

	return 0;
}

BOOL CMainFrame::PreCreateWindow(CREATESTRUCT& cs)
{
	if( !CFrameWnd::PreCreateWindow(cs) )
		return FALSE;
	// TODO: Modify the Window class or styles here by modifying
	//  the CREATESTRUCT cs

	cs.dwExStyle &= ~WS_EX_CLIENTEDGE;
	cs.lpszClass = AfxRegisterWndClass(0);
	return TRUE;
}

// CMainFrame diagnostics

#ifdef _DEBUG
void CMainFrame::AssertValid() const
{
	CFrameWnd::AssertValid();
}

void CMainFrame::Dump(CDumpContext& dc) const
{
	CFrameWnd::Dump(dc);
}
#endif //_DEBUG


// CMainFrame message handlers

void CMainFrame::OnSetFocus(CWnd* /*pOldWnd*/)
{
	// forward focus to the view window
//	m_wndView.SetFocus();
}

BOOL CMainFrame::OnCmdMsg(UINT nID, int nCode, void* pExtra, AFX_CMDHANDLERINFO* pHandlerInfo)
{
	// let the view have first crack at the command
//	if (m_wndView.OnCmdMsg(nID, nCode, pExtra, pHandlerInfo))
//		return TRUE;

	// otherwise, do default handling
	return CFrameWnd::OnCmdMsg(nID, nCode, pExtra, pHandlerInfo);
}

void CMainFrame::OnPaint()
{
	CPaintDC dc(this); // device context for painting
//	m_memDC.DeleteDC();
	
	CBrush brush;
	brush.CreateSysColorBrush(0);
	RECT rect;
	GetClientRect(&rect);
	int w = rect.right-rect.left;
	int h = rect.bottom-rect.top;
	DATATYPE* p0 = m_pData+IMAGEWIDTH*IMAGEHEIGHT*m_currentImage;
	double* interp = new double[IMAGEWIDTH+1];
	// set value at each pixel, use linear interpolation
	DATATYPE* lo;
	DATATYPE* hi;
	BYTE v = 0;
	BYTE* pv = m_interp;
	for (int j=0; j<h; ++j) {
		double* mid = interp;
		double x = double((2*j+1)*IMAGEHEIGHT-h)/double(2*h);
		if (x>0.0) {
			int index = x;
			x -= index;
			lo = p0+index*IMAGEWIDTH;
			hi = lo+IMAGEWIDTH;
		} else {
			x += 1.0;
			hi = p0;
			if (hi>m_pData)
				lo = hi - IMAGEWIDTH;
			else
				lo = m_pData + IMAGEWIDTH*(IMAGEHEIGHT*IMAGELENGTH-1);
		}
		double y = 1.0-x;
		for (int m=0; m<IMAGEWIDTH; ++m) {
			*mid = *lo*y+*hi*x;
			++mid; ++lo; ++hi;
		}
		*mid = *interp;	// repeat the first point at the end
		for (int i=0; i<w; ++i) {
			x = double((2*i+1)*IMAGEWIDTH-w)/double(2*w);
			if (x>0.0)
			{
				int index = x;
				x -= index;
				double s = *(interp+index)*(1-x)+*(interp+index+1)*x;
				v = m_lut[(int)s];
				*pv = m_lut[(int)s];
				++pv;
				m_memDC.SetPixel(i, j, RGB(v,v,v));
			}
		}
	}
	delete [] interp;
	dc.BitBlt(0, 0, w, h, &m_memDC, 0, 0, SRCCOPY);
	++m_currentImage;
	if (m_go) {
		if (m_currentImage < IMAGELENGTH-1)
			this->InvalidateRect(&rect, FALSE);
//		PostMessage(WM_PAINT);
		else {
			LARGE_INTEGER now, freq;
			::QueryPerformanceCounter(&now);
			::QueryPerformanceFrequency(&freq);
			LONGLONG span = now.QuadPart-m_tmr.QuadPart;
			LONGLONG fact = freq.QuadPart;
			while (span > 10000) {
				span = span >> 1;
				fact = fact >> 1;
			}

			double fps = double(m_currentImage)*fact/double(span);
			CString msg;
			msg.Format(_T("fps: %7.4f."), fps);
			MessageBox(msg);
			m_currentImage = 0;
			m_go = false;
			Invalidate(FALSE);
		}
	} else if (m_currentImage == IMAGELENGTH-1)
		m_currentImage = 0;

	// Do not call CFrameWnd::OnPaint() for painting messages
}

void CMainFrame::OnSize(UINT nType, int cx, int cy)
{
	CFrameWnd::OnSize(nType, cx, cy);

	RECT rect;
	GetClientRect(&rect);
	int w = rect.right-rect.left;
	int h = rect.bottom-rect.top;
	m_bitmap.DeleteObject();
	m_bitmap.CreateCompatibleBitmap(GetDC(), w, h);
	m_memDC.SelectObject(&m_bitmap);
	delete [] m_interp;
	m_interp = new BYTE[300*300*512];
	InvalidateRect(&rect, FALSE);
}

BOOL CMainFrame::OnMouseWheel(UINT nFlags, short zDelta, CPoint pt)
{
	Invalidate(FALSE);

	return CFrameWnd::OnMouseWheel(nFlags, zDelta, pt);
}

BOOL CMainFrame::Create(LPCTSTR lpszClassName, LPCTSTR lpszWindowName, DWORD dwStyle , const RECT& rect , CWnd* pParentWnd , LPCTSTR lpszMenuName , DWORD dwExStyle , CCreateContext* pContext)
{
	RECT wRect = {0,0,300,300};
	AdjustWindowRectEx(&wRect, dwStyle, TRUE, dwExStyle);
	int w = wRect.right-wRect.left;
	int h = wRect.bottom-wRect.top;
	wRect.top = 0;
	wRect.left = 0;
	wRect.bottom = wRect.top + h;
	wRect.right = wRect.left + w;

	return CFrameWnd::Create(lpszClassName, lpszWindowName, dwStyle, wRect, pParentWnd, lpszMenuName, dwExStyle, pContext);
}

BOOL CMainFrame::CreateEx(DWORD dwExStyle, LPCTSTR lpszClassName, LPCTSTR lpszWindowName, DWORD dwStyle, const RECT& rect, CWnd* pParentWnd, UINT nID, LPVOID lpParam)
{
	// TODO: Add your specialized code here and/or call the base class

	return CFrameWnd::CreateEx(dwExStyle, lpszClassName, lpszWindowName, dwStyle, rect, pParentWnd, nID, lpParam);
}

void CMainFrame::OnActionRace()
{
	m_go = true;
	m_currentImage = 0;
	::QueryPerformanceCounter(&m_tmr);
	Invalidate(FALSE);
}

void CMainFrame::OnActionMath()
{
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
		Trail(IMAGEWIDTH, IMAGEHEIGHT, i);
		QueryPerformanceCounter(&now);

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
	return;
/*
	
	::QueryPerformanceCounter(&now);
	LONGLONG span = now.QuadPart-m_tmr.QuadPart;
	LONGLONG fact = freq.QuadPart;
	while (span > 10000) {
		span = span >> 1;
		fact = fact >> 1;
	}

	double fps = double(m_currentImage)*fact/double(span);
	CString msg;
	msg.Format(_T("fps: %7.4f."), fps);
	m_currentImage = 0;
	MessageBox(msg);
*/}
void CMainFrame::Trail(int iw, int ih, int il)
{
	int w = 300;
	int h = 300;
	double* interp = new double[iw+1];
	// set value at each pixel, use linear interpolation
	DATATYPE* lo;
	DATATYPE* hi;
	BYTE v = 0;
	int currentImage = 0;
	do {
	BYTE* pv = m_interp;
	for (int j=0; j<h; ++j) {
		DATATYPE* p0 = m_pData+iw*ih*currentImage;
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
			if (hi>m_pData)
				lo = hi - iw;
			else
				lo = m_pData + iw*(ih*il);
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
				v = *(m_lut+(int)s);
//				v = m_lut[(int)s];
				*pv = v;
				++pv;
			}
		}
	}
	} while (++currentImage <= il);
}

void CMainFrame::OnActionMathdll()
{
#ifdef _DEBUG
	std::ofstream fs(".\\clrBench\\DLL_CppNativeHostDebug.txt");
#else
	std::ofstream fs(".\\clrBench\\DLL_CppNativeHostRelease.txt");
#endif
	HMODULE hModule = ::LoadLibrary("interpolator.dll");
	PFUNC pfunc = (PFUNC)::GetProcAddress(hModule, "fninterpolator");

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
		pfunc(m_pData, IMAGEWIDTH, IMAGEHEIGHT, 0, i, m_interp, m_lut);
		QueryPerformanceCounter(&now);

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
	::FreeLibrary(hModule);
	return;
}
