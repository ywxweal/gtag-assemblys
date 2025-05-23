using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000A77 RID: 2679
	// (Invoke) Token: 0x06003FB2 RID: 16306
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void IAPurchaseCallback(int code, [MarshalAs(UnmanagedType.LPStr)] string message);
}
