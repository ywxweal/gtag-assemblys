using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x020008A8 RID: 2216
public class Luau
{
	// Token: 0x0600356F RID: 13679
	[DllImport("luau")]
	public unsafe static extern lua_State* luaL_newstate();

	// Token: 0x06003570 RID: 13680
	[DllImport("luau")]
	public unsafe static extern void luaL_openlibs(lua_State* L);

	// Token: 0x06003571 RID: 13681
	[DllImport("luau")]
	public unsafe static extern sbyte* luau_compile([MarshalAs(UnmanagedType.LPStr)] string source, [NativeInteger] UIntPtr size, lua_CompileOptions* options, [NativeInteger] UIntPtr* outsize);

	// Token: 0x06003572 RID: 13682
	[DllImport("luau")]
	public unsafe static extern int luau_load(lua_State* L, [MarshalAs(UnmanagedType.LPStr)] string chunkname, sbyte* data, [NativeInteger] UIntPtr size, int env);

	// Token: 0x06003573 RID: 13683
	[DllImport("luau")]
	public unsafe static extern void lua_pushvalue(lua_State* L, int idx);

	// Token: 0x06003574 RID: 13684
	[DllImport("luau")]
	public unsafe static extern void lua_pushcclosurek(lua_State* L, lua_CFunction fn, [MarshalAs(UnmanagedType.LPStr)] string debugname, int nup, lua_Continuation cont);

	// Token: 0x06003575 RID: 13685
	[DllImport("luau")]
	public unsafe static extern void lua_pushcclosurek(lua_State* L, FunctionPointer<lua_CFunction> fn, [MarshalAs(UnmanagedType.LPStr)] string debugname, int nup, lua_Continuation cont);

	// Token: 0x06003576 RID: 13686
	[DllImport("luau")]
	public unsafe static extern void lua_pushcclosurek(lua_State* L, FunctionPointer<lua_CFunction> fn, byte* debugname, int nup, int* cont);

	// Token: 0x06003577 RID: 13687 RVA: 0x00103BF0 File Offset: 0x00101DF0
	public unsafe static void lua_pushcfunction(lua_State* L, FunctionPointer<lua_CFunction> fn, [MarshalAs(UnmanagedType.LPStr)] string debugname)
	{
		Luau.lua_pushcclosurek(L, fn, debugname, 0, null);
	}

	// Token: 0x06003578 RID: 13688 RVA: 0x00103BFC File Offset: 0x00101DFC
	public unsafe static void lua_pushcfunction(lua_State* L, lua_CFunction fn, [MarshalAs(UnmanagedType.LPStr)] string debugname)
	{
		Luau.lua_pushcclosurek(L, fn, debugname, 0, null);
	}

	// Token: 0x06003579 RID: 13689
	[DllImport("luau")]
	public unsafe static extern void lua_settop(lua_State* L, int idx);

	// Token: 0x0600357A RID: 13690
	[DllImport("luau")]
	public unsafe static extern int lua_gettop(lua_State* L);

	// Token: 0x0600357B RID: 13691
	[DllImport("luau")]
	public unsafe static extern sbyte* lua_tolstring(lua_State* L, int idx, int* len);

	// Token: 0x0600357C RID: 13692
	[DllImport("luau")]
	public unsafe static extern int lua_resume(lua_State* L, lua_State* from, int nargs);

	// Token: 0x0600357D RID: 13693
	[DllImport("luau")]
	public unsafe static extern void lua_setfield(lua_State* L, int index, [MarshalAs(UnmanagedType.LPStr)] string k);

	// Token: 0x0600357E RID: 13694
	[DllImport("luau")]
	public unsafe static extern void lua_setfield(lua_State* L, int index, byte* k);

	// Token: 0x0600357F RID: 13695 RVA: 0x00103C08 File Offset: 0x00101E08
	public unsafe static void lua_setglobal(lua_State* L, string s)
	{
		Luau.lua_setfield(L, -10002, s);
	}

