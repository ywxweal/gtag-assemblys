using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020003F9 RID: 1017
public class ChestObjectHysteresis : MonoBehaviour, ISpawnable
{
	// Token: 0x170002BB RID: 699
	// (get) Token: 0x06001883 RID: 6275 RVA: 0x00077239 File Offset: 0x00075439
	// (set) Token: 0x06001884 RID: 6276 RVA: 0x00077241 File Offset: 0x00075441
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170002BC RID: 700
	// (get) Token: 0x06001885 RID: 6277 RVA: 0x0007724A File Offset: 0x0007544A
	// (set) Token: 0x06001886 RID: 6278 RVA: 0x00077252 File Offset: 0x00075452
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001887 RID: 6279 RVA: 0x0007725C File Offset: 0x0007545C
	void ISpawnable.OnSpawn(VRRig rig)
	{
		if (!this.angleFollower && (string.IsNullOrEmpty(this.angleFollower_path) || base.transform.TryFindByPath(this.angleFollower_path, out this.angleFollower, false)))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"ChestObjectHysteresis: DEACTIVATING! Could not find `angleFollower` using path: \"",
				this.angleFollower_path,
				"\". For component at: \"",
				this.GetComponentPath(int.MaxValue),
				"\""
			}), this);
			base.gameObject.SetActive(false);
			return;
		}
	}

	// Token: 0x06001888 RID: 6280 RVA: 0x000023F4 File Offset: 0x000005F4
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001889 RID: 6281 RVA: 0x000772EA File Offset: 0x000754EA
	private void Start()
	{
		this.lastAngleQuat = base.transform.rotation;
		this.currentAngleQuat = base.transform.rotation;
	}

	// Token: 0x0600188A RID: 6282 RVA: 0x0007730E File Offset: 0x0007550E
	private void OnEnable()
	{
		ChestObjectHysteresisManager.RegisterCH(this);
	}

	// Token: 0x0600188B RID: 6283 RVA: 0x00077316 File Offset: 0x00075516
	private void OnDisable()
	{
		ChestObjectHysteresisManager.UnregisterCH(this);
	}

	// Token: 0x0600188C RID: 6284 RVA: 0x00077320 File Offset: 0x00075520
	public void InvokeUpdate()
	{
		this.currentAngleQuat = this.angleFollower.rotation;
		this.angleBetween = Quaternion.Angle(this.currentAngleQuat, this.lastAngleQuat);
		if (this.angleBetween > this.angleHysteresis)
		{
			base.transform.rotation = Quaternion.Slerp(this.currentAngleQuat, this.lastAngleQuat, this.angleHysteresis / this.angleBetween);
			this.lastAngleQuat = base.transform.rotation;
		}
		base.transform.rotation = this.lastAngleQuat;
	}

	// Token: 0x04001B52 RID: 6994
	public float angleHysteresis;

	// Token: 0x04001B53 RID: 6995
	public float angleBetween;

	// Token: 0x04001B54 RID: 6996
	public Transform angleFollower;

	// Token: 0x04001B55 RID: 6997
	[Delayed]
	public string angleFollower_path;

	// Token: 0x04001B56 RID: 6998
	private Quaternion lastAngleQuat;

	// Token: 0x04001B57 RID: 6999
	private Quaternion currentAngleQuat;
}
