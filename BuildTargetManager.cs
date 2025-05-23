using System;
using UnityEngine;

// Token: 0x0200095A RID: 2394
public class BuildTargetManager : MonoBehaviour
{
	// Token: 0x060039FD RID: 14845 RVA: 0x0011696B File Offset: 0x00114B6B
	public string GetPath()
	{
		return this.path;
	}

	// Token: 0x04003EFD RID: 16125
	public BuildTargetManager.BuildTowards newBuildTarget;

	// Token: 0x04003EFE RID: 16126
	public bool isBeta;

	// Token: 0x04003EFF RID: 16127
	public bool isQA;

	// Token: 0x04003F00 RID: 16128
	public bool spoofIDs;

	// Token: 0x04003F01 RID: 16129
	public bool spoofChild;

	// Token: 0x04003F02 RID: 16130
	public bool enableAllCosmetics;

	// Token: 0x04003F03 RID: 16131
	public OVRManager ovrManager;

	// Token: 0x04003F04 RID: 16132
	private string path = "Assets/csc.rsp";

	// Token: 0x04003F05 RID: 16133
	public BuildTargetManager.BuildTowards currentBuildTargetDONOTCHANGE;

	// Token: 0x04003F06 RID: 16134
	public GorillaTagger gorillaTagger;

	// Token: 0x04003F07 RID: 16135
	public GameObject[] betaDisableObjects;

	// Token: 0x04003F08 RID: 16136
	public GameObject[] betaEnableObjects;

	// Token: 0x04003F09 RID: 16137
	public BuildTargetManager.NetworkBackend networkBackend;

	// Token: 0x0200095B RID: 2395
	public enum BuildTowards
	{
		// Token: 0x04003F0B RID: 16139
		Steam,
		// Token: 0x04003F0C RID: 16140
		OculusPC,
		// Token: 0x04003F0D RID: 16141
		Quest,
		// Token: 0x04003F0E RID: 16142
		Viveport
	}

	// Token: 0x0200095C RID: 2396
	public enum NetworkBackend
	{
		// Token: 0x04003F10 RID: 16144
		Pun,
		// Token: 0x04003F11 RID: 16145
		Fusion
	}
}
