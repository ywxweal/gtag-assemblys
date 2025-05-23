using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000413 RID: 1043
public class OneStringGuitar : TransferrableObject
{
	// Token: 0x06001976 RID: 6518 RVA: 0x0007B0E4 File Offset: 0x000792E4
	public override Matrix4x4 GetDefaultTransformationMatrix()
	{
		return Matrix4x4.identity;
	}

	// Token: 0x06001977 RID: 6519 RVA: 0x0007B0EC File Offset: 0x000792EC
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.chestColliderLeft = this._GetChestColliderByPath(rig, "RigAnchor/rig/body/Old Cosmetics Body/OneStringGuitarStick/Center/BaseTransformLeft");
		this.chestColliderRight = this._GetChestColliderByPath(rig, "RigAnchor/rig/body/Old Cosmetics Body/OneStringGuitarStick/Center/BaseTransformRight");
		this.currentChestCollider = this.chestColliderLeft;
		Transform[] array;
		string text;
		if (!GTHardCodedBones.TryGetBoneXforms(rig, out array, out text))
		{
			Debug.LogError("OneStringGuitar: Error getting bone Transforms: " + text, this);
			return;
		}
		this.parentHandLeft = array[9];
		this.parentHandRight = array[27];
		this.parentHand = this.parentHandRight;
		this.leftHandIndicator = GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.rightHandIndicator = GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.sphereRadius = this.leftHandIndicator.GetComponent<SphereCollider>().radius;
		this.itemState = TransferrableObject.ItemStates.State0;
		this.nullHit = default(RaycastHit);
		this.strumList.Add(this.strumCollider);
		this.lastState = OneStringGuitar.GuitarStates.Club;
		this.startingLeftChestOffset = this.chestOffsetLeft;
		this.startingRightChestOffset = this.chestOffsetRight;
		this.startingUnsnapDistance = this.unsnapDistance;
		this.selfInstrumentIndex = rig.AssignInstrumentToInstrumentSelfOnly(this);
		for (int i = 0; i < this.frets.Length; i++)
		{
			this.fretsList.Add(this.frets[i]);
		}
	}

	// Token: 0x06001978 RID: 6520 RVA: 0x0007B234 File Offset: 0x00079434
	private Collider _GetChestColliderByPath(VRRig vrRig, string chestColliderLeftPath)
	{
		Transform transform;
		if (!vrRig.transform.TryFindByExactPath(chestColliderLeftPath, out transform))
		{
			Debug.LogError("DEACTIVATING! do you move this without updating the script? could not find this transform: \"" + chestColliderLeftPath + "\"");
			base.gameObject.SetActive(false);
		}
		Collider component = transform.GetComponent<Collider>();
		if (!component)
		{
			Debug.LogError("DEACTIVATING! found transform but couldn't find collider at path: \"" + chestColliderLeftPath + "\"");
			base.gameObject.SetActive(false);
		}
		return component;
	}

	// Token: 0x06001979 RID: 6521 RVA: 0x0007B2A4 File Offset: 0x000794A4
	internal override void OnEnable()
	{
		base.OnEnable();
		if (this.currentState == TransferrableObject.PositionState.InLeftHand)
		{
			this.fretHandIndicator = this.leftHandIndicator;
			this.strumHandIndicator = this.rightHandIndicator;
			if (base.IsLocalObject())
			{
				this.parentHand = GTPlayer.Instance.leftHandFollower;
			}
		}
		else
		{
			this.fretHandIndicator = this.rightHandIndicator;
			this.strumHandIndicator = this.leftHandIndicator;
			if (base.IsLocalObject())
			{
				this.parentHand = GTPlayer.Instance.rightHandFollower;
			}
		}
		this.initOffset = Vector3.zero;
		this.initRotation = Quaternion.identity;
	}

	// Token: 0x0600197A RID: 6522 RVA: 0x0007B338 File Offset: 0x00079538
	internal override void OnDisable()
	{
		base.OnDisable();
		this.angleSnapped = false;
		this.positionSnapped = false;
		this.lastState = OneStringGuitar.GuitarStates.Club;
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x0600197B RID: 6523 RVA: 0x0007B35C File Offset: 0x0007955C
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (!this.CanDeactivate())
		{
			return false;
		}
		if (base.InHand())
		{
			return false;
		}
		this.itemState = TransferrableObject.ItemStates.State0;
		return true;
	}

	// Token: 0x0600197C RID: 6524 RVA: 0x0007B388 File Offset: 0x00079588
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.lastState != (OneStringGuitar.GuitarStates)this.itemState)
		{
			this.angleSnapped = false;
			this.positionSnapped = false;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			Vector3 vector = ((this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.startPositionLeft : this.startPositionRight);
			Quaternion quaternion = ((this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.startQuatLeft : this.startQuatRight);
			this.UpdateNonPlayingPosition(vector, quaternion);
		}
		else if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			Vector3 vector2 = ((this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.reverseGripPositionLeft : this.reverseGripPositionRight);
			Quaternion quaternion2 = ((this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.reverseGripQuatLeft : this.reverseGripQuatRight);
			this.UpdateNonPlayingPosition(vector2, quaternion2);
			if (this.IsMyItem() && (this.chestTouch.transform.position - this.currentChestCollider.transform.position).magnitude < this.snapDistance)
			{
				this.itemState = TransferrableObject.ItemStates.State2;
				this.angleSnapped = false;
				this.positionSnapped = false;
			}
		}
		else if (this.itemState == TransferrableObject.ItemStates.State2)
		{
			Quaternion quaternion3 = ((this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.holdingOffsetRotationLeft : this.holdingOffsetRotationRight);
			Vector3 vector3 = ((this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.chestOffsetLeft : this.chestOffsetRight);
			Quaternion quaternion4 = Quaternion.LookRotation(this.parentHand.position - this.currentChestCollider.transform.position) * quaternion3;
			if (!this.angleSnapped && Quaternion.Angle(base.transform.rotation, quaternion4) > this.angleLerpSnap)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion4, this.lerpValue);
			}
			else
			{
				this.angleSnapped = true;
				base.transform.rotation = quaternion4;
			}
			Vector3 vector4 = this.currentChestCollider.transform.position + base.transform.rotation * vector3;
			if (!this.positionSnapped && (base.transform.position - vector4).magnitude > this.vectorLerpSnap)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, this.currentChestCollider.transform.position + base.transform.rotation * vector3, this.lerpValue);
			}
			else
			{
				this.positionSnapped = true;
				base.transform.position = vector4;
			}
			if (this.currentState == TransferrableObject.PositionState.InRightHand)
			{
				this.parentHand = this.parentHandRight;
			}
			else
			{
				this.parentHand = this.parentHandLeft;
			}
			if (this.IsMyItem())
			{
				this.unsnapDistance = this.startingUnsnapDistance * base.myRig.transform.localScale.x;
				if (this.currentState == TransferrableObject.PositionState.InRightHand)
				{
					this.chestOffsetRight = Vector3.Scale(this.startingRightChestOffset, base.myRig.transform.localScale);
					this.currentChestCollider = this.chestColliderRight;
					this.fretHandIndicator = this.rightHandIndicator;
					this.strumHandIndicator = this.leftHandIndicator;
				}
				else
				{
					this.chestOffsetLeft = Vector3.Scale(this.startingLeftChestOffset, base.myRig.transform.localScale);
					this.currentChestCollider = this.chestColliderLeft;
					this.fretHandIndicator = this.leftHandIndicator;
					this.strumHandIndicator = this.rightHandIndicator;
				}
				if (this.Unsnap())
				{
					this.itemState = TransferrableObject.ItemStates.State1;
					this.angleSnapped = false;
					this.positionSnapped = false;
					if (this.currentState == TransferrableObject.PositionState.InLeftHand)
					{
						EquipmentInteractor.instance.wasLeftGrabPressed = true;
					}
					else
					{
						EquipmentInteractor.instance.wasRightGrabPressed = true;
					}
				}
				else
				{
					if (!this.handIn)
					{
						this.CheckFretFinger(this.fretHandIndicator.transform);
						HitChecker.CheckHandHit(ref this.collidersHitCount, this.interactableMask, this.sphereRadius, ref this.nullHit, ref this.raycastHits, ref this.raycastHitList, ref this.spherecastSweep, ref this.strumHandIndicator);
						if (this.collidersHitCount > 0)
						{
							int i = 0;
							while (i < this.collidersHitCount)
							{
								if (this.raycastHits[i].collider != null && this.strumCollider == this.raycastHits[i].collider)
								{
									GorillaTagger.Instance.StartVibration(this.strumHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
									this.PlayNote(this.currentFretIndex, Mathf.Max(Mathf.Min(1f, this.strumHandIndicator.currentVelocity.magnitude / this.maxVelocity) * this.maxVolume, this.minVolume));
									if (!NetworkSystem.Instance.InRoom || this.selfInstrumentIndex <= -1)
									{
										break;
									}
									NetworkView myVRRig = GorillaTagger.Instance.myVRRig;
									if (myVRRig == null)
									{
										break;
									}
									myVRRig.SendRPC("RPC_PlaySelfOnlyInstrument", RpcTarget.Others, new object[]
									{
										this.selfInstrumentIndex,
										this.currentFretIndex,
										this.audioSource.volume
									});
									break;
								}
								else
								{
									i++;
								}
							}
						}
					}
					this.handIn = HitChecker.CheckHandIn(ref this.anyHit, ref this.collidersHit, this.sphereRadius * base.transform.lossyScale.x, this.interactableMask, ref this.strumHandIndicator, ref this.strumList);
				}
			}
		}
		this.lastState = (OneStringGuitar.GuitarStates)this.itemState;
	}

	// Token: 0x0600197D RID: 6525 RVA: 0x0007B924 File Offset: 0x00079B24
	public override void PlayNote(int note, float volume)
	{
		this.audioSource.time = 0.005f;
		this.audioSource.clip = this.audioClips[note];
		this.audioSource.volume = volume;
		this.audioSource.GTPlay();
		base.PlayNote(note, volume);
	}

	// Token: 0x0600197E RID: 6526 RVA: 0x0007B974 File Offset: 0x00079B74
	private bool Unsnap()
	{
		return (this.parentHand.position - this.chestTouch.position).magnitude > this.unsnapDistance;
	}

	// Token: 0x0600197F RID: 6527 RVA: 0x0007B9AC File Offset: 0x00079BAC
	private void CheckFretFinger(Transform finger)
	{
		for (int i = 0; i < this.collidersHit.Length; i++)
		{
			this.collidersHit[i] = null;
		}
		this.collidersHitCount = Physics.OverlapSphereNonAlloc(finger.position, this.sphereRadius, this.collidersHit, this.interactableMask, QueryTriggerInteraction.Collide);
		this.currentFretIndex = 5;
		if (this.collidersHitCount > 0)
		{
			for (int j = 0; j < this.collidersHit.Length; j++)
			{
				if (this.fretsList.Contains(this.collidersHit[j]))
				{
					this.currentFretIndex = this.fretsList.IndexOf(this.collidersHit[j]);
					if (this.currentFretIndex != this.lastFretIndex)
					{
						GorillaTagger.Instance.StartVibration(this.fretHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
					}
					this.lastFretIndex = this.currentFretIndex;
					return;
				}
			}
			return;
		}
		if (this.lastFretIndex != -1)
		{
			GorillaTagger.Instance.StartVibration(this.fretHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
		}
		this.lastFretIndex = -1;
	}

	// Token: 0x06001980 RID: 6528 RVA: 0x0007BAE0 File Offset: 0x00079CE0
	public void UpdateNonPlayingPosition(Vector3 positionTarget, Quaternion rotationTarget)
	{
		if (!this.angleSnapped)
		{
			if (Quaternion.Angle(rotationTarget, base.transform.localRotation) < this.angleLerpSnap)
			{
				this.angleSnapped = true;
				base.transform.localRotation = rotationTarget;
			}
			else
			{
				base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, rotationTarget, this.lerpValue);
			}
		}
		if (!this.positionSnapped)
		{
			if ((base.transform.localPosition - positionTarget).magnitude < this.vectorLerpSnap)
			{
				this.positionSnapped = true;
				base.transform.localPosition = positionTarget;
				return;
			}
			base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, positionTarget, this.lerpValue);
		}
	}

	// Token: 0x06001981 RID: 6529 RVA: 0x0007BBA4 File Offset: 0x00079DA4
	public override bool CanDeactivate()
	{
		return !base.gameObject.activeSelf || this.itemState == TransferrableObject.ItemStates.State0 || this.itemState == TransferrableObject.ItemStates.State1;
	}

	// Token: 0x06001982 RID: 6530 RVA: 0x0007BBC7 File Offset: 0x00079DC7
	public override bool CanActivate()
	{
		return this.itemState == TransferrableObject.ItemStates.State0 || this.itemState == TransferrableObject.ItemStates.State1;
	}

	// Token: 0x06001983 RID: 6531 RVA: 0x0007BBDD File Offset: 0x00079DDD
	public override void OnActivate()
	{
		base.OnActivate();
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			this.itemState = TransferrableObject.ItemStates.State1;
			return;
		}
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06001984 RID: 6532 RVA: 0x0007BC00 File Offset: 0x00079E00
	public void GenerateVectorOffsetLeft()
	{
		this.chestOffsetLeft = base.transform.position - this.chestColliderLeft.transform.position;
		this.holdingOffsetRotationLeft = Quaternion.LookRotation(base.transform.position - this.chestColliderLeft.transform.position);
	}

	// Token: 0x06001985 RID: 6533 RVA: 0x0007BC60 File Offset: 0x00079E60
	public void GenerateVectorOffsetRight()
	{
		this.chestOffsetRight = base.transform.position - this.chestColliderRight.transform.position;
		this.holdingOffsetRotationRight = Quaternion.LookRotation(base.transform.position - this.chestColliderRight.transform.position);
	}

	// Token: 0x06001986 RID: 6534 RVA: 0x0007BCBE File Offset: 0x00079EBE
	public void GenerateReverseGripOffsetLeft()
	{
		this.reverseGripPositionLeft = base.transform.localPosition;
		this.reverseGripQuatLeft = base.transform.localRotation;
	}

	// Token: 0x06001987 RID: 6535 RVA: 0x0007BCE2 File Offset: 0x00079EE2
	public void GenerateClubOffsetLeft()
	{
		this.startPositionLeft = base.transform.localPosition;
		this.startQuatLeft = base.transform.localRotation;
	}

	// Token: 0x06001988 RID: 6536 RVA: 0x0007BD06 File Offset: 0x00079F06
	public void GenerateReverseGripOffsetRight()
	{
		this.reverseGripPositionRight = base.transform.localPosition;
		this.reverseGripQuatRight = base.transform.localRotation;
	}

	// Token: 0x06001989 RID: 6537 RVA: 0x0007BD2A File Offset: 0x00079F2A
	public void GenerateClubOffsetRight()
	{
		this.startPositionRight = base.transform.localPosition;
		this.startQuatRight = base.transform.localRotation;
	}

	// Token: 0x0600198A RID: 6538 RVA: 0x0007BD4E File Offset: 0x00079F4E
	public void TestClubPositionRight()
	{
		base.transform.localPosition = this.startPositionRight;
		base.transform.localRotation = this.startQuatRight;
	}

	// Token: 0x0600198B RID: 6539 RVA: 0x0007BD72 File Offset: 0x00079F72
	public void TestReverseGripPositionRight()
	{
		base.transform.localPosition = this.reverseGripPositionRight;
		base.transform.localRotation = this.reverseGripQuatRight;
	}

	// Token: 0x0600198C RID: 6540 RVA: 0x0007BD98 File Offset: 0x00079F98
	public void TestPlayingPositionRight()
	{
		base.transform.rotation = Quaternion.LookRotation(this.parentHand.position - this.currentChestCollider.transform.position) * this.holdingOffsetRotationRight;
		base.transform.position = this.chestColliderRight.transform.position + base.transform.rotation * this.chestOffsetRight;
	}

	// Token: 0x04001C36 RID: 7222
	public Vector3 chestOffsetLeft;

	// Token: 0x04001C37 RID: 7223
	public Vector3 chestOffsetRight;

	// Token: 0x04001C38 RID: 7224
	public Quaternion holdingOffsetRotationLeft;

	// Token: 0x04001C39 RID: 7225
	public Quaternion holdingOffsetRotationRight;

	// Token: 0x04001C3A RID: 7226
	public Quaternion chestRotationOffset;

	// Token: 0x04001C3B RID: 7227
	[NonSerialized]
	public Collider currentChestCollider;

	// Token: 0x04001C3C RID: 7228
	[NonSerialized]
	public Collider chestColliderLeft;

	// Token: 0x04001C3D RID: 7229
	[NonSerialized]
	public Collider chestColliderRight;

	// Token: 0x04001C3E RID: 7230
	public float lerpValue = 0.25f;

	// Token: 0x04001C3F RID: 7231
	public AudioSource audioSource;

	// Token: 0x04001C40 RID: 7232
	private Transform parentHand;

	// Token: 0x04001C41 RID: 7233
	private Transform parentHandLeft;

	// Token: 0x04001C42 RID: 7234
	private Transform parentHandRight;

	// Token: 0x04001C43 RID: 7235
	public float unsnapDistance;

	// Token: 0x04001C44 RID: 7236
	public float snapDistance;

	// Token: 0x04001C45 RID: 7237
	public Vector3 startPositionLeft;

	// Token: 0x04001C46 RID: 7238
	public Quaternion startQuatLeft;

	// Token: 0x04001C47 RID: 7239
	public Vector3 reverseGripPositionLeft;

	// Token: 0x04001C48 RID: 7240
	public Quaternion reverseGripQuatLeft;

	// Token: 0x04001C49 RID: 7241
	public Vector3 startPositionRight;

	// Token: 0x04001C4A RID: 7242
	public Quaternion startQuatRight;

	// Token: 0x04001C4B RID: 7243
	public Vector3 reverseGripPositionRight;

	// Token: 0x04001C4C RID: 7244
	public Quaternion reverseGripQuatRight;

	// Token: 0x04001C4D RID: 7245
	public float angleLerpSnap = 1f;

	// Token: 0x04001C4E RID: 7246
	public float vectorLerpSnap = 0.01f;

	// Token: 0x04001C4F RID: 7247
	private bool angleSnapped;

	// Token: 0x04001C50 RID: 7248
	private bool positionSnapped;

	// Token: 0x04001C51 RID: 7249
	public Transform chestTouch;

	// Token: 0x04001C52 RID: 7250
	private int collidersHitCount;

	// Token: 0x04001C53 RID: 7251
	private Collider[] collidersHit = new Collider[20];

	// Token: 0x04001C54 RID: 7252
	private RaycastHit[] raycastHits = new RaycastHit[20];

	// Token: 0x04001C55 RID: 7253
	private List<RaycastHit> raycastHitList = new List<RaycastHit>();

	// Token: 0x04001C56 RID: 7254
	private RaycastHit nullHit;

	// Token: 0x04001C57 RID: 7255
	public Collider[] collidersToBeIn;

	// Token: 0x04001C58 RID: 7256
	public LayerMask interactableMask;

	// Token: 0x04001C59 RID: 7257
	public int currentFretIndex;

	// Token: 0x04001C5A RID: 7258
	public int lastFretIndex;

	// Token: 0x04001C5B RID: 7259
	public Collider[] frets;

	// Token: 0x04001C5C RID: 7260
	private List<Collider> fretsList = new List<Collider>();

	// Token: 0x04001C5D RID: 7261
	public AudioClip[] audioClips;

	// Token: 0x04001C5E RID: 7262
	private GorillaTriggerColliderHandIndicator leftHandIndicator;

	// Token: 0x04001C5F RID: 7263
	private GorillaTriggerColliderHandIndicator rightHandIndicator;

	// Token: 0x04001C60 RID: 7264
	private GorillaTriggerColliderHandIndicator fretHandIndicator;

	// Token: 0x04001C61 RID: 7265
	private GorillaTriggerColliderHandIndicator strumHandIndicator;

	// Token: 0x04001C62 RID: 7266
	private float sphereRadius;

	// Token: 0x04001C63 RID: 7267
	private bool anyHit;

	// Token: 0x04001C64 RID: 7268
	private bool handIn;

	// Token: 0x04001C65 RID: 7269
	private Vector3 spherecastSweep;

	// Token: 0x04001C66 RID: 7270
	public Collider strumCollider;

	// Token: 0x04001C67 RID: 7271
	public float maxVolume = 1f;

	// Token: 0x04001C68 RID: 7272
	public float minVolume = 0.05f;

	// Token: 0x04001C69 RID: 7273
	public float maxVelocity = 2f;

	// Token: 0x04001C6A RID: 7274
	private List<Collider> strumList = new List<Collider>();

	// Token: 0x04001C6B RID: 7275
	private int selfInstrumentIndex = -1;

	// Token: 0x04001C6C RID: 7276
	private OneStringGuitar.GuitarStates lastState;

	// Token: 0x04001C6D RID: 7277
	private Vector3 startingLeftChestOffset;

	// Token: 0x04001C6E RID: 7278
	private Vector3 startingRightChestOffset;

	// Token: 0x04001C6F RID: 7279
	private float startingUnsnapDistance;

	// Token: 0x02000414 RID: 1044
	private enum GuitarStates
	{
		// Token: 0x04001C71 RID: 7281
		Club = 1,
		// Token: 0x04001C72 RID: 7282
		HeldReverseGrip,
		// Token: 0x04001C73 RID: 7283
		Playing = 4
	}
}
