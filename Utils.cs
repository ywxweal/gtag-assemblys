using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020009DF RID: 2527
public static class Utils
{
	// Token: 0x06003C60 RID: 15456 RVA: 0x00120460 File Offset: 0x0011E660
	public static void Disable(this GameObject target)
	{
		if (!target.activeSelf)
		{
			return;
		}
		PooledList<IPreDisable> pooledList = Utils.g_listPool.Take();
		List<IPreDisable> list = pooledList.List;
		target.GetComponents<IPreDisable>(list);
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			try
			{
				list[i].PreDisable();
			}
			catch (Exception)
			{
			}
		}
		target.SetActive(false);
		Utils.g_listPool.Return(pooledList);
	}

	// Token: 0x06003C61 RID: 15457 RVA: 0x001204D8 File Offset: 0x0011E6D8
	public static void AddIfNew<T>(this List<T> list, T item)
	{
		if (!list.Contains(item))
		{
			list.Add(item);
		}
	}

	// Token: 0x06003C62 RID: 15458 RVA: 0x001204EA File Offset: 0x0011E6EA
	public static bool InRoom(this NetPlayer player)
	{
		return NetworkSystem.Instance.InRoom && NetworkSystem.Instance.AllNetPlayers.Contains(player);
	}

	// Token: 0x06003C63 RID: 15459 RVA: 0x0012050C File Offset: 0x0011E70C
	public static bool PlayerInRoom(int actorNumber)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
			for (int i = 0; i < allNetPlayers.Length; i++)
			{
				if (allNetPlayers[i].ActorNumber == actorNumber)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06003C64 RID: 15460 RVA: 0x0012054C File Offset: 0x0011E74C
	public static bool PlayerInRoom(int actorNumer, out Player photonPlayer)
	{
		photonPlayer = null;
		return PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.Players.TryGetValue(actorNumer, out photonPlayer);
	}

	// Token: 0x06003C65 RID: 15461 RVA: 0x0012056B File Offset: 0x0011E76B
	public static bool PlayerInRoom(int actorNumber, out NetPlayer player)
	{
		if (NetworkSystem.Instance == null)
		{
			player = null;
			return false;
		}
		player = NetworkSystem.Instance.GetPlayer(actorNumber);
		return NetworkSystem.Instance.InRoom && player != null;
	}

	// Token: 0x06003C66 RID: 15462 RVA: 0x001205A0 File Offset: 0x0011E7A0
	public static long PackVector3ToLong(Vector3 vector)
	{
		long num = (long)Mathf.Clamp(Mathf.RoundToInt(vector.x * 1024f) + 1048576, 0, 2097151);
		long num2 = (long)Mathf.Clamp(Mathf.RoundToInt(vector.y * 1024f) + 1048576, 0, 2097151);
		long num3 = (long)Mathf.Clamp(Mathf.RoundToInt(vector.z * 1024f) + 1048576, 0, 2097151);
		return num + (num2 << 21) + (num3 << 42);
	}

	// Token: 0x06003C67 RID: 15463 RVA: 0x00120624 File Offset: 0x0011E824
	public static Vector3 UnpackVector3FromLong(long data)
	{
		float num = (float)(data & 2097151L);
		long num2 = (data >> 21) & 2097151L;
		long num3 = (data >> 42) & 2097151L;
		return new Vector3((float)((long)num - 1048576L) * 0.0009765625f, (float)(num2 - 1048576L) * 0.0009765625f, (float)(num3 - 1048576L) * 0.0009765625f);
	}

	// Token: 0x06003C68 RID: 15464 RVA: 0x00120682 File Offset: 0x0011E882
	public static bool IsASCIILetterOrDigit(char c)
	{
		return (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z');
	}

	// Token: 0x06003C69 RID: 15465 RVA: 0x000023F4 File Offset: 0x000005F4
	public static void Log(object message)
	{
	}

	// Token: 0x06003C6A RID: 15466 RVA: 0x000023F4 File Offset: 0x000005F4
	public static void Log(object message, Object context)
	{
	}

	// Token: 0x06003C6B RID: 15467 RVA: 0x001206AC File Offset: 0x0011E8AC
	public static bool ValidateServerTime(double time, double maximumLatency)
	{
		double simTime = NetworkSystem.Instance.SimTime;
		double num = 4294967.295 - maximumLatency;
		double num2;
		if (simTime > maximumLatency || time < maximumLatency)
		{
			if (time > simTime)
			{
				return false;
			}
			num2 = simTime - time;
		}
		else
		{
			double num3 = num + simTime;
			if (time > simTime && time < num3)
			{
				return false;
			}
			num2 = simTime + (4294967.295 - time);
		}
		return num2 <= maximumLatency;
	}

	// Token: 0x06003C6C RID: 15468 RVA: 0x0012070C File Offset: 0x0011E90C
	public static double CalculateNetworkDeltaTime(double prevTime, double newTime)
	{
		if (newTime >= prevTime)
		{
			return newTime - prevTime;
		}
		double num = 4294967.295 - prevTime;
		return newTime + num;
	}

	// Token: 0x0400406C RID: 16492
	private static ObjectPool<PooledList<IPreDisable>> g_listPool = new ObjectPool<PooledList<IPreDisable>>(2, 10);

	// Token: 0x0400406D RID: 16493
	private static StringBuilder reusableSB = new StringBuilder();
}
