using System;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000EC RID: 236
public class PartyHornTransferableObject : TransferrableObject
{
	// Token: 0x06000602 RID: 1538 RVA: 0x00022B62 File Offset: 0x00020D62
	internal override void OnEnable()
	{
		base.OnEnable();
		this.localHead = GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform;
		this.InitToDefault();
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x00022B8F File Offset: 0x00020D8F
	internal override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x06000604 RID: 1540 RVA: 0x00022B97 File Offset: 0x00020D97
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x06000605 RID: 1541 RVA: 0x00022BA8 File Offset: 0x00020DA8
	protected Vector3 CalcMouthPiecePos()
	{
		Transform transform = base.transform;
		Vector3 vector = transform.position;
		if (this.mouthPiece)
		{
			vector += transform.InverseTransformPoint(this.mouthPiece.position);
		}
		else
		{
			vector += transform.forward * this.mouthPieceZOffset;
		}
		return vector;
	}

	// Token: 0x06000606 RID: 1542 RVA: 0x00022C04 File Offset: 0x00020E04
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!base.InHand())
		{
			return;
		}
		if (this.itemState != TransferrableObject.ItemStates.State0)
		{
			return;
		}
		if (!GorillaParent.hasInstance)
		{
			return;
		}
		Transform transform = base.transform;
		Vector3 vector = this.CalcMouthPiecePos();
		float num = this.mouthPieceRadius * this.mouthPieceRadius * GTPlayer.Instance.scale * GTPlayer.Instance.scale;
		bool flag = (this.localHead.TransformPoint(this.mouthOffset) - vector).sqrMagnitude < num;
		if (this.soundActivated && PhotonNetwork.InRoom)
		{
			bool flag2;
			if (flag)
			{
				GorillaTagger instance = GorillaTagger.Instance;
				if (instance == null)
				{
					flag2 = false;
				}
				else
				{
					Recorder myRecorder = instance.myRecorder;
					bool? flag3 = ((myRecorder != null) ? new bool?(myRecorder.IsCurrentlyTransmitting) : null);
					bool flag4 = true;
					flag2 = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
				}
			}
			else
			{
				flag2 = false;
			}
			flag = flag2;
		}
		for (int i = 0; i < GorillaParent.instance.vrrigs.Count; i++)
		{
			VRRig vrrig = GorillaParent.instance.vrrigs[i];
			if (vrrig.head == null || vrrig.head.rigTarget == null || flag)
			{
				break;
			}
			flag = (vrrig.head.rigTarget.transform.TransformPoint(this.mouthOffset) - vector).sqrMagnitude < num;
			if (this.soundActivated)
			{
				bool flag5;
				if (flag)
				{
					RigContainer rigContainer = vrrig.rigContainer;
					if (rigContainer == null)
					{
						flag5 = false;
					}
					else
					{
						PhotonVoiceView voice = rigContainer.Voice;
						bool? flag3 = ((voice != null) ? new bool?(voice.IsSpeaking) : null);
						bool flag4 = true;
						flag5 = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
					}
				}
				else
				{
					flag5 = false;
				}
				flag = flag5;
			}
		}
		this.itemState = (flag ? TransferrableObject.ItemStates.State1 : this.itemState);
	}

	// Token: 0x06000607 RID: 1543 RVA: 0x00022DD4 File Offset: 0x00020FD4
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (TransferrableObject.ItemStates.State1 != this.itemState)
		{
			return;
		}
		if (!this.localWasActivated)
		{
			this.effectsGameObject.SetActive(true);
			this.cooldownRemaining = this.cooldown;
			this.localWasActivated = true;
			UnityEvent onCooldownStart = this.OnCooldownStart;
			if (onCooldownStart != null)
			{
				onCooldownStart.Invoke();
			}
		}
		this.cooldownRemaining -= Time.deltaTime;
		if (this.cooldownRemaining <= 0f)
		{
			this.InitToDefault();
		}
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x00022E4E File Offset: 0x0002104E
	private void InitToDefault()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		this.effectsGameObject.SetActive(false);
		this.cooldownRemaining = this.cooldown;
		this.localWasActivated = false;
		UnityEvent onCooldownReset = this.OnCooldownReset;
		if (onCooldownReset == null)
		{
			return;
		}
		onCooldownReset.Invoke();
	}

	// Token: 0x04000703 RID: 1795
	[Tooltip("This GameObject will activate when held to any gorilla's mouth.")]
	public GameObject effectsGameObject;

	// Token: 0x04000704 RID: 1796
	public float cooldown = 2f;

	// Token: 0x04000705 RID: 1797
	public float mouthPieceZOffset = -0.18f;

	// Token: 0x04000706 RID: 1798
	public float mouthPieceRadius = 0.05f;

	// Token: 0x04000707 RID: 1799
	public Transform mouthPiece;

	// Token: 0x04000708 RID: 1800
	public Vector3 mouthOffset = new Vector3(0f, 0.02f, 0.17f);

	// Token: 0x04000709 RID: 1801
	public bool soundActivated;

	// Token: 0x0400070A RID: 1802
	public UnityEvent OnCooldownStart;

	// Token: 0x0400070B RID: 1803
	public UnityEvent OnCooldownReset;

	// Token: 0x0400070C RID: 1804
	private float cooldownRemaining;

	// Token: 0x0400070D RID: 1805
	private Transform localHead;

	// Token: 0x0400070E RID: 1806
	private PartyHornTransferableObject.PartyHornState partyHornStateLastFrame;

	// Token: 0x0400070F RID: 1807
	private bool localWasActivated;

	// Token: 0x020000ED RID: 237
	private enum PartyHornState
	{
		// Token: 0x04000711 RID: 1809
		None = 1,
		// Token: 0x04000712 RID: 1810
		CoolingDown
	}
}
