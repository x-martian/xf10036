
// Render2d.h : main header file for the Render2d application
//
#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"       // main symbols


// CRander2dApp:
// See Render2d.cpp for the implementation of this class
//

class CRander2dApp : public CWinApp
{
public:
	CRander2dApp();


// Overrides
public:
	virtual BOOL InitInstance();

// Implementation

public:
	afx_msg void OnAppAbout();
	DECLARE_MESSAGE_MAP()
};

extern CRander2dApp theApp;