	// Token: 0x06003580 RID: 13696 RVA: 0x00103C18 File Offset: 0x00101E18
	public unsafe static void lua_register(lua_State* L, lua_CFunction f, string n)
	{
		lua_Continuation lua_Continuation = null;
		Luau.lua_pushcclosurek(L, f, n, 0, lua_Continuation);
		Luau.lua_setglobal(L, n);
	}

	// Token: 0x06003581 RID: 13697 RVA: 0x00103C38 File Offset: 0x00101E38
	public unsafe static void lua_pop(lua_State* L, int n)
	{
		Luau.lua_settop(L, -n - 1);
	}

	// Token: 0x06003582 RID: 13698 RVA: 0x00103C44 File Offset: 0x00101E44
	public unsafe static sbyte* lua_tostring(lua_State* L, int idx)
	{
		return Luau.lua_tolstring(L, idx, null);
	}

	// Token: 0x06003583 RID: 13699
	[DllImport("luau")]
	public unsafe static extern int lua_isstring(lua_State* L, int index);

	// Token: 0x06003584 RID: 13700
	[DllImport("luau")]
	public unsafe static extern int lua_type(lua_State* L, int index);

	// Token: 0x06003585 RID: 13701
	[DllImport("luau")]
	public unsafe static extern int lua_pushstring(lua_State* L, [MarshalAs(UnmanagedType.LPStr)] string s);

	// Token: 0x06003586 RID: 13702
	[DllImport("luau")]
	public unsafe static extern int lua_pushstring(lua_State* L, byte* s);

	// Token: 0x06003587 RID: 13703
	[DllImport("luau")]
	public unsafe static extern int lua_error(lua_State* L);

	// Token: 0x06003588 RID: 13704
	[DllImport("luau")]
	public unsafe static extern void luaL_errorL(lua_State* L, [MarshalAs(UnmanagedType.LPStr)] string fmt, [MarshalAs(UnmanagedType.LPStr)] params string[] a);

	// Token: 0x06003589 RID: 13705
	[DllImport("luau")]
	public unsafe static extern void luaL_errorL(lua_State* L, sbyte* fmt);

	// Token: 0x0600358A RID: 13706
	[DllImport("luau")]
	public unsafe static extern int lua_toboolean(lua_State* L, int index);

	// Token: 0x0600358B RID: 13707
	[DllImport("luau")]
	public unsafe static extern byte* lua_debugtrace(lua_State* L);

	// Token: 0x0600358C RID: 13708
	[DllImport("luau")]
	public unsafe static extern void lua_close(lua_State* L);

	// Token: 0x0600358D RID: 13709
	[DllImport("luau")]
	public unsafe static extern void* lua_touserdatatagged(lua_State* L, int idx, int tag);

	// Token: 0x0600358E RID: 13710
	[DllImport("luau")]
	public unsafe static extern void* lua_touserdata(lua_State* L, int index);

	// Token: 0x0600358F RID: 13711
	[DllImport("luau")]
	public unsafe static extern void* lua_newuserdatatagged(lua_State* L, int sz, int tag);

	// Token: 0x06003590 RID: 13712
	[DllImport("luau")]
	public unsafe static extern void lua_getuserdatametatable(lua_State* L, int tag);

	// Token: 0x06003591 RID: 13713
	[DllImport("luau")]
	public unsafe static extern void lua_setuserdatametatable(lua_State* L, int tag, int idx);

	// Token: 0x06003592 RID: 13714
	[DllImport("luau")]
	public unsafe static extern int lua_setmetatable(lua_State* L, int objindex);

	// Token: 0x06003593 RID: 13715
	[DllImport("luau")]
	public unsafe static extern int luaL_newmetatable(lua_State* L, [MarshalAs(UnmanagedType.LPStr)] string tname);

