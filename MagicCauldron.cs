using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Fusion;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000674 RID: 1652
[NetworkBehaviourWeaved(4)]
public class MagicCauldron : NetworkComponent
{
	// Token: 0x06002945 RID: 10565 RVA: 0x000CD1EC File Offset: 0x000CB3EC
	private new void Awake()
	{
		this.currentIngredients.Clear();
		this.witchesComponent.Clear();
		this.currentStateElapsedTime = 0f;
		this.currentRecipeIndex = -1;
		this.ingredientIndex = -1;
		if (this.flyingWitchesContainer != null)
		{
			for (int i = 0; i < this.flyingWitchesContainer.transform.childCount; i++)
			{
				NoncontrollableBroomstick componentInChildren = this.flyingWitchesContainer.transform.GetChild(i).gameObject.GetComponentInChildren<NoncontrollableBroomstick>();
				this.witchesComponent.Add(componentInChildren);
				if (componentInChildren)
				{
					componentInChildren.gameObject.SetActive(false);
				}
			}
		}
		if (this.reusableFXContext == null)
		{
			this.reusableFXContext = new MagicCauldron.IngrediantFXContext();
		}
		if (this.reusableIngrediantArgs == null)
		{
			this.reusableIngrediantArgs = new MagicCauldron.IngredientArgs();
		}
		this.reusableFXContext.fxCallBack = new MagicCauldron.IngrediantFXContext.Callback(this.OnIngredientAdd);
	}

	// Token: 0x06002946 RID: 10566 RVA: 0x000CD2CA File Offset: 0x000CB4CA
	private new void Start()
	{
		this.ChangeState(MagicCauldron.CauldronState.notReady);
	}

	// Token: 0x06002947 RID: 10567 RVA: 0x000CD2D3 File Offset: 0x000CB4D3
	private void LateUpdate()
	{
		this.UpdateState();
	}

	// Token: 0x06002948 RID: 10568 RVA: 0x000CD2DB File Offset: 0x000CB4DB
	private IEnumerator LevitationSpellCoroutine()
	{
		GTPlayer.Instance.SetHalloweenLevitation(this.levitationStrength, this.levitationDuration, this.levitationBlendOutDuration, this.levitationBonusStrength, this.levitationBonusOffAtYSpeed, this.levitationBonusFullAtYSpeed);
		yield return new WaitForSeconds(this.levitationSpellDuration);
		GTPlayer.Instance.SetHalloweenLevitation(0f, this.levitationDuration, this.levitationBlendOutDuration, 0f, this.levitationBonusOffAtYSpeed, this.levitationBonusFullAtYSpeed);
		yield break;
	}

	// Token: 0x06002949 RID: 10569 RVA: 0x000CD2EC File Offset: 0x000CB4EC
	private void ChangeState(MagicCauldron.CauldronState state)
	{
		this.currentState = state;
		if (base.IsMine)
		{
			this.currentStateElapsedTime = 0f;
		}
		bool flag = state == MagicCauldron.CauldronState.summoned;
		foreach (NoncontrollableBroomstick noncontrollableBroomstick in this.witchesComponent)
		{
			if (noncontrollableBroomstick.gameObject.activeSelf != flag)
			{
				noncontrollableBroomstick.gameObject.SetActive(flag);
			}
		}
		if (this.currentState == MagicCauldron.CauldronState.summoned && Vector3.Distance(GTPlayer.Instance.transform.position, base.transform.position) < this.levitationRadius)
		{
			base.StartCoroutine(this.LevitationSpellCoroutine());
		}
		switch (this.currentState)
		{
		case MagicCauldron.CauldronState.notReady:
			this.currentIngredients.Clear();
			this.UpdateCauldronColor(this.CauldronNotReadyColor);
			return;
		case MagicCauldron.CauldronState.ready:
			this.UpdateCauldronColor(this.CauldronActiveColor);
			return;
		case MagicCauldron.CauldronState.recipeCollecting:
			if (this.ingredientIndex >= 0 && this.ingredientIndex < this.allIngredients.Length)
			{
				this.UpdateCauldronColor(this.allIngredients[this.ingredientIndex].color);
				return;
			}
			break;
		case MagicCauldron.CauldronState.recipeActivated:
			if (this.audioSource && this.recipes[this.currentRecipeIndex].successAudio)
			{
				this.audioSource.GTPlayOneShot(this.recipes[this.currentRecipeIndex].successAudio, 1f);
			}
			if (this.successParticle)
			{
				this.successParticle.Play();
				return;
			}
			break;
		case MagicCauldron.CauldronState.summoned:
			break;
		case MagicCauldron.CauldronState.failed:
			this.currentIngredients.Clear();
			this.UpdateCauldronColor(this.CauldronFailedColor);
			this.audioSource.GTPlayOneShot(this.recipeFailedAudio, 1f);
			return;
		case MagicCauldron.CauldronState.cooldown:
			this.currentIngredients.Clear();
			this.UpdateCauldronColor(this.CauldronFailedColor);
			break;
		default:
			return;
		}
	}

