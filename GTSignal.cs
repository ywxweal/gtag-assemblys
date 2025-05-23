using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000651 RID: 1617
public static class GTSignal
{
	// Token: 0x06002859 RID: 10329 RVA: 0x000C91E0 File Offset: 0x000C73E0
	private static void _Emit(GTSignal.EmitMode mode, int signalID, object[] data)
	{
		object[] array = GTSignal._ToEventContent(signalID, PhotonNetwork.Time, data);
		PhotonNetwork.RaiseEvent(186, array, GTSignal.gTargetsToOptions[mode], GTSignal.gSendOptions);
	}

	// Token: 0x0600285A RID: 10330 RVA: 0x000C9218 File Offset: 0x000C7418
	private static void _Emit(int[] targetActors, int signalID, object[] data)
	{
		if (targetActors.IsNullOrEmpty<int>())
		{
			return;
		}
		GTSignal.gCustomTargetOptions.TargetActors = targetActors;
		object[] array = GTSignal._ToEventContent(signalID, PhotonNetwork.Time, data);
		PhotonNetwork.RaiseEvent(186, array, GTSignal.gCustomTargetOptions, GTSignal.gSendOptions);
	}

	// Token: 0x0600285B RID: 10331 RVA: 0x000C925C File Offset: 0x000C745C
	private static object[] _ToEventContent(int signalID, double time, object[] data)
	{
		int num = data.Length;
		int num2 = num + 2;
		object[] array;
		if (!GTSignal.gLengthToContentArray.TryGetValue(num2, out array))
		{
			array = new object[num2];
			GTSignal.gLengthToContentArray.Add(num2, array);
		}
		array[0] = signalID;
		array[1] = time;
		for (int i = 0; i < num; i++)
		{
			array[i + 2] = data[i];
		}
		return array;
	}

	// Token: 0x0600285C RID: 10332 RVA: 0x000C92BA File Offset: 0x000C74BA
	public static int ComputeID(string s)
	{
		if (!string.IsNullOrWhiteSpace(s))
		{
			return XXHash32.Compute(s.Trim(), 0U);
		}
		return 0;
	}

	// Token: 0x0600285D RID: 10333 RVA: 0x000C92D4 File Offset: 0x000C74D4
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitializeOnLoad()
	{
		GTSignal.gCustomTargetOptions = RaiseEventOptions.Default;
		GTSignal.gSendOptions = SendOptions.SendReliable;
		GTSignal.gSendOptions.Encrypt = true;
		GTSignal.gTargetsToOptions = new Dictionary<GTSignal.EmitMode, RaiseEventOptions>(3);
		RaiseEventOptions @default = RaiseEventOptions.Default;
		@default.Receivers = ReceiverGroup.All;
		GTSignal.gTargetsToOptions.Add(GTSignal.EmitMode.All, @default);
		RaiseEventOptions default2 = RaiseEventOptions.Default;
		default2.Receivers = ReceiverGroup.Others;
		GTSignal.gTargetsToOptions.Add(GTSignal.EmitMode.Others, default2);
		RaiseEventOptions default3 = RaiseEventOptions.Default;
		default3.Receivers = ReceiverGroup.MasterClient;
		GTSignal.gTargetsToOptions.Add(GTSignal.EmitMode.Host, default3);
	}

	// Token: 0x0600285E RID: 10334 RVA: 0x000C9356 File Offset: 0x000C7556
	public static void Emit(string signal, params object[] data)
	{
		GTSignal._Emit(GTSignal.EmitMode.All, GTSignal.ComputeID(signal), data);
	}

	// Token: 0x0600285F RID: 10335 RVA: 0x000C9365 File Offset: 0x000C7565
	public static void Emit(GTSignal.EmitMode mode, string signal, params object[] data)
	{
		GTSignal._Emit(mode, GTSignal.ComputeID(signal), data);
	}

	// Token: 0x06002860 RID: 10336 RVA: 0x000C9374 File Offset: 0x000C7574
	public static void Emit(int signalID, params object[] data)
	{
		GTSignal._Emit(GTSignal.EmitMode.All, signalID, data);
	}

	// Token: 0x06002861 RID: 10337 RVA: 0x000C937E File Offset: 0x000C757E
	public static void Emit(GTSignal.EmitMode mode, int signalID, params object[] data)
	{
		GTSignal._Emit(mode, signalID, data);
	}

