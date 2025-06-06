﻿using System;
using GorillaExtensions;
using GorillaLocomotion;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200066D RID: 1645
public class HoverboardVisual : MonoBehaviour, ICallBack
{
	// Token: 0x170003E7 RID: 999
	// (get) Token: 0x06002918 RID: 10520 RVA: 0x000CCB52 File Offset: 0x000CAD52
	// (set) Token: 0x06002919 RID: 10521 RVA: 0x000CCB5A File Offset: 0x000CAD5A
	public Color boardColor { get; private set; }

	// Token: 0x0600291A RID: 10522 RVA: 0x000CCB64 File Offset: 0x000CAD64
	private void Awake()
	{
		Material[] sharedMaterials = this.boardMesh.sharedMaterials;
		this.colorMaterial = new Material(sharedMaterials[1]);
		sharedMaterials[1] = this.colorMaterial;
		this.boardMesh.sharedMaterials = sharedMaterials;
	}

	// Token: 0x170003E8 RID: 1000
	// (get) Token: 0x0600291B RID: 10523 RVA: 0x000CCBA0 File Offset: 0x000CADA0
	// (set) Token: 0x0600291C RID: 10524 RVA: 0x000CCBA8 File Offset: 0x000CADA8
	public bool IsHeld { get; private set; }

	// Token: 0x170003E9 RID: 1001
	// (get) Token: 0x0600291D RID: 10525 RVA: 0x000CCBB1 File Offset: 0x000CADB1
	// (set) Token: 0x0600291E RID: 10526 RVA: 0x000CCBB9 File Offset: 0x000CADB9
	public bool IsLeftHanded { get; private set; }

	// Token: 0x170003EA RID: 1002
	// (get) Token: 0x0600291F RID: 10527 RVA: 0x000CCBC2 File Offset: 0x000CADC2
	// (set) Token: 0x06002920 RID: 10528 RVA: 0x000CCBCA File Offset: 0x000CADCA
	public Vector3 NominalLocalPosition { get; private set; }

	// Token: 0x170003EB RID: 1003
	// (get) Token: 0x06002921 RID: 10529 RVA: 0x000CCBD3 File Offset: 0x000CADD3
	// (set) Token: 0x06002922 RID: 10530 RVA: 0x000CCBDB File Offset: 0x000CADDB
	public Quaternion NominalLocalRotation { get; private set; }

	// Token: 0x170003EC RID: 1004
	// (get) Token: 0x06002923 RID: 10531 RVA: 0x000CCBE4 File Offset: 0x000CADE4
	private Transform NominalParentTransform
	{
		get
		{
			if (!this.IsHeld)
			{
				return base.transform.parent;
			}
			return (this.IsLeftHanded ? this.parentRig.leftHand : this.parentRig.rightHand).rigTarget.transform;
		}
	}

	// Token: 0x06002924 RID: 10532 RVA: 0x000CCC24 File Offset: 0x000CAE24
	public void SetIsHeld(bool isHeldLeftHanded, Vector3 localPosition, Quaternion localRotation, Color boardColor)
	{
		if (!this.isCallbackActive)
		{
			this.parentRig.AddLateUpdateCallback(this);
			this.isCallbackActive = true;
		}
		this.IsHeld = true;
		base.gameObject.SetActive(true);
		this.IsLeftHanded = isHeldLeftHanded;
		this.NominalLocalPosition = localPosition;
		this.NominalLocalRotation = localRotation;
		Transform nominalParentTransform = this.NominalParentTransform;
		this.interpolatedLocalPosition = nominalParentTransform.InverseTransformPoint(base.transform.position);
		this.interpolatedLocalRotation = nominalParentTransform.InverseTransformRotation(base.transform.rotation);
		this.positionLerpSpeed = (this.interpolatedLocalPosition - this.NominalLocalPosition).magnitude / this.lerpIntoHandDuration;
		float num;
		Vector3 vector;
		(Quaternion.Inverse(this.interpolatedLocalRotation) * this.NominalLocalRotation).ToAngleAxis(out num, out vector);
		this.rotationLerpSpeed = num / this.lerpIntoHandDuration;
		if (this.parentRig.isLocal)
		{
			GTPlayer.Instance.SetHoverActive(true);
		}
		this.colorMaterial.color = boardColor;
		this.boardColor = boardColor;
	}

	// Token: 0x06002925 RID: 10533 RVA: 0x000CCD2D File Offset: 0x000CAF2D
	public void SetNotHeld(bool isLeftHanded)
	{
		this.IsLeftHanded = isLeftHanded;
		this.SetNotHeld();
	}

	// Token: 0x06002926 RID: 10534 RVA: 0x000CCD3C File Offset: 0x000CAF3C
	public void SetNotHeld()
	{
		bool isHeld = this.IsHeld;
		base.gameObject.SetActive(false);
		this.IsHeld = false;
		this.interpolatedLocalPosition = base.transform.localPosition;
		this.interpolatedLocalRotation = base.transform.localRotation;
		this.positionLerpSpeed = (this.interpolatedLocalPosition - this.NominalLocalPosition).magnitude / this.lerpIntoHandDuration;
		float num;
		Vector3 vector;
		(Quaternion.Inverse(this.interpolatedLocalRotation) * this.NominalLocalRotation).ToAngleAxis(out num, out vector);
		this.rotationLerpSpeed = num / this.lerpIntoHandDuration;
		if (!isHeld)
		{
			base.transform.position = base.transform.parent.TransformPoint(this.NominalLocalPosition);
			base.transform.rotation = base.transform.parent.TransformRotation(this.NominalLocalRotation);
		}
		if (this.parentRig.isLocal)
		{
			GTPlayer.Instance.SetHoverActive(false);
		}
		this.hoverboardAudio.Stop();
	}

