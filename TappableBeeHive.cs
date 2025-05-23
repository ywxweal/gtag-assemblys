using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020006BB RID: 1723
public class TappableBeeHive : Tappable
{
	// Token: 0x06002AFF RID: 11007 RVA: 0x000D3944 File Offset: 0x000D1B44
	private void Awake()
	{
		if (this.swarmEmergeFromPoint == null || this.swarmEmergeToPoint == null)
		{
			Debug.LogError("TappableBeeHive: Disabling because swarmEmergePoint is null at: " + base.transform.GetPath(), this);
			base.enabled = false;
			return;
		}
		base.GetComponent<SlingshotProjectileHitNotifier>().OnProjectileHit += this.OnSlingshotHit;
	}

	// Token: 0x06002B00 RID: 11008 RVA: 0x000D39A8 File Offset: 0x000D1BA8
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.swarmEmergeFromPoint == null || this.swarmEmergeToPoint == null)
		{
			return;
		}
		if (NetworkSystem.Instance.IsMasterClient && AngryBeeSwarm.instance.isDormant)
		{
			AngryBeeSwarm.instance.Emerge(this.swarmEmergeFromPoint.transform.position, this.swarmEmergeToPoint.transform.position);
		}
	}

	// Token: 0x06002B01 RID: 11009 RVA: 0x000D3A1C File Offset: 0x000D1C1C
	public void OnSlingshotHit(SlingshotProjectile projectile, Collision collision)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.swarmEmergeFromPoint == null || this.swarmEmergeToPoint == null)
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient && AngryBeeSwarm.instance.isDormant)
		{
			AngryBeeSwarm.instance.Emerge(this.swarmEmergeFromPoint.transform.position, this.swarmEmergeToPoint.transform.position);
		}
	}

	// Token: 0x04002FF4 RID: 12276
	[SerializeField]
	private GameObject swarmEmergeFromPoint;

	// Token: 0x04002FF5 RID: 12277
	[SerializeField]
	private GameObject swarmEmergeToPoint;

	// Token: 0x04002FF6 RID: 12278
	[SerializeField]
	private GameObject honeycombSurface;

	// Token: 0x04002FF7 RID: 12279
	[SerializeField]
	private float honeycombDisableDuration;

	// Token: 0x04002FF8 RID: 12280
	[NonSerialized]
	private TimeSince _timeSinceLastTap;

	// Token: 0x04002FF9 RID: 12281
	private float reenableHoneycombAtTimestamp;

	// Token: 0x04002FFA RID: 12282
	private Coroutine reenableHoneycombCoroutine;
}
