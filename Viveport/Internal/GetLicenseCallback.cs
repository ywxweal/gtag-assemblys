using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000A6A RID: 2666
	// (Invoke) Token: 0x06003FA2 RID: 16290
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void GetLicenseCallback([MarshalAs(UnmanagedType.LPStr)] string message, [MarshalAs(UnmanagedType.LPStr)] string signature);
}
