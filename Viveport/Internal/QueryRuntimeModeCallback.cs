﻿using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000A6D RID: 2669
	// (Invoke) Token: 0x06003FAF RID: 16303
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void QueryRuntimeModeCallback(int nResult, int nMode);
}
