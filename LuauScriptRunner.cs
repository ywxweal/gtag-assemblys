using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x020008B3 RID: 2227
public class LuauScriptRunner
{
	// Token: 0x060035DE RID: 13790 RVA: 0x00104C74 File Offset: 0x00102E74
	public unsafe static bool ErrorCheck(lua_State* L, int status)
	{
		if (status == 2)
		{
			sbyte* ptr = Luau.lua_tostring(L, -1);
			LuauHud.Instance.LuauLog(new string(ptr));
			sbyte* ptr2 = (sbyte*)Luau.lua_debugtrace(L);
			LuauHud.Instance.LuauLog(new string(ptr2));
			Luau.lua_close(L);
			return true;
		}
		return false;
	}

	// Token: 0x060035DF RID: 13791 RVA: 0x00104CC0 File Offset: 0x00102EC0
	public bool Tick(float deltaTime)
	{
		if (!this.ShouldTick)
		{
			return false;
		}
		Luau.lua_getfield(this.L, -10002, "tick");
		if (Luau.lua_type(this.L, -1) == 7)
		{
			this.preTickCallback(this.L);
			Luau.lua_pushnumber(this.L, (double)deltaTime);
			int num = Luau.lua_pcall(this.L, 1, 0, 0);
			this.ShouldTick = !LuauScriptRunner.ErrorCheck(this.L, num);
			if (this.ShouldTick)
			{
				this.postTickCallback(this.L);
			}
			return this.ShouldTick;
		}
		Luau.lua_pop(this.L, 1);
		return false;
	}

	// Token: 0x060035E0 RID: 13792 RVA: 0x00104D70 File Offset: 0x00102F70
	public unsafe LuauScriptRunner(string script, string name, [CanBeNull] lua_CFunction bindings = null, [CanBeNull] lua_CFunction preTick = null, [CanBeNull] lua_CFunction postTick = null)
	{
		this.Script = script;
		this.ScriptName = name;
		this.L = Luau.luaL_newstate();
		LuauScriptRunner.ScriptRunners.Add(this);
		Luau.luaL_openlibs(this.L);
		Bindings.Vec3Builder(this.L);
		Bindings.QuatBuilder(this.L);
		if (bindings != null)
		{
			bindings(this.L);
		}
		this.postTickCallback = postTick;
		this.preTickCallback = preTick;
		UIntPtr uintPtr = (UIntPtr)((IntPtr)0);
		Luau.lua_register(this.L, new lua_CFunction(Luau.lua_print), "print");
		byte[] bytes = Encoding.UTF8.GetBytes(script);
		sbyte* ptr = Luau.luau_compile(script, (UIntPtr)((IntPtr)bytes.Length), null, &uintPtr);
		Luau.luau_load(this.L, name, ptr, uintPtr, 0);
		int num = Luau.lua_resume(this.L, null, 0);
		this.ShouldTick = !LuauScriptRunner.ErrorCheck(this.L, num);
	}

	// Token: 0x060035E1 RID: 13793 RVA: 0x00104E57 File Offset: 0x00103057
	public LuauScriptRunner FromFile(string filePath, [CanBeNull] lua_CFunction bindings = null, [CanBeNull] lua_CFunction tick = null)
	{
		return new LuauScriptRunner(File.ReadAllText(Path.Join(Application.persistentDataPath, "Scripts", filePath)), filePath, bindings, tick, null);
	}

	// Token: 0x060035E2 RID: 13794 RVA: 0x00104E88 File Offset: 0x00103088
	~LuauScriptRunner()
	{
		LuauVm.ClassBuilders.Clear();
		Bindings.LuauPlayerList.Clear();
		Bindings.LuauGameObjectList.Clear();
		Bindings.LuauVRRigList.Clear();
		ReflectionMetaNames.ReflectedNames.Clear();
		if (BurstClassInfo.ClassList.InfoFields.Data.IsCreated)
		{
			BurstClassInfo.ClassList.InfoFields.Data.Clear();
		}
	}

	// Token: 0x04003BA2 RID: 15266
	public static List<LuauScriptRunner> ScriptRunners = new List<LuauScriptRunner>();

	// Token: 0x04003BA3 RID: 15267
	public bool ShouldTick;

	// Token: 0x04003BA4 RID: 15268
	private lua_CFunction postTickCallback;

	// Token: 0x04003BA5 RID: 15269
	private lua_CFunction preTickCallback;

	// Token: 0x04003BA6 RID: 15270
	public string ScriptName;

	// Token: 0x04003BA7 RID: 15271
	public string Script;

	// Token: 0x04003BA8 RID: 15272
	public unsafe lua_State* L;
}
