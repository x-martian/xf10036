
// MainFrm.h : interface of the CMainFrame class
//

#pragma once
#include "ChildView.h"

#define IMAGEWIDTH 512
#define IMAGEHEIGHT 512
#define IMAGELENGTH 512

class CMainFrame : public CFrameWnd
{
	typedef short DATATYPE;
public:
	CMainFrame();
protected: 
	DECLARE_DYNAMIC(CMainFrame)

// Attributes
public:

// Operations
public:

// Overrides
public:
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
	virtual BOOL OnCmdMsg(UINT nID, int nCode, void* pExtra, AFX_CMDHANDLERINFO* pHandlerInfo);

// Implementation
public:
	virtual ~CMainFrame();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:  // control bar embedded members
	CStatusBar        m_wndStatusBar;
//	CChildView    m_wndView;

// Generated message map functions
protected:
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnSetFocus(CWnd *pOldWnd);
	DECLARE_MESSAGE_MAP()

public:
	afx_msg void OnPaint();

private:
	DATATYPE* m_pData;
	BYTE* m_lut;
	CDC m_memDC;
	CBitmap m_bitmap;
	BYTE* m_interp;

	int m_currentImage;
	LARGE_INTEGER m_tmr;
	bool m_go;

	void Trail(int iw, int ih, int il);
public:
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg BOOL OnMouseWheel(UINT nFlags, short zDelta, CPoint pt);
	virtual BOOL Create(LPCTSTR lpszClassName, LPCTSTR lpszWindowName, DWORD dwStyle = WS_OVERLAPPEDWINDOW, const RECT& rect = rectDefault, CWnd* pParentWnd = NULL, LPCTSTR lpszMenuName = NULL, DWORD dwExStyle = 0, CCreateContext* pContext = NULL);
	virtual BOOL CreateEx(DWORD dwExStyle, LPCTSTR lpszClassName, LPCTSTR lpszWindowName, DWORD dwStyle, const RECT& rect, CWnd* pParentWnd, UINT nID, LPVOID lpParam = NULL);
	afx_msg void OnActionRace();
	afx_msg void OnActionMath();
	afx_msg void OnActionMathdll();
};