	// Token: 0x06002927 RID: 10535 RVA: 0x000CCE44 File Offset: 0x000CB044
	void ICallBack.CallBack()
	{
		Transform nominalParentTransform = this.NominalParentTransform;
		if ((this.interpolatedLocalPosition - this.NominalLocalPosition).IsShorterThan(0.01f))
		{
			base.transform.position = nominalParentTransform.TransformPoint(this.NominalLocalPosition);
			base.transform.rotation = nominalParentTransform.TransformRotation(this.NominalLocalRotation);
			if (!this.IsHeld)
			{
				this.parentRig.RemoveLateUpdateCallback(this);
				this.isCallbackActive = false;
			}
		}
		else
		{
			this.interpolatedLocalPosition = Vector3.MoveTowards(this.interpolatedLocalPosition, this.NominalLocalPosition, this.positionLerpSpeed * Time.deltaTime);
			this.interpolatedLocalRotation = Quaternion.RotateTowards(this.interpolatedLocalRotation, this.NominalLocalRotation, this.rotationLerpSpeed * Time.deltaTime);
			base.transform.position = nominalParentTransform.TransformPoint(this.interpolatedLocalPosition);
			base.transform.rotation = nominalParentTransform.TransformRotation(this.interpolatedLocalRotation);
		}
		if (this.IsHeld)
		{
			if (this.parentRig.isLocal)
			{
				GTPlayer.Instance.SetHoverboardPosRot(base.transform.position, base.transform.rotation);
				return;
			}
			this.hoverboardAudio.UpdateAudioLoop(this.parentRig.LatestVelocity().magnitude, 0f, 0f, 0f);
		}
	}

	// Token: 0x06002928 RID: 10536 RVA: 0x000CCF9A File Offset: 0x000CB19A
	public void PlayGrindHaptic()
	{
		if (this.IsHeld)
		{
			GorillaTagger.Instance.StartVibration(this.IsLeftHanded, this.grindHapticStrength, this.grindHapticDuration);
		}
	}

	// Token: 0x06002929 RID: 10537 RVA: 0x000CCFC0 File Offset: 0x000CB1C0
	public void PlayCarveHaptic(float carveForce)
	{
		if (this.IsHeld)
		{
			GorillaTagger.Instance.StartVibration(this.IsLeftHanded, carveForce * this.carveHapticStrength, this.carveHapticDuration);
		}
	}

	// Token: 0x0600292A RID: 10538 RVA: 0x000CCFE8 File Offset: 0x000CB1E8
	public void ProxyGrabHandle(bool isLeftHand)
	{
		EquipmentInteractor.instance.UpdateHandEquipment(this.handlePosition, isLeftHand);
	}

	// Token: 0x0600292B RID: 10539 RVA: 0x000CCFFD File Offset: 0x000CB1FD
	public void DropFreeBoard()
	{
		FreeHoverboardManager.instance.SendDropBoardRPC(base.transform.position, base.transform.rotation, this.velocityEstimator.linearVelocity, this.velocityEstimator.angularVelocity, this.boardColor);
	}

	// Token: 0x0600292C RID: 10540 RVA: 0x000CD03B File Offset: 0x000CB23B
	public void SetRaceDisplay(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			this.racePositionReadout.gameObject.SetActive(false);
			return;
		}
		this.racePositionReadout.gameObject.SetActive(true);
		this.racePositionReadout.text = text;
	}

	// Token: 0x0600292D RID: 10541 RVA: 0x000CD074 File Offset: 0x000CB274
	public void SetRaceLapsDisplay(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			this.raceLapsReadout.gameObject.SetActive(false);
			return;
		}
		this.raceLapsReadout.gameObject.SetActive(true);
		this.raceLapsReadout.text = text;
	}

	// Token: 0x04002E28 RID: 11816
	[SerializeField]
	private VRRig parentRig;

	// Token: 0x04002E29 RID: 11817
	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04002E2A RID: 11818
	[SerializeField]
	[FormerlySerializedAs("audio")]
	private HoverboardAudio hoverboardAudio;

	// Token: 0x04002E2B RID: 11819
	[SerializeField]
	private HoverboardHandle handlePosition;

	// Token: 0x04002E2C RID: 11820
	[SerializeField]
	private float grindHapticStrength;

	// Token: 0x04002E2D RID: 11821
	[SerializeField]
	private float grindHapticDuration;

	// Token: 0x04002E2E RID: 11822
	[SerializeField]
	private float carveHapticStrength;

	// Token: 0x04002E2F RID: 11823
	[SerializeField]
	private float carveHapticDuration;

	// Token: 0x04002E30 RID: 11824
	[SerializeField]
	private MeshRenderer boardMesh;

	// Token: 0x04002E31 RID: 11825
	[SerializeField]
	private InteractionPoint handleInteractionPoint;

	// Token: 0x04002E32 RID: 11826
	[SerializeField]
	private TextMeshPro racePositionReadout;

	// Token: 0x04002E33 RID: 11827
	[SerializeField]
	private TextMeshPro raceLapsReadout;

	// Token: 0x04002E34 RID: 11828
	private Material colorMaterial;

	// Token: 0x04002E3A RID: 11834
	private Vector3 interpolatedLocalPosition;

	// Token: 0x04002E3B RID: 11835
	private Quaternion interpolatedLocalRotation;

	// Token: 0x04002E3C RID: 11836
	[SerializeField]
	private float lerpIntoHandDuration;

	// Token: 0x04002E3D RID: 11837
	private float positionLerpSpeed;

	// Token: 0x04002E3E RID: 11838
	private float rotationLerpSpeed;

	// Token: 0x04002E3F RID: 11839
	private bool isCallbackActive;
}
