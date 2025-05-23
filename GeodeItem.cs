using System;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000403 RID: 1027
public class GeodeItem : TransferrableObject
{
	// Token: 0x060018D9 RID: 6361 RVA: 0x00078939 File Offset: 0x00076B39
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.hasEffectsGameObject = this.effectsGameObject != null;
		this.effectsHaveBeenPlayed = false;
	}

	// Token: 0x060018DA RID: 6362 RVA: 0x0007895B File Offset: 0x00076B5B
	protected override void Start()
	{
		base.Start();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.prevItemState = TransferrableObject.ItemStates.State0;
		this.InitToDefault();
	}

	// Token: 0x060018DB RID: 6363 RVA: 0x00078977 File Offset: 0x00076B77
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x060018DC RID: 6364 RVA: 0x0007898C File Offset: 0x00076B8C
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return base.OnRelease(zoneReleased, releasingHand) && this.itemState != TransferrableObject.ItemStates.State0 && !base.InHand();
	}

	// Token: 0x060018DD RID: 6365 RVA: 0x000789B0 File Offset: 0x00076BB0
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		UnityEvent<GeodeItem> onGeodeGrabbed = this.OnGeodeGrabbed;
		if (onGeodeGrabbed == null)
		{
			return;
		}
		onGeodeGrabbed.Invoke(this);
	}

	// Token: 0x060018DE RID: 6366 RVA: 0x000789CC File Offset: 0x00076BCC
	private void InitToDefault()
	{
		this.cooldownRemaining = 0f;
		this.effectsHaveBeenPlayed = false;
		if (this.hasEffectsGameObject)
		{
			this.effectsGameObject.SetActive(false);
		}
		this.geodeFullMesh.SetActive(true);
		for (int i = 0; i < this.geodeCrackedMeshes.Length; i++)
		{
			this.geodeCrackedMeshes[i].SetActive(false);
		}
		this.hitLastFrame = false;
	}

	// Token: 0x060018DF RID: 6367 RVA: 0x00078A34 File Offset: 0x00076C34
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			this.cooldownRemaining -= Time.deltaTime;
			if (this.cooldownRemaining <= 0f)
			{
				this.itemState = TransferrableObject.ItemStates.State0;
				this.OnItemStateChanged();
			}
			return;
		}
		if (this.velocityEstimator.linearVelocity.magnitude < this.minHitVelocity)
		{
			return;
		}
		if (base.InHand())
		{
			int num = Physics.SphereCastNonAlloc(this.geodeFullMesh.transform.position, this.sphereRayRadius * Mathf.Abs(this.geodeFullMesh.transform.lossyScale.x), this.geodeFullMesh.transform.TransformDirection(Vector3.forward), this.collidersHit, this.rayCastMaxDistance, this.collisionLayerMask, QueryTriggerInteraction.Collide);
			this.hitLastFrame = num > 0;
		}
		if (!this.hitLastFrame)
		{
			return;
		}
		if (!GorillaParent.hasInstance)
		{
			return;
		}
		UnityEvent<GeodeItem> onGeodeCracked = this.OnGeodeCracked;
		if (onGeodeCracked != null)
		{
			onGeodeCracked.Invoke(this);
		}
		this.itemState = TransferrableObject.ItemStates.State1;
		this.cooldownRemaining = this.cooldown;
		this.index = (this.randomizeGeode ? this.RandomPickCrackedGeode() : 0);
	}

	// Token: 0x060018E0 RID: 6368 RVA: 0x00078B5C File Offset: 0x00076D5C
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		this.currentItemState = this.itemState;
		if (this.currentItemState != this.prevItemState)
		{
			this.OnItemStateChanged();
		}
		this.prevItemState = this.currentItemState;
	}

	// Token: 0x060018E1 RID: 6369 RVA: 0x00078B90 File Offset: 0x00076D90
	private void OnItemStateChanged()
	{
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			this.InitToDefault();
			return;
		}
		this.geodeFullMesh.SetActive(false);
		for (int i = 0; i < this.geodeCrackedMeshes.Length; i++)
		{
			this.geodeCrackedMeshes[i].SetActive(i == this.index);
		}
		RigContainer rigContainer;
		if (NetworkSystem.Instance.InRoom && GorillaGameManager.instance != null && !this.effectsHaveBeenPlayed && VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.LocalPlayer, out rigContainer))
		{
			rigContainer.Rig.netView.SendRPC("RPC_PlayGeodeEffect", RpcTarget.All, new object[] { this.geodeFullMesh.transform.position });
			this.effectsHaveBeenPlayed = true;
		}
		if (!NetworkSystem.Instance.InRoom && !this.effectsHaveBeenPlayed)
		{
			if (this.audioSource)
			{
				this.audioSource.GTPlay();
			}
			this.effectsHaveBeenPlayed = true;
		}
	}

	// Token: 0x060018E2 RID: 6370 RVA: 0x00078C89 File Offset: 0x00076E89
	private int RandomPickCrackedGeode()
	{
		return Random.Range(0, this.geodeCrackedMeshes.Length);
	}

	// Token: 0x04001BB2 RID: 7090
	[Tooltip("This GameObject will activate when the geode hits the ground with enough force.")]
	public GameObject effectsGameObject;

	// Token: 0x04001BB3 RID: 7091
	public LayerMask collisionLayerMask;

	// Token: 0x04001BB4 RID: 7092
	[Tooltip("Used to calculate velocity of the geode.")]
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04001BB5 RID: 7093
	public float cooldown = 5f;

	// Token: 0x04001BB6 RID: 7094
	[Tooltip("The velocity of the geode must be greater than this value to activate the effect.")]
	public float minHitVelocity = 0.2f;

	// Token: 0x04001BB7 RID: 7095
	[Tooltip("Geode's full mesh before cracking")]
	public GameObject geodeFullMesh;

	// Token: 0x04001BB8 RID: 7096
	[Tooltip("Geode's cracked open half different meshes, picked randomly")]
	public GameObject[] geodeCrackedMeshes;

	// Token: 0x04001BB9 RID: 7097
	[Tooltip("The distance between te geode and the layer mask to detect whether it hits it")]
	public float rayCastMaxDistance = 0.2f;

	// Token: 0x04001BBA RID: 7098
	[FormerlySerializedAs("collisionRadius")]
	public float sphereRayRadius = 0.05f;

	// Token: 0x04001BBB RID: 7099
	[DebugReadout]
	private float cooldownRemaining;

	// Token: 0x04001BBC RID: 7100
	[DebugReadout]
	private bool hitLastFrame;

	// Token: 0x04001BBD RID: 7101
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04001BBE RID: 7102
	public bool randomizeGeode = true;

	// Token: 0x04001BBF RID: 7103
	public UnityEvent<GeodeItem> OnGeodeCracked;

	// Token: 0x04001BC0 RID: 7104
	public UnityEvent<GeodeItem> OnGeodeGrabbed;

	// Token: 0x04001BC1 RID: 7105
	private bool hasEffectsGameObject;

	// Token: 0x04001BC2 RID: 7106
	private bool effectsHaveBeenPlayed;

	// Token: 0x04001BC3 RID: 7107
	private RaycastHit hit;

	// Token: 0x04001BC4 RID: 7108
	private RaycastHit[] collidersHit = new RaycastHit[20];

	// Token: 0x04001BC5 RID: 7109
	private TransferrableObject.ItemStates currentItemState;

	// Token: 0x04001BC6 RID: 7110
	private TransferrableObject.ItemStates prevItemState;

	// Token: 0x04001BC7 RID: 7111
	private int index;
}
