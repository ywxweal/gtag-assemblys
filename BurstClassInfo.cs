﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x02000895 RID: 2197
[BurstCompile]
public static class BurstClassInfo
{
	// Token: 0x06003532 RID: 13618 RVA: 0x00102D84 File Offset: 0x00100F84
	public unsafe static void NewClass<T>(string className, Dictionary<int, FieldInfo> fieldList, Dictionary<int, lua_CFunction> functionList, Dictionary<int, FunctionPointer<lua_CFunction>> functionPtrList)
	{
		if (!BurstClassInfo.ClassList.InfoFields.Data.IsCreated)
		{
			*BurstClassInfo.ClassList.InfoFields.Data = new NativeHashMap<int, BurstClassInfo.ClassInfo>(20, Allocator.Persistent);
		}
		BurstClassInfo.ClassList.MetatableNames<T>.Name = className;
		ReflectionMetaNames.ReflectedNames.TryAdd(typeof(T), className);
		BurstClassInfo.ClassInfo classInfo = default(BurstClassInfo.ClassInfo);
		classInfo.NameHash = LuaHashing.ByteHash(className);
		if (className.Length > 30)
		{
			throw new Exception("Name to long");
		}
		classInfo.Name = className;
		classInfo.FieldList = new NativeHashMap<int, BurstClassInfo.BurstFieldInfo>(fieldList.Count, Allocator.Persistent);
		foreach (KeyValuePair<int, FieldInfo> keyValuePair in fieldList)
		{
			BurstClassInfo.BurstFieldInfo burstFieldInfo = default(BurstClassInfo.BurstFieldInfo);
			burstFieldInfo.NameHash = keyValuePair.Key;
			burstFieldInfo.Name = keyValuePair.Value.Name;
			burstFieldInfo.Offset = (int)Marshal.OffsetOf<T>(keyValuePair.Value.Name);
			Type fieldType = keyValuePair.Value.FieldType;
			if (fieldType == typeof(float))
			{
				burstFieldInfo.FieldType = BurstClassInfo.EFieldTypes.Float;
			}
			else if (fieldType == typeof(int))
			{
				burstFieldInfo.FieldType = BurstClassInfo.EFieldTypes.Int;
			}
			else if (fieldType == typeof(double))
			{
				burstFieldInfo.FieldType = BurstClassInfo.EFieldTypes.Double;
			}
			else if (fieldType == typeof(bool))
			{
				burstFieldInfo.FieldType = BurstClassInfo.EFieldTypes.Bool;
			}
			else if (fieldType == typeof(FixedString32Bytes))
			{
				burstFieldInfo.FieldType = BurstClassInfo.EFieldTypes.String;
			}
			else if (!fieldType.IsPrimitive)
			{
				burstFieldInfo.FieldType = BurstClassInfo.EFieldTypes.LightUserData;
				ReflectionMetaNames.ReflectedNames.TryGetValue(fieldType, out burstFieldInfo.MetatableName);
			}
			burstFieldInfo.Size = Marshal.SizeOf(fieldType);
			classInfo.FieldList.TryAdd(keyValuePair.Key, burstFieldInfo);
		}
		classInfo.FunctionList = new NativeHashMap<int, IntPtr>(functionList.Count + functionPtrList.Count, Allocator.Persistent);
		foreach (KeyValuePair<int, lua_CFunction> keyValuePair2 in functionList)
		{
			classInfo.FunctionList.TryAdd(keyValuePair2.Key, Marshal.GetFunctionPointerForDelegate<lua_CFunction>(keyValuePair2.Value));
		}
		foreach (KeyValuePair<int, FunctionPointer<lua_CFunction>> keyValuePair3 in functionPtrList)
		{
			classInfo.FunctionList.TryAdd(keyValuePair3.Key, keyValuePair3.Value.Value);
		}
		BurstClassInfo.ClassList.InfoFields.Data.Add(classInfo.NameHash, classInfo);
	}

	// Token: 0x06003533 RID: 13619 RVA: 0x001030B8 File Offset: 0x001012B8
	[BurstCompile]
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int Index(lua_State* L)
	{
		return BurstClassInfo.Index_00003420$BurstDirectCall.Invoke(L);
	}

	// Token: 0x06003534 RID: 13620 RVA: 0x001030C0 File Offset: 0x001012C0
	[BurstCompile]
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int NewIndex(lua_State* L)
	{
		return BurstClassInfo.NewIndex_00003421$BurstDirectCall.Invoke(L);
	}

