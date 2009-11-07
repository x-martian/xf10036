typedef unsigned char BYTE;

// This is an example of an exported function.
void fninterpolator(void* imageArray, int iw, int ih, int il, void* disp, void* lut)
{
	int w = 300;
	int h = 300;
	double* interp = new double[iw+1];
	// set value at each pixel, use linear interpolation
	short* lo;
	short* hi;
	BYTE v = 0;
	int l = 0;
	do {
	BYTE* pv = (BYTE*)disp;
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
				v = *((BYTE*)lut+(int)s);
				*pv = v;
				++pv;
			}
		}
	}
	} while (++l <= il);

	return;
}