	// Token: 0x06003594 RID: 13716
	[DllImport("luau")]
	public unsafe static extern int lua_getfield(lua_State* L, int idx, [MarshalAs(UnmanagedType.LPStr)] string k);

	// Token: 0x06003595 RID: 13717
	[DllImport("luau")]
	public unsafe static extern int lua_getfield(lua_State* L, int idx, byte* k);

	// Token: 0x06003596 RID: 13718
	[DllImport("luau")]
	public unsafe static extern int luaL_getmetafield(lua_State* L, int idx, byte* k);

	// Token: 0x06003597 RID: 13719
	[DllImport("luau")]
	public unsafe static extern int luaL_getmetafield(lua_State* L, int idx, [MarshalAs(UnmanagedType.LPStr)] string k);

	// Token: 0x06003598 RID: 13720 RVA: 0x00103C4F File Offset: 0x00101E4F
	public unsafe static void luaL_getmetatable(lua_State* L, string n)
	{
		Luau.lua_getfield(L, -10000, n);
	}

	// Token: 0x06003599 RID: 13721 RVA: 0x00103C5E File Offset: 0x00101E5E
	public unsafe static void luaL_getmetatable(lua_State* L, byte* n)
	{
		Luau.lua_getfield(L, -10000, n);
	}

	// Token: 0x0600359A RID: 13722 RVA: 0x00103C6D File Offset: 0x00101E6D
	public unsafe static void lua_getglobal(lua_State* L, string n)
	{
		Luau.lua_getfield(L, -10002, n);
	}

	// Token: 0x0600359B RID: 13723
	[DllImport("luau")]
	public unsafe static extern int lua_getmetatable(lua_State* L, int objindex);

	// Token: 0x0600359C RID: 13724
	[DllImport("luau")]
	public unsafe static extern byte* lua_namecallatom(lua_State* L, int* atom);

	// Token: 0x0600359D RID: 13725
	[DllImport("luau")]
	public unsafe static extern byte* luaL_checklstring(lua_State* L, int numArg, int* l);

	// Token: 0x0600359E RID: 13726 RVA: 0x00103C7C File Offset: 0x00101E7C
	public unsafe static byte* luaL_checkstring(lua_State* L, int n)
	{
		return Luau.luaL_checklstring(L, n, null);
	}

	// Token: 0x0600359F RID: 13727
	[DllImport("luau")]
	public unsafe static extern void lua_pushnumber(lua_State* L, double n);

	// Token: 0x060035A0 RID: 13728
	[DllImport("luau")]
	public unsafe static extern double luaL_checknumber(lua_State* L, int numArg);

	// Token: 0x060035A1 RID: 13729
	[DllImport("luau")]
	public unsafe static extern void lua_setreadonly(lua_State* L, int idx, int enabled);

	// Token: 0x060035A2 RID: 13730
	[DllImport("luau")]
	public unsafe static extern double lua_tonumberx(lua_State* L, int index, int* isnum);

	// Token: 0x060035A3 RID: 13731
	[DllImport("luau")]
	public unsafe static extern int lua_gc(lua_State* L, int what, int data);

	// Token: 0x060035A4 RID: 13732
	[DllImport("luau")]
	public unsafe static extern void lua_call(lua_State* L, int nargs, int nresults);

	// Token: 0x060035A5 RID: 13733
	[DllImport("luau")]
	public unsafe static extern int lua_pcall(lua_State* L, int nargs, int nresults, int fn);

	// Token: 0x060035A6 RID: 13734
	[DllImport("luau")]
	public unsafe static extern int lua_status(lua_State* L);

	// Token: 0x060035A7 RID: 13735
	[DllImport("luau")]
	public unsafe static extern void* luaL_checkudata(lua_State* L, int arg, [MarshalAs(UnmanagedType.LPStr)] string tname);

	// Token: 0x060035A8 RID: 13736
	[DllImport("luau")]
	public unsafe static extern void* luaL_checkudata(lua_State* L, int arg, byte* tname);

