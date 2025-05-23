using System;
using GorillaExtensions;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200046F RID: 1135
public class RubberDuck : TransferrableObject
{
	// Token: 0x17000301 RID: 769
	// (get) Token: 0x06001BDD RID: 7133 RVA: 0x00089067 File Offset: 0x00087267
	// (set) Token: 0x06001BDE RID: 7134 RVA: 0x00089079 File Offset: 0x00087279
	public bool fxActive
	{
		get
		{
			return this.hasParticleFX && this._fxActive;
		}
		set
		{
			if (!this.hasParticleFX)
			{
				return;
			}
			this.pFXEmissionModule.enabled = value;
			this._fxActive = value;
		}
	}

	// Token: 0x17000302 RID: 770
	// (get) Token: 0x06001BDF RID: 7135 RVA: 0x00089097 File Offset: 0x00087297
	public int SqueezeSound
	{
		get
		{
			if (this.squeezeSoundBank.Length > 1)
			{
				return this.squeezeSoundBank[Random.Range(0, this.squeezeSoundBank.Length)];
			}
			if (this.squeezeSoundBank.Length == 1)
			{
				return this.squeezeSoundBank[0];
			}
			return this.squeezeSound;
		}
	}

	// Token: 0x17000303 RID: 771
	// (get) Token: 0x06001BE0 RID: 7136 RVA: 0x000890D4 File Offset: 0x000872D4
	public int SqueezeReleaseSound
	{
		get
		{
			if (this.squeezeReleaseSoundBank.Length > 1)
			{
				return this.squeezeReleaseSoundBank[Random.Range(0, this.squeezeReleaseSoundBank.Length)];
			}
			if (this.squeezeReleaseSoundBank.Length == 1)
			{
				return this.squeezeReleaseSoundBank[0];
			}
			return this.squeezeReleaseSound;
		}
	}

