using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Token: 0x020009D9 RID: 2521
public static class UnsafeUtils
{
	// Token: 0x06003C58 RID: 15448 RVA: 0x001204D0 File Offset: 0x0011E6D0
	public unsafe static readonly ref T[] GetInternalArray<T>(this List<T> list)
	{
		if (list == null)
		{
			return Unsafe.NullRef<T[]>();
		}
		return ref Unsafe.As<List<T>, StrongBox<T[]>>(ref list)->Value;
	}

	// Token: 0x06003C59 RID: 15449 RVA: 0x001204E8 File Offset: 0x0011E6E8
	public unsafe static readonly ref T[] GetInvocationListUnsafe<T>(this T @delegate) where T : MulticastDelegate
	{
		if (@delegate == null)
		{
			return Unsafe.NullRef<T[]>();
		}
		return Unsafe.As<Delegate[], T[]>(ref Unsafe.As<T, UnsafeUtils._MultiDelegateFields>(ref @delegate)->delegates);
	}

	// Token: 0x020009DA RID: 2522
	[StructLayout(LayoutKind.Sequential)]
	private class _MultiDelegateFields : UnsafeUtils._DelegateFields
	{
		// Token: 0x0400405B RID: 16475
		public Delegate[] delegates;
	}

	// Token: 0x020009DB RID: 2523
	[StructLayout(LayoutKind.Sequential)]
	private class _DelegateFields
	{
		// Token: 0x0400405C RID: 16476
		public IntPtr method_ptr;

		// Token: 0x0400405D RID: 16477
		public IntPtr invoke_impl;

		// Token: 0x0400405E RID: 16478
		public object m_target;

		// Token: 0x0400405F RID: 16479
		public IntPtr method;

		// Token: 0x04004060 RID: 16480
		public IntPtr delegate_trampoline;

		// Token: 0x04004061 RID: 16481
		public IntPtr extra_arg;

		// Token: 0x04004062 RID: 16482
		public IntPtr method_code;

		// Token: 0x04004063 RID: 16483
		public IntPtr interp_method;

		// Token: 0x04004064 RID: 16484
		public IntPtr interp_invoke_impl;

		// Token: 0x04004065 RID: 16485
		public MethodInfo method_info;

		// Token: 0x04004066 RID: 16486
		public MethodInfo original_method_info;

		// Token: 0x04004067 RID: 16487
		public UnsafeUtils._DelegateData data;

		// Token: 0x04004068 RID: 16488
		public bool method_is_virtual;
	}

	// Token: 0x020009DC RID: 2524
	[StructLayout(LayoutKind.Sequential)]
	private class _DelegateData
	{
		// Token: 0x04004069 RID: 16489
		public Type target_type;

		// Token: 0x0400406A RID: 16490
		public string method_name;

		// Token: 0x0400406B RID: 16491
		public bool curried_first_arg;
	}
}