	// Token: 0x060035A9 RID: 13737
	[DllImport("luau")]
	public unsafe static extern int lua_objlen(lua_State* L, int index);

	// Token: 0x060035AA RID: 13738
	[DllImport("luau")]
	public unsafe static extern double luaL_optnumber(lua_State* L, int narg, double d);

	// Token: 0x060035AB RID: 13739
	[DllImport("luau")]
	public unsafe static extern void lua_createtable(lua_State* L, int narr, int nrec);

	// Token: 0x060035AC RID: 13740
	[DllImport("luau")]
	public unsafe static extern void lua_pushlightuserdatatagged(lua_State* L, void* p, int tag);

	// Token: 0x060035AD RID: 13741
	[DllImport("luau")]
	public unsafe static extern void lua_pushnil(lua_State* L);

	// Token: 0x060035AE RID: 13742
	[DllImport("luau")]
	public unsafe static extern int lua_next(lua_State* L, int index);

	// Token: 0x060035AF RID: 13743 RVA: 0x00103C87 File Offset: 0x00101E87
	public unsafe static void lua_pushlightuserdata(lua_State* L, void* p)
	{
		Luau.lua_pushlightuserdatatagged(L, p, 0);
	}

	// Token: 0x060035B0 RID: 13744
	[DllImport("luau")]
	public unsafe static extern void lua_rawseti(lua_State* L, int idx, int n);

	// Token: 0x060035B1 RID: 13745
	[DllImport("luau")]
	public unsafe static extern void lua_rawgeti(lua_State* L, int index, int n);

	// Token: 0x060035B2 RID: 13746
	[DllImport("luau")]
	public unsafe static extern void lua_rawget(lua_State* L, int index);

	// Token: 0x060035B3 RID: 13747
	[DllImport("luau")]
	public unsafe static extern void lua_remove(lua_State* L, int index);

	// Token: 0x060035B4 RID: 13748
	[DllImport("luau")]
	public unsafe static extern void lua_pushboolean(lua_State* L, int b);

	// Token: 0x060035B5 RID: 13749
	[DllImport("luau")]
	public unsafe static extern int lua_rawequal(lua_State* L, int a, int b);

	// Token: 0x060035B6 RID: 13750 RVA: 0x00103C91 File Offset: 0x00101E91
	public unsafe static void* lua_newuserdata(lua_State* L, int size)
	{
		return Luau.lua_newuserdatatagged(L, size, 0);
	}

	// Token: 0x060035B7 RID: 13751 RVA: 0x00103C9B File Offset: 0x00101E9B
	public unsafe static double lua_tonumber(lua_State* L, int index)
	{
		return Luau.lua_tonumberx(L, index, null);
	}

	// Token: 0x060035B8 RID: 13752 RVA: 0x00103CA8 File Offset: 0x00101EA8
	public unsafe static T* lua_class_push<[IsUnmanaged] T>(lua_State* L) where T : struct, ValueType
	{
		T* ptr = (T*)Luau.lua_newuserdata(L, sizeof(T));
		FixedString32Bytes name = BurstClassInfo.ClassList.MetatableNames<T>.Name;
		Luau.luaL_getmetatable(L, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2);
		Luau.lua_setmetatable(L, -2);
		return ptr;
	}

	// Token: 0x060035B9 RID: 13753 RVA: 0x00103CE4 File Offset: 0x00101EE4
	public unsafe static T* lua_class_push<[IsUnmanaged] T>(lua_State* L, FixedString32Bytes name) where T : struct, ValueType
	{
		T* ptr = (T*)Luau.lua_newuserdata(L, sizeof(T));
		Luau.luaL_getmetatable(L, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2);
		Luau.lua_setmetatable(L, -2);
		return ptr;
	}

