using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000531 RID: 1329
public abstract class CosmeticCritterHoldable : MonoBehaviour
{
	// Token: 0x17000342 RID: 834
	// (get) Token: 0x06002042 RID: 8258 RVA: 0x000A24F3 File Offset: 0x000A06F3
	// (set) Token: 0x06002043 RID: 8259 RVA: 0x000A24FB File Offset: 0x000A06FB
	public int OwnerID { get; private set; }

	// Token: 0x17000343 RID: 835
	// (get) Token: 0x06002044 RID: 8260 RVA: 0x000A2504 File Offset: 0x000A0704
	public bool IsLocal
	{
		get
		{
			return this.transferrableObject.IsLocalObject();
		}
	}

	// Token: 0x06002045 RID: 8261 RVA: 0x000A2511 File Offset: 0x000A0711
	public bool OwningPlayerMatches(PhotonMessageInfoWrapped info)
	{
		return this.transferrableObject.targetRig.creator == info.Sender;
	}

	// Token: 0x06002046 RID: 8262 RVA: 0x000A252C File Offset: 0x000A072C
	protected virtual CallLimiter CreateCallLimiter()
	{
		return new CallLimiter(10, 2f, 0.5f);
	}

	// Token: 0x06002047 RID: 8263 RVA: 0x000A253F File Offset: 0x000A073F
	public void ResetCallLimiter()
	{
		this.callLimiter.Reset();
	}

	// Token: 0x06002048 RID: 8264 RVA: 0x000A254C File Offset: 0x000A074C
	private void TrySetID()
	{
		if (this.IsLocal)
		{
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			if (instance != null)
			{
				string playFabPlayerId = instance.GetPlayFabPlayerId();
				Type type = base.GetType();
				this.OwnerID = (playFabPlayerId + ((type != null) ? type.ToString() : null)).GetStaticHash();
				return;
			}
		}
		else if (this.transferrableObject.targetRig != null && this.transferrableObject.targetRig.creator != null)
		{
			string userId = this.transferrableObject.targetRig.creator.UserId;
			Type type2 = base.GetType();
			this.OwnerID = (userId + ((type2 != null) ? type2.ToString() : null)).GetStaticHash();
		}
	}

	// Token: 0x06002049 RID: 8265 RVA: 0x000A25FA File Offset: 0x000A07FA
	protected virtual void Awake()
	{
		this.transferrableObject = base.GetComponentInParent<TransferrableObject>();
		this.callLimiter = this.CreateCallLimiter();
		if (this.IsLocal)
		{
			CosmeticCritterManager.Instance.RegisterLocalHoldable(this);
		}
	}

	// Token: 0x0600204A RID: 8266 RVA: 0x000A2627 File Offset: 0x000A0827
	protected virtual void OnEnable()
	{
		this.TrySetID();
	}

	// Token: 0x0600204B RID: 8267 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnDisable()
	{
	}

	// Token: 0x04002449 RID: 9289
	protected TransferrableObject transferrableObject;

	// Token: 0x0400244B RID: 9291
	protected CallLimiter callLimiter;
}
