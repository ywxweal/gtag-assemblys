using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020006F0 RID: 1776
public class GorillaFireball : GorillaThrowable, IPunInstantiateMagicCallback
{
	// Token: 0x06002C3C RID: 11324 RVA: 0x000D9F0E File Offset: 0x000D810E
	public override void Start()
	{
		base.Start();
		this.canExplode = false;
		this.explosionStartTime = 0f;
	}

	// Token: 0x06002C3D RID: 11325 RVA: 0x000D9F28 File Offset: 0x000D8128
	private void Update()
	{
		if (this.explosionStartTime != 0f)
		{
			float num = (Time.time - this.explosionStartTime) / this.totalExplosionTime * (this.maxExplosionScale - 0.25f) + 0.25f;
			base.gameObject.transform.localScale = new Vector3(num, num, num);
			if (base.photonView.IsMine && Time.time > this.explosionStartTime + this.totalExplosionTime)
			{
				PhotonNetwork.Destroy(PhotonView.Get(this));
			}
		}
	}

	// Token: 0x06002C3E RID: 11326 RVA: 0x000D9FB0 File Offset: 0x000D81B0
	public override void LateUpdate()
	{
		base.LateUpdate();
		if (this.rigidbody.useGravity)
		{
			this.rigidbody.AddForce(Physics.gravity * -this.gravityStrength * this.rigidbody.mass);
		}
	}

	// Token: 0x06002C3F RID: 11327 RVA: 0x000D9FFC File Offset: 0x000D81FC
	public override void ThrowThisThingo()
	{
		base.ThrowThisThingo();
		this.canExplode = true;
	}

	// Token: 0x06002C40 RID: 11328 RVA: 0x000DA00B File Offset: 0x000D820B
	private new void OnCollisionEnter(Collision collision)
	{
		if (base.photonView.IsMine && this.canExplode)
		{
			base.photonView.RPC("Explode", RpcTarget.All, null);
		}
	}

	// Token: 0x06002C41 RID: 11329 RVA: 0x000DA034 File Offset: 0x000D8234
	public void LocalExplode()
	{
		this.rigidbody.isKinematic = true;
		this.canExplode = false;
		this.explosionStartTime = Time.time;
	}

	// Token: 0x06002C42 RID: 11330 RVA: 0x000DA054 File Offset: 0x000D8254
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (base.photonView.IsMine)
		{
			if ((bool)base.photonView.InstantiationData[0])
			{
				base.transform.parent = GorillaPlaySpace.Instance.myVRRig.leftHandTransform;
				return;
			}
			base.transform.parent = GorillaPlaySpace.Instance.myVRRig.rightHandTransform;
		}
	}

	// Token: 0x06002C43 RID: 11331 RVA: 0x000DA0B7 File Offset: 0x000D82B7
	[PunRPC]
	public void Explode()
	{
		this.LocalExplode();
	}

	// Token: 0x04003265 RID: 12901
	public float maxExplosionScale;

	// Token: 0x04003266 RID: 12902
	public float totalExplosionTime;

	// Token: 0x04003267 RID: 12903
	public float gravityStrength;

	// Token: 0x04003268 RID: 12904
	private bool canExplode;

	// Token: 0x04003269 RID: 12905
	private float explosionStartTime;
}
