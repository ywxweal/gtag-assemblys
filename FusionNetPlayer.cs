using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

// Token: 0x0200027F RID: 639
public class FusionNetPlayer : NetPlayer
{
	// Token: 0x1700016F RID: 367
	// (get) Token: 0x06000EC0 RID: 3776 RVA: 0x0004A444 File Offset: 0x00048644
	// (set) Token: 0x06000EC1 RID: 3777 RVA: 0x0004A44C File Offset: 0x0004864C
	public PlayerRef PlayerRef { get; private set; }

	// Token: 0x06000EC2 RID: 3778 RVA: 0x0004A458 File Offset: 0x00048658
	public FusionNetPlayer()
	{
		this.PlayerRef = default(PlayerRef);
	}

	// Token: 0x06000EC3 RID: 3779 RVA: 0x0004A47A File Offset: 0x0004867A
	public FusionNetPlayer(PlayerRef playerRef)
	{
		this.PlayerRef = playerRef;
	}

	// Token: 0x17000170 RID: 368
	// (get) Token: 0x06000EC4 RID: 3780 RVA: 0x0004A489 File Offset: 0x00048689
	private NetworkRunner runner
	{
		get
		{
			return ((NetworkSystemFusion)NetworkSystem.Instance).runner;
		}
	}

	// Token: 0x17000171 RID: 369
	// (get) Token: 0x06000EC5 RID: 3781 RVA: 0x0004A49C File Offset: 0x0004869C
	public override bool IsValid
	{
		get
		{
			return this.validPlayer && this.PlayerRef.IsRealPlayer;
		}
	}

	// Token: 0x17000172 RID: 370
	// (get) Token: 0x06000EC6 RID: 3782 RVA: 0x0004A4C4 File Offset: 0x000486C4
	public override int ActorNumber
	{
		get
		{
			return this.PlayerRef.PlayerId;
		}
	}

	// Token: 0x17000173 RID: 371
	// (get) Token: 0x06000EC7 RID: 3783 RVA: 0x0004A4E0 File Offset: 0x000486E0
	public override string UserId
	{
		get
		{
			return NetworkSystem.Instance.GetUserID(this.PlayerRef.PlayerId);
		}
	}

	// Token: 0x17000174 RID: 372
	// (get) Token: 0x06000EC8 RID: 3784 RVA: 0x0004A508 File Offset: 0x00048708
	public override bool IsMasterClient
	{
		get
		{
			if (!(this.runner == null))
			{
				return (this.IsLocal && this.runner.IsSharedModeMasterClient) || NetworkSystem.Instance.MasterClient == this;
			}
			return this.PlayerRef == default(PlayerRef);
		}
	}

	// Token: 0x17000175 RID: 373
	// (get) Token: 0x06000EC9 RID: 3785 RVA: 0x0004A55C File Offset: 0x0004875C
	public override bool IsLocal
	{
		get
		{
			if (!(this.runner == null))
			{
				return this.PlayerRef == this.runner.LocalPlayer;
			}
			return this.PlayerRef == default(PlayerRef);
		}
	}

	// Token: 0x17000176 RID: 374
	// (get) Token: 0x06000ECA RID: 3786 RVA: 0x0004A5A2 File Offset: 0x000487A2
	public override bool IsNull
	{
		get
		{
			PlayerRef playerRef = this.PlayerRef;
			return false;
		}
	}

	// Token: 0x17000177 RID: 375
	// (get) Token: 0x06000ECB RID: 3787 RVA: 0x0004A5AC File Offset: 0x000487AC
	public override string NickName
	{
		get
		{
			return NetworkSystem.Instance.GetNickName(this);
		}
	}

	// Token: 0x17000178 RID: 376
	// (get) Token: 0x06000ECC RID: 3788 RVA: 0x0004A5BC File Offset: 0x000487BC
	public override string DefaultName
	{
		get
		{
			if (string.IsNullOrEmpty(this._defaultName))
			{
				this._defaultName = "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0');
			}
			return this._defaultName;
		}
	}

	// Token: 0x17000179 RID: 377
	// (get) Token: 0x06000ECD RID: 3789 RVA: 0x0004A608 File Offset: 0x00048808
	public override bool InRoom
	{
		get
		{
			using (IEnumerator<PlayerRef> enumerator = this.runner.ActivePlayers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == this.PlayerRef)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	// Token: 0x06000ECE RID: 3790 RVA: 0x0004A668 File Offset: 0x00048868
	public override bool Equals(NetPlayer myPlayer, NetPlayer other)
	{
		return myPlayer != null && other != null && ((FusionNetPlayer)myPlayer).PlayerRef.Equals(((FusionNetPlayer)other).PlayerRef);
	}

	// Token: 0x06000ECF RID: 3791 RVA: 0x0004A69B File Offset: 0x0004889B
	public void InitPlayer(PlayerRef player)
	{
		this.PlayerRef = player;
		this.validPlayer = true;
	}

	// Token: 0x06000ED0 RID: 3792 RVA: 0x0004A6AC File Offset: 0x000488AC
	public override void OnReturned()
	{
		base.OnReturned();
		this.PlayerRef = default(PlayerRef);
		if (this.PlayerRef.PlayerId != -1)
		{
			Debug.LogError("Returned Player to pool but isnt -1, broken");
		}
	}

	// Token: 0x06000ED1 RID: 3793 RVA: 0x0004A6E9 File Offset: 0x000488E9
	public override void OnTaken()
	{
		base.OnTaken();
	}

	// Token: 0x040011D2 RID: 4562
	private string _defaultName;

	// Token: 0x040011D3 RID: 4563
	private bool validPlayer;
}
