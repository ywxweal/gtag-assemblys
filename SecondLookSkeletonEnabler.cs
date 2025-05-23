using System;
using UnityEngine;

// Token: 0x020000D4 RID: 212
public class SecondLookSkeletonEnabler : Tappable
{
	// Token: 0x06000543 RID: 1347 RVA: 0x0001ECD7 File Offset: 0x0001CED7
	private void Awake()
	{
		this.isTapped = false;
		this.skele = Object.FindFirstObjectByType<SecondLookSkeleton>();
		this.skele.spookyText = this.spookyText;
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x0001ECFC File Offset: 0x0001CEFC
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		if (!this.isTapped)
		{
			base.OnTapLocal(tapStrength, tapTime, info);
			if (this.skele != null)
			{
				this.skele.tapped = true;
			}
			base.gameObject.SetActive(false);
			this.isTapped = true;
			this.playOnDisappear.GTPlay();
			this.particles.Play();
		}
	}

	// Token: 0x04000636 RID: 1590
	public bool isTapped;

	// Token: 0x04000637 RID: 1591
	public AudioSource playOnDisappear;

	// Token: 0x04000638 RID: 1592
	public ParticleSystem particles;

	// Token: 0x04000639 RID: 1593
	public GameObject spookyText;

	// Token: 0x0400063A RID: 1594
	private SecondLookSkeleton skele;
}
