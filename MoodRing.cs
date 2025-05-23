using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000461 RID: 1121
public class MoodRing : MonoBehaviour, ISpawnable
{
	// Token: 0x170002FB RID: 763
	// (get) Token: 0x06001B86 RID: 7046 RVA: 0x0008723B File Offset: 0x0008543B
	// (set) Token: 0x06001B87 RID: 7047 RVA: 0x00087243 File Offset: 0x00085443
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170002FC RID: 764
	// (get) Token: 0x06001B88 RID: 7048 RVA: 0x0008724C File Offset: 0x0008544C
	// (set) Token: 0x06001B89 RID: 7049 RVA: 0x00087254 File Offset: 0x00085454
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001B8A RID: 7050 RVA: 0x000023F4 File Offset: 0x000005F4
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001B8B RID: 7051 RVA: 0x0008725D File Offset: 0x0008545D
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06001B8C RID: 7052 RVA: 0x00087268 File Offset: 0x00085468
	private void Update()
	{
		if ((this.attachedToLeftHand ? this.myRig.leftIndex.calcT : this.myRig.rightIndex.calcT) > 0.5f)
		{
			if (!this.isCycling)
			{
				this.animRedValue = this.myRig.playerColor.r;
				this.animGreenValue = this.myRig.playerColor.g;
				this.animBlueValue = this.myRig.playerColor.b;
			}
			this.isCycling = true;
			this.RainbowCycle(ref this.animRedValue, ref this.animGreenValue, ref this.animBlueValue);
			this.myRig.InitializeNoobMaterialLocal(this.animRedValue, this.animGreenValue, this.animBlueValue);
			return;
		}
		if (this.isCycling)
		{
			this.isCycling = false;
			if (this.myRig.isOfflineVRRig)
			{
				this.animRedValue = Mathf.Round(this.animRedValue * 9f) / 9f;
				this.animGreenValue = Mathf.Round(this.animGreenValue * 9f) / 9f;
				this.animBlueValue = Mathf.Round(this.animBlueValue * 9f) / 9f;
				GorillaTagger.Instance.UpdateColor(this.animRedValue, this.animGreenValue, this.animBlueValue);
				if (NetworkSystem.Instance.InRoom)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { this.animRedValue, this.animGreenValue, this.animBlueValue });
				}
				PlayerPrefs.SetFloat("redValue", this.animRedValue);
				PlayerPrefs.SetFloat("greenValue", this.animGreenValue);
				PlayerPrefs.SetFloat("blueValue", this.animBlueValue);
				PlayerPrefs.Save();
			}
		}
	}

	// Token: 0x06001B8D RID: 7053 RVA: 0x0008744C File Offset: 0x0008564C
	private void RainbowCycle(ref float r, ref float g, ref float b)
	{
		float num = this.furCycleSpeed * Time.deltaTime;
		if (r == 1f)
		{
			if (b > 0f)
			{
				b = Mathf.Clamp01(b - num);
				return;
			}
			if (g < 1f)
			{
				g = Mathf.Clamp01(g + num);
				return;
			}
			r = Mathf.Clamp01(r - num);
			return;
		}
		else if (g == 1f)
		{
			if (r > 0f)
			{
				r = Mathf.Clamp01(r - num);
				return;
			}
			if (b < 1f)
			{
				b = Mathf.Clamp01(b + num);
				return;
			}
			g = Mathf.Clamp01(g - num);
			return;
		}
		else
		{
			if (b != 1f)
			{
				r = Mathf.Clamp01(r + num);
				return;
			}
			if (g > 0f)
			{
				g = Mathf.Clamp01(g - num);
				return;
			}
			if (r < 1f)
			{
				r = Mathf.Clamp01(r + num);
				return;
			}
			b = Mathf.Clamp01(b - num);
			return;
		}
	}

	// Token: 0x04001E83 RID: 7811
	[SerializeField]
	private bool attachedToLeftHand;

	// Token: 0x04001E84 RID: 7812
	private VRRig myRig;

	// Token: 0x04001E85 RID: 7813
	[SerializeField]
	private float rotationSpeed;

	// Token: 0x04001E86 RID: 7814
	[SerializeField]
	private float furCycleSpeed;

	// Token: 0x04001E87 RID: 7815
	private float nextFurCycleTimestamp;

	// Token: 0x04001E88 RID: 7816
	private float animRedValue;

	// Token: 0x04001E89 RID: 7817
	private float animGreenValue;

	// Token: 0x04001E8A RID: 7818
	private float animBlueValue;

	// Token: 0x04001E8B RID: 7819
	private bool isCycling;
}