	// Token: 0x060035BA RID: 13754 RVA: 0x00103D18 File Offset: 0x00101F18
	public unsafe static void lua_class_push(lua_State* L, FixedString32Bytes name, IntPtr ptr)
	{
		FixedString32Bytes fixedString32Bytes = "__ptr";
		Luau.lua_createtable(L, 0, 0);
		Luau.lua_pushlightuserdata(L, (void*)ptr);
		Luau.lua_setfield(L, -2, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2);
		Luau.luaL_getmetatable(L, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2);
		Luau.lua_setmetatable(L, -2);
	}

	// Token: 0x060035BB RID: 13755 RVA: 0x00103D70 File Offset: 0x00101F70
	public unsafe static void lua_class_push<[IsUnmanaged] T>(lua_State* L, T* ptr) where T : struct, ValueType
	{
		FixedString32Bytes fixedString32Bytes = "__ptr";
		FixedString32Bytes name = BurstClassInfo.ClassList.MetatableNames<T>.Name;
		Luau.lua_createtable(L, 0, 0);
		Luau.lua_pushlightuserdata(L, (void*)ptr);
		Luau.lua_setfield(L, -2, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2);
		Luau.luaL_getmetatable(L, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2);
		Luau.lua_setmetatable(L, -2);
	}

	// Token: 0x060035BC RID: 13756 RVA: 0x00103DC8 File Offset: 0x00101FC8
	public unsafe static T* lua_class_get<[IsUnmanaged] T>(lua_State* L, int idx) where T : struct, ValueType
	{
		int num = Luau.lua_type(L, idx);
		FixedString32Bytes name = BurstClassInfo.ClassList.MetatableNames<T>.Name;
		byte* ptr = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2;
		if (num == 8)
		{
			T* ptr2 = (T*)Luau.luaL_checkudata(L, idx, ptr);
			if (ptr2 != null)
			{
				return ptr2;
			}
		}
		if (num == 6)
		{
			Luau.lua_getmetatable(L, idx);
			Luau.luaL_getmetatable(L, ptr);
			bool flag = Luau.lua_rawequal(L, -1, -2) == 1;
			Luau.lua_pop(L, 2);
			if (flag)
			{
				Luau.lua_getfield(L, idx, "__ptr");
				if (Luau.lua_type(L, -1) == 2)
				{
					T* ptr3 = (T*)Luau.lua_touserdata(L, -1);
					Luau.lua_pop(L, 1);
					if (ptr3 != null)
					{
						return ptr3;
					}
				}
				Luau.lua_pop(L, 1);
			}
		}
		FixedString32Bytes fixedString32Bytes = "\"Invalid Type\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
		return null;
	}

	// Token: 0x060035BD RID: 13757 RVA: 0x00103E80 File Offset: 0x00102080
	public unsafe static T* lua_class_get<[IsUnmanaged] T>(lua_State* L, int idx, FixedString32Bytes name) where T : struct, ValueType
	{
		int num = Luau.lua_type(L, idx);
		byte* ptr = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2;
		if (num == 8)
		{
			T* ptr2 = (T*)Luau.luaL_checkudata(L, idx, ptr);
			if (ptr2 != null)
			{
				return ptr2;
			}
		}
		if (num == 6)
		{
			Luau.lua_getmetatable(L, idx);
			Luau.luaL_getmetatable(L, ptr);
			bool flag = Luau.lua_rawequal(L, -1, -2) == 1;
			Luau.lua_pop(L, 1);
			if (flag)
			{
				FixedString32Bytes fixedString32Bytes = "__ptr";
				Luau.lua_getfield(L, idx, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2);
				if (Luau.lua_type(L, -1) == 2)
				{
					T* ptr3 = (T*)Luau.lua_touserdata(L, -1);
					Luau.lua_pop(L, 1);
					if (ptr3 != null)
					{
						return ptr3;
					}
				}
				Luau.lua_pop(L, 1);
			}
		}
		FixedString32Bytes fixedString32Bytes2 = "\"Invalid Type\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
		return null;
	}

