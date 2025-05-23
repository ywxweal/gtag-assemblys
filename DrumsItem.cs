using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;

// Token: 0x020003FF RID: 1023
public class DrumsItem : MonoBehaviour, ISpawnable
{
	// Token: 0x170002BF RID: 703
	// (get) Token: 0x060018BE RID: 6334 RVA: 0x00077E4E File Offset: 0x0007604E
	// (set) Token: 0x060018BF RID: 6335 RVA: 0x00077E56 File Offset: 0x00076056
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170002C0 RID: 704
	// (get) Token: 0x060018C0 RID: 6336 RVA: 0x00077E5F File Offset: 0x0007605F
	// (set) Token: 0x060018C1 RID: 6337 RVA: 0x00077E67 File Offset: 0x00076067
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060018C2 RID: 6338 RVA: 0x00077E70 File Offset: 0x00076070
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		this.leftHandIndicator = GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.rightHandIndicator = GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.sphereRadius = this.leftHandIndicator.GetComponent<SphereCollider>().radius;
		for (int i = 0; i < this.collidersForThisDrum.Length; i++)
		{
			this.collidersForThisDrumList.Add(this.collidersForThisDrum[i]);
		}
		for (int j = 0; j < this.drumsAS.Length; j++)
		{
			this.myRig.AssignDrumToMusicDrums(j + this.onlineOffset, this.drumsAS[j]);
		}
	}

	// Token: 0x060018C3 RID: 6339 RVA: 0x000023F4 File Offset: 0x000005F4
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060018C4 RID: 6340 RVA: 0x00077F18 File Offset: 0x00076118
	private void LateUpdate()
	{
		this.CheckHandHit(ref this.leftHandIn, ref this.leftHandIndicator, true);
		this.CheckHandHit(ref this.rightHandIn, ref this.rightHandIndicator, false);
	}

	// Token: 0x060018C5 RID: 6341 RVA: 0x00077F40 File Offset: 0x00076140
	private void CheckHandHit(ref bool handIn, ref GorillaTriggerColliderHandIndicator handIndicator, bool isLeftHand)
	{
		this.spherecastSweep = handIndicator.transform.position - handIndicator.lastPosition;
		if (this.spherecastSweep.magnitude < 0.0001f)
		{
			this.spherecastSweep = Vector3.up * 0.0001f;
		}
		for (int i = 0; i < this.collidersHit.Length; i++)
		{
			this.collidersHit[i] = this.nullHit;
		}
		this.collidersHitCount = Physics.SphereCastNonAlloc(handIndicator.lastPosition, this.sphereRadius, this.spherecastSweep.normalized, this.collidersHit, this.spherecastSweep.magnitude, this.drumsTouchable, QueryTriggerInteraction.Collide);
		this.drumHit = false;
		if (this.collidersHitCount > 0)
		{
			this.hitList.Clear();
			for (int j = 0; j < this.collidersHit.Length; j++)
			{
				if (this.collidersHit[j].collider != null && this.collidersForThisDrumList.Contains(this.collidersHit[j].collider) && this.collidersHit[j].collider.gameObject.activeSelf)
				{
					this.hitList.Add(this.collidersHit[j]);
				}
			}
			this.hitList.Sort(new Comparison<RaycastHit>(this.RayCastHitCompare));
			int k = 0;
			while (k < this.hitList.Count)
			{
				this.tempDrum = this.hitList[k].collider.GetComponent<Drum>();
				if (this.tempDrum != null)
				{
					this.drumHit = true;
					if (!handIn && !this.tempDrum.disabler)
					{
						this.DrumHit(this.tempDrum, isLeftHand, handIndicator.currentVelocity.magnitude);
						break;
					}
					break;
				}
				else
				{
					k++;
				}
			}
		}
		if (!this.drumHit & handIn)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration);
		}
		handIn = this.drumHit;
	}

	// Token: 0x060018C6 RID: 6342 RVA: 0x0007815B File Offset: 0x0007635B
	private int RayCastHitCompare(RaycastHit a, RaycastHit b)
	{
		if (a.distance < b.distance)
		{
			return -1;
		}
		if (a.distance == b.distance)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x060018C7 RID: 6343 RVA: 0x00078184 File Offset: 0x00076384
	public void DrumHit(Drum tempDrumInner, bool isLeftHand, float hitVelocity)
	{
		if (isLeftHand)
		{
			if (this.leftHandIn)
			{
				return;
			}
			this.leftHandIn = true;
		}
		else
		{
			if (this.rightHandIn)
			{
				return;
			}
			this.rightHandIn = true;
		}
		this.volToPlay = Mathf.Max(Mathf.Min(1f, hitVelocity / this.maxDrumVolumeVelocity) * this.maxDrumVolume, this.minDrumVolume);
		if (NetworkSystem.Instance.InRoom)
		{
			if (!this.myRig.isOfflineVRRig)
			{
				NetworkView netView = this.myRig.netView;
				if (netView != null)
				{
					netView.SendRPC("RPC_PlayDrum", RpcTarget.Others, new object[]
					{
						tempDrumInner.myIndex + this.onlineOffset,
						this.volToPlay
					});
				}
			}
			else
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayDrum", RpcTarget.Others, new object[]
				{
					tempDrumInner.myIndex + this.onlineOffset,
					this.volToPlay
				});
			}
		}
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 4f, GorillaTagger.Instance.tapHapticDuration);
		this.drumsAS[tempDrumInner.myIndex].volume = this.maxDrumVolume;
		this.drumsAS[tempDrumInner.myIndex].GTPlayOneShot(this.drumsAS[tempDrumInner.myIndex].clip, this.volToPlay);
	}

	// Token: 0x04001B84 RID: 7044
	[Tooltip("Array of colliders for this specific drum.")]
	public Collider[] collidersForThisDrum;

	// Token: 0x04001B85 RID: 7045
	private List<Collider> collidersForThisDrumList = new List<Collider>();

	// Token: 0x04001B86 RID: 7046
	[Tooltip("AudioSources where each index must match the index given to the corresponding Drum component.")]
	public AudioSource[] drumsAS;

	// Token: 0x04001B87 RID: 7047
	[Tooltip("Max volume a drum can reach.")]
	public float maxDrumVolume = 0.2f;

	// Token: 0x04001B88 RID: 7048
	[Tooltip("Min volume a drum can reach.")]
	public float minDrumVolume = 0.05f;

	// Token: 0x04001B89 RID: 7049
	[Tooltip("Multiplies against actual velocity before capping by min & maxDrumVolume values.")]
	public float maxDrumVolumeVelocity = 1f;

	// Token: 0x04001B8A RID: 7050
	private bool rightHandIn;

	// Token: 0x04001B8B RID: 7051
	private bool leftHandIn;

	// Token: 0x04001B8C RID: 7052
	private float volToPlay;

	// Token: 0x04001B8D RID: 7053
	private GorillaTriggerColliderHandIndicator rightHandIndicator;

	// Token: 0x04001B8E RID: 7054
	private GorillaTriggerColliderHandIndicator leftHandIndicator;

	// Token: 0x04001B8F RID: 7055
	private RaycastHit[] collidersHit = new RaycastHit[20];

	// Token: 0x04001B90 RID: 7056
	private Collider[] actualColliders = new Collider[20];

	// Token: 0x04001B91 RID: 7057
	public LayerMask drumsTouchable;

	// Token: 0x04001B92 RID: 7058
	private float sphereRadius;

	// Token: 0x04001B93 RID: 7059
	private Vector3 spherecastSweep;

	// Token: 0x04001B94 RID: 7060
	private int collidersHitCount;

	// Token: 0x04001B95 RID: 7061
	private List<RaycastHit> hitList = new List<RaycastHit>(20);

	// Token: 0x04001B96 RID: 7062
	private Drum tempDrum;

	// Token: 0x04001B97 RID: 7063
	private bool drumHit;

	// Token: 0x04001B98 RID: 7064
	private RaycastHit nullHit;

	// Token: 0x04001B99 RID: 7065
	public int onlineOffset;

	// Token: 0x04001B9A RID: 7066
	[Tooltip("VRRig object of the player, used to determine if it is an offline rig.")]
	private VRRig myRig;
}
