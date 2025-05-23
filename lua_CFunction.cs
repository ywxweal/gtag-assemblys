using System;
using System.Runtime.InteropServices;

// Token: 0x020008A6 RID: 2214
// (Invoke) Token: 0x06003568 RID: 13672
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate int lua_CFunction(lua_State* L);
