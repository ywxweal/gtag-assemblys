using System;
using Fusion;
using Fusion.CodeGen;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200026E RID: 622
[NetworkBehaviourWeaved(128)]
public class ArcadeMachine : NetworkComponent
{
	// Token: 0x06000E51 RID: 3665 RVA: 0x00048B74 File Offset: 0x00046D74
	protected override void Awake()
	{
		base.Awake();
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x06000E52 RID: 3666 RVA: 0x00048B88 File Offset: 0x00046D88
	protected override void Start()
	{
		base.Start();
		if (this.arcadeGame != null && this.arcadeGame.Scale.x > 0f && this.arcadeGame.Scale.y > 0f)
		{
			this.arcadeGameInstance = global::UnityEngine.Object.Instantiate<ArcadeGame>(this.arcadeGame, this.screen.transform);
			this.arcadeGameInstance.transform.localScale = new Vector3(1f / this.arcadeGameInstance.Scale.x, 1f / this.arcadeGameInstance.Scale.y, 1f);
			this.screen.forceRenderingOff = true;
			this.arcadeGameInstance.SetMachine(this);
		}
	}

	// Token: 0x06000E53 RID: 3667 RVA: 0x00048C58 File Offset: 0x00046E58
	public void PlaySound(int soundId, int priority)
	{
		if (!this.audioSource.isPlaying || this.audioSourcePriority >= priority)
		{
			this.audioSource.GTStop();
			this.audioSourcePriority = priority;
			this.audioSource.clip = this.arcadeGameInstance.audioClips[soundId];
			this.audioSource.GTPlay();
			if (this.networkSynchronized && base.IsMine)
			{
				base.GetView.RPC("ArcadeGameInstance_OnPlaySound_RPC", RpcTarget.Others, new object[] { soundId });
			}
		}
	}

	// Token: 0x06000E54 RID: 3668 RVA: 0x00048CE0 File Offset: 0x00046EE0
	public bool IsPlayerLocallyControlled(int player)
	{
		return this.sticks[player].heldByLocalPlayer;
	}

	// Token: 0x06000E55 RID: 3669 RVA: 0x00048CF0 File Offset: 0x00046EF0
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		for (int i = 0; i < this.sticks.Length; i++)
		{
			this.sticks[i].Init(this, i);
		}
	}

	// Token: 0x06000E56 RID: 3670 RVA: 0x00048D2B File Offset: 0x00046F2B
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
	}

	// Token: 0x06000E57 RID: 3671 RVA: 0x00048D3C File Offset: 0x00046F3C
	[PunRPC]
	private void ArcadeGameInstance_OnPlaySound_RPC(int id, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient || id > this.arcadeGameInstance.audioClips.Length || id < 0 || !this.soundCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		this.audioSource.Stop();
		this.audioSource.clip = this.arcadeGameInstance.audioClips[id];
		this.audioSource.Play();
	}

	// Token: 0x06000E58 RID: 3672 RVA: 0x00048DAB File Offset: 0x00046FAB
	public void OnJoystickStateChange(int player, ArcadeButtons buttons)
	{
		if (this.arcadeGameInstance != null)
		{
			this.arcadeGameInstance.OnInputStateChange(player, buttons);
		}
	}

	// Token: 0x06000E59 RID: 3673 RVA: 0x00048DC4 File Offset: 0x00046FC4
	public bool IsControllerInUse(int player)
	{
		if (base.IsMine)
		{
			return this.playersPerJoystick[player] != null && Time.time < this.playerIdleTimeouts[player];
		}
		return (this.buttonsStateValue & (1 << player * 8)) != 0;
	}

	// Token: 0x1700016B RID: 363
	// (get) Token: 0x06000E5A RID: 3674 RVA: 0x00048DFC File Offset: 0x00046FFC
	[Networked]
	[Capacity(128)]
	[NetworkedWeaved(0, 128)]
	public unsafe NetworkArray<byte> Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ArcadeMachine.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return new NetworkArray<byte>((byte*)(this.Ptr + 0), 128, ReaderWriter@System_Byte.GetInstance());
		}
	}

	// Token: 0x06000E5B RID: 3675 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06000E5C RID: 3676 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06000E5D RID: 3677 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06000E5E RID: 3678 RVA: 0x00048E3C File Offset: 0x0004703C
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06000E5F RID: 3679 RVA: 0x00048E49 File Offset: 0x00047049
	public void ReadPlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
		this.arcadeGameInstance.ReadPlayerDataPUN(player, stream, info);
	}

	// Token: 0x06000E60 RID: 3680 RVA: 0x00048E59 File Offset: 0x00047059
	public void WritePlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
		this.arcadeGameInstance.WritePlayerDataPUN(player, stream, info);
	}

	// Token: 0x06000E62 RID: 3682 RVA: 0x00048E90 File Offset: 0x00047090
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		NetworkBehaviourUtils.InitializeNetworkArray<byte>(this.Data, this._Data, "Data");
	}

	// Token: 0x06000E63 RID: 3683 RVA: 0x00048EB2 File Offset: 0x000470B2
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		NetworkBehaviourUtils.CopyFromNetworkArray<byte>(this.Data, ref this._Data);
	}

	// Token: 0x0400119C RID: 4508
	[SerializeField]
	private ArcadeGame arcadeGame;

	// Token: 0x0400119D RID: 4509
	[SerializeField]
	private ArcadeMachineJoystick[] sticks;

	// Token: 0x0400119E RID: 4510
	[SerializeField]
	private Renderer screen;

	// Token: 0x0400119F RID: 4511
	[SerializeField]
	private bool networkSynchronized = true;

	// Token: 0x040011A0 RID: 4512
	[SerializeField]
	private CallLimiter soundCallLimit;

	// Token: 0x040011A1 RID: 4513
	private int buttonsStateValue;

	// Token: 0x040011A2 RID: 4514
	private AudioSource audioSource;

	// Token: 0x040011A3 RID: 4515
	private int audioSourcePriority;

	// Token: 0x040011A4 RID: 4516
	private ArcadeGame arcadeGameInstance;

	// Token: 0x040011A5 RID: 4517
	private Player[] playersPerJoystick = new Player[4];

	// Token: 0x040011A6 RID: 4518
	private float[] playerIdleTimeouts = new float[4];

	// Token: 0x040011A7 RID: 4519
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 128)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private byte[] _Data;
}
