using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000170 RID: 368
public class DJScratchSoundPlayer : MonoBehaviour, ISpawnable
{
	// Token: 0x170000DA RID: 218
	// (get) Token: 0x06000936 RID: 2358 RVA: 0x00031C6D File Offset: 0x0002FE6D
	// (set) Token: 0x06000937 RID: 2359 RVA: 0x00031C75 File Offset: 0x0002FE75
	public bool IsSpawned { get; set; }

	// Token: 0x170000DB RID: 219
	// (get) Token: 0x06000938 RID: 2360 RVA: 0x00031C7E File Offset: 0x0002FE7E
	// (set) Token: 0x06000939 RID: 2361 RVA: 0x00031C86 File Offset: 0x0002FE86
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x0600093A RID: 2362 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnDespawn()
	{
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x00031C90 File Offset: 0x0002FE90
	private void OnEnable()
	{
		if (this._events.IsNull())
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			NetPlayer netPlayer = ((this.myRig != null) ? ((this.myRig.creator != null) ? this.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
			if (netPlayer != null)
			{
				this._events.Init(netPlayer);
			}
		}
		this._events.Activate += this.OnPlayEvent;
	}

	// Token: 0x0600093C RID: 2364 RVA: 0x00031D22 File Offset: 0x0002FF22
	private void OnDisable()
	{
		if (this._events.IsNotNull())
		{
			this._events.Activate -= this.OnPlayEvent;
			this._events.Dispose();
		}
	}

	// Token: 0x0600093D RID: 2365 RVA: 0x00031D5E File Offset: 0x0002FF5E
	public void OnSpawn(VRRig rig)
	{
		this.myRig = rig;
		if (!rig.isLocal)
		{
			this.scratchTableLeft.enabled = false;
			this.scratchTableRight.enabled = false;
		}
	}

	// Token: 0x0600093E RID: 2366 RVA: 0x00031D87 File Offset: 0x0002FF87
	public void Play(ScratchSoundType type, bool isLeft)
	{
		if (this.myRig.isLocal)
		{
			this.PlayLocal(type, isLeft);
			this._events.Activate.RaiseOthers(new object[] { (int)(type + (isLeft ? 100 : 0)) });
		}
	}

	// Token: 0x0600093F RID: 2367 RVA: 0x00031DC8 File Offset: 0x0002FFC8
	public void OnPlayEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != target)
		{
			return;
		}
		if (info.senderID != this.myRig.creator.ActorNumber)
		{
			return;
		}
		if (args.Length != 1)
		{
			Debug.LogError(string.Format("Invalid DJ Scratch Event - expected 1 arg, got {0}", args.Length));
			return;
		}
		int num = (int)args[0];
		bool flag = num >= 100;
		if (flag)
		{
			num -= 100;
		}
		ScratchSoundType scratchSoundType = (ScratchSoundType)num;
		if (scratchSoundType < ScratchSoundType.Pause || scratchSoundType > ScratchSoundType.Back)
		{
			return;
		}
		this.PlayLocal(scratchSoundType, flag);
	}

	// Token: 0x06000940 RID: 2368 RVA: 0x00031E40 File Offset: 0x00030040
	public void PlayLocal(ScratchSoundType type, bool isLeft)
	{
		switch (type)
		{
		case ScratchSoundType.Pause:
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).PauseTrack();
			this.scratchPause.Play();
			return;
		case ScratchSoundType.Resume:
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).ResumeTrack();
			this.scratchResume.Play();
			return;
		case ScratchSoundType.Forward:
			this.scratchForward.Play();
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).PauseTrack();
			return;
		case ScratchSoundType.Back:
			this.scratchBack.Play();
			(isLeft ? this.scratchTableLeft : this.scratchTableRight).PauseTrack();
			return;
		default:
			return;
		}
	}

	// Token: 0x04000B0A RID: 2826
	[SerializeField]
	private SoundBankPlayer scratchForward;

	// Token: 0x04000B0B RID: 2827
	[SerializeField]
	private SoundBankPlayer scratchBack;

	// Token: 0x04000B0C RID: 2828
	[SerializeField]
	private SoundBankPlayer scratchPause;

	// Token: 0x04000B0D RID: 2829
	[SerializeField]
	private SoundBankPlayer scratchResume;

	// Token: 0x04000B0E RID: 2830
	[SerializeField]
	private DJScratchtable scratchTableLeft;

	// Token: 0x04000B0F RID: 2831
	[SerializeField]
	private DJScratchtable scratchTableRight;

	// Token: 0x04000B10 RID: 2832
	private RubberDuckEvents _events;

	// Token: 0x04000B11 RID: 2833
	private VRRig myRig;
}
