// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the INTERPOLATOR_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// INTERPOLATOR_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef INTERPOLATOR_EXPORTS
#define INTERPOLATOR_API __declspec(dllexport)
#else
#define INTERPOLATOR_API __declspec(dllimport)
#endif

// This class is exported from the interpolator.dll
class INTERPOLATOR_API Cinterpolator {
public:
	Cinterpolator(void);
	// TODO: add your methods here.
};

extern INTERPOLATOR_API int ninterpolator;

extern "C" {
//INTERPOLATOR_API void fninterpolator(void* imageArray, int iw, int ih, int il, void* disp, void* lut);
INTERPOLATOR_API int fninterpolator(short* imageArray, int iw, int ih, int il, unsigned char* disp, unsigned char* lut);
INTERPOLATOR_API void fnalloc(void*& imageArray, int iw, int ih, int il, void*& disp, void*&lut);
}