	// Token: 0x0600294A RID: 10570 RVA: 0x000CD4E4 File Offset: 0x000CB6E4
	private void UpdateState()
	{
		if (base.IsMine)
		{
			this.currentStateElapsedTime += Time.deltaTime;
			switch (this.currentState)
			{
			case MagicCauldron.CauldronState.notReady:
			case MagicCauldron.CauldronState.ready:
				break;
			case MagicCauldron.CauldronState.recipeCollecting:
				if (this.currentStateElapsedTime >= this.maxTimeToAddAllIngredients && !this.CheckIngredients())
				{
					this.ChangeState(MagicCauldron.CauldronState.failed);
					return;
				}
				break;
			case MagicCauldron.CauldronState.recipeActivated:
				if (this.currentStateElapsedTime >= this.waitTimeToSummonWitches)
				{
					this.ChangeState(MagicCauldron.CauldronState.summoned);
					return;
				}
				break;
			case MagicCauldron.CauldronState.summoned:
				if (this.currentStateElapsedTime >= this.summonWitchesDuration)
				{
					this.ChangeState(MagicCauldron.CauldronState.cooldown);
					return;
				}
				break;
			case MagicCauldron.CauldronState.failed:
				if (this.currentStateElapsedTime >= this.recipeFailedDuration)
				{
					this.ChangeState(MagicCauldron.CauldronState.ready);
					return;
				}
				break;
			case MagicCauldron.CauldronState.cooldown:
				if (this.currentStateElapsedTime >= this.cooldownDuration)
				{
					this.ChangeState(MagicCauldron.CauldronState.ready);
				}
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x0600294B RID: 10571 RVA: 0x000CD5AD File Offset: 0x000CB7AD
	public void OnEventStart()
	{
		this.ChangeState(MagicCauldron.CauldronState.ready);
	}

	// Token: 0x0600294C RID: 10572 RVA: 0x000CD2CA File Offset: 0x000CB4CA
	public void OnEventEnd()
	{
		this.ChangeState(MagicCauldron.CauldronState.notReady);
	}

	// Token: 0x0600294D RID: 10573 RVA: 0x000CD5B6 File Offset: 0x000CB7B6
	[PunRPC]
	public void OnIngredientAdd(int _ingredientIndex, PhotonMessageInfo info)
	{
		this.OnIngredientAddShared(_ingredientIndex, info);
	}

	// Token: 0x0600294E RID: 10574 RVA: 0x000CD5C8 File Offset: 0x000CB7C8
	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public unsafe void RPC_OnIngredientAdd(int _ingredientIndex, RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 1) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void MagicCauldron::RPC_OnIngredientAdd(System.Int32,Fusion.RpcInfo)", base.Object, 1);
				}
				else
				{
					if (base.Runner.HasAnyActiveConnections())
					{
						int num = 8;
						num += 4;
						SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1), data);
						*(int*)(data + num2) = _ingredientIndex;
						num2 += 4;
						ptr->Offset = num2 * 8;
						base.Runner.SendRpc(ptr);
					}
					if ((localAuthorityMask & 7) != 0)
					{
						info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
						goto IL_0012;
					}
				}
			}
			return;
		}
		this.InvokeRpc = false;
		IL_0012:
		this.OnIngredientAddShared(_ingredientIndex, info);
	}

	// Token: 0x0600294F RID: 10575 RVA: 0x000CD708 File Offset: 0x000CB908
	private void OnIngredientAddShared(int _ingredientIndex, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "OnIngredientAdd");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		this.reusableFXContext.playerSettings = rigContainer.Rig.fxSettings;
		this.reusableIngrediantArgs.key = _ingredientIndex;
		FXSystem.PlayFX<MagicCauldron.IngredientArgs>(FXType.HWIngredients, this.reusableFXContext, this.reusableIngrediantArgs, info);
	}

	// Token: 0x06002950 RID: 10576 RVA: 0x000CD76C File Offset: 0x000CB96C
	private void OnIngredientAdd(int _ingredientIndex)
	{
		if (this.audioSource)
		{
			this.audioSource.GTPlayOneShot(this.ingredientAddedAudio, 1f);
		}
		if (!RoomSystem.AmITheHost)
		{
			return;
		}
		if (_ingredientIndex < 0 || _ingredientIndex >= this.allIngredients.Length || (this.currentState != MagicCauldron.CauldronState.ready && this.currentState != MagicCauldron.CauldronState.recipeCollecting))
		{
			return;
		}
		MagicIngredientType magicIngredientType = this.allIngredients[_ingredientIndex];
		Debug.Log(string.Format("Received ingredient RPC {0} = {1}", _ingredientIndex, magicIngredientType));
		MagicIngredientType magicIngredientType2 = null;
		if (this.recipes[0].recipeIngredients.Count > this.currentIngredients.Count)
		{
			magicIngredientType2 = this.recipes[0].recipeIngredients[this.currentIngredients.Count];
		}
		if (!(magicIngredientType == magicIngredientType2))
		{
			Debug.Log(string.Format("Failure: Expected ingredient {0}, got {1} from recipe[{2}]", magicIngredientType2, magicIngredientType, this.currentIngredients.Count));
			this.ChangeState(MagicCauldron.CauldronState.failed);
			return;
		}
		this.ingredientIndex = _ingredientIndex;
		this.currentIngredients.Add(magicIngredientType);
		if (this.CheckIngredients())
		{
			this.ChangeState(MagicCauldron.CauldronState.recipeActivated);
			return;
		}
		if (this.currentState == MagicCauldron.CauldronState.ready)
		{
			this.ChangeState(MagicCauldron.CauldronState.recipeCollecting);
			return;
		}
		this.UpdateCauldronColor(magicIngredientType.color);
	}

	// Token: 0x06002951 RID: 10577 RVA: 0x000CD8A0 File Offset: 0x000CBAA0
	private bool CheckIngredients()
	{
		foreach (MagicCauldron.Recipe recipe in this.recipes)
		{
			if (this.currentIngredients.SequenceEqual(recipe.recipeIngredients))
			{
				this.currentRecipeIndex = this.recipes.IndexOf(recipe);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002952 RID: 10578 RVA: 0x000CD918 File Offset: 0x000CBB18
	private void UpdateCauldronColor(Color color)
	{
		if (this.bubblesParticle)
		{
			if (this.bubblesParticle.isPlaying)
			{
				if (this.currentState == MagicCauldron.CauldronState.failed || this.currentState == MagicCauldron.CauldronState.notReady)
				{
					this.bubblesParticle.Stop();
				}
			}
			else
			{
				this.bubblesParticle.Play();
			}
		}
		this.currentColor = this.cauldronColor;
		if (this.currentColor == color)
		{
			return;
		}
		if (this.rendr)
		{
			this._liquid.AnimateColorFromTo(this.cauldronColor, color, 1f);
			this.cauldronColor = color;
		}
		if (this.bubblesParticle)
		{
			this.bubblesParticle.main.startColor = color;
		}
	}

	// Token: 0x06002953 RID: 10579 RVA: 0x000CD9D4 File Offset: 0x000CBBD4
	private void OnTriggerEnter(Collider other)
	{
		ThrowableSetDressing componentInParent = other.GetComponentInParent<ThrowableSetDressing>();
		if (componentInParent == null || componentInParent.IngredientTypeSO == null || componentInParent.InHand())
		{
			return;
		}
		if (componentInParent.IsLocalOwnedWorldShareable)
		{
			if (componentInParent.IngredientTypeSO != null && (this.currentState == MagicCauldron.CauldronState.ready || this.currentState == MagicCauldron.CauldronState.recipeCollecting))
			{
				int num = this.allIngredients.IndexOfRef(componentInParent.IngredientTypeSO);
				Debug.Log(string.Format("Sending ingredient RPC {0} = {1}", componentInParent.IngredientTypeSO, num));
				base.SendRPC("OnIngredientAdd", RpcTarget.Others, new object[] { num });
				this.OnIngredientAdd(num);
			}
			componentInParent.StartRespawnTimer(0f);
		}
		if (componentInParent.IngredientTypeSO != null && this.splashParticle)
		{
			this.splashParticle.Play();
		}
	}

	// Token: 0x06002954 RID: 10580 RVA: 0x000CDAB0 File Offset: 0x000CBCB0
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		this.currentIngredients.Clear();
	}

	// Token: 0x170003F5 RID: 1013
	// (get) Token: 0x06002955 RID: 10581 RVA: 0x000CDAC9 File Offset: 0x000CBCC9
	// (set) Token: 0x06002956 RID: 10582 RVA: 0x000CDAF3 File Offset: 0x000CBCF3
	[Networked]
	[NetworkedWeaved(0, 4)]
	private unsafe MagicCauldron.MagicCauldronData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MagicCauldron.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(MagicCauldron.MagicCauldronData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MagicCauldron.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(MagicCauldron.MagicCauldronData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06002957 RID: 10583 RVA: 0x000CDB1E File Offset: 0x000CBD1E
	public override void WriteDataFusion()
	{
		this.Data = new MagicCauldron.MagicCauldronData(this.currentStateElapsedTime, this.currentRecipeIndex, this.currentState, this.ingredientIndex);
	}

	// Token: 0x06002958 RID: 10584 RVA: 0x000CDB44 File Offset: 0x000CBD44
	public override void ReadDataFusion()
	{
		this.ReadDataShared(this.Data.CurrentStateElapsedTime, this.Data.CurrentRecipeIndex, this.Data.CurrentState, this.Data.IngredientIndex);
	}

	// Token: 0x06002959 RID: 10585 RVA: 0x000CDB90 File Offset: 0x000CBD90
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.currentStateElapsedTime);
		stream.SendNext(this.currentRecipeIndex);
		stream.SendNext(this.currentState);
		stream.SendNext(this.ingredientIndex);
	}

	// Token: 0x0600295A RID: 10586 RVA: 0x000CDBF0 File Offset: 0x000CBDF0
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		float num = (float)stream.ReceiveNext();
		int num2 = (int)stream.ReceiveNext();
		MagicCauldron.CauldronState cauldronState = (MagicCauldron.CauldronState)stream.ReceiveNext();
		int num3 = (int)stream.ReceiveNext();
		this.ReadDataShared(num, num2, cauldronState, num3);
	}

	// Token: 0x0600295B RID: 10587 RVA: 0x000CDC48 File Offset: 0x000CBE48
	private void ReadDataShared(float stateElapsedTime, int recipeIndex, MagicCauldron.CauldronState state, int ingredientIndex)
	{
		MagicCauldron.CauldronState cauldronState = this.currentState;
		this.currentStateElapsedTime = stateElapsedTime;
		this.currentRecipeIndex = recipeIndex;
		this.currentState = state;
		this.ingredientIndex = ingredientIndex;
		if (cauldronState != this.currentState)
		{
			this.ChangeState(this.currentState);
			return;
		}
		if (this.currentState == MagicCauldron.CauldronState.recipeCollecting && ingredientIndex != ingredientIndex && ingredientIndex >= 0 && ingredientIndex < this.allIngredients.Length)
		{
			this.UpdateCauldronColor(this.allIngredients[ingredientIndex].color);
		}
	}

	// Token: 0x0600295D RID: 10589 RVA: 0x000CDD45 File Offset: 0x000CBF45
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x0600295E RID: 10590 RVA: 0x000CDD5D File Offset: 0x000CBF5D
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x0600295F RID: 10591 RVA: 0x000CDD74 File Offset: 0x000CBF74
	[NetworkRpcWeavedInvoker(1, 1, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_OnIngredientAdd@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int num3 = num2;
		RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((MagicCauldron)behaviour).RPC_OnIngredientAdd(num3, rpcInfo);
	}

	// Token: 0x04002E47 RID: 11847
	public List<MagicCauldron.Recipe> recipes = new List<MagicCauldron.Recipe>();

	// Token: 0x04002E48 RID: 11848
	public float maxTimeToAddAllIngredients = 30f;

	// Token: 0x04002E49 RID: 11849
	public float summonWitchesDuration = 20f;

	// Token: 0x04002E4A RID: 11850
	public float recipeFailedDuration = 5f;

	// Token: 0x04002E4B RID: 11851
	public float cooldownDuration = 30f;

	// Token: 0x04002E4C RID: 11852
	public MagicIngredientType[] allIngredients;

	// Token: 0x04002E4D RID: 11853
	public GameObject flyingWitchesContainer;

	// Token: 0x04002E4E RID: 11854
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04002E4F RID: 11855
	public AudioClip ingredientAddedAudio;

	// Token: 0x04002E50 RID: 11856
	public AudioClip recipeFailedAudio;

	// Token: 0x04002E51 RID: 11857
	public ParticleSystem bubblesParticle;

	// Token: 0x04002E52 RID: 11858
	public ParticleSystem successParticle;

	// Token: 0x04002E53 RID: 11859
	public ParticleSystem splashParticle;

	// Token: 0x04002E54 RID: 11860
	public Color CauldronActiveColor;

	// Token: 0x04002E55 RID: 11861
	public Color CauldronFailedColor;

	// Token: 0x04002E56 RID: 11862
	[Tooltip("only if we are using the time of day event")]
	public Color CauldronNotReadyColor;

	// Token: 0x04002E57 RID: 11863
	private readonly List<NoncontrollableBroomstick> witchesComponent = new List<NoncontrollableBroomstick>();

	// Token: 0x04002E58 RID: 11864
	private readonly List<MagicIngredientType> currentIngredients = new List<MagicIngredientType>();

	// Token: 0x04002E59 RID: 11865
	private float currentStateElapsedTime;

	// Token: 0x04002E5A RID: 11866
	private MagicCauldron.CauldronState currentState;

	// Token: 0x04002E5B RID: 11867
	[SerializeField]
	private Renderer rendr;

	// Token: 0x04002E5C RID: 11868
	private Color cauldronColor;

	// Token: 0x04002E5D RID: 11869
	private Color currentColor;

	// Token: 0x04002E5E RID: 11870
	private int currentRecipeIndex;

	// Token: 0x04002E5F RID: 11871
	private int ingredientIndex;

	// Token: 0x04002E60 RID: 11872
	private float waitTimeToSummonWitches = 2f;

	// Token: 0x04002E61 RID: 11873
	[Space]
	[SerializeField]
	private MagicCauldronLiquid _liquid;

	// Token: 0x04002E62 RID: 11874
	private MagicCauldron.IngrediantFXContext reusableFXContext = new MagicCauldron.IngrediantFXContext();

	// Token: 0x04002E63 RID: 11875
	private MagicCauldron.IngredientArgs reusableIngrediantArgs = new MagicCauldron.IngredientArgs();

	// Token: 0x04002E64 RID: 11876
	public bool testLevitationAlwaysOn;

	// Token: 0x04002E65 RID: 11877
	public float levitationRadius;

	// Token: 0x04002E66 RID: 11878
	public float levitationSpellDuration;

	// Token: 0x04002E67 RID: 11879
	public float levitationStrength;

	// Token: 0x04002E68 RID: 11880
	public float levitationDuration;

	// Token: 0x04002E69 RID: 11881
	public float levitationBlendOutDuration;

	// Token: 0x04002E6A RID: 11882
	public float levitationBonusStrength;

	// Token: 0x04002E6B RID: 11883
	public float levitationBonusOffAtYSpeed;

	// Token: 0x04002E6C RID: 11884
	public float levitationBonusFullAtYSpeed;

	// Token: 0x04002E6D RID: 11885
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 4)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private MagicCauldron.MagicCauldronData _Data;

	// Token: 0x02000675 RID: 1653
	private enum CauldronState
	{
		// Token: 0x04002E6F RID: 11887
		notReady,
		// Token: 0x04002E70 RID: 11888
		ready,
		// Token: 0x04002E71 RID: 11889
		recipeCollecting,
		// Token: 0x04002E72 RID: 11890
		recipeActivated,
		// Token: 0x04002E73 RID: 11891
		summoned,
		// Token: 0x04002E74 RID: 11892
		failed,
		// Token: 0x04002E75 RID: 11893
		cooldown
	}

	// Token: 0x02000676 RID: 1654
	[Serializable]
	public struct Recipe
	{
		// Token: 0x04002E76 RID: 11894
		public List<MagicIngredientType> recipeIngredients;

		// Token: 0x04002E77 RID: 11895
		public AudioClip successAudio;
	}

	// Token: 0x02000677 RID: 1655
	private class IngredientArgs : FXSArgs
	{
		// Token: 0x04002E78 RID: 11896
		public int key;
	}

	// Token: 0x02000678 RID: 1656
	private class IngrediantFXContext : IFXContextParems<MagicCauldron.IngredientArgs>
	{
		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x06002961 RID: 10593 RVA: 0x000CDDE3 File Offset: 0x000CBFE3
		FXSystemSettings IFXContextParems<MagicCauldron.IngredientArgs>.settings
		{
			get
			{
				return this.playerSettings;
			}
		}

		// Token: 0x06002962 RID: 10594 RVA: 0x000CDDEB File Offset: 0x000CBFEB
		void IFXContextParems<MagicCauldron.IngredientArgs>.OnPlayFX(MagicCauldron.IngredientArgs args)
		{
			this.fxCallBack(args.key);
		}

		// Token: 0x04002E79 RID: 11897
		public FXSystemSettings playerSettings;

		// Token: 0x04002E7A RID: 11898
		public MagicCauldron.IngrediantFXContext.Callback fxCallBack;

		// Token: 0x02000679 RID: 1657
		// (Invoke) Token: 0x06002965 RID: 10597
		public delegate void Callback(int key);
	}

	// Token: 0x0200067A RID: 1658
	[NetworkStructWeaved(4)]
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	private struct MagicCauldronData : INetworkStruct
	{
		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x06002968 RID: 10600 RVA: 0x000CDDFE File Offset: 0x000CBFFE
		// (set) Token: 0x06002969 RID: 10601 RVA: 0x000CDE06 File Offset: 0x000CC006
		public float CurrentStateElapsedTime { readonly get; set; }

		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x0600296A RID: 10602 RVA: 0x000CDE0F File Offset: 0x000CC00F
		// (set) Token: 0x0600296B RID: 10603 RVA: 0x000CDE17 File Offset: 0x000CC017
		public int CurrentRecipeIndex { readonly get; set; }

		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x0600296C RID: 10604 RVA: 0x000CDE20 File Offset: 0x000CC020
		// (set) Token: 0x0600296D RID: 10605 RVA: 0x000CDE28 File Offset: 0x000CC028
		public MagicCauldron.CauldronState CurrentState { readonly get; set; }

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x0600296E RID: 10606 RVA: 0x000CDE31 File Offset: 0x000CC031
		// (set) Token: 0x0600296F RID: 10607 RVA: 0x000CDE39 File Offset: 0x000CC039
		public int IngredientIndex { readonly get; set; }

		// Token: 0x06002970 RID: 10608 RVA: 0x000CDE42 File Offset: 0x000CC042
		public MagicCauldronData(float stateElapsedTime, int recipeIndex, MagicCauldron.CauldronState state, int ingredientIndex)
		{
			this.CurrentStateElapsedTime = stateElapsedTime;
			this.CurrentRecipeIndex = recipeIndex;
			this.CurrentState = state;
			this.IngredientIndex = ingredientIndex;
		}
	}
}
