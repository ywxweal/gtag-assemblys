using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ExitGames.Client.Photon;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200029D RID: 669
public static class NetCrossoverUtils
{
	// Token: 0x06000F96 RID: 3990 RVA: 0x0004EA1D File Offset: 0x0004CC1D
	public static void Prewarm()
	{
		NetCrossoverUtils.FixedBuffer = new byte[2048];
	}

	// Token: 0x06000F97 RID: 3991 RVA: 0x0004EA30 File Offset: 0x0004CC30
	public static void WriteNetDataToBuffer<T>(this T data, PhotonStream stream) where T : struct, INetworkStruct
	{
		if (stream.IsReading)
		{
			Debug.LogError("Attempted to write data to a reading stream!");
			return;
		}
		IntPtr intPtr = 0;
		try
		{
			int num = Marshal.SizeOf(typeof(T));
			intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr<T>(data, intPtr, true);
			Marshal.Copy(intPtr, NetCrossoverUtils.FixedBuffer, 0, num);
			stream.SendNext(num);
			for (int i = 0; i < num; i++)
			{
				stream.SendNext(NetCrossoverUtils.FixedBuffer[i]);
			}
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	// Token: 0x06000F98 RID: 3992 RVA: 0x0004EAC8 File Offset: 0x0004CCC8
	public static object ReadNetDataFromBuffer<T>(PhotonStream stream) where T : struct, INetworkStruct
	{
		if (stream.IsWriting)
		{
			Debug.LogError("Attmpted to read data from a writing stream!");
			return null;
		}
		IntPtr intPtr = 0;
		object obj;
		try
		{
			Type typeFromHandle = typeof(T);
			int num = (int)stream.ReceiveNext();
			int num2 = Marshal.SizeOf(typeFromHandle);
			if (num != num2)
			{
				Debug.LogError(string.Format("Expected datasize {0} when reading data for type '{1}',", num2, typeFromHandle.Name) + string.Format("but {0} data is available!", num));
				obj = null;
			}
			else
			{
				intPtr = Marshal.AllocHGlobal(num2);
				for (int i = 0; i < num2; i++)
				{
					NetCrossoverUtils.FixedBuffer[i] = (byte)stream.ReceiveNext();
				}
				Marshal.Copy(NetCrossoverUtils.FixedBuffer, 0, intPtr, num2);
				obj = (T)((object)Marshal.PtrToStructure(intPtr, typeFromHandle));
			}
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
		return obj;
	}

	// Token: 0x06000F99 RID: 3993 RVA: 0x0004EBB0 File Offset: 0x0004CDB0
	public static void WriteNetDataToBuffer(this object data, PhotonStream stream)
	{
		if (stream.IsReading)
		{
			Debug.LogError("Attempted to write data to a reading stream!");
			return;
		}
		IntPtr intPtr = 0;
		try
		{
			int num = Marshal.SizeOf(data.GetType());
			intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr(data, intPtr, true);
			Marshal.Copy(intPtr, NetCrossoverUtils.FixedBuffer, 0, num);
			stream.SendNext(num);
			for (int i = 0; i < num; i++)
			{
				stream.SendNext(NetCrossoverUtils.FixedBuffer[i]);
			}
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	// Token: 0x06000F9A RID: 3994 RVA: 0x0004EC44 File Offset: 0x0004CE44
	public static void SerializeToRPCData<T>(this RPCArgBuffer<T> argBuffer) where T : struct
	{
		IntPtr intPtr = 0;
		try
		{
			int num = Marshal.SizeOf(typeof(T));
			intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr<T>(argBuffer.Args, intPtr, true);
			Marshal.Copy(intPtr, argBuffer.Data, 0, num);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	// Token: 0x06000F9B RID: 3995 RVA: 0x0004ECA4 File Offset: 0x0004CEA4
	public static void PopulateWithRPCData<T>(this RPCArgBuffer<T> argBuffer, byte[] data) where T : struct
	{
		IntPtr intPtr = 0;
		try
		{
			int num = Marshal.SizeOf(typeof(T));
			intPtr = Marshal.AllocHGlobal(num);
			Marshal.Copy(data, 0, intPtr, num);
			argBuffer.Args = Marshal.PtrToStructure<T>(intPtr);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	// Token: 0x06000F9C RID: 3996 RVA: 0x0004ED00 File Offset: 0x0004CF00
	public static Dictionary<string, SessionProperty> ToPropDict(this Hashtable hash)
	{
		Dictionary<string, SessionProperty> dictionary = new Dictionary<string, SessionProperty>();
		foreach (DictionaryEntry dictionaryEntry in hash)
		{
			dictionary.Add((string)dictionaryEntry.Key, (string)dictionaryEntry.Value);
		}
		return dictionary;
	}

	// Token: 0x04001281 RID: 4737
	private const int MaxParameterByteLength = 2048;

	// Token: 0x04001282 RID: 4738
	private static byte[] FixedBuffer;
}