	// Token: 0x06002862 RID: 10338 RVA: 0x000C9388 File Offset: 0x000C7588
	public static void Emit(int target, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[1];
		array[0] = target;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06002863 RID: 10339 RVA: 0x000C93A0 File Offset: 0x000C75A0
	public static void Emit(int target1, int target2, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[2];
		array[0] = target1;
		array[1] = target2;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06002864 RID: 10340 RVA: 0x000C93BC File Offset: 0x000C75BC
	public static void Emit(int target1, int target2, int target3, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[3];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06002865 RID: 10341 RVA: 0x000C93DD File Offset: 0x000C75DD
	public static void Emit(int target1, int target2, int target3, int target4, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[4];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06002866 RID: 10342 RVA: 0x000C9403 File Offset: 0x000C7603
	public static void Emit(int target1, int target2, int target3, int target4, int target5, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[5];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		array[4] = target5;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06002867 RID: 10343 RVA: 0x000C942E File Offset: 0x000C762E
	public static void Emit(int target1, int target2, int target3, int target4, int target5, int target6, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[6];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		array[4] = target5;
		array[5] = target6;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06002868 RID: 10344 RVA: 0x000C945E File Offset: 0x000C765E
	public static void Emit(int target1, int target2, int target3, int target4, int target5, int target6, int target7, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[7];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		array[4] = target5;
		array[5] = target6;
		array[6] = target7;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x06002869 RID: 10345 RVA: 0x000C9493 File Offset: 0x000C7693
	public static void Emit(int target1, int target2, int target3, int target4, int target5, int target6, int target7, int target8, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[8];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		array[4] = target5;
		array[5] = target6;
		array[6] = target7;
		array[7] = target8;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x0600286A RID: 10346 RVA: 0x000C94CD File Offset: 0x000C76CD
	public static void Emit(int target1, int target2, int target3, int target4, int target5, int target6, int target7, int target8, int target9, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[9];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		array[4] = target5;
		array[5] = target6;
		array[6] = target7;
		array[7] = target8;
		array[8] = target9;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x0600286B RID: 10347 RVA: 0x000C9510 File Offset: 0x000C7710
	public static void Emit(int target1, int target2, int target3, int target4, int target5, int target6, int target7, int target8, int target9, int target10, int signalID, params object[] data)
	{
		int[] array = GTSignal.gLengthToTargetsArray[10];
		array[0] = target1;
		array[1] = target2;
		array[2] = target3;
		array[3] = target4;
		array[4] = target5;
		array[5] = target6;
		array[6] = target7;
		array[7] = target8;
		array[8] = target9;
		array[9] = target10;
		GTSignal._Emit(array, signalID, data);
	}

	// Token: 0x0600286C RID: 10348 RVA: 0x000C9564 File Offset: 0x000C7764
	// Note: this type is marked as 'beforefieldinit'.
	static GTSignal()
	{
		Dictionary<int, object[]> dictionary = new Dictionary<int, object[]>();
		dictionary[1] = new object[1];
		dictionary[2] = new object[2];
		dictionary[3] = new object[3];
		dictionary[4] = new object[4];
		dictionary[5] = new object[5];
		dictionary[6] = new object[6];
		dictionary[7] = new object[7];
		dictionary[8] = new object[8];
		dictionary[9] = new object[9];
		dictionary[10] = new object[10];
		dictionary[11] = new object[11];
		dictionary[12] = new object[12];
		dictionary[13] = new object[13];
		dictionary[14] = new object[14];
		dictionary[15] = new object[15];
		dictionary[16] = new object[16];
		GTSignal.gLengthToContentArray = dictionary;
		Dictionary<int, int[]> dictionary2 = new Dictionary<int, int[]>();
		dictionary2[1] = new int[1];
		dictionary2[2] = new int[2];
		dictionary2[3] = new int[3];
		dictionary2[4] = new int[4];
		dictionary2[5] = new int[5];
		dictionary2[6] = new int[6];
		dictionary2[7] = new int[7];
		dictionary2[8] = new int[8];
		dictionary2[9] = new int[9];
		dictionary2[10] = new int[10];
		GTSignal.gLengthToTargetsArray = dictionary2;
	}

	// Token: 0x04002D3D RID: 11581
	public const byte PHOTON_CODE = 186;

	// Token: 0x04002D3E RID: 11582
	private static Dictionary<GTSignal.EmitMode, RaiseEventOptions> gTargetsToOptions;

	// Token: 0x04002D3F RID: 11583
	private static Dictionary<int, object[]> gLengthToContentArray;

	// Token: 0x04002D40 RID: 11584
	private static Dictionary<int, int[]> gLengthToTargetsArray;

	// Token: 0x04002D41 RID: 11585
	private static SendOptions gSendOptions;

	// Token: 0x04002D42 RID: 11586
	private static RaiseEventOptions gCustomTargetOptions;

	// Token: 0x02000652 RID: 1618
	public enum EmitMode
	{
		// Token: 0x04002D44 RID: 11588
		None = -1,
		// Token: 0x04002D45 RID: 11589
		Others,
		// Token: 0x04002D46 RID: 11590
		Targets,
		// Token: 0x04002D47 RID: 11591
		All,
		// Token: 0x04002D48 RID: 11592
		Host
	}
}