	// Token: 0x06003535 RID: 13621 RVA: 0x001030C8 File Offset: 0x001012C8
	[BurstCompile]
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int NameCall(lua_State* L)
	{
		return BurstClassInfo.NameCall_00003422$BurstDirectCall.Invoke(L);
	}

	// Token: 0x06003537 RID: 13623 RVA: 0x001030E4 File Offset: 0x001012E4
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static int Index$BurstManaged(lua_State* L)
	{
		FixedString32Bytes k_metatableLookup = BurstClassInfo._k_metatableLookup;
		byte* ptr = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref k_metatableLookup) + 2;
		Luau.luaL_getmetafield(L, 1, ptr);
		BurstClassInfo.ClassInfo classInfo;
		if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
		{
			FixedString32Bytes fixedString32Bytes = "\"Internal Class Info Error\"";
			Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
			return 0;
		}
		Luau.lua_pop(L, 1);
		byte* ptr2 = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref classInfo.Name) + 2;
		IntPtr intPtr = IntPtr.Zero;
		Luau.lua_Types lua_Types = (Luau.lua_Types)Luau.lua_type(L, 1);
		if (lua_Types == Luau.lua_Types.LUA_TUSERDATA)
		{
			intPtr = (IntPtr)Luau.luaL_checkudata(L, 1, ptr2);
		}
		else
		{
			if (lua_Types != Luau.lua_Types.LUA_TTABLE)
			{
				FixedString32Bytes fixedString32Bytes2 = "\"Unknown type for __index\"";
				Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
				return 0;
			}
			intPtr = Luau.lua_light_ptr(L, 1);
		}
		int num = Luau.lua_objlen(L, 2);
		int num2 = LuaHashing.ByteHash(Luau.luaL_checkstring(L, 2), num);
		BurstClassInfo.BurstFieldInfo burstFieldInfo;
		if (classInfo.FieldList.TryGetValue(num2, out burstFieldInfo))
		{
			IntPtr intPtr2 = intPtr + burstFieldInfo.Offset;
			switch (burstFieldInfo.FieldType)
			{
			case BurstClassInfo.EFieldTypes.Float:
				Luau.lua_pushnumber(L, (double)(*(float*)(void*)intPtr2));
				return 1;
			case BurstClassInfo.EFieldTypes.Int:
				Luau.lua_pushnumber(L, (double)(*(int*)(void*)intPtr2));
				return 1;
			case BurstClassInfo.EFieldTypes.Double:
				Luau.lua_pushnumber(L, *(double*)(void*)intPtr2);
				return 1;
			case BurstClassInfo.EFieldTypes.Bool:
				Luau.lua_pushboolean(L, (*(byte*)(void*)intPtr2 != 0) ? 1 : 0);
				return 1;
			case BurstClassInfo.EFieldTypes.String:
				Luau.lua_pushstring(L, (byte*)(void*)intPtr2 + 2);
				return 1;
			case BurstClassInfo.EFieldTypes.LightUserData:
				Luau.lua_class_push(L, burstFieldInfo.MetatableName, intPtr2);
				return 1;
			}
		}
		IntPtr intPtr3;
		if (classInfo.FunctionList.TryGetValue(num2, out intPtr3))
		{
			FunctionPointer<lua_CFunction> functionPointer = new FunctionPointer<lua_CFunction>(intPtr3);
			FixedString32Bytes fixedString32Bytes3 = "";
			Luau.lua_pushcclosurek(L, functionPointer, (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes3) + 2, 0, null);
			return 1;
		}
		FixedString32Bytes fixedString32Bytes4 = "\"Unknown Type?\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes4) + 2));
		return 0;
	}

	// Token: 0x06003538 RID: 13624 RVA: 0x001032D8 File Offset: 0x001014D8
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static int NewIndex$BurstManaged(lua_State* L)
	{
		FixedString32Bytes k_metatableLookup = BurstClassInfo._k_metatableLookup;
		byte* ptr = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref k_metatableLookup) + 2;
		Luau.luaL_getmetafield(L, 1, ptr);
		BurstClassInfo.ClassInfo classInfo;
		if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
		{
			FixedString32Bytes fixedString32Bytes = "\"Internal Class Info Error\"";
			Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
			return 0;
		}
		Luau.lua_pop(L, 1);
		byte* ptr2 = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref classInfo.Name) + 2;
		IntPtr intPtr = IntPtr.Zero;
		Luau.lua_Types lua_Types = (Luau.lua_Types)Luau.lua_type(L, 1);
		if (lua_Types == Luau.lua_Types.LUA_TUSERDATA)
		{
			intPtr = (IntPtr)Luau.luaL_checkudata(L, 1, ptr2);
		}
		else
		{
			if (lua_Types != Luau.lua_Types.LUA_TTABLE)
			{
				FixedString32Bytes fixedString32Bytes2 = "\"Unknown type for __newindex\"";
				Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
				return 0;
			}
			intPtr = Luau.lua_light_ptr(L, 1);
		}
		int num = Luau.lua_objlen(L, 2);
		int num2 = LuaHashing.ByteHash(Luau.luaL_checkstring(L, 2), num);
		BurstClassInfo.BurstFieldInfo burstFieldInfo;
		if (classInfo.FieldList.TryGetValue(num2, out burstFieldInfo))
		{
			IntPtr intPtr2 = intPtr + burstFieldInfo.Offset;
			switch (burstFieldInfo.FieldType)
			{
			case BurstClassInfo.EFieldTypes.Float:
				*(float*)(void*)intPtr2 = (float)Luau.luaL_checknumber(L, 3);
				return 0;
			case BurstClassInfo.EFieldTypes.Int:
				*(int*)(void*)intPtr2 = (int)Luau.luaL_checknumber(L, 3);
				return 0;
			case BurstClassInfo.EFieldTypes.Double:
				*(double*)(void*)intPtr2 = Luau.luaL_checknumber(L, 3);
				return 0;
			case BurstClassInfo.EFieldTypes.Bool:
				*(byte*)(void*)intPtr2 = ((Luau.lua_toboolean(L, 3) != 0) ? 1 : 0);
				return 0;
			case BurstClassInfo.EFieldTypes.LightUserData:
				Buffer.MemoryCopy((void*)((IntPtr)((void*)Luau.lua_class_get(L, 3, burstFieldInfo.MetatableName))), (void*)intPtr2, (long)burstFieldInfo.Size, (long)burstFieldInfo.Size);
				return 0;
			}
		}
		FixedString32Bytes fixedString32Bytes3 = "\"Unknown Type\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes3) + 2));
		return 0;
	}

	// Token: 0x06003539 RID: 13625 RVA: 0x001034A8 File Offset: 0x001016A8
	[BurstCompile]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static int NameCall$BurstManaged(lua_State* L)
	{
		FixedString32Bytes k_metatableLookup = BurstClassInfo._k_metatableLookup;
		byte* ptr = (byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref k_metatableLookup) + 2;
		Luau.luaL_getmetafield(L, 1, ptr);
		BurstClassInfo.ClassInfo classInfo;
		if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
		{
			FixedString32Bytes fixedString32Bytes = "\"Internal Class Info Error\"";
			Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes) + 2));
			return 0;
		}
		Luau.lua_pop(L, 1);
		int num = LuaHashing.ByteHash(Luau.lua_namecallatom(L, null));
		IntPtr intPtr;
		if (classInfo.FunctionList.TryGetValue(num, out intPtr))
		{
			FunctionPointer<lua_CFunction> functionPointer = new FunctionPointer<lua_CFunction>(intPtr);
			return functionPointer.Invoke(L);
		}
		FixedString32Bytes fixedString32Bytes2 = "\"Function not found in function list\"";
		Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
		return 0;
	}

	// Token: 0x04003B44 RID: 15172
	private static readonly FixedString32Bytes _k_metatableLookup = "metahash";

	// Token: 0x02000896 RID: 2198
	public enum EFieldTypes
	{
		// Token: 0x04003B46 RID: 15174
		Float,
		// Token: 0x04003B47 RID: 15175
		Int,
		// Token: 0x04003B48 RID: 15176
		Double,
		// Token: 0x04003B49 RID: 15177
		Bool,
		// Token: 0x04003B4A RID: 15178
		String,
		// Token: 0x04003B4B RID: 15179
		LightUserData
	}

	// Token: 0x02000897 RID: 2199
	[BurstCompile]
	public struct BurstFieldInfo
	{
		// Token: 0x04003B4C RID: 15180
		public int NameHash;

		// Token: 0x04003B4D RID: 15181
		public FixedString32Bytes Name;

		// Token: 0x04003B4E RID: 15182
		public FixedString32Bytes MetatableName;

		// Token: 0x04003B4F RID: 15183
		public int Offset;

		// Token: 0x04003B50 RID: 15184
		public BurstClassInfo.EFieldTypes FieldType;

		// Token: 0x04003B51 RID: 15185
		public int Size;
	}

	// Token: 0x02000898 RID: 2200
	[BurstCompile]
	public struct ClassInfo
	{
		// Token: 0x04003B52 RID: 15186
		public int NameHash;

		// Token: 0x04003B53 RID: 15187
		public FixedString32Bytes Name;

		// Token: 0x04003B54 RID: 15188
		public NativeHashMap<int, BurstClassInfo.BurstFieldInfo> FieldList;

		// Token: 0x04003B55 RID: 15189
		public NativeHashMap<int, IntPtr> FunctionList;
	}

	// Token: 0x02000899 RID: 2201
	public abstract class ClassList
	{
		// Token: 0x04003B56 RID: 15190
		public static readonly SharedStatic<NativeHashMap<int, BurstClassInfo.ClassInfo>> InfoFields = SharedStatic<NativeHashMap<int, BurstClassInfo.ClassInfo>>.GetOrCreateUnsafe(0U, -7258312696341931442L, -7445903157129162016L);

		// Token: 0x0200089A RID: 2202
		private class FieldKey
		{
		}

		// Token: 0x0200089B RID: 2203
		public static class MetatableNames<T>
		{
			// Token: 0x04003B57 RID: 15191
			public static FixedString32Bytes Name;
		}
	}

	// Token: 0x0200089C RID: 2204
	// (Invoke) Token: 0x0600353E RID: 13630
	public unsafe delegate int Index_00003420$PostfixBurstDelegate(lua_State* L);

	// Token: 0x0200089D RID: 2205
	internal static class Index_00003420$BurstDirectCall
	{
		// Token: 0x06003541 RID: 13633 RVA: 0x0010357E File Offset: 0x0010177E
		[BurstDiscard]
		private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
		{
			if (BurstClassInfo.Index_00003420$BurstDirectCall.Pointer == 0)
			{
				BurstClassInfo.Index_00003420$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(BurstClassInfo.Index_00003420$BurstDirectCall.DeferredCompilation, methodof(BurstClassInfo.Index$BurstManaged(lua_State*)).MethodHandle, typeof(BurstClassInfo.Index_00003420$PostfixBurstDelegate).TypeHandle);
			}
			A_0 = BurstClassInfo.Index_00003420$BurstDirectCall.Pointer;
		}

		// Token: 0x06003542 RID: 13634 RVA: 0x001035AC File Offset: 0x001017AC
		private static IntPtr GetFunctionPointer()
		{
			IntPtr intPtr = (IntPtr)0;
			BurstClassInfo.Index_00003420$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
			return intPtr;
		}

		// Token: 0x06003543 RID: 13635 RVA: 0x001035C4 File Offset: 0x001017C4
		public unsafe static void Constructor()
		{
			BurstClassInfo.Index_00003420$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(BurstClassInfo.Index(lua_State*)).MethodHandle);
		}

		// Token: 0x06003544 RID: 13636 RVA: 0x000023F4 File Offset: 0x000005F4
		public static void Initialize()
		{
		}

		// Token: 0x06003545 RID: 13637 RVA: 0x001035D5 File Offset: 0x001017D5
		// Note: this type is marked as 'beforefieldinit'.
		static Index_00003420$BurstDirectCall()
		{
			BurstClassInfo.Index_00003420$BurstDirectCall.Constructor();
		}

		// Token: 0x06003546 RID: 13638 RVA: 0x001035DC File Offset: 0x001017DC
		public unsafe static int Invoke(lua_State* L)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = BurstClassInfo.Index_00003420$BurstDirectCall.GetFunctionPointer();
				if (functionPointer != 0)
				{
					return calli(System.Int32(lua_State*), L, functionPointer);
				}
			}
			return BurstClassInfo.Index$BurstManaged(L);
		}

		// Token: 0x04003B58 RID: 15192
		private static IntPtr Pointer;

		// Token: 0x04003B59 RID: 15193
		private static IntPtr DeferredCompilation;
	}

	// Token: 0x0200089E RID: 2206
	// (Invoke) Token: 0x06003548 RID: 13640
	public unsafe delegate int NewIndex_00003421$PostfixBurstDelegate(lua_State* L);

	// Token: 0x0200089F RID: 2207
	internal static class NewIndex_00003421$BurstDirectCall
	{
		// Token: 0x0600354B RID: 13643 RVA: 0x0010360D File Offset: 0x0010180D
		[BurstDiscard]
		private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
		{
			if (BurstClassInfo.NewIndex_00003421$BurstDirectCall.Pointer == 0)
			{
				BurstClassInfo.NewIndex_00003421$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(BurstClassInfo.NewIndex_00003421$BurstDirectCall.DeferredCompilation, methodof(BurstClassInfo.NewIndex$BurstManaged(lua_State*)).MethodHandle, typeof(BurstClassInfo.NewIndex_00003421$PostfixBurstDelegate).TypeHandle);
			}
			A_0 = BurstClassInfo.NewIndex_00003421$BurstDirectCall.Pointer;
		}

		// Token: 0x0600354C RID: 13644 RVA: 0x0010363C File Offset: 0x0010183C
		private static IntPtr GetFunctionPointer()
		{
			IntPtr intPtr = (IntPtr)0;
			BurstClassInfo.NewIndex_00003421$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
			return intPtr;
		}

		// Token: 0x0600354D RID: 13645 RVA: 0x00103654 File Offset: 0x00101854
		public unsafe static void Constructor()
		{
			BurstClassInfo.NewIndex_00003421$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(BurstClassInfo.NewIndex(lua_State*)).MethodHandle);
		}

		// Token: 0x0600354E RID: 13646 RVA: 0x000023F4 File Offset: 0x000005F4
		public static void Initialize()
		{
		}

		// Token: 0x0600354F RID: 13647 RVA: 0x00103665 File Offset: 0x00101865
		// Note: this type is marked as 'beforefieldinit'.
		static NewIndex_00003421$BurstDirectCall()
		{
			BurstClassInfo.NewIndex_00003421$BurstDirectCall.Constructor();
		}

		// Token: 0x06003550 RID: 13648 RVA: 0x0010366C File Offset: 0x0010186C
		public unsafe static int Invoke(lua_State* L)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = BurstClassInfo.NewIndex_00003421$BurstDirectCall.GetFunctionPointer();
				if (functionPointer != 0)
				{
					return calli(System.Int32(lua_State*), L, functionPointer);
				}
			}
			return BurstClassInfo.NewIndex$BurstManaged(L);
		}

		// Token: 0x04003B5A RID: 15194
		private static IntPtr Pointer;

		// Token: 0x04003B5B RID: 15195
		private static IntPtr DeferredCompilation;
	}

	// Token: 0x020008A0 RID: 2208
	// (Invoke) Token: 0x06003552 RID: 13650
	public unsafe delegate int NameCall_00003422$PostfixBurstDelegate(lua_State* L);

	// Token: 0x020008A1 RID: 2209
	internal static class NameCall_00003422$BurstDirectCall
	{
		// Token: 0x06003555 RID: 13653 RVA: 0x0010369D File Offset: 0x0010189D
		[BurstDiscard]
		private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
		{
			if (BurstClassInfo.NameCall_00003422$BurstDirectCall.Pointer == 0)
			{
				BurstClassInfo.NameCall_00003422$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(BurstClassInfo.NameCall_00003422$BurstDirectCall.DeferredCompilation, methodof(BurstClassInfo.NameCall$BurstManaged(lua_State*)).MethodHandle, typeof(BurstClassInfo.NameCall_00003422$PostfixBurstDelegate).TypeHandle);
			}
			A_0 = BurstClassInfo.NameCall_00003422$BurstDirectCall.Pointer;
		}

		// Token: 0x06003556 RID: 13654 RVA: 0x001036CC File Offset: 0x001018CC
		private static IntPtr GetFunctionPointer()
		{
			IntPtr intPtr = (IntPtr)0;
			BurstClassInfo.NameCall_00003422$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
			return intPtr;
		}

		// Token: 0x06003557 RID: 13655 RVA: 0x001036E4 File Offset: 0x001018E4
		public unsafe static void Constructor()
		{
			BurstClassInfo.NameCall_00003422$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(BurstClassInfo.NameCall(lua_State*)).MethodHandle);
		}

		// Token: 0x06003558 RID: 13656 RVA: 0x000023F4 File Offset: 0x000005F4
		public static void Initialize()
		{
		}

		// Token: 0x06003559 RID: 13657 RVA: 0x001036F5 File Offset: 0x001018F5
		// Note: this type is marked as 'beforefieldinit'.
		static NameCall_00003422$BurstDirectCall()
		{
			BurstClassInfo.NameCall_00003422$BurstDirectCall.Constructor();
		}

		// Token: 0x0600355A RID: 13658 RVA: 0x001036FC File Offset: 0x001018FC
		public unsafe static int Invoke(lua_State* L)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = BurstClassInfo.NameCall_00003422$BurstDirectCall.GetFunctionPointer();
				if (functionPointer != 0)
				{
					return calli(System.Int32(lua_State*), L, functionPointer);
				}
			}
			return BurstClassInfo.NameCall$BurstManaged(L);
		}

		// Token: 0x04003B5C RID: 15196
		private static IntPtr Pointer;

		// Token: 0x04003B5D RID: 15197
		private static IntPtr DeferredCompilation;
	}
}
