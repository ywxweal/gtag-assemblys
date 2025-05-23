using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020000EA RID: 234
public class SnowballThrowable : HoldableObject
{
	// Token: 0x17000077 RID: 119
	// (get) Token: 0x060005E3 RID: 1507 RVA: 0x00022161 File Offset: 0x00020361
	internal int ProjectileHash
	{
		get
		{
			return PoolUtils.GameObjHashCode(this.randomModelSelection ? this.localModels[this.randModelIndex].projectilePrefab : this.projectilePrefab);
		}
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x00022190 File Offset: 0x00020390
	protected virtual void Awake()
	{
		if (this.awakeHasBeenCalled)
		{
			return;
		}
		this.awakeHasBeenCalled = true;
		this.targetRig = base.GetComponentInParent<VRRig>(true);
		this.isOfflineRig = this.targetRig != null && this.targetRig.isOfflineVRRig;
		this.renderers = base.GetComponentsInChildren<Renderer>();
		this.matPropBlock = new MaterialPropertyBlock();
		this.randModelIndex = -1;
		foreach (BucketThrowable bucketThrowable in this.localModels)
		{
			if (bucketThrowable != null)
			{
				BucketThrowable bucketThrowable2 = bucketThrowable;
				bucketThrowable2.OnTriggerEntered = (UnityAction<bool>)Delegate.Combine(bucketThrowable2.OnTriggerEntered, new UnityAction<bool>(this.HandleOnGorillaHeadTriggerEntered));
			}
		}
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x00022264 File Offset: 0x00020464
	public bool IsMine()
	{
		return this.targetRig != null && this.targetRig.isOfflineVRRig;
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x00022284 File Offset: 0x00020484
	public virtual void OnEnable()
	{
		if (this.targetRig == null)
		{
			Debug.LogError("SnowballThrowable: targetRig is null! Deactivating.");
			base.gameObject.SetActive(false);
			return;
		}
		if (!this.targetRig.isOfflineVRRig)
		{
			if (this.targetRig.netView != null && this.targetRig.netView.IsMine)
			{
				base.gameObject.SetActive(false);
				return;
			}
			Color32 throwableProjectileColor = this.targetRig.GetThrowableProjectileColor(this.isLeftHanded);
			this.ApplyColor(throwableProjectileColor);
			if (this.randomModelSelection)
			{
				foreach (BucketThrowable bucketThrowable in this.localModels)
				{
					bucketThrowable.gameObject.SetActive(false);
				}
				this.randModelIndex = this.targetRig.GetRandomThrowableModelIndex();
				this.EnableRandomModel(this.randModelIndex, true);
			}
		}
		this.AnchorToHand();
		this.OnEnableHasBeenCalled = true;
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void OnDisable()
	{
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnDestroy()
	{
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x00022394 File Offset: 0x00020594
	public void SetSnowballActiveLocal(bool enabled)
	{
		if (!this.awakeHasBeenCalled)
		{
			this.Awake();
		}
		if (!this.OnEnableHasBeenCalled)
		{
			this.OnEnable();
		}
		if (this.isLeftHanded)
		{
			this.targetRig.LeftThrowableProjectileIndex = (enabled ? this.throwableMakerIndex : (-1));
		}
		else
		{
			this.targetRig.RightThrowableProjectileIndex = (enabled ? this.throwableMakerIndex : (-1));
		}
		bool flag = !base.gameObject.activeSelf && enabled;
		base.gameObject.SetActive(enabled);
		if (flag && this.pickupSoundBankPlayer != null)
		{
			this.pickupSoundBankPlayer.Play();
		}
		if (this.randomModelSelection)
		{
			if (enabled)
			{
				this.EnableRandomModel(this.GetRandomModelIndex(), true);
			}
			else
			{
				this.EnableRandomModel(this.randModelIndex, false);
			}
			this.targetRig.SetRandomThrowableModelIndex(this.randModelIndex);
		}
		EquipmentInteractor.instance.UpdateHandEquipment(enabled ? this : null, this.isLeftHanded);
		if (this.randomizeColor)
		{
			Color color = (enabled ? GTColor.RandomHSV(this.randomColorHSVRanges) : Color.white);
			this.targetRig.SetThrowableProjectileColor(this.isLeftHanded, color);
			this.ApplyColor(color);
		}
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x000224BC File Offset: 0x000206BC
	private int GetRandomModelIndex()
	{
		if (this.localModels.Count == 0)
		{
			return -1;
		}
		this.randModelIndex = Random.Range(0, this.localModels.Count);
		if ((float)Random.Range(1, 100) <= this.localModels[this.randModelIndex].weightedChance * 100f)
		{
			return this.randModelIndex;
		}
		return this.GetRandomModelIndex();
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x00022524 File Offset: 0x00020724
	private void EnableRandomModel(int index, bool enable)
	{
		if (this.randModelIndex >= 0 && this.randModelIndex < this.localModels.Count)
		{
			this.localModels[this.randModelIndex].gameObject.SetActive(enable);
			if (enable && this.localModels[this.randModelIndex].destroyAfterSeconds > 0f)
			{
				this.destroyTimer = 0f;
			}
			return;
		}
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x00022598 File Offset: 0x00020798
	protected virtual void LateUpdateLocal()
	{
		if (this.randomModelSelection && this.randModelIndex > -1 && this.localModels[this.randModelIndex].destroyAfterSeconds > 0f)
		{
			this.destroyTimer += Time.deltaTime;
			if (this.destroyTimer > this.localModels[this.randModelIndex].destroyAfterSeconds)
			{
				if (this.localModels[this.randModelIndex].gameObject.activeSelf)
				{
					this.PerformSnowballThrowAuthority();
				}
				this.destroyTimer = -1f;
			}
		}
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x000023F4 File Offset: 0x000005F4
	protected void LateUpdateReplicated()
	{
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x000023F4 File Offset: 0x000005F4
	protected void LateUpdateShared()
	{
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x00022634 File Offset: 0x00020834
	private Transform Anchor()
	{
		return base.transform.parent;
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x00022644 File Offset: 0x00020844
	private void AnchorToHand()
	{
		BodyDockPositions myBodyDockPositions = this.targetRig.myBodyDockPositions;
		Transform transform = this.Anchor();
		if (this.isLeftHanded)
		{
			transform.parent = myBodyDockPositions.leftHandTransform;
		}
		else
		{
			transform.parent = myBodyDockPositions.rightHandTransform;
		}
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
	}

	// Token: 0x060005F1 RID: 1521 RVA: 0x0002269C File Offset: 0x0002089C
	protected void LateUpdate()
	{
		if (this.IsMine())
		{
			this.LateUpdateLocal();
		}
		else
		{
			this.LateUpdateReplicated();
		}
		this.LateUpdateShared();
	}

	// Token: 0x060005F2 RID: 1522 RVA: 0x000226BA File Offset: 0x000208BA
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		this.OnSnowballRelease();
		return true;
	}

	// Token: 0x060005F3 RID: 1523 RVA: 0x000226CF File Offset: 0x000208CF
	protected virtual void OnSnowballRelease()
	{
		this.PerformSnowballThrowAuthority();
	}

	// Token: 0x060005F4 RID: 1524 RVA: 0x000226D8 File Offset: 0x000208D8
	protected virtual void PerformSnowballThrowAuthority()
	{
		if (!(this.targetRig != null) || this.targetRig.creator == null || !this.targetRig.creator.IsLocal)
		{
			return;
		}
		Vector3 vector = Vector3.zero;
		Rigidbody component = GorillaTagger.Instance.GetComponent<Rigidbody>();
		if (component != null)
		{
			vector = component.velocity;
		}
		Vector3 vector2 = this.velocityEstimator.linearVelocity - vector;
		float magnitude = vector2.magnitude;
		if (magnitude > 0.001f)
		{
			float num = Mathf.Clamp(magnitude * this.linSpeedMultiplier, 0f, this.maxLinSpeed);
			vector2 *= num / magnitude;
		}
		Vector3 vector3 = vector2 + vector;
		Color32 throwableProjectileColor = this.targetRig.GetThrowableProjectileColor(this.isLeftHanded);
		Transform transform = base.transform;
		Vector3 position = transform.position;
		float x = transform.lossyScale.x;
		SlingshotProjectile slingshotProjectile = this.LaunchSnowballLocal(position, vector3, x, this.randomizeColor, throwableProjectileColor);
		this.SetSnowballActiveLocal(false);
		if (this.randModelIndex > -1 && this.randModelIndex < this.localModels.Count && this.localModels[this.randModelIndex].destroyAfterRelease)
		{
			slingshotProjectile.DestroyAfterRelease();
		}
		if (NetworkSystem.Instance.InRoom)
		{
			RoomSystem.SendLaunchProjectile(position, vector3, this.isLeftHanded ? RoomSystem.ProjectileSource.LeftHand : RoomSystem.ProjectileSource.RightHand, slingshotProjectile.myProjectileCount, this.randomizeColor, throwableProjectileColor.r, throwableProjectileColor.g, throwableProjectileColor.b, throwableProjectileColor.a);
		}
	}

	// Token: 0x060005F5 RID: 1525 RVA: 0x0002285C File Offset: 0x00020A5C
	protected virtual SlingshotProjectile LaunchSnowballLocal(Vector3 location, Vector3 velocity, float scale, bool randomColour, Color colour)
	{
		SlingshotProjectile component = ObjectPools.instance.Instantiate(this.randomModelSelection ? this.localModels[this.randModelIndex].projectilePrefab : this.projectilePrefab, true).GetComponent<SlingshotProjectile>();
		int num = ProjectileTracker.AddAndIncrementLocalProjectile(component, velocity, location, scale);
		component.Launch(location, velocity, NetworkSystem.Instance.LocalPlayer, false, false, num, scale, randomColour, colour);
		if (string.IsNullOrEmpty(this.throwEventName))
		{
			PlayerGameEvents.LaunchedProjectile(this.projectilePrefab.name);
		}
		else
		{
			PlayerGameEvents.LaunchedProjectile(this.throwEventName);
		}
		component.OnImpact += this.OnProjectileImpact;
		return component;
	}

	// Token: 0x060005F6 RID: 1526 RVA: 0x00022900 File Offset: 0x00020B00
	protected virtual SlingshotProjectile SpawnProjectile()
	{
		return ObjectPools.instance.Instantiate(this.randomModelSelection ? this.localModels[this.randModelIndex].projectilePrefab : this.projectilePrefab, true).GetComponent<SlingshotProjectile>();
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x00022938 File Offset: 0x00020B38
	protected virtual void OnProjectileImpact(SlingshotProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer)
	{
		if (hitPlayer != null)
		{
			ScienceExperimentManager instance = ScienceExperimentManager.instance;
			if (instance != null && this.projectilePrefab != null && this.projectilePrefab == instance.waterBalloonPrefab)
			{
				instance.OnWaterBalloonHitPlayer(hitPlayer);
			}
		}
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x00022984 File Offset: 0x00020B84
	private void ApplyColor(Color newColor)
	{
		foreach (Renderer renderer in this.renderers)
		{
			if (renderer)
			{
				foreach (Material material in renderer.materials)
				{
					if (!(material == null))
					{
						if (material.HasProperty("_BaseColor"))
						{
							material.SetColor("_BaseColor", newColor);
						}
						if (material.HasProperty("_Color"))
						{
							material.SetColor("_Color", newColor);
						}
					}
				}
			}
		}
	}

	// Token: 0x060005F9 RID: 1529 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void DropItemCleanup()
	{
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x00022A11 File Offset: 0x00020C11
	private void HandleOnGorillaHeadTriggerEntered(bool enable)
	{
		this.SetSnowballActiveLocal(enable);
	}

	// Token: 0x040006E3 RID: 1763
	[GorillaSoundLookup]
	public List<int> matDataIndexes = new List<int> { 32 };

	// Token: 0x040006E4 RID: 1764
	public GameObject projectilePrefab;

	// Token: 0x040006E5 RID: 1765
	[FormerlySerializedAs("shouldColorize")]
	public bool randomizeColor;

	// Token: 0x040006E6 RID: 1766
	public GTColor.HSVRanges randomColorHSVRanges = new GTColor.HSVRanges(0f, 1f, 0.7f, 1f, 1f, 1f);

	// Token: 0x040006E7 RID: 1767
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x040006E8 RID: 1768
	public SoundBankPlayer pickupSoundBankPlayer;

	// Token: 0x040006E9 RID: 1769
	public float linSpeedMultiplier = 1f;

	// Token: 0x040006EA RID: 1770
	public float maxLinSpeed = 12f;

	// Token: 0x040006EB RID: 1771
	public float maxWristSpeed = 4f;

	// Token: 0x040006EC RID: 1772
	public bool isLeftHanded;

	// Token: 0x040006ED RID: 1773
	[Tooltip("Check this part only if we want to randomize the prefab meshes and projectile")]
	public bool randomModelSelection;

	// Token: 0x040006EE RID: 1774
	public List<BucketThrowable> localModels;

	// Token: 0x040006EF RID: 1775
	[Tooltip("This needs to match the index of the projectilePrefab in Body Dock Position")]
	public int throwableMakerIndex;

	// Token: 0x040006F0 RID: 1776
	public string throwEventName;

	// Token: 0x040006F1 RID: 1777
	protected VRRig targetRig;

	// Token: 0x040006F2 RID: 1778
	protected bool isOfflineRig;

	// Token: 0x040006F3 RID: 1779
	private bool awakeHasBeenCalled;

	// Token: 0x040006F4 RID: 1780
	private bool OnEnableHasBeenCalled;

	// Token: 0x040006F5 RID: 1781
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x040006F6 RID: 1782
	private static readonly int colorShaderProp = Shader.PropertyToID("_Color");

	// Token: 0x040006F7 RID: 1783
	private Renderer[] renderers;

	// Token: 0x040006F8 RID: 1784
	protected int randModelIndex;

	// Token: 0x040006F9 RID: 1785
	private float destroyTimer = -1f;
}