	// Token: 0x06001BE1 RID: 7137 RVA: 0x00089114 File Offset: 0x00087314
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		if (this.skinRenderer == null)
		{
			this.skinRenderer = base.GetComponentInChildren<SkinnedMeshRenderer>(true);
		}
		this.hasSkinRenderer = this.skinRenderer != null;
		this.myThreshold = 0.7f;
		this.hysterisis = 0.3f;
		this.hasParticleFX = this.particleFX != null;
		if (this.hasParticleFX)
		{
			this.pFXEmissionModule = this.particleFX.emission;
			this.pFXEmissionModule.rateOverTime = this.particleFXEmissionIdle;
		}
		this.fxActive = false;
	}

	// Token: 0x06001BE2 RID: 7138 RVA: 0x000891B4 File Offset: 0x000873B4
	internal override void OnEnable()
	{
		base.OnEnable();
		if (this._events == null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			NetPlayer netPlayer = ((base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null));
			if (netPlayer != null)
			{
				this._events.Init(netPlayer);
			}
			else
			{
				Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
			}
		}
		if (this._events != null)
		{
			this._events.Activate += this.OnSqueezeActivate;
			this._events.Deactivate += this.OnSqueezeDeactivate;
		}
	}

	// Token: 0x06001BE3 RID: 7139 RVA: 0x000892A4 File Offset: 0x000874A4
	internal override void OnDisable()
	{
		base.OnDisable();
		if (this._events != null)
		{
			this._events.Activate -= this.OnSqueezeActivate;
			this._events.Deactivate -= this.OnSqueezeDeactivate;
			this._events.Dispose();
			this._events = null;
		}
	}

	// Token: 0x06001BE4 RID: 7140 RVA: 0x0008931B File Offset: 0x0008751B
	private void OnSqueezeActivate(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != target)
		{
			return;
		}
		if (info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		this.SqueezeActivateLocal();
	}

	// Token: 0x06001BE5 RID: 7141 RVA: 0x00089342 File Offset: 0x00087542
	private void SqueezeActivateLocal()
	{
		this.PlayParticleFX(this.particleFXEmissionSqueeze);
		if (this._sfxActivate && !this._sfxActivate.isPlaying)
		{
			this._sfxActivate.PlayNext(0f, 1f);
		}
	}

	// Token: 0x06001BE6 RID: 7142 RVA: 0x0008937F File Offset: 0x0008757F
	private void OnSqueezeDeactivate(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != target)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "OnSqueezeDeactivate");
		if (info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		this.SqueezeDeactivateLocal();
	}

	// Token: 0x06001BE7 RID: 7143 RVA: 0x000893B2 File Offset: 0x000875B2
	private void SqueezeDeactivateLocal()
	{
		this.PlayParticleFX(this.particleFXEmissionIdle);
	}

	// Token: 0x06001BE8 RID: 7144 RVA: 0x000893C0 File Offset: 0x000875C0
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		float num = 0f;
		if (base.InHand())
		{
			this.tempHandPos = ((base.myOnlineRig != null) ? base.myOnlineRig.ReturnHandPosition() : base.myRig.ReturnHandPosition());
			if (this.currentState == TransferrableObject.PositionState.InLeftHand)
			{
				num = (float)Mathf.FloorToInt((float)(this.tempHandPos % 10000) / 1000f);
			}
			else
			{
				num = (float)Mathf.FloorToInt((float)(this.tempHandPos % 10) / 1f);
			}
		}
		if (this.hasSkinRenderer)
		{
			this.skinRenderer.SetBlendShapeWeight(0, Mathf.Lerp(this.skinRenderer.GetBlendShapeWeight(0), num * 11.1f, this.blendShapeMaxWeight));
		}
		if (this.fxActive)
		{
			this.squeezeTimeElapsed += Time.deltaTime;
			this.pFXEmissionModule.rateOverTime = Mathf.Lerp(this.particleFXEmissionIdle, this.particleFXEmissionSqueeze, this.particleFXEmissionCooldownCurve.Evaluate(this.squeezeTimeElapsed));
			if (this.squeezeTimeElapsed > this.particleFXEmissionSqueeze)
			{
				this.fxActive = false;
			}
		}
	}

	// Token: 0x06001BE9 RID: 7145 RVA: 0x000894DC File Offset: 0x000876DC
	public override void OnActivate()
	{
		base.OnActivate();
		if (this.IsMyItem())
		{
			bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
			RigContainer localRig = VRRigCache.Instance.localRig;
			int num = this.SqueezeSound;
			localRig.Rig.PlayHandTapLocal(num, flag, 0.33f);
			if (localRig.netView)
			{
				localRig.netView.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { num, flag, 0.33f });
			}
			GorillaTagger.Instance.StartVibration(flag, this.squeezeStrength, Time.deltaTime);
		}
		if (this._raiseActivate)
		{
			if (RoomSystem.JoinedRoom)
			{
				RubberDuckEvents events = this._events;
				if (events == null)
				{
					return;
				}
				PhotonEvent activate = events.Activate;
				if (activate == null)
				{
					return;
				}
				activate.RaiseAll(Array.Empty<object>());
				return;
			}
			else
			{
				this.SqueezeActivateLocal();
			}
		}
	}

	// Token: 0x06001BEA RID: 7146 RVA: 0x000895B8 File Offset: 0x000877B8
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		if (this.IsMyItem())
		{
			bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
			int num = this.SqueezeReleaseSound;
			Debug.Log("Squeezy Deactivate: " + num.ToString());
			VRRigCache.Instance.localRig.Rig.PlayHandTapLocal(num, flag, 0.33f);
			RigContainer rigContainer;
			if (GorillaGameManager.instance && VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.LocalPlayer, out rigContainer))
			{
				rigContainer.Rig.netView.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { num, flag, 0.33f });
			}
			GorillaTagger.Instance.StartVibration(flag, this.releaseStrength, Time.deltaTime);
		}
		if (this._raiseDeactivate)
		{
			if (RoomSystem.JoinedRoom)
			{
				RubberDuckEvents events = this._events;
				if (events == null)
				{
					return;
				}
				PhotonEvent deactivate = events.Deactivate;
				if (deactivate == null)
				{
					return;
				}
				deactivate.RaiseAll(Array.Empty<object>());
				return;
			}
			else
			{
				this.SqueezeDeactivateLocal();
			}
		}
	}

	// Token: 0x06001BEB RID: 7147 RVA: 0x000896C4 File Offset: 0x000878C4
	public void PlayParticleFX(float rate)
	{
		if (!this.hasParticleFX)
		{
			return;
		}
		if (this.currentState != TransferrableObject.PositionState.InLeftHand && this.currentState != TransferrableObject.PositionState.InRightHand)
		{
			return;
		}
		if (!this.fxActive)
		{
			this.fxActive = true;
		}
		this.squeezeTimeElapsed = 0f;
		this.pFXEmissionModule.rateOverTime = rate;
	}

	// Token: 0x06001BEC RID: 7148 RVA: 0x00089718 File Offset: 0x00087918
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x06001BED RID: 7149 RVA: 0x00089723 File Offset: 0x00087923
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x04001EFA RID: 7930
	[DebugOption]
	public bool disableActivation;

	// Token: 0x04001EFB RID: 7931
	[DebugOption]
	public bool disableDeactivation;

	// Token: 0x04001EFC RID: 7932
	private SkinnedMeshRenderer skinRenderer;

	// Token: 0x04001EFD RID: 7933
	[FormerlySerializedAs("duckieLerp")]
	public float blendShapeMaxWeight = 1f;

	// Token: 0x04001EFE RID: 7934
	private int tempHandPos;

	// Token: 0x04001EFF RID: 7935
	[GorillaSoundLookup]
	[SerializeField]
	private int squeezeSound = 75;

	// Token: 0x04001F00 RID: 7936
	[GorillaSoundLookup]
	[SerializeField]
	private int squeezeReleaseSound = 76;

	// Token: 0x04001F01 RID: 7937
	[GorillaSoundLookup]
	public int[] squeezeSoundBank;

	// Token: 0x04001F02 RID: 7938
	[GorillaSoundLookup]
	public int[] squeezeReleaseSoundBank;

	// Token: 0x04001F03 RID: 7939
	public float squeezeStrength = 0.05f;

	// Token: 0x04001F04 RID: 7940
	public float releaseStrength = 0.03f;

	// Token: 0x04001F05 RID: 7941
	public ParticleSystem particleFX;

	// Token: 0x04001F06 RID: 7942
	[Tooltip("The emission rate of the particle effect when not squeezed.")]
	public float particleFXEmissionIdle = 0.8f;

	// Token: 0x04001F07 RID: 7943
	[Tooltip("The emission rate of the particle effect when squeezed.")]
	public float particleFXEmissionSqueeze = 10f;

	// Token: 0x04001F08 RID: 7944
	[Tooltip("The animation of the particle effect returning to the idle emission rate. X axis is time, Y axis is the emission lerp value where 0 is idle, 1 is squeezed.")]
	public AnimationCurve particleFXEmissionCooldownCurve;

	// Token: 0x04001F09 RID: 7945
	private bool hasSkinRenderer;

	// Token: 0x04001F0A RID: 7946
	private ParticleSystem.EmissionModule pFXEmissionModule;

	// Token: 0x04001F0B RID: 7947
	private bool hasParticleFX;

	// Token: 0x04001F0C RID: 7948
	private float squeezeTimeElapsed;

	// Token: 0x04001F0D RID: 7949
	[SerializeField]
	private RubberDuckEvents _events;

	// Token: 0x04001F0E RID: 7950
	[SerializeField]
	private bool _raiseActivate = true;

	// Token: 0x04001F0F RID: 7951
	[SerializeField]
	private bool _raiseDeactivate = true;

	// Token: 0x04001F10 RID: 7952
	[SerializeField]
	private SoundEffects _sfxActivate;

	// Token: 0x04001F11 RID: 7953
	[SerializeField]
	private bool _fxActive;
}
