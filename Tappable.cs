using System;
using System.Diagnostics;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000691 RID: 1681
public class Tappable : MonoBehaviour
{
	// Token: 0x06002A06 RID: 10758 RVA: 0x000CFCFA File Offset: 0x000CDEFA
	public void Validate()
	{
		this.CalculateId(true);
	}

	// Token: 0x06002A07 RID: 10759 RVA: 0x000CFD03 File Offset: 0x000CDF03
	protected virtual void OnEnable()
	{
		if (!this.useStaticId)
		{
			this.CalculateId(false);
		}
		TappableManager.Register(this);
	}

	// Token: 0x06002A08 RID: 10760 RVA: 0x000CFD1A File Offset: 0x000CDF1A
	protected virtual void OnDisable()
	{
		TappableManager.Unregister(this);
	}

	// Token: 0x06002A09 RID: 10761 RVA: 0x00047642 File Offset: 0x00045842
	public virtual bool CanTap(bool isLeftHand)
	{
		return true;
	}

	// Token: 0x06002A0A RID: 10762 RVA: 0x000CFD24 File Offset: 0x000CDF24
	public void OnTap(float tapStrength)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (!this.manager)
		{
			return;
		}
		this.manager.photonView.RPC("SendOnTapRPC", RpcTarget.All, new object[] { this.tappableId, tapStrength });
	}

	// Token: 0x06002A0B RID: 10763 RVA: 0x000CFD80 File Offset: 0x000CDF80
	public void OnGrab()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (!this.manager)
		{
			return;
		}
		this.manager.photonView.RPC("SendOnGrabRPC", RpcTarget.All, new object[] { this.tappableId });
	}

	// Token: 0x06002A0C RID: 10764 RVA: 0x000CFDD4 File Offset: 0x000CDFD4
	public void OnRelease()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (!this.manager)
		{
			return;
		}
		this.manager.photonView.RPC("SendOnReleaseRPC", RpcTarget.All, new object[] { this.tappableId });
	}

	// Token: 0x06002A0D RID: 10765 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped sender)
	{
	}

	// Token: 0x06002A0E RID: 10766 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void OnGrabLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
	}

	// Token: 0x06002A0F RID: 10767 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void OnReleaseLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
	}

	// Token: 0x06002A10 RID: 10768 RVA: 0x000CFCFA File Offset: 0x000CDEFA
	private void EdRecalculateId()
	{
		this.CalculateId(true);
	}

	// Token: 0x06002A11 RID: 10769 RVA: 0x000CFE28 File Offset: 0x000CE028
	private void CalculateId(bool force = false)
	{
		Transform transform = base.transform;
		int hashCode = TransformUtils.ComputePathHash(transform).ToId128().GetHashCode();
		int staticHash = base.GetType().Name.GetStaticHash();
		int hashCode2 = transform.position.QuantizedId128().GetHashCode();
		int num = StaticHash.Compute(hashCode, staticHash, hashCode2);
		if (this.useStaticId)
		{
			if (string.IsNullOrEmpty(this.staticId) || force)
			{
				int instanceID = transform.GetInstanceID();
				int num2 = StaticHash.Compute(num, instanceID);
				this.staticId = string.Format("#ID_{0:X8}", num2);
			}
			this.tappableId = this.staticId.GetStaticHash();
			return;
		}
		this.tappableId = (Application.isPlaying ? num : 0);
	}

	// Token: 0x06002A12 RID: 10770 RVA: 0x000CFEF2 File Offset: 0x000CE0F2
	[Conditional("UNITY_EDITOR")]
	private void OnValidate()
	{
		this.CalculateId(false);
	}

	// Token: 0x04002F24 RID: 12068
	public int tappableId;

	// Token: 0x04002F25 RID: 12069
	public string staticId;

	// Token: 0x04002F26 RID: 12070
	public bool useStaticId;

	// Token: 0x04002F27 RID: 12071
	[Tooltip("If true, tap cooldown will be ignored.  Tapping will be allowed/disallowed based on result of CanTap()")]
	public bool overrideTapCooldown;

	// Token: 0x04002F28 RID: 12072
	[Space]
	public TappableManager manager;

	// Token: 0x04002F29 RID: 12073
	public RpcTarget rpcTarget;
}