	// Token: 0x060035BE RID: 13758 RVA: 0x00103F40 File Offset: 0x00102140
	public unsafe static byte* lua_class_get(lua_State* L, int idx, FixedString32Bytes name)
	{
		int num = Luau.lua_type(L, idx);
		byte* ptr = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref name) + 2;
		if (num == 8)
		{
			byte* ptr2 = (byte*)Luau.luaL_checkudata(L, idx, ptr);
			if (ptr2 != null)
			{
				return ptr2;
			}
		}
		if (num == 6)
		{
			Luau.lua_getmetatable(L, idx);
			Luau.luaL_getmetatable(L, ptr);
			bool flag = Luau.lua_rawequal(L, -1, -2) == 1;
			Luau.lua_pop(L, 1);
			if (flag)
			{
				FixedString32Bytes fixedString32Bytes = "__ptr";
				Luau.lua_getfield(L, idx, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2);
				if (Luau.lua_type(L, -1) == 2)
				{
					byte* ptr3 = (byte*)Luau.lua_touserdata(L, -1);
					Luau.lua_pop(L, 1);
					if (ptr3 != null)
					{
						return ptr3;
					}
				}
				Luau.lua_pop(L, 1);
			}
		}
		FixedString32Bytes fixedString32Bytes2 = "\"Invalid Type\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
		return null;
	}

	// Token: 0x060035BF RID: 13759 RVA: 0x00104000 File Offset: 0x00102200
	public unsafe static IntPtr lua_light_ptr(lua_State* L, int idx)
	{
		FixedString32Bytes fixedString32Bytes = "__ptr";
		Luau.lua_getfield(L, idx, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2);
		if (Luau.lua_type(L, -1) == 2)
		{
			void* ptr = Luau.lua_touserdata(L, -1);
			Luau.lua_pop(L, 1);
			if (ptr != null)
			{
				return (IntPtr)ptr;
			}
		}
		return IntPtr.Zero;
	}

	// Token: 0x060035C0 RID: 13760 RVA: 0x00104053 File Offset: 0x00102253
	public unsafe static bool lua_class_check<[IsUnmanaged] T>(lua_State* L, int idx) where T : struct, ValueType
	{
		return Luau.lua_objlen(L, idx) == sizeof(T);
	}

	// Token: 0x060035C1 RID: 13761 RVA: 0x00104064 File Offset: 0x00102264
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int lua_print(lua_State* L)
	{
		string text = "";
		int num = Luau.lua_gettop(L);
		for (int i = 1; i <= num; i++)
		{
			int num2 = Luau.lua_type(L, i);
			if (num2 == 5 || num2 == 3)
			{
				sbyte* ptr = Luau.lua_tostring(L, i);
				text += Marshal.PtrToStringAnsi((IntPtr)((void*)ptr));
			}
			else
			{
				if (num2 != 1)
				{
					Luau.luaL_errorL(L, "Invalid String", Array.Empty<string>());
					return 0;
				}
				int num3 = Luau.lua_toboolean(L, i);
				text += ((num3 == 1) ? "true" : "false");
			}
		}
		LuauHud.Instance.LuauLog(text);
		return 0;
	}

	// Token: 0x04003B68 RID: 15208
	public const int LUA_GLOBALSINDEX = -10002;

	// Token: 0x04003B69 RID: 15209
	public const int LUA_REGISTRYINDEX = -10000;

	// Token: 0x020008A9 RID: 2217
	public enum lua_Types
	{
		// Token: 0x04003B6B RID: 15211
		LUA_TNIL,
		// Token: 0x04003B6C RID: 15212
		LUA_TBOOLEAN,
		// Token: 0x04003B6D RID: 15213
		LUA_TLIGHTUSERDATA,
		// Token: 0x04003B6E RID: 15214
		LUA_TNUMBER,
		// Token: 0x04003B6F RID: 15215
		LUA_TVECTOR,
		// Token: 0x04003B70 RID: 15216
		LUA_TSTRING,
		// Token: 0x04003B71 RID: 15217
		LUA_TTABLE,
		// Token: 0x04003B72 RID: 15218
		LUA_TFUNCTION,
		// Token: 0x04003B73 RID: 15219
		LUA_TUSERDATA,
		// Token: 0x04003B74 RID: 15220
		LUA_TTHREAD,
		// Token: 0x04003B75 RID: 15221
		LUA_TBUFFER,
		// Token: 0x04003B76 RID: 15222
		LUA_TPROTO,
		// Token: 0x04003B77 RID: 15223
		LUA_TUPVAL,
		// Token: 0x04003B78 RID: 15224
		LUA_TDEADKEY,
		// Token: 0x04003B79 RID: 15225
		LUA_T_COUNT = 11
	}

	// Token: 0x020008AA RID: 2218
	public enum lua_Status
	{
		// Token: 0x04003B7B RID: 15227
		LUA_OK,
		// Token: 0x04003B7C RID: 15228
		LUA_YIELD,
		// Token: 0x04003B7D RID: 15229
		LUA_ERRRUN,
		// Token: 0x04003B7E RID: 15230
		LUA_ERRSYNTAX,
		// Token: 0x04003B7F RID: 15231
		LUA_ERRMEM,
		// Token: 0x04003B80 RID: 15232
		LUA_ERRERR,
		// Token: 0x04003B81 RID: 15233
		LUA_BREAK
	}

	// Token: 0x020008AB RID: 2219
	public enum gc_status
	{
		// Token: 0x04003B83 RID: 15235
		LUA_GCSTOP,
		// Token: 0x04003B84 RID: 15236
		LUA_GCRESTART,
		// Token: 0x04003B85 RID: 15237
		LUA_GCCOLLECT,
		// Token: 0x04003B86 RID: 15238
		LUA_GCCOUNT,
		// Token: 0x04003B87 RID: 15239
		LUA_GCISRUNNING,
		// Token: 0x04003B88 RID: 15240
		LUA_GCSTEP,
		// Token: 0x04003B89 RID: 15241
		LUA_GCSETGOAL,
		// Token: 0x04003B8A RID: 15242
		LUA_GCSETSTEPMUL,
		// Token: 0x04003B8B RID: 15243
		LUA_GCSETSTEPSIZE
	}

	// Token: 0x020008AC RID: 2220
	public static class lua_TypeID
	{
		// Token: 0x060035C3 RID: 13763 RVA: 0x00104100 File Offset: 0x00102300
		public static string get(Type t)
		{
			string text;
			if (Luau.lua_TypeID.names.TryGetValue(t, out text))
			{
				return text;
			}
			return "";
		}

		// Token: 0x060035C4 RID: 13764 RVA: 0x00104123 File Offset: 0x00102323
		public static void push(Type t, string name)
		{
			Luau.lua_TypeID.names.TryAdd(t, name);
		}

		// Token: 0x04003B8C RID: 15244
		private static Dictionary<Type, string> names = new Dictionary<Type, string>();
	}

	// Token: 0x020008AD RID: 2221
	public static class lua_ClassFields<T>
	{
		// Token: 0x060035C6 RID: 13766 RVA: 0x00104140 File Offset: 0x00102340
		public static FieldInfo Get(string name)
		{
			Dictionary<int, FieldInfo> dictionary;
			FieldInfo fieldInfo;
			if (Luau.lua_ClassFields<T>.classDictionarys.TryGetValue(typeof(T).GetHashCode(), out dictionary) && dictionary.TryGetValue(name.GetHashCode(), out fieldInfo))
			{
				return fieldInfo;
			}
			return null;
		}

		// Token: 0x060035C7 RID: 13767 RVA: 0x00104180 File Offset: 0x00102380
		public static void Add(string name, FieldInfo field)
		{
			Dictionary<int, FieldInfo> dictionary;
			if (Luau.lua_ClassFields<T>.classDictionarys.TryGetValue(typeof(T).GetHashCode(), out dictionary))
			{
				dictionary.TryAdd(name.GetHashCode(), field);
				return;
			}
			Dictionary<int, FieldInfo> dictionary2 = new Dictionary<int, FieldInfo>();
			dictionary2.TryAdd(name.GetHashCode(), field);
			Luau.lua_ClassFields<T>.classDictionarys.TryAdd(typeof(T).GetHashCode(), dictionary2);
		}

		// Token: 0x04003B8D RID: 15245
		private static Dictionary<int, Dictionary<int, FieldInfo>> classDictionarys = new Dictionary<int, Dictionary<int, FieldInfo>>();
	}

	// Token: 0x020008AE RID: 2222
	public static class lua_ClassProperties<T>
	{
		// Token: 0x060035C9 RID: 13769 RVA: 0x001041F4 File Offset: 0x001023F4
		public static lua_CFunction Get(string name)
		{
			Dictionary<string, lua_CFunction> dictionary;
			lua_CFunction lua_CFunction;
			if (Luau.lua_ClassProperties<T>.classProperties.TryGetValue(typeof(T), out dictionary) && dictionary.TryGetValue(name, out lua_CFunction))
			{
				return lua_CFunction;
			}
			return null;
		}

		// Token: 0x060035CA RID: 13770 RVA: 0x00104228 File Offset: 0x00102428
		public static void Add(string name, lua_CFunction field)
		{
			Dictionary<string, lua_CFunction> dictionary;
			if (Luau.lua_ClassProperties<T>.classProperties.TryGetValue(typeof(T), out dictionary))
			{
				dictionary.TryAdd(name, field);
				return;
			}
			Dictionary<string, lua_CFunction> dictionary2 = new Dictionary<string, lua_CFunction>();
			dictionary2.TryAdd(name, field);
			Luau.lua_ClassProperties<T>.classProperties.TryAdd(typeof(T), dictionary2);
		}

		// Token: 0x04003B8E RID: 15246
		private static Dictionary<Type, Dictionary<string, lua_CFunction>> classProperties = new Dictionary<Type, Dictionary<string, lua_CFunction>>();
	}

	// Token: 0x020008AF RID: 2223
	public static class lua_ClassFunctions<T>
	{
		// Token: 0x060035CC RID: 13772 RVA: 0x00104288 File Offset: 0x00102488
		public static lua_CFunction Get(string name)
		{
			Dictionary<string, lua_CFunction> dictionary;
			lua_CFunction lua_CFunction;
			if (Luau.lua_ClassFunctions<T>.classProperties.TryGetValue(typeof(T), out dictionary) && dictionary.TryGetValue(name, out lua_CFunction))
			{
				return lua_CFunction;
			}
			return null;
		}

		// Token: 0x060035CD RID: 13773 RVA: 0x001042BC File Offset: 0x001024BC
		public static void Add(string name, lua_CFunction field)
		{
			Dictionary<string, lua_CFunction> dictionary;
			if (Luau.lua_ClassFunctions<T>.classProperties.TryGetValue(typeof(T), out dictionary))
			{
				dictionary.TryAdd(name, field);
				return;
			}
			Dictionary<string, lua_CFunction> dictionary2 = new Dictionary<string, lua_CFunction>();
			dictionary2.TryAdd(name, field);
			Luau.lua_ClassFunctions<T>.classProperties.TryAdd(typeof(T), dictionary2);
		}

		// Token: 0x04003B8F RID: 15247
		private static Dictionary<Type, Dictionary<string, lua_CFunction>> classProperties = new Dictionary<Type, Dictionary<string, lua_CFunction>>();
	}
}
