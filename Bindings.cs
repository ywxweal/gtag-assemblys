﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

// Token: 0x02000855 RID: 2133
[BurstCompile]
public static class Bindings
{
	// Token: 0x060033E9 RID: 13289 RVA: 0x001007D0 File Offset: 0x000FE9D0
	public unsafe static void GameObjectBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.LuauGameObject>("GameObject").AddField("position", "Position").AddField("rotation", "Rotation").AddField("scale", "Scale")
			.AddStaticFunction("findGameObject", new lua_CFunction(Bindings.GameObjectFunctions.FindGameObject))
			.AddFunction("setCollision", new lua_CFunction(Bindings.GameObjectFunctions.SetCollision))
			.AddFunction("setVisibility", new lua_CFunction(Bindings.GameObjectFunctions.SetVisibility))
			.AddFunction("setActive", new lua_CFunction(Bindings.GameObjectFunctions.SetActive))
			.AddFunction("setText", new lua_CFunction(Bindings.GameObjectFunctions.SetText))
			.Build(L, true));
	}

	// Token: 0x060033EA RID: 13290 RVA: 0x00100894 File Offset: 0x000FEA94
	public unsafe static void GorillaLocomotionSettingsBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.GorillaLocomotionSettings>("PSettings").AddField("velocityLimit", null).AddField("slideVelocityLimit", null).AddField("maxJumpSpeed", null)
			.AddField("jumpMultiplier", null)
			.Build(L, false));
		Bindings.LocomotionSettings = Luau.lua_class_push<Bindings.GorillaLocomotionSettings>(L);
		Bindings.LocomotionSettings->velocityLimit = GTPlayer.Instance.velocityLimit;
		Bindings.LocomotionSettings->slideVelocityLimit = GTPlayer.Instance.slideVelocityLimit;
		Bindings.LocomotionSettings->maxJumpSpeed = 6.5f;
		Bindings.LocomotionSettings->jumpMultiplier = 1.1f;
		Luau.lua_setglobal(L, "PlayerSettings");
	}

	// Token: 0x060033EB RID: 13291 RVA: 0x00100948 File Offset: 0x000FEB48
	public unsafe static void Vec3Builder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Vector3>("Vec3").AddField("x", null).AddField("y", null).AddField("z", null)
			.AddStaticFunction("new", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.New)))
			.AddFunction("__add", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Add)))
			.AddFunction("__sub", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Sub)))
			.AddFunction("__mul", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Mul)))
			.AddFunction("__div", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Div)))
			.AddFunction("__unm", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Unm)))
			.AddFunction("__eq", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Eq)))
			.AddFunction("__tostring", new lua_CFunction(Bindings.Vec3Functions.ToSring))
			.AddFunction("toString", new lua_CFunction(Bindings.Vec3Functions.ToSring))
			.AddFunction("dot", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Dot)))
			.AddFunction("cross", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Cross)))
			.AddFunction("projectOnTo", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Project)))
			.AddFunction("length", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Length)))
			.AddFunction("normalize", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Normalize)))
			.AddFunction("getSafeNormal", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.SafeNormal)))
			.AddStaticFunction("rotate", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Rotate)))
			.AddFunction("rotate", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Rotate)))
			.AddStaticFunction("distance", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Distance)))
			.AddFunction("distance", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Distance)))
			.AddStaticFunction("lerp", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Lerp)))
			.AddFunction("lerp", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.Lerp)))
			.AddProperty("zeroVector", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.ZeroVector)))
			.AddProperty("oneVector", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.Vec3Functions.OneVector)))
			.Build(L, true));
	}

	// Token: 0x060033EC RID: 13292 RVA: 0x00100BF8 File Offset: 0x000FEDF8
	public unsafe static void QuatBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Quaternion>("Quat").AddField("x", null).AddField("y", null).AddField("z", null)
			.AddField("w", null)
			.AddStaticFunction("new", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.New)))
			.AddFunction("__mul", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.Mul)))
			.AddFunction("__eq", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.Eq)))
			.AddFunction("__tostring", new lua_CFunction(Bindings.QuatFunctions.ToString))
			.AddFunction("toString", new lua_CFunction(Bindings.QuatFunctions.ToString))
			.AddStaticFunction("fromEuler", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.FromEuler)))
			.AddStaticFunction("fromDirection", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.FromDirection)))
			.AddFunction("getUpVector", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.GetUpVector)))
			.AddFunction("euler", BurstCompiler.CompileFunctionPointer<lua_CFunction>(new lua_CFunction(Bindings.QuatFunctions.Euler)))
			.Build(L, true));
	}

	// Token: 0x060033ED RID: 13293 RVA: 0x00100D38 File Offset: 0x000FEF38
	public unsafe static void PlayerBuilder(lua_State* L)
	{
		LuauVm.ClassBuilders.Append(new LuauClassBuilder<Bindings.LuauPlayer>("Player").AddField("playerID", "PlayerID").AddField("playerName", "PlayerName").AddField("playerMaterial", "PlayerMaterial")
			.AddField("isMasterClient", "IsMasterClient")
			.AddField("bodyPosition", "BodyPosition")
			.AddField("leftHandPosition", "LeftHandPosition")
			.AddField("rightHandPosition", "RightHandPosition")
			.AddField("headRotation", "HeadRotation")
			.AddField("leftHandRotation", "LeftHandRotation")
			.AddField("rightHandRotation", "RightHandRotation")
			.AddStaticFunction("getPlayerByID", new lua_CFunction(Bindings.PlayerFunctions.GetPlayerByID))
			.Build(L, true));
	}

	// Token: 0x060033EE RID: 13294 RVA: 0x00100E10 File Offset: 0x000FF010
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int LuaStartVibration(lua_State* L)
	{
		bool flag = Luau.lua_toboolean(L, 1) == 1;
		float num = (float)Luau.luaL_checknumber(L, 2);
		float num2 = (float)Luau.luaL_checknumber(L, 3);
		GorillaTagger.Instance.StartVibration(flag, num, num2);
		return 0;
	}

	// Token: 0x060033EF RID: 13295 RVA: 0x00100E48 File Offset: 0x000FF048
	[MonoPInvokeCallback(typeof(lua_CFunction))]
	public unsafe static int LuaPlaySound(lua_State* L)
	{
		int num = (int)Luau.luaL_checknumber(L, 1);
		Vector3 vector = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
		float num2 = (float)Luau.luaL_checknumber(L, 3);
		if (num < 0 || num >= VRRig.LocalRig.clipToPlay.Length)
		{
			return 0;
		}
		AudioSource.PlayClipAtPoint(VRRig.LocalRig.clipToPlay[num], vector, num2);
		return 0;
	}

	// Token: 0x04003AF6 RID: 15094
	public static Dictionary<GameObject, IntPtr> LuauGameObjectList = new Dictionary<GameObject, IntPtr>();

	// Token: 0x04003AF7 RID: 15095
	public static Dictionary<int, IntPtr> LuauPlayerList = new Dictionary<int, IntPtr>();

	// Token: 0x04003AF8 RID: 15096
	public static Dictionary<int, VRRig> LuauVRRigList = new Dictionary<int, VRRig>();

	// Token: 0x04003AF9 RID: 15097
	public unsafe static Bindings.GorillaLocomotionSettings* LocomotionSettings;

	// Token: 0x02000856 RID: 2134
	public static class LuaEmit
	{
		// Token: 0x060033F1 RID: 13297 RVA: 0x00100EC8 File Offset: 0x000FF0C8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Emit(lua_State* L)
		{
			if (Bindings.LuaEmit.callTime < Time.time - 1f)
			{
				Bindings.LuaEmit.callTime = Time.time - 1f;
			}
			Bindings.LuaEmit.callTime += 1f / Bindings.LuaEmit.callCount;
			if (Bindings.LuaEmit.callTime > Time.time)
			{
				LuauHud.Instance.LuauLog("Emit rate limit reached, event not sent");
				return 0;
			}
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions
			{
				Receivers = ReceiverGroup.Others
			};
			if (Luau.lua_type(L, 2) != 6)
			{
				Luau.luaL_errorL(L, "Argument 2 must be a table", Array.Empty<string>());
				return 0;
			}
			Luau.lua_pushnil(L);
			int num = 0;
			List<object> list = new List<object>();
			list.Add(Marshal.PtrToStringAnsi((IntPtr)((void*)Luau.luaL_checkstring(L, 1))));
			while (Luau.lua_next(L, 2) != 0 && num++ < 10)
			{
				Luau.lua_Types lua_Types = (Luau.lua_Types)Luau.lua_type(L, -1);
				if (lua_Types <= Luau.lua_Types.LUA_TNUMBER)
				{
					if (lua_Types == Luau.lua_Types.LUA_TBOOLEAN)
					{
						list.Add(Luau.lua_toboolean(L, -1) == 1);
						Luau.lua_pop(L, 1);
						continue;
					}
					if (lua_Types == Luau.lua_Types.LUA_TNUMBER)
					{
						list.Add(Luau.luaL_checknumber(L, -1));
						Luau.lua_pop(L, 1);
						continue;
					}
				}
				else if (lua_Types == Luau.lua_Types.LUA_TTABLE || lua_Types == Luau.lua_Types.LUA_TUSERDATA)
				{
					Luau.luaL_getmetafield(L, -1, "metahash");
					BurstClassInfo.ClassInfo classInfo;
					if (!BurstClassInfo.ClassList.InfoFields.Data.TryGetValue((int)Luau.luaL_checknumber(L, -1), out classInfo))
					{
						FixedString64Bytes fixedString64Bytes = "\"Internal Class Info Error No Metatable Found\"";
						Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString64Bytes>(ref fixedString64Bytes) + 2));
						return 0;
					}
					Luau.lua_pop(L, 1);
					FixedString32Bytes fixedString32Bytes = "Vec3";
					if ((in classInfo.Name) == (in fixedString32Bytes))
					{
						list.Add(*Luau.lua_class_get<Vector3>(L, -1));
						Luau.lua_pop(L, 1);
						continue;
					}
					fixedString32Bytes = "Quat";
					if ((in classInfo.Name) == (in fixedString32Bytes))
					{
						list.Add(*Luau.lua_class_get<Quaternion>(L, -1));
						Luau.lua_pop(L, 1);
						continue;
					}
					fixedString32Bytes = "Player";
					if ((in classInfo.Name) == (in fixedString32Bytes))
					{
						int playerID = Luau.lua_class_get<Bindings.LuauPlayer>(L, -1)->PlayerID;
						NetPlayer netPlayer = null;
						foreach (NetPlayer netPlayer2 in RoomSystem.PlayersInRoom)
						{
							if (netPlayer2.ActorNumber == playerID)
							{
								netPlayer = netPlayer2;
							}
						}
						if (netPlayer == null)
						{
							list.Add(null);
						}
						else
						{
							list.Add(netPlayer.GetPlayerRef());
						}
						Luau.lua_pop(L, 1);
						continue;
					}
					FixedString32Bytes fixedString32Bytes2 = "\"Unknown Type in table\"";
					Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes2) + 2));
					continue;
				}
				FixedString32Bytes fixedString32Bytes3 = "\"Unknown Type in table\"";
				Luau.luaL_errorL(L, (sbyte*)((byte*)UnsafeUtility.AddressOf<FixedString32Bytes>(ref fixedString32Bytes3) + 2));
				return 0;
			}
			if (PhotonNetwork.InRoom)
			{
				PhotonNetwork.RaiseEvent(180, list.ToArray(), raiseEventOptions, SendOptions.SendReliable);
			}
			return 0;
		}

		// Token: 0x04003AFA RID: 15098
		private static float callTime = 0f;

		// Token: 0x04003AFB RID: 15099
		private static float callCount = 20f;
	}

	// Token: 0x02000857 RID: 2135
	[BurstCompile]
	public struct LuauGameObject
	{
		// Token: 0x04003AFC RID: 15100
		public Vector3 Position;

		// Token: 0x04003AFD RID: 15101
		public Quaternion Rotation;

		// Token: 0x04003AFE RID: 15102
		public Vector3 Scale;
	}

	// Token: 0x02000858 RID: 2136
	[BurstCompile]
	public static class GameObjectFunctions
	{
		// Token: 0x060033F3 RID: 13299 RVA: 0x001011D4 File Offset: 0x000FF3D4
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int New(lua_State* L)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			Bindings.LuauGameObject* ptr = Luau.lua_class_push<Bindings.LuauGameObject>(L);
			ptr->Position = gameObject.transform.position;
			ptr->Rotation = gameObject.transform.rotation;
			ptr->Scale = gameObject.transform.localScale;
			Bindings.LuauGameObjectList.TryAdd(gameObject, (IntPtr)((void*)ptr));
			return 1;
		}

		// Token: 0x060033F4 RID: 13300 RVA: 0x00101238 File Offset: 0x000FF438
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FindGameObject(lua_State* L)
		{
			GameObject gameObject = GameObject.Find(new string((sbyte*)Luau.luaL_checkstring(L, 1)));
			if (!(gameObject != null))
			{
				return 0;
			}
			if (!CustomMapLoader.IsCustomScene(gameObject.scene.name))
			{
				return 0;
			}
			IntPtr intPtr;
			if (Bindings.LuauGameObjectList.TryGetValue(gameObject, out intPtr))
			{
				Luau.lua_class_push(L, "GameObject", intPtr);
			}
			else
			{
				Bindings.LuauGameObject* ptr = Luau.lua_class_push<Bindings.LuauGameObject>(L);
				ptr->Position = gameObject.transform.position;
				ptr->Rotation = gameObject.transform.rotation;
				ptr->Scale = gameObject.transform.localScale;
				Bindings.LuauGameObjectList.TryAdd(gameObject, (IntPtr)((void*)ptr));
			}
			return 1;
		}

		// Token: 0x060033F5 RID: 13301 RVA: 0x001012EC File Offset: 0x000FF4EC
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetCollision(lua_State* L)
		{
			Bindings.LuauGameObject* data = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			Bindings.LuauGameObjectList.FirstOrDefault((KeyValuePair<GameObject, IntPtr> g) => g.Value == (IntPtr)((void*)data)).Key.GetComponent<Collider>().enabled = Luau.lua_toboolean(L, 2) == 1;
			return 0;
		}

		// Token: 0x060033F6 RID: 13302 RVA: 0x0010134C File Offset: 0x000FF54C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetVisibility(lua_State* L)
		{
			Bindings.LuauGameObject* data = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			Bindings.LuauGameObjectList.FirstOrDefault((KeyValuePair<GameObject, IntPtr> g) => g.Value == (IntPtr)((void*)data)).Key.GetComponent<MeshRenderer>().enabled = Luau.lua_toboolean(L, 2) == 1;
			return 0;
		}

		// Token: 0x060033F7 RID: 13303 RVA: 0x001013AC File Offset: 0x000FF5AC
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetActive(lua_State* L)
		{
			Bindings.LuauGameObject* data = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			Bindings.LuauGameObjectList.FirstOrDefault((KeyValuePair<GameObject, IntPtr> g) => g.Value == (IntPtr)((void*)data)).Key.SetActive(Luau.lua_toboolean(L, 2) == 1);
			return 0;
		}

		// Token: 0x060033F8 RID: 13304 RVA: 0x00101404 File Offset: 0x000FF604
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SetText(lua_State* L)
		{
			Bindings.LuauGameObject* data = Luau.lua_class_get<Bindings.LuauGameObject>(L, 1, "GameObject");
			GameObject key = Bindings.LuauGameObjectList.FirstOrDefault((KeyValuePair<GameObject, IntPtr> g) => g.Value == (IntPtr)((void*)data)).Key;
			string text = new string(Luau.lua_tostring(L, 2));
			TextMeshPro component = key.GetComponent<TextMeshPro>();
			if (component.IsNotNull())
			{
				component.text = text;
			}
			else
			{
				TextMesh component2 = key.GetComponent<TextMesh>();
				if (component2.IsNotNull())
				{
					component2.text = text;
				}
			}
			return 0;
		}
	}

	// Token: 0x0200085D RID: 2141
	[BurstCompile]
	public struct LuauPlayer
	{
		// Token: 0x04003B03 RID: 15107
		public int PlayerID;

		// Token: 0x04003B04 RID: 15108
		public FixedString32Bytes PlayerName;

		// Token: 0x04003B05 RID: 15109
		public int PlayerMaterial;

		// Token: 0x04003B06 RID: 15110
		public bool IsMasterClient;

		// Token: 0x04003B07 RID: 15111
		public Vector3 BodyPosition;

		// Token: 0x04003B08 RID: 15112
		public Vector3 LeftHandPosition;

		// Token: 0x04003B09 RID: 15113
		public Vector3 RightHandPosition;

		// Token: 0x04003B0A RID: 15114
		public Quaternion HeadRotation;

		// Token: 0x04003B0B RID: 15115
		public Quaternion LeftHandRotation;

		// Token: 0x04003B0C RID: 15116
		public Quaternion RightHandRotation;
	}

	// Token: 0x0200085E RID: 2142
	[BurstCompile]
	public static class PlayerFunctions
	{
		// Token: 0x06003401 RID: 13313 RVA: 0x001014F4 File Offset: 0x000FF6F4
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetPlayerByID(lua_State* L)
		{
			int num = (int)Luau.luaL_checknumber(L, 1);
			foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
			{
				if (netPlayer.ActorNumber == num)
				{
					IntPtr intPtr;
					if (Bindings.LuauPlayerList.TryGetValue(netPlayer.ActorNumber, out intPtr))
					{
						Luau.lua_class_push(L, "Player", intPtr);
					}
					else
					{
						Bindings.LuauPlayer* ptr = Luau.lua_class_push<Bindings.LuauPlayer>(L);
						ptr->PlayerID = netPlayer.ActorNumber;
						ptr->PlayerMaterial = 0;
						ptr->IsMasterClient = netPlayer.IsMasterClient;
						Bindings.LuauPlayerList[netPlayer.ActorNumber] = (IntPtr)((void*)ptr);
						GorillaGameManager instance = GorillaGameManager.instance;
						VRRig vrrig = ((instance != null) ? instance.FindPlayerVRRig(netPlayer) : null);
						if (vrrig != null)
						{
							ptr->PlayerName = vrrig.playerNameVisible;
							Bindings.LuauVRRigList[netPlayer.ActorNumber] = vrrig;
							Bindings.PlayerFunctions.UpdatePlayer(L, vrrig, ptr);
							Bindings.LuauPlayerList[netPlayer.ActorNumber] = (IntPtr)((void*)ptr);
						}
					}
				}
			}
			return 0;
		}

		// Token: 0x06003402 RID: 13314 RVA: 0x0010162C File Offset: 0x000FF82C
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static void UpdatePlayer(lua_State* L, VRRig p, Bindings.LuauPlayer* data)
		{
			data->BodyPosition = p.transform.position;
			data->LeftHandPosition = p.leftHandTransform.position;
			data->RightHandPosition = p.rightHandTransform.position;
			data->HeadRotation = p.transform.rotation;
			data->LeftHandRotation = p.leftHandTransform.rotation;
			data->RightHandRotation = p.rightHandTransform.rotation;
		}
	}

	// Token: 0x0200085F RID: 2143
	[BurstCompile]
	public static class Vec3Functions
	{
		// Token: 0x06003403 RID: 13315 RVA: 0x0010169F File Offset: 0x000FF89F
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int New(lua_State* L)
		{
			return Bindings.Vec3Functions.New_00003403$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003404 RID: 13316 RVA: 0x001016A7 File Offset: 0x000FF8A7
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Add(lua_State* L)
		{
			return Bindings.Vec3Functions.Add_00003404$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003405 RID: 13317 RVA: 0x001016AF File Offset: 0x000FF8AF
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Sub(lua_State* L)
		{
			return Bindings.Vec3Functions.Sub_00003405$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003406 RID: 13318 RVA: 0x001016B7 File Offset: 0x000FF8B7
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Mul(lua_State* L)
		{
			return Bindings.Vec3Functions.Mul_00003406$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003407 RID: 13319 RVA: 0x001016BF File Offset: 0x000FF8BF
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Div(lua_State* L)
		{
			return Bindings.Vec3Functions.Div_00003407$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003408 RID: 13320 RVA: 0x001016C7 File Offset: 0x000FF8C7
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Unm(lua_State* L)
		{
			return Bindings.Vec3Functions.Unm_00003408$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003409 RID: 13321 RVA: 0x001016CF File Offset: 0x000FF8CF
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Eq(lua_State* L)
		{
			return Bindings.Vec3Functions.Eq_00003409$BurstDirectCall.Invoke(L);
		}

		// Token: 0x0600340A RID: 13322 RVA: 0x001016D8 File Offset: 0x000FF8D8
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ToSring(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Luau.lua_pushstring(L, vector.ToString());
			return 1;
		}

		// Token: 0x0600340B RID: 13323 RVA: 0x00101711 File Offset: 0x000FF911
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Dot(lua_State* L)
		{
			return Bindings.Vec3Functions.Dot_0000340B$BurstDirectCall.Invoke(L);
		}

		// Token: 0x0600340C RID: 13324 RVA: 0x00101719 File Offset: 0x000FF919
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Cross(lua_State* L)
		{
			return Bindings.Vec3Functions.Cross_0000340C$BurstDirectCall.Invoke(L);
		}

		// Token: 0x0600340D RID: 13325 RVA: 0x00101721 File Offset: 0x000FF921
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Project(lua_State* L)
		{
			return Bindings.Vec3Functions.Project_0000340D$BurstDirectCall.Invoke(L);
		}

		// Token: 0x0600340E RID: 13326 RVA: 0x00101729 File Offset: 0x000FF929
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Length(lua_State* L)
		{
			return Bindings.Vec3Functions.Length_0000340E$BurstDirectCall.Invoke(L);
		}

		// Token: 0x0600340F RID: 13327 RVA: 0x00101731 File Offset: 0x000FF931
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Normalize(lua_State* L)
		{
			return Bindings.Vec3Functions.Normalize_0000340F$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003410 RID: 13328 RVA: 0x00101739 File Offset: 0x000FF939
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int SafeNormal(lua_State* L)
		{
			return Bindings.Vec3Functions.SafeNormal_00003410$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003411 RID: 13329 RVA: 0x00101741 File Offset: 0x000FF941
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Distance(lua_State* L)
		{
			return Bindings.Vec3Functions.Distance_00003411$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003412 RID: 13330 RVA: 0x00101749 File Offset: 0x000FF949
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Lerp(lua_State* L)
		{
			return Bindings.Vec3Functions.Lerp_00003412$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003413 RID: 13331 RVA: 0x00101751 File Offset: 0x000FF951
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Rotate(lua_State* L)
		{
			return Bindings.Vec3Functions.Rotate_00003413$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003414 RID: 13332 RVA: 0x00101759 File Offset: 0x000FF959
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ZeroVector(lua_State* L)
		{
			return Bindings.Vec3Functions.ZeroVector_00003414$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003415 RID: 13333 RVA: 0x00101761 File Offset: 0x000FF961
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int OneVector(lua_State* L)
		{
			return Bindings.Vec3Functions.OneVector_00003415$BurstDirectCall.Invoke(L);
		}

		// Token: 0x06003416 RID: 13334 RVA: 0x0010176C File Offset: 0x000FF96C
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int New$BurstManaged(lua_State* L)
		{
			Vector3* ptr = Luau.lua_class_push<Vector3>(L, "Vec3");
			ptr->x = (float)Luau.luaL_optnumber(L, 1, 0.0);
			ptr->y = (float)Luau.luaL_optnumber(L, 2, 0.0);
			ptr->z = (float)Luau.luaL_optnumber(L, 3, 0.0);
			return 1;
		}

		// Token: 0x06003417 RID: 13335 RVA: 0x001017D0 File Offset: 0x000FF9D0
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Add$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = vector + vector2;
			return 1;
		}

		// Token: 0x06003418 RID: 13336 RVA: 0x00101828 File Offset: 0x000FFA28
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Sub$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = vector - vector2;
			return 1;
		}

		// Token: 0x06003419 RID: 13337 RVA: 0x00101880 File Offset: 0x000FFA80
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Mul$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			float num = (float)Luau.luaL_checknumber(L, 2);
			*Luau.lua_class_push<Vector3>(L, "Vec3") = vector * num;
			return 1;
		}

		// Token: 0x0600341A RID: 13338 RVA: 0x001018CC File Offset: 0x000FFACC
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Div$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			float num = (float)Luau.luaL_checknumber(L, 2);
			*Luau.lua_class_push<Vector3>(L, "Vec3") = vector / num;
			return 1;
		}

		// Token: 0x0600341B RID: 13339 RVA: 0x00101918 File Offset: 0x000FFB18
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Unm$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = -vector;
			return 1;
		}

		// Token: 0x0600341C RID: 13340 RVA: 0x00101958 File Offset: 0x000FFB58
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Eq$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			int num = ((vector == vector2) ? 1 : 0);
			Luau.lua_pushnumber(L, (double)num);
			return 1;
		}

		// Token: 0x0600341D RID: 13341 RVA: 0x001019A8 File Offset: 0x000FFBA8
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Dot$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			double num = (double)Vector3.Dot(vector, vector2);
			Luau.lua_pushnumber(L, num);
			return 1;
		}

		// Token: 0x0600341E RID: 13342 RVA: 0x001019F4 File Offset: 0x000FFBF4
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Cross$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Vector3.Cross(vector, vector2);
			return 1;
		}

		// Token: 0x0600341F RID: 13343 RVA: 0x00101A4C File Offset: 0x000FFC4C
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Project$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Vector3.Project(vector, vector2);
			return 1;
		}

		// Token: 0x06003420 RID: 13344 RVA: 0x00101AA4 File Offset: 0x000FFCA4
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Length$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Luau.lua_pushnumber(L, (double)Vector3.Magnitude(vector));
			return 1;
		}

		// Token: 0x06003421 RID: 13345 RVA: 0x00101AD6 File Offset: 0x000FFCD6
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Normalize$BurstManaged(lua_State* L)
		{
			Luau.lua_class_get<Vector3>(L, 1, "Vec3")->Normalize();
			return 0;
		}

		// Token: 0x06003422 RID: 13346 RVA: 0x00101AF0 File Offset: 0x000FFCF0
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int SafeNormal$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = vector.normalized;
			return 1;
		}

		// Token: 0x06003423 RID: 13347 RVA: 0x00101B34 File Offset: 0x000FFD34
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Distance$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			Luau.lua_pushnumber(L, (double)Vector3.Distance(vector, vector2));
			return 1;
		}

		// Token: 0x06003424 RID: 13348 RVA: 0x00101B80 File Offset: 0x000FFD80
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Lerp$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Vector3 vector2 = *Luau.lua_class_get<Vector3>(L, 2, "Vec3");
			double num = Luau.luaL_checknumber(L, 3);
			*Luau.lua_class_push<Vector3>(L, "Vec3") = Vector3.Lerp(vector, vector2, (float)num);
			return 1;
		}

		// Token: 0x06003425 RID: 13349 RVA: 0x00101BE4 File Offset: 0x000FFDE4
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Rotate$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 2, "Quat");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = quaternion * vector;
			return 1;
		}

		// Token: 0x06003426 RID: 13350 RVA: 0x00101C3C File Offset: 0x000FFE3C
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int ZeroVector$BurstManaged(lua_State* L)
		{
			Vector3* ptr = Luau.lua_class_push<Vector3>(L, "Vec3");
			ptr->x = 0f;
			ptr->y = 0f;
			ptr->z = 0f;
			return 1;
		}

		// Token: 0x06003427 RID: 13351 RVA: 0x00101C6F File Offset: 0x000FFE6F
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int OneVector$BurstManaged(lua_State* L)
		{
			Vector3* ptr = Luau.lua_class_push<Vector3>(L, "Vec3");
			ptr->x = 1f;
			ptr->y = 1f;
			ptr->z = 1f;
			return 1;
		}

		// Token: 0x02000860 RID: 2144
		// (Invoke) Token: 0x06003429 RID: 13353
		public unsafe delegate int New_00003403$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000861 RID: 2145
		internal static class New_00003403$BurstDirectCall
		{
			// Token: 0x0600342C RID: 13356 RVA: 0x00101CA2 File Offset: 0x000FFEA2
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.New_00003403$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.New_00003403$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.New_00003403$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.New$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.New_00003403$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.New_00003403$BurstDirectCall.Pointer;
			}

			// Token: 0x0600342D RID: 13357 RVA: 0x00101CD0 File Offset: 0x000FFED0
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.New_00003403$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x0600342E RID: 13358 RVA: 0x00101CE8 File Offset: 0x000FFEE8
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.New_00003403$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.New(lua_State*)).MethodHandle);
			}

			// Token: 0x0600342F RID: 13359 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x06003430 RID: 13360 RVA: 0x00101CF9 File Offset: 0x000FFEF9
			// Note: this type is marked as 'beforefieldinit'.
			static New_00003403$BurstDirectCall()
			{
				Bindings.Vec3Functions.New_00003403$BurstDirectCall.Constructor();
			}

			// Token: 0x06003431 RID: 13361 RVA: 0x00101D00 File Offset: 0x000FFF00
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.New_00003403$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.New$BurstManaged(L);
			}

			// Token: 0x04003B0D RID: 15117
			private static IntPtr Pointer;

			// Token: 0x04003B0E RID: 15118
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000862 RID: 2146
		// (Invoke) Token: 0x06003433 RID: 13363
		public unsafe delegate int Add_00003404$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000863 RID: 2147
		internal static class Add_00003404$BurstDirectCall
		{
			// Token: 0x06003436 RID: 13366 RVA: 0x00101D31 File Offset: 0x000FFF31
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Add_00003404$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Add_00003404$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Add_00003404$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Add$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Add_00003404$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Add_00003404$BurstDirectCall.Pointer;
			}

			// Token: 0x06003437 RID: 13367 RVA: 0x00101D60 File Offset: 0x000FFF60
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Add_00003404$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06003438 RID: 13368 RVA: 0x00101D78 File Offset: 0x000FFF78
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Add_00003404$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Add(lua_State*)).MethodHandle);
			}

			// Token: 0x06003439 RID: 13369 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x0600343A RID: 13370 RVA: 0x00101D89 File Offset: 0x000FFF89
			// Note: this type is marked as 'beforefieldinit'.
			static Add_00003404$BurstDirectCall()
			{
				Bindings.Vec3Functions.Add_00003404$BurstDirectCall.Constructor();
			}

			// Token: 0x0600343B RID: 13371 RVA: 0x00101D90 File Offset: 0x000FFF90
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Add_00003404$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Add$BurstManaged(L);
			}

			// Token: 0x04003B0F RID: 15119
			private static IntPtr Pointer;

			// Token: 0x04003B10 RID: 15120
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000864 RID: 2148
		// (Invoke) Token: 0x0600343D RID: 13373
		public unsafe delegate int Sub_00003405$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000865 RID: 2149
		internal static class Sub_00003405$BurstDirectCall
		{
			// Token: 0x06003440 RID: 13376 RVA: 0x00101DC1 File Offset: 0x000FFFC1
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Sub_00003405$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Sub_00003405$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Sub_00003405$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Sub$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Sub_00003405$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Sub_00003405$BurstDirectCall.Pointer;
			}

			// Token: 0x06003441 RID: 13377 RVA: 0x00101DF0 File Offset: 0x000FFFF0
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Sub_00003405$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06003442 RID: 13378 RVA: 0x00101E08 File Offset: 0x00100008
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Sub_00003405$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Sub(lua_State*)).MethodHandle);
			}

			// Token: 0x06003443 RID: 13379 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x06003444 RID: 13380 RVA: 0x00101E19 File Offset: 0x00100019
			// Note: this type is marked as 'beforefieldinit'.
			static Sub_00003405$BurstDirectCall()
			{
				Bindings.Vec3Functions.Sub_00003405$BurstDirectCall.Constructor();
			}

			// Token: 0x06003445 RID: 13381 RVA: 0x00101E20 File Offset: 0x00100020
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Sub_00003405$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Sub$BurstManaged(L);
			}

			// Token: 0x04003B11 RID: 15121
			private static IntPtr Pointer;

			// Token: 0x04003B12 RID: 15122
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000866 RID: 2150
		// (Invoke) Token: 0x06003447 RID: 13383
		public unsafe delegate int Mul_00003406$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000867 RID: 2151
		internal static class Mul_00003406$BurstDirectCall
		{
			// Token: 0x0600344A RID: 13386 RVA: 0x00101E51 File Offset: 0x00100051
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Mul_00003406$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Mul_00003406$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Mul_00003406$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Mul$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Mul_00003406$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Mul_00003406$BurstDirectCall.Pointer;
			}

			// Token: 0x0600344B RID: 13387 RVA: 0x00101E80 File Offset: 0x00100080
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Mul_00003406$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x0600344C RID: 13388 RVA: 0x00101E98 File Offset: 0x00100098
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Mul_00003406$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Mul(lua_State*)).MethodHandle);
			}

			// Token: 0x0600344D RID: 13389 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x0600344E RID: 13390 RVA: 0x00101EA9 File Offset: 0x001000A9
			// Note: this type is marked as 'beforefieldinit'.
			static Mul_00003406$BurstDirectCall()
			{
				Bindings.Vec3Functions.Mul_00003406$BurstDirectCall.Constructor();
			}

			// Token: 0x0600344F RID: 13391 RVA: 0x00101EB0 File Offset: 0x001000B0
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Mul_00003406$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Mul$BurstManaged(L);
			}

			// Token: 0x04003B13 RID: 15123
			private static IntPtr Pointer;

			// Token: 0x04003B14 RID: 15124
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000868 RID: 2152
		// (Invoke) Token: 0x06003451 RID: 13393
		public unsafe delegate int Div_00003407$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000869 RID: 2153
		internal static class Div_00003407$BurstDirectCall
		{
			// Token: 0x06003454 RID: 13396 RVA: 0x00101EE1 File Offset: 0x001000E1
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Div_00003407$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Div_00003407$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Div_00003407$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Div$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Div_00003407$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Div_00003407$BurstDirectCall.Pointer;
			}

			// Token: 0x06003455 RID: 13397 RVA: 0x00101F10 File Offset: 0x00100110
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Div_00003407$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06003456 RID: 13398 RVA: 0x00101F28 File Offset: 0x00100128
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Div_00003407$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Div(lua_State*)).MethodHandle);
			}

			// Token: 0x06003457 RID: 13399 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x06003458 RID: 13400 RVA: 0x00101F39 File Offset: 0x00100139
			// Note: this type is marked as 'beforefieldinit'.
			static Div_00003407$BurstDirectCall()
			{
				Bindings.Vec3Functions.Div_00003407$BurstDirectCall.Constructor();
			}

			// Token: 0x06003459 RID: 13401 RVA: 0x00101F40 File Offset: 0x00100140
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Div_00003407$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Div$BurstManaged(L);
			}

			// Token: 0x04003B15 RID: 15125
			private static IntPtr Pointer;

			// Token: 0x04003B16 RID: 15126
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x0200086A RID: 2154
		// (Invoke) Token: 0x0600345B RID: 13403
		public unsafe delegate int Unm_00003408$PostfixBurstDelegate(lua_State* L);

		// Token: 0x0200086B RID: 2155
		internal static class Unm_00003408$BurstDirectCall
		{
			// Token: 0x0600345E RID: 13406 RVA: 0x00101F71 File Offset: 0x00100171
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Unm_00003408$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Unm_00003408$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Unm_00003408$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Unm$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Unm_00003408$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Unm_00003408$BurstDirectCall.Pointer;
			}

			// Token: 0x0600345F RID: 13407 RVA: 0x00101FA0 File Offset: 0x001001A0
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Unm_00003408$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06003460 RID: 13408 RVA: 0x00101FB8 File Offset: 0x001001B8
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Unm_00003408$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Unm(lua_State*)).MethodHandle);
			}

			// Token: 0x06003461 RID: 13409 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x06003462 RID: 13410 RVA: 0x00101FC9 File Offset: 0x001001C9
			// Note: this type is marked as 'beforefieldinit'.
			static Unm_00003408$BurstDirectCall()
			{
				Bindings.Vec3Functions.Unm_00003408$BurstDirectCall.Constructor();
			}

			// Token: 0x06003463 RID: 13411 RVA: 0x00101FD0 File Offset: 0x001001D0
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Unm_00003408$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Unm$BurstManaged(L);
			}

			// Token: 0x04003B17 RID: 15127
			private static IntPtr Pointer;

			// Token: 0x04003B18 RID: 15128
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x0200086C RID: 2156
		// (Invoke) Token: 0x06003465 RID: 13413
		public unsafe delegate int Eq_00003409$PostfixBurstDelegate(lua_State* L);

		// Token: 0x0200086D RID: 2157
		internal static class Eq_00003409$BurstDirectCall
		{
			// Token: 0x06003468 RID: 13416 RVA: 0x00102001 File Offset: 0x00100201
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Eq_00003409$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Eq_00003409$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Eq_00003409$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Eq$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Eq_00003409$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Eq_00003409$BurstDirectCall.Pointer;
			}

			// Token: 0x06003469 RID: 13417 RVA: 0x00102030 File Offset: 0x00100230
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Eq_00003409$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x0600346A RID: 13418 RVA: 0x00102048 File Offset: 0x00100248
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Eq_00003409$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Eq(lua_State*)).MethodHandle);
			}

			// Token: 0x0600346B RID: 13419 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x0600346C RID: 13420 RVA: 0x00102059 File Offset: 0x00100259
			// Note: this type is marked as 'beforefieldinit'.
			static Eq_00003409$BurstDirectCall()
			{
				Bindings.Vec3Functions.Eq_00003409$BurstDirectCall.Constructor();
			}

			// Token: 0x0600346D RID: 13421 RVA: 0x00102060 File Offset: 0x00100260
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Eq_00003409$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Eq$BurstManaged(L);
			}

			// Token: 0x04003B19 RID: 15129
			private static IntPtr Pointer;

			// Token: 0x04003B1A RID: 15130
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x0200086E RID: 2158
		// (Invoke) Token: 0x0600346F RID: 13423
		public unsafe delegate int Dot_0000340B$PostfixBurstDelegate(lua_State* L);

		// Token: 0x0200086F RID: 2159
		internal static class Dot_0000340B$BurstDirectCall
		{
			// Token: 0x06003472 RID: 13426 RVA: 0x00102091 File Offset: 0x00100291
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Dot_0000340B$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Dot_0000340B$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Dot_0000340B$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Dot$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Dot_0000340B$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Dot_0000340B$BurstDirectCall.Pointer;
			}

			// Token: 0x06003473 RID: 13427 RVA: 0x001020C0 File Offset: 0x001002C0
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Dot_0000340B$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06003474 RID: 13428 RVA: 0x001020D8 File Offset: 0x001002D8
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Dot_0000340B$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Dot(lua_State*)).MethodHandle);
			}

			// Token: 0x06003475 RID: 13429 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x06003476 RID: 13430 RVA: 0x001020E9 File Offset: 0x001002E9
			// Note: this type is marked as 'beforefieldinit'.
			static Dot_0000340B$BurstDirectCall()
			{
				Bindings.Vec3Functions.Dot_0000340B$BurstDirectCall.Constructor();
			}

			// Token: 0x06003477 RID: 13431 RVA: 0x001020F0 File Offset: 0x001002F0
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Dot_0000340B$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Dot$BurstManaged(L);
			}

			// Token: 0x04003B1B RID: 15131
			private static IntPtr Pointer;

			// Token: 0x04003B1C RID: 15132
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000870 RID: 2160
		// (Invoke) Token: 0x06003479 RID: 13433
		public unsafe delegate int Cross_0000340C$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000871 RID: 2161
		internal static class Cross_0000340C$BurstDirectCall
		{
			// Token: 0x0600347C RID: 13436 RVA: 0x00102121 File Offset: 0x00100321
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Cross_0000340C$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Cross_0000340C$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Cross_0000340C$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Cross$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Cross_0000340C$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Cross_0000340C$BurstDirectCall.Pointer;
			}

			// Token: 0x0600347D RID: 13437 RVA: 0x00102150 File Offset: 0x00100350
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Cross_0000340C$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x0600347E RID: 13438 RVA: 0x00102168 File Offset: 0x00100368
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Cross_0000340C$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Cross(lua_State*)).MethodHandle);
			}

			// Token: 0x0600347F RID: 13439 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x06003480 RID: 13440 RVA: 0x00102179 File Offset: 0x00100379
			// Note: this type is marked as 'beforefieldinit'.
			static Cross_0000340C$BurstDirectCall()
			{
				Bindings.Vec3Functions.Cross_0000340C$BurstDirectCall.Constructor();
			}

			// Token: 0x06003481 RID: 13441 RVA: 0x00102180 File Offset: 0x00100380
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Cross_0000340C$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Cross$BurstManaged(L);
			}

			// Token: 0x04003B1D RID: 15133
			private static IntPtr Pointer;

			// Token: 0x04003B1E RID: 15134
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000872 RID: 2162
		// (Invoke) Token: 0x06003483 RID: 13443
		public unsafe delegate int Project_0000340D$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000873 RID: 2163
		internal static class Project_0000340D$BurstDirectCall
		{
			// Token: 0x06003486 RID: 13446 RVA: 0x001021B1 File Offset: 0x001003B1
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Project_0000340D$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Project_0000340D$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Project_0000340D$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Project$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Project_0000340D$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Project_0000340D$BurstDirectCall.Pointer;
			}

			// Token: 0x06003487 RID: 13447 RVA: 0x001021E0 File Offset: 0x001003E0
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Project_0000340D$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06003488 RID: 13448 RVA: 0x001021F8 File Offset: 0x001003F8
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Project_0000340D$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Project(lua_State*)).MethodHandle);
			}

			// Token: 0x06003489 RID: 13449 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x0600348A RID: 13450 RVA: 0x00102209 File Offset: 0x00100409
			// Note: this type is marked as 'beforefieldinit'.
			static Project_0000340D$BurstDirectCall()
			{
				Bindings.Vec3Functions.Project_0000340D$BurstDirectCall.Constructor();
			}

			// Token: 0x0600348B RID: 13451 RVA: 0x00102210 File Offset: 0x00100410
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Project_0000340D$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Project$BurstManaged(L);
			}

			// Token: 0x04003B1F RID: 15135
			private static IntPtr Pointer;

			// Token: 0x04003B20 RID: 15136
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000874 RID: 2164
		// (Invoke) Token: 0x0600348D RID: 13453
		public unsafe delegate int Length_0000340E$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000875 RID: 2165
		internal static class Length_0000340E$BurstDirectCall
		{
			// Token: 0x06003490 RID: 13456 RVA: 0x00102241 File Offset: 0x00100441
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Length_0000340E$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Length_0000340E$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Length_0000340E$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Length$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Length_0000340E$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Length_0000340E$BurstDirectCall.Pointer;
			}

			// Token: 0x06003491 RID: 13457 RVA: 0x00102270 File Offset: 0x00100470
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Length_0000340E$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06003492 RID: 13458 RVA: 0x00102288 File Offset: 0x00100488
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Length_0000340E$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Length(lua_State*)).MethodHandle);
			}

			// Token: 0x06003493 RID: 13459 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x06003494 RID: 13460 RVA: 0x00102299 File Offset: 0x00100499
			// Note: this type is marked as 'beforefieldinit'.
			static Length_0000340E$BurstDirectCall()
			{
				Bindings.Vec3Functions.Length_0000340E$BurstDirectCall.Constructor();
			}

			// Token: 0x06003495 RID: 13461 RVA: 0x001022A0 File Offset: 0x001004A0
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Length_0000340E$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Length$BurstManaged(L);
			}

			// Token: 0x04003B21 RID: 15137
			private static IntPtr Pointer;

			// Token: 0x04003B22 RID: 15138
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000876 RID: 2166
		// (Invoke) Token: 0x06003497 RID: 13463
		public unsafe delegate int Normalize_0000340F$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000877 RID: 2167
		internal static class Normalize_0000340F$BurstDirectCall
		{
			// Token: 0x0600349A RID: 13466 RVA: 0x001022D1 File Offset: 0x001004D1
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Normalize_0000340F$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Normalize_0000340F$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Normalize_0000340F$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Normalize$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Normalize_0000340F$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Normalize_0000340F$BurstDirectCall.Pointer;
			}

			// Token: 0x0600349B RID: 13467 RVA: 0x00102300 File Offset: 0x00100500
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Normalize_0000340F$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x0600349C RID: 13468 RVA: 0x00102318 File Offset: 0x00100518
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Normalize_0000340F$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Normalize(lua_State*)).MethodHandle);
			}

			// Token: 0x0600349D RID: 13469 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x0600349E RID: 13470 RVA: 0x00102329 File Offset: 0x00100529
			// Note: this type is marked as 'beforefieldinit'.
			static Normalize_0000340F$BurstDirectCall()
			{
				Bindings.Vec3Functions.Normalize_0000340F$BurstDirectCall.Constructor();
			}

			// Token: 0x0600349F RID: 13471 RVA: 0x00102330 File Offset: 0x00100530
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Normalize_0000340F$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Normalize$BurstManaged(L);
			}

			// Token: 0x04003B23 RID: 15139
			private static IntPtr Pointer;

			// Token: 0x04003B24 RID: 15140
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000878 RID: 2168
		// (Invoke) Token: 0x060034A1 RID: 13473
		public unsafe delegate int SafeNormal_00003410$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000879 RID: 2169
		internal static class SafeNormal_00003410$BurstDirectCall
		{
			// Token: 0x060034A4 RID: 13476 RVA: 0x00102361 File Offset: 0x00100561
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.SafeNormal_00003410$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.SafeNormal_00003410$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.SafeNormal_00003410$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.SafeNormal$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.SafeNormal_00003410$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.SafeNormal_00003410$BurstDirectCall.Pointer;
			}

			// Token: 0x060034A5 RID: 13477 RVA: 0x00102390 File Offset: 0x00100590
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.SafeNormal_00003410$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x060034A6 RID: 13478 RVA: 0x001023A8 File Offset: 0x001005A8
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.SafeNormal_00003410$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.SafeNormal(lua_State*)).MethodHandle);
			}

			// Token: 0x060034A7 RID: 13479 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x060034A8 RID: 13480 RVA: 0x001023B9 File Offset: 0x001005B9
			// Note: this type is marked as 'beforefieldinit'.
			static SafeNormal_00003410$BurstDirectCall()
			{
				Bindings.Vec3Functions.SafeNormal_00003410$BurstDirectCall.Constructor();
			}

			// Token: 0x060034A9 RID: 13481 RVA: 0x001023C0 File Offset: 0x001005C0
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.SafeNormal_00003410$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.SafeNormal$BurstManaged(L);
			}

			// Token: 0x04003B25 RID: 15141
			private static IntPtr Pointer;

			// Token: 0x04003B26 RID: 15142
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x0200087A RID: 2170
		// (Invoke) Token: 0x060034AB RID: 13483
		public unsafe delegate int Distance_00003411$PostfixBurstDelegate(lua_State* L);

		// Token: 0x0200087B RID: 2171
		internal static class Distance_00003411$BurstDirectCall
		{
			// Token: 0x060034AE RID: 13486 RVA: 0x001023F1 File Offset: 0x001005F1
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Distance_00003411$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Distance_00003411$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Distance_00003411$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Distance$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Distance_00003411$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Distance_00003411$BurstDirectCall.Pointer;
			}

			// Token: 0x060034AF RID: 13487 RVA: 0x00102420 File Offset: 0x00100620
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Distance_00003411$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x060034B0 RID: 13488 RVA: 0x00102438 File Offset: 0x00100638
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Distance_00003411$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Distance(lua_State*)).MethodHandle);
			}

			// Token: 0x060034B1 RID: 13489 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x060034B2 RID: 13490 RVA: 0x00102449 File Offset: 0x00100649
			// Note: this type is marked as 'beforefieldinit'.
			static Distance_00003411$BurstDirectCall()
			{
				Bindings.Vec3Functions.Distance_00003411$BurstDirectCall.Constructor();
			}

			// Token: 0x060034B3 RID: 13491 RVA: 0x00102450 File Offset: 0x00100650
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Distance_00003411$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Distance$BurstManaged(L);
			}

			// Token: 0x04003B27 RID: 15143
			private static IntPtr Pointer;

			// Token: 0x04003B28 RID: 15144
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x0200087C RID: 2172
		// (Invoke) Token: 0x060034B5 RID: 13493
		public unsafe delegate int Lerp_00003412$PostfixBurstDelegate(lua_State* L);

		// Token: 0x0200087D RID: 2173
		internal static class Lerp_00003412$BurstDirectCall
		{
			// Token: 0x060034B8 RID: 13496 RVA: 0x00102481 File Offset: 0x00100681
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Lerp_00003412$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Lerp_00003412$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Lerp_00003412$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Lerp$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Lerp_00003412$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Lerp_00003412$BurstDirectCall.Pointer;
			}

			// Token: 0x060034B9 RID: 13497 RVA: 0x001024B0 File Offset: 0x001006B0
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Lerp_00003412$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x060034BA RID: 13498 RVA: 0x001024C8 File Offset: 0x001006C8
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Lerp_00003412$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Lerp(lua_State*)).MethodHandle);
			}

			// Token: 0x060034BB RID: 13499 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x060034BC RID: 13500 RVA: 0x001024D9 File Offset: 0x001006D9
			// Note: this type is marked as 'beforefieldinit'.
			static Lerp_00003412$BurstDirectCall()
			{
				Bindings.Vec3Functions.Lerp_00003412$BurstDirectCall.Constructor();
			}

			// Token: 0x060034BD RID: 13501 RVA: 0x001024E0 File Offset: 0x001006E0
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Lerp_00003412$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Lerp$BurstManaged(L);
			}

			// Token: 0x04003B29 RID: 15145
			private static IntPtr Pointer;

			// Token: 0x04003B2A RID: 15146
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x0200087E RID: 2174
		// (Invoke) Token: 0x060034BF RID: 13503
		public unsafe delegate int Rotate_00003413$PostfixBurstDelegate(lua_State* L);

		// Token: 0x0200087F RID: 2175
		internal static class Rotate_00003413$BurstDirectCall
		{
			// Token: 0x060034C2 RID: 13506 RVA: 0x00102511 File Offset: 0x00100711
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.Rotate_00003413$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.Rotate_00003413$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.Rotate_00003413$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.Rotate$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.Rotate_00003413$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.Rotate_00003413$BurstDirectCall.Pointer;
			}

			// Token: 0x060034C3 RID: 13507 RVA: 0x00102540 File Offset: 0x00100740
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.Rotate_00003413$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x060034C4 RID: 13508 RVA: 0x00102558 File Offset: 0x00100758
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.Rotate_00003413$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.Rotate(lua_State*)).MethodHandle);
			}

			// Token: 0x060034C5 RID: 13509 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x060034C6 RID: 13510 RVA: 0x00102569 File Offset: 0x00100769
			// Note: this type is marked as 'beforefieldinit'.
			static Rotate_00003413$BurstDirectCall()
			{
				Bindings.Vec3Functions.Rotate_00003413$BurstDirectCall.Constructor();
			}

			// Token: 0x060034C7 RID: 13511 RVA: 0x00102570 File Offset: 0x00100770
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.Rotate_00003413$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.Rotate$BurstManaged(L);
			}

			// Token: 0x04003B2B RID: 15147
			private static IntPtr Pointer;

			// Token: 0x04003B2C RID: 15148
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000880 RID: 2176
		// (Invoke) Token: 0x060034C9 RID: 13513
		public unsafe delegate int ZeroVector_00003414$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000881 RID: 2177
		internal static class ZeroVector_00003414$BurstDirectCall
		{
			// Token: 0x060034CC RID: 13516 RVA: 0x001025A1 File Offset: 0x001007A1
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.ZeroVector_00003414$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.ZeroVector_00003414$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.ZeroVector_00003414$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.ZeroVector$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.ZeroVector_00003414$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.ZeroVector_00003414$BurstDirectCall.Pointer;
			}

			// Token: 0x060034CD RID: 13517 RVA: 0x001025D0 File Offset: 0x001007D0
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.ZeroVector_00003414$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x060034CE RID: 13518 RVA: 0x001025E8 File Offset: 0x001007E8
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.ZeroVector_00003414$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.ZeroVector(lua_State*)).MethodHandle);
			}

			// Token: 0x060034CF RID: 13519 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x060034D0 RID: 13520 RVA: 0x001025F9 File Offset: 0x001007F9
			// Note: this type is marked as 'beforefieldinit'.
			static ZeroVector_00003414$BurstDirectCall()
			{
				Bindings.Vec3Functions.ZeroVector_00003414$BurstDirectCall.Constructor();
			}

			// Token: 0x060034D1 RID: 13521 RVA: 0x00102600 File Offset: 0x00100800
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.ZeroVector_00003414$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.ZeroVector$BurstManaged(L);
			}

			// Token: 0x04003B2D RID: 15149
			private static IntPtr Pointer;

			// Token: 0x04003B2E RID: 15150
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000882 RID: 2178
		// (Invoke) Token: 0x060034D3 RID: 13523
		public unsafe delegate int OneVector_00003415$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000883 RID: 2179
		internal static class OneVector_00003415$BurstDirectCall
		{
			// Token: 0x060034D6 RID: 13526 RVA: 0x00102631 File Offset: 0x00100831
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.Vec3Functions.OneVector_00003415$BurstDirectCall.Pointer == 0)
				{
					Bindings.Vec3Functions.OneVector_00003415$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.Vec3Functions.OneVector_00003415$BurstDirectCall.DeferredCompilation, methodof(Bindings.Vec3Functions.OneVector$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.Vec3Functions.OneVector_00003415$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.Vec3Functions.OneVector_00003415$BurstDirectCall.Pointer;
			}

			// Token: 0x060034D7 RID: 13527 RVA: 0x00102660 File Offset: 0x00100860
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.Vec3Functions.OneVector_00003415$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x060034D8 RID: 13528 RVA: 0x00102678 File Offset: 0x00100878
			public unsafe static void Constructor()
			{
				Bindings.Vec3Functions.OneVector_00003415$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.Vec3Functions.OneVector(lua_State*)).MethodHandle);
			}

			// Token: 0x060034D9 RID: 13529 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x060034DA RID: 13530 RVA: 0x00102689 File Offset: 0x00100889
			// Note: this type is marked as 'beforefieldinit'.
			static OneVector_00003415$BurstDirectCall()
			{
				Bindings.Vec3Functions.OneVector_00003415$BurstDirectCall.Constructor();
			}

			// Token: 0x060034DB RID: 13531 RVA: 0x00102690 File Offset: 0x00100890
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.Vec3Functions.OneVector_00003415$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.Vec3Functions.OneVector$BurstManaged(L);
			}

			// Token: 0x04003B2F RID: 15151
			private static IntPtr Pointer;

			// Token: 0x04003B30 RID: 15152
			private static IntPtr DeferredCompilation;
		}
	}

	// Token: 0x02000884 RID: 2180
	[BurstCompile]
	public static class QuatFunctions
	{
		// Token: 0x060034DC RID: 13532 RVA: 0x001026C1 File Offset: 0x001008C1
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int New(lua_State* L)
		{
			return Bindings.QuatFunctions.New_00003416$BurstDirectCall.Invoke(L);
		}

		// Token: 0x060034DD RID: 13533 RVA: 0x001026C9 File Offset: 0x001008C9
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Mul(lua_State* L)
		{
			return Bindings.QuatFunctions.Mul_00003417$BurstDirectCall.Invoke(L);
		}

		// Token: 0x060034DE RID: 13534 RVA: 0x001026D1 File Offset: 0x001008D1
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Eq(lua_State* L)
		{
			return Bindings.QuatFunctions.Eq_00003418$BurstDirectCall.Invoke(L);
		}

		// Token: 0x060034DF RID: 13535 RVA: 0x001026DC File Offset: 0x001008DC
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int ToString(lua_State* L)
		{
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			Luau.lua_pushstring(L, quaternion.ToString());
			return 1;
		}

		// Token: 0x060034E0 RID: 13536 RVA: 0x00102715 File Offset: 0x00100915
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FromEuler(lua_State* L)
		{
			return Bindings.QuatFunctions.FromEuler_0000341A$BurstDirectCall.Invoke(L);
		}

		// Token: 0x060034E1 RID: 13537 RVA: 0x0010271D File Offset: 0x0010091D
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int FromDirection(lua_State* L)
		{
			return Bindings.QuatFunctions.FromDirection_0000341B$BurstDirectCall.Invoke(L);
		}

		// Token: 0x060034E2 RID: 13538 RVA: 0x00102725 File Offset: 0x00100925
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int GetUpVector(lua_State* L)
		{
			return Bindings.QuatFunctions.GetUpVector_0000341C$BurstDirectCall.Invoke(L);
		}

		// Token: 0x060034E3 RID: 13539 RVA: 0x0010272D File Offset: 0x0010092D
		[BurstCompile]
		[MonoPInvokeCallback(typeof(lua_CFunction))]
		public unsafe static int Euler(lua_State* L)
		{
			return Bindings.QuatFunctions.Euler_0000341D$BurstDirectCall.Invoke(L);
		}

		// Token: 0x060034E4 RID: 13540 RVA: 0x00102738 File Offset: 0x00100938
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int New$BurstManaged(lua_State* L)
		{
			Quaternion* ptr = Luau.lua_class_push<Quaternion>(L, "Quat");
			ptr->x = (float)Luau.luaL_optnumber(L, 1, 0.0);
			ptr->y = (float)Luau.luaL_optnumber(L, 2, 0.0);
			ptr->z = (float)Luau.luaL_optnumber(L, 3, 0.0);
			ptr->w = (float)Luau.luaL_optnumber(L, 4, 0.0);
			return 1;
		}

		// Token: 0x060034E5 RID: 13541 RVA: 0x001027B4 File Offset: 0x001009B4
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Mul$BurstManaged(lua_State* L)
		{
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			Quaternion quaternion2 = *Luau.lua_class_get<Quaternion>(L, 2, "Quat");
			*Luau.lua_class_push<Quaternion>(L, "Quat") = quaternion * quaternion2;
			return 1;
		}

		// Token: 0x060034E6 RID: 13542 RVA: 0x0010280C File Offset: 0x00100A0C
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Eq$BurstManaged(lua_State* L)
		{
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			Quaternion quaternion2 = *Luau.lua_class_get<Quaternion>(L, 2, "Quat");
			int num = ((quaternion == quaternion2) ? 1 : 0);
			Luau.lua_pushnumber(L, (double)num);
			return 1;
		}

		// Token: 0x060034E7 RID: 13543 RVA: 0x0010285C File Offset: 0x00100A5C
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int FromEuler$BurstManaged(lua_State* L)
		{
			float num = (float)Luau.luaL_optnumber(L, 1, 0.0);
			float num2 = (float)Luau.luaL_optnumber(L, 2, 0.0);
			float num3 = (float)Luau.luaL_optnumber(L, 3, 0.0);
			Luau.lua_class_push<Quaternion>(L, "Quat")->eulerAngles = new Vector3(num, num2, num3);
			return 1;
		}

		// Token: 0x060034E8 RID: 13544 RVA: 0x001028C0 File Offset: 0x00100AC0
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int FromDirection$BurstManaged(lua_State* L)
		{
			Vector3 vector = *Luau.lua_class_get<Vector3>(L, 1, "Vec3");
			Luau.lua_class_push<Quaternion>(L, "Quat")->SetLookRotation(vector);
			return 1;
		}

		// Token: 0x060034E9 RID: 13545 RVA: 0x001028FC File Offset: 0x00100AFC
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int GetUpVector$BurstManaged(lua_State* L)
		{
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = quaternion * Vector3.up;
			return 1;
		}

		// Token: 0x060034EA RID: 13546 RVA: 0x00102944 File Offset: 0x00100B44
		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Euler$BurstManaged(lua_State* L)
		{
			Quaternion quaternion = *Luau.lua_class_get<Quaternion>(L, 1, "Quat");
			*Luau.lua_class_push<Vector3>(L, "Vec3") = quaternion.eulerAngles;
			return 1;
		}

		// Token: 0x02000885 RID: 2181
		// (Invoke) Token: 0x060034EC RID: 13548
		public unsafe delegate int New_00003416$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000886 RID: 2182
		internal static class New_00003416$BurstDirectCall
		{
			// Token: 0x060034EF RID: 13551 RVA: 0x00102985 File Offset: 0x00100B85
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.New_00003416$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.New_00003416$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.QuatFunctions.New_00003416$BurstDirectCall.DeferredCompilation, methodof(Bindings.QuatFunctions.New$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.QuatFunctions.New_00003416$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.QuatFunctions.New_00003416$BurstDirectCall.Pointer;
			}

			// Token: 0x060034F0 RID: 13552 RVA: 0x001029B4 File Offset: 0x00100BB4
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.QuatFunctions.New_00003416$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x060034F1 RID: 13553 RVA: 0x001029CC File Offset: 0x00100BCC
			public unsafe static void Constructor()
			{
				Bindings.QuatFunctions.New_00003416$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.QuatFunctions.New(lua_State*)).MethodHandle);
			}

			// Token: 0x060034F2 RID: 13554 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x060034F3 RID: 13555 RVA: 0x001029DD File Offset: 0x00100BDD
			// Note: this type is marked as 'beforefieldinit'.
			static New_00003416$BurstDirectCall()
			{
				Bindings.QuatFunctions.New_00003416$BurstDirectCall.Constructor();
			}

			// Token: 0x060034F4 RID: 13556 RVA: 0x001029E4 File Offset: 0x00100BE4
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.New_00003416$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.New$BurstManaged(L);
			}

			// Token: 0x04003B31 RID: 15153
			private static IntPtr Pointer;

			// Token: 0x04003B32 RID: 15154
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000887 RID: 2183
		// (Invoke) Token: 0x060034F6 RID: 13558
		public unsafe delegate int Mul_00003417$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000888 RID: 2184
		internal static class Mul_00003417$BurstDirectCall
		{
			// Token: 0x060034F9 RID: 13561 RVA: 0x00102A15 File Offset: 0x00100C15
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.Mul_00003417$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.Mul_00003417$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.QuatFunctions.Mul_00003417$BurstDirectCall.DeferredCompilation, methodof(Bindings.QuatFunctions.Mul$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.QuatFunctions.Mul_00003417$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.QuatFunctions.Mul_00003417$BurstDirectCall.Pointer;
			}

			// Token: 0x060034FA RID: 13562 RVA: 0x00102A44 File Offset: 0x00100C44
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.QuatFunctions.Mul_00003417$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x060034FB RID: 13563 RVA: 0x00102A5C File Offset: 0x00100C5C
			public unsafe static void Constructor()
			{
				Bindings.QuatFunctions.Mul_00003417$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.QuatFunctions.Mul(lua_State*)).MethodHandle);
			}

			// Token: 0x060034FC RID: 13564 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x060034FD RID: 13565 RVA: 0x00102A6D File Offset: 0x00100C6D
			// Note: this type is marked as 'beforefieldinit'.
			static Mul_00003417$BurstDirectCall()
			{
				Bindings.QuatFunctions.Mul_00003417$BurstDirectCall.Constructor();
			}

			// Token: 0x060034FE RID: 13566 RVA: 0x00102A74 File Offset: 0x00100C74
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.Mul_00003417$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.Mul$BurstManaged(L);
			}

			// Token: 0x04003B33 RID: 15155
			private static IntPtr Pointer;

			// Token: 0x04003B34 RID: 15156
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000889 RID: 2185
		// (Invoke) Token: 0x06003500 RID: 13568
		public unsafe delegate int Eq_00003418$PostfixBurstDelegate(lua_State* L);

		// Token: 0x0200088A RID: 2186
		internal static class Eq_00003418$BurstDirectCall
		{
			// Token: 0x06003503 RID: 13571 RVA: 0x00102AA5 File Offset: 0x00100CA5
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.Eq_00003418$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.Eq_00003418$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.QuatFunctions.Eq_00003418$BurstDirectCall.DeferredCompilation, methodof(Bindings.QuatFunctions.Eq$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.QuatFunctions.Eq_00003418$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.QuatFunctions.Eq_00003418$BurstDirectCall.Pointer;
			}

			// Token: 0x06003504 RID: 13572 RVA: 0x00102AD4 File Offset: 0x00100CD4
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.QuatFunctions.Eq_00003418$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06003505 RID: 13573 RVA: 0x00102AEC File Offset: 0x00100CEC
			public unsafe static void Constructor()
			{
				Bindings.QuatFunctions.Eq_00003418$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.QuatFunctions.Eq(lua_State*)).MethodHandle);
			}

			// Token: 0x06003506 RID: 13574 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x06003507 RID: 13575 RVA: 0x00102AFD File Offset: 0x00100CFD
			// Note: this type is marked as 'beforefieldinit'.
			static Eq_00003418$BurstDirectCall()
			{
				Bindings.QuatFunctions.Eq_00003418$BurstDirectCall.Constructor();
			}

			// Token: 0x06003508 RID: 13576 RVA: 0x00102B04 File Offset: 0x00100D04
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.Eq_00003418$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.Eq$BurstManaged(L);
			}

			// Token: 0x04003B35 RID: 15157
			private static IntPtr Pointer;

			// Token: 0x04003B36 RID: 15158
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x0200088B RID: 2187
		// (Invoke) Token: 0x0600350A RID: 13578
		public unsafe delegate int FromEuler_0000341A$PostfixBurstDelegate(lua_State* L);

		// Token: 0x0200088C RID: 2188
		internal static class FromEuler_0000341A$BurstDirectCall
		{
			// Token: 0x0600350D RID: 13581 RVA: 0x00102B35 File Offset: 0x00100D35
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.FromEuler_0000341A$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.FromEuler_0000341A$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.QuatFunctions.FromEuler_0000341A$BurstDirectCall.DeferredCompilation, methodof(Bindings.QuatFunctions.FromEuler$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.QuatFunctions.FromEuler_0000341A$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.QuatFunctions.FromEuler_0000341A$BurstDirectCall.Pointer;
			}

			// Token: 0x0600350E RID: 13582 RVA: 0x00102B64 File Offset: 0x00100D64
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.QuatFunctions.FromEuler_0000341A$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x0600350F RID: 13583 RVA: 0x00102B7C File Offset: 0x00100D7C
			public unsafe static void Constructor()
			{
				Bindings.QuatFunctions.FromEuler_0000341A$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.QuatFunctions.FromEuler(lua_State*)).MethodHandle);
			}

			// Token: 0x06003510 RID: 13584 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x06003511 RID: 13585 RVA: 0x00102B8D File Offset: 0x00100D8D
			// Note: this type is marked as 'beforefieldinit'.
			static FromEuler_0000341A$BurstDirectCall()
			{
				Bindings.QuatFunctions.FromEuler_0000341A$BurstDirectCall.Constructor();
			}

			// Token: 0x06003512 RID: 13586 RVA: 0x00102B94 File Offset: 0x00100D94
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.FromEuler_0000341A$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.FromEuler$BurstManaged(L);
			}

			// Token: 0x04003B37 RID: 15159
			private static IntPtr Pointer;

			// Token: 0x04003B38 RID: 15160
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x0200088D RID: 2189
		// (Invoke) Token: 0x06003514 RID: 13588
		public unsafe delegate int FromDirection_0000341B$PostfixBurstDelegate(lua_State* L);

		// Token: 0x0200088E RID: 2190
		internal static class FromDirection_0000341B$BurstDirectCall
		{
			// Token: 0x06003517 RID: 13591 RVA: 0x00102BC5 File Offset: 0x00100DC5
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.FromDirection_0000341B$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.FromDirection_0000341B$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.QuatFunctions.FromDirection_0000341B$BurstDirectCall.DeferredCompilation, methodof(Bindings.QuatFunctions.FromDirection$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.QuatFunctions.FromDirection_0000341B$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.QuatFunctions.FromDirection_0000341B$BurstDirectCall.Pointer;
			}

			// Token: 0x06003518 RID: 13592 RVA: 0x00102BF4 File Offset: 0x00100DF4
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.QuatFunctions.FromDirection_0000341B$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06003519 RID: 13593 RVA: 0x00102C0C File Offset: 0x00100E0C
			public unsafe static void Constructor()
			{
				Bindings.QuatFunctions.FromDirection_0000341B$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.QuatFunctions.FromDirection(lua_State*)).MethodHandle);
			}

			// Token: 0x0600351A RID: 13594 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x0600351B RID: 13595 RVA: 0x00102C1D File Offset: 0x00100E1D
			// Note: this type is marked as 'beforefieldinit'.
			static FromDirection_0000341B$BurstDirectCall()
			{
				Bindings.QuatFunctions.FromDirection_0000341B$BurstDirectCall.Constructor();
			}

			// Token: 0x0600351C RID: 13596 RVA: 0x00102C24 File Offset: 0x00100E24
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.FromDirection_0000341B$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.FromDirection$BurstManaged(L);
			}

			// Token: 0x04003B39 RID: 15161
			private static IntPtr Pointer;

			// Token: 0x04003B3A RID: 15162
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x0200088F RID: 2191
		// (Invoke) Token: 0x0600351E RID: 13598
		public unsafe delegate int GetUpVector_0000341C$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000890 RID: 2192
		internal static class GetUpVector_0000341C$BurstDirectCall
		{
			// Token: 0x06003521 RID: 13601 RVA: 0x00102C55 File Offset: 0x00100E55
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.GetUpVector_0000341C$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.GetUpVector_0000341C$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.QuatFunctions.GetUpVector_0000341C$BurstDirectCall.DeferredCompilation, methodof(Bindings.QuatFunctions.GetUpVector$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.QuatFunctions.GetUpVector_0000341C$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.QuatFunctions.GetUpVector_0000341C$BurstDirectCall.Pointer;
			}

			// Token: 0x06003522 RID: 13602 RVA: 0x00102C84 File Offset: 0x00100E84
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.QuatFunctions.GetUpVector_0000341C$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x06003523 RID: 13603 RVA: 0x00102C9C File Offset: 0x00100E9C
			public unsafe static void Constructor()
			{
				Bindings.QuatFunctions.GetUpVector_0000341C$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.QuatFunctions.GetUpVector(lua_State*)).MethodHandle);
			}

			// Token: 0x06003524 RID: 13604 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x06003525 RID: 13605 RVA: 0x00102CAD File Offset: 0x00100EAD
			// Note: this type is marked as 'beforefieldinit'.
			static GetUpVector_0000341C$BurstDirectCall()
			{
				Bindings.QuatFunctions.GetUpVector_0000341C$BurstDirectCall.Constructor();
			}

			// Token: 0x06003526 RID: 13606 RVA: 0x00102CB4 File Offset: 0x00100EB4
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.GetUpVector_0000341C$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.GetUpVector$BurstManaged(L);
			}

			// Token: 0x04003B3B RID: 15163
			private static IntPtr Pointer;

			// Token: 0x04003B3C RID: 15164
			private static IntPtr DeferredCompilation;
		}

		// Token: 0x02000891 RID: 2193
		// (Invoke) Token: 0x06003528 RID: 13608
		public unsafe delegate int Euler_0000341D$PostfixBurstDelegate(lua_State* L);

		// Token: 0x02000892 RID: 2194
		internal static class Euler_0000341D$BurstDirectCall
		{
			// Token: 0x0600352B RID: 13611 RVA: 0x00102CE5 File Offset: 0x00100EE5
			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (Bindings.QuatFunctions.Euler_0000341D$BurstDirectCall.Pointer == 0)
				{
					Bindings.QuatFunctions.Euler_0000341D$BurstDirectCall.Pointer = BurstCompiler.GetILPPMethodFunctionPointer2(Bindings.QuatFunctions.Euler_0000341D$BurstDirectCall.DeferredCompilation, methodof(Bindings.QuatFunctions.Euler$BurstManaged(lua_State*)).MethodHandle, typeof(Bindings.QuatFunctions.Euler_0000341D$PostfixBurstDelegate).TypeHandle);
				}
				A_0 = Bindings.QuatFunctions.Euler_0000341D$BurstDirectCall.Pointer;
			}

			// Token: 0x0600352C RID: 13612 RVA: 0x00102D14 File Offset: 0x00100F14
			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				Bindings.QuatFunctions.Euler_0000341D$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			// Token: 0x0600352D RID: 13613 RVA: 0x00102D2C File Offset: 0x00100F2C
			public unsafe static void Constructor()
			{
				Bindings.QuatFunctions.Euler_0000341D$BurstDirectCall.DeferredCompilation = BurstCompiler.CompileILPPMethod2(methodof(Bindings.QuatFunctions.Euler(lua_State*)).MethodHandle);
			}

			// Token: 0x0600352E RID: 13614 RVA: 0x000023F4 File Offset: 0x000005F4
			public static void Initialize()
			{
			}

			// Token: 0x0600352F RID: 13615 RVA: 0x00102D3D File Offset: 0x00100F3D
			// Note: this type is marked as 'beforefieldinit'.
			static Euler_0000341D$BurstDirectCall()
			{
				Bindings.QuatFunctions.Euler_0000341D$BurstDirectCall.Constructor();
			}

			// Token: 0x06003530 RID: 13616 RVA: 0x00102D44 File Offset: 0x00100F44
			public unsafe static int Invoke(lua_State* L)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = Bindings.QuatFunctions.Euler_0000341D$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(lua_State*), L, functionPointer);
					}
				}
				return Bindings.QuatFunctions.Euler$BurstManaged(L);
			}

			// Token: 0x04003B3D RID: 15165
			private static IntPtr Pointer;

			// Token: 0x04003B3E RID: 15166
			private static IntPtr DeferredCompilation;
		}
	}

	// Token: 0x02000893 RID: 2195
	public struct GorillaLocomotionSettings
	{
		// Token: 0x04003B3F RID: 15167
		public float velocityLimit;

		// Token: 0x04003B40 RID: 15168
		public float slideVelocityLimit;

		// Token: 0x04003B41 RID: 15169
		public float maxJumpSpeed;

		// Token: 0x04003B42 RID: 15170
		public float jumpMultiplier;
	}
}
