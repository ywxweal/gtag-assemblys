using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000A6C RID: 2668
	// (Invoke) Token: 0x06003FAA RID: 16298
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void StatusCallback2(int nResult, [MarshalAs(UnmanagedType.LPStr)] string message);
}
