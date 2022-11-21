#ifndef _DLL_H_
#define _DLL_H_

extern "C"
{
	__declspec(dllexport) const char * __stdcall Hash(const char * source);
	__declspec(dllexport) const char * __stdcall GetVersion();
}

#endif
