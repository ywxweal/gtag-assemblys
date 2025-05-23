﻿using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x0200007C RID: 124
public class MenagerieCritter : MonoBehaviour, IHoldableObject, IEyeScannable
{
	// Token: 0x17000030 RID: 48
	// (get) Token: 0x0600031D RID: 797 RVA: 0x00013346 File Offset: 0x00011546
	public Menagerie.CritterData CritterData
	{
		get
		{
			return this._critterData;
		}
	}

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x0600031E RID: 798 RVA: 0x0001334E File Offset: 0x0001154E
	// (set) Token: 0x0600031F RID: 799 RVA: 0x00013358 File Offset: 0x00011558
	public MenagerieSlot Slot
	{
		get
		{
			return this._slot;
		}
		set
		{
			if (value == this._slot)
			{
				return;
			}
			if (this._slot && this._slot.critter == this)
			{
				this._slot.critter = null;
			}
			this._slot = value;
			if (this._slot)
			{
				this._slot.critter = this;
			}
		}
	}

	// Token: 0x06000320 RID: 800 RVA: 0x000133C0 File Offset: 0x000115C0
	private void Update()
	{
		this.UpdateAnimation();
	}

	// Token: 0x06000321 RID: 801 RVA: 0x000133C8 File Offset: 0x000115C8
	public void ApplyCritterData(Menagerie.CritterData critterData)
	{
		this._critterData = critterData;
		this._critterConfiguration = this._critterData.GetConfiguration();
		this._critterData.instance = this;
		this._critterData.GetConfiguration().ApplyVisualsTo(this.visuals, false);
		this.visuals.SetAppearance(this._critterData.appearance);
		this._animRoot = this.visuals.bodyRoot;
		this._bodyScale = this._animRoot.localScale;
		this.PlayAnimation(this.heldAnimation, global::UnityEngine.Random.value);
	}

	// Token: 0x06000322 RID: 802 RVA: 0x0001345C File Offset: 0x0001165C
	private void PlayAnimation(CrittersAnim anim, float time = 0f)
	{
		this._currentAnim = anim;
		this._currentAnimTime = time;
		if (this._currentAnim == null)
		{
			this._animRoot.localPosition = Vector3.zero;
			this._animRoot.localRotation = Quaternion.identity;
			this._animRoot.localScale = this._bodyScale;
		}
	}

	// Token: 0x06000323 RID: 803 RVA: 0x000134B0 File Offset: 0x000116B0
	private void UpdateAnimation()
	{
		if (this._currentAnim != null)
		{
			this._currentAnimTime += Time.deltaTime * this._currentAnim.playSpeed;
			this._currentAnimTime %= 1f;
			float num = this._currentAnim.squashAmount.Evaluate(this._currentAnimTime);
			float num2 = this._currentAnim.forwardOffset.Evaluate(this._currentAnimTime);
			float num3 = this._currentAnim.horizontalOffset.Evaluate(this._currentAnimTime);
			float num4 = this._currentAnim.verticalOffset.Evaluate(this._currentAnimTime);
			this._animRoot.localPosition = Vector3.Scale(this._bodyScale, new Vector3(num3, num4, num2));
			float num5 = 1f - num;
			num5 *= 0.5f;
			num5 += 1f;
			this._animRoot.localScale = Vector3.Scale(this._bodyScale, new Vector3(num5, num, num5));
		}
	}

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x06000324 RID: 804 RVA: 0x00002076 File Offset: 0x00000276
	public bool TwoHanded
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000325 RID: 805 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06000326 RID: 806 RVA: 0x000135B0 File Offset: 0x000117B0
	public void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		this.isHeld = true;
		this.isHeldLeftHand = grabbingHand == EquipmentInteractor.instance.leftHand;
		if (this.grabbedHaptics)
		{
			CrittersManager.PlayHaptics(this.grabbedHaptics, this.grabbedHapticsStrength, this.isHeldLeftHand);
		}
		if (this.grabbedFX)
		{
			this.grabbedFX.SetActive(true);
		}
		EquipmentInteractor.instance.UpdateHandEquipment(this, this.isHeldLeftHand);
		base.transform.parent = grabbingHand.transform;
		this.isHeld = true;
		this.heldBy = grabbingHand;
		Action onDataChange = this.OnDataChange;
		if (onDataChange == null)
		{
			return;
		}
		onDataChange();
	}

	// Token: 0x06000327 RID: 807 RVA: 0x0001365C File Offset: 0x0001185C
	public bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.rightHand)
		{
			return false;
		}
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.leftHand)
		{
			return false;
		}
		if (this.grabbedHaptics)
		{
			CrittersManager.StopHaptics(this.isHeldLeftHand);
		}
		if (this.grabbedFX)
		{
			this.grabbedFX.SetActive(false);
		}
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.isHeldLeftHand);
		this.isHeld = false;
		this.isHeldLeftHand = false;
		Action<MenagerieCritter> onReleased = this.OnReleased;
		if (onReleased != null)
		{
			onReleased(this);
		}
		Action onDataChange = this.OnDataChange;
		if (onDataChange != null)
		{
			onDataChange();
		}
		this.ResetToTransform();
		return true;
	}

	// Token: 0x06000328 RID: 808 RVA: 0x0001372F File Offset: 0x0001192F
	public void ResetToTransform()
	{
		base.transform.parent = this._slot.transform;
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = quaternion.identity;
	}

	// Token: 0x06000329 RID: 809 RVA: 0x000023F4 File Offset: 0x000005F4
	public void DropItemCleanup()
	{
	}

	// Token: 0x17000033 RID: 51
	// (get) Token: 0x0600032A RID: 810 RVA: 0x0000FE4F File Offset: 0x0000E04F
	int IEyeScannable.scannableId
	{
		get
		{
			return base.gameObject.GetInstanceID();
		}
	}

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x0600032B RID: 811 RVA: 0x0001376C File Offset: 0x0001196C
	Vector3 IEyeScannable.Position
	{
		get
		{
			return this.bodyCollider.bounds.center;
		}
	}

	// Token: 0x17000035 RID: 53
	// (get) Token: 0x0600032C RID: 812 RVA: 0x0001378C File Offset: 0x0001198C
	Bounds IEyeScannable.Bounds
	{
		get
		{
			return this.bodyCollider.bounds;
		}
	}

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x0600032D RID: 813 RVA: 0x00013799 File Offset: 0x00011999
	IList<KeyValueStringPair> IEyeScannable.Entries
	{
		get
		{
			return this.BuildEyeScannerData();
		}
	}

	// Token: 0x0600032E RID: 814 RVA: 0x000137A1 File Offset: 0x000119A1
	public void OnEnable()
	{
		EyeScannerMono.Register(this);
	}

	// Token: 0x0600032F RID: 815 RVA: 0x000137A9 File Offset: 0x000119A9
	public void OnDisable()
	{
		EyeScannerMono.Unregister(this);
	}

	// Token: 0x06000330 RID: 816 RVA: 0x000137B4 File Offset: 0x000119B4
	private IList<KeyValueStringPair> BuildEyeScannerData()
	{
		this.eyeScanData[0] = new KeyValueStringPair("Name", this._critterConfiguration.critterName);
		this.eyeScanData[1] = new KeyValueStringPair("Type", this._critterConfiguration.animalType.ToString());
		this.eyeScanData[2] = new KeyValueStringPair("Temperament", this._critterConfiguration.behaviour.temperament);
		this.eyeScanData[3] = new KeyValueStringPair("Habitat", this._critterConfiguration.biome.GetHabitatDescription());
		this.eyeScanData[4] = new KeyValueStringPair("Size", this.visuals.Appearance.size.ToString("0.00"));
		this.eyeScanData[5] = new KeyValueStringPair("State", this.GetCurrentStateName());
		return this.eyeScanData;
	}

	// Token: 0x14000006 RID: 6
	// (add) Token: 0x06000331 RID: 817 RVA: 0x000138B0 File Offset: 0x00011AB0
	// (remove) Token: 0x06000332 RID: 818 RVA: 0x000138E8 File Offset: 0x00011AE8
	public event Action OnDataChange;

	// Token: 0x06000333 RID: 819 RVA: 0x0001391D File Offset: 0x00011B1D
	private string GetCurrentStateName()
	{
		if (!this.isHeld)
		{
			return "Content";
		}
		return "Happy";
	}

	// Token: 0x06000335 RID: 821 RVA: 0x00013963 File Offset: 0x00011B63
	GameObject IHoldableObject.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x06000336 RID: 822 RVA: 0x0001396B File Offset: 0x00011B6B
	string IHoldableObject.get_name()
	{
		return base.name;
	}

	// Token: 0x06000337 RID: 823 RVA: 0x00013973 File Offset: 0x00011B73
	void IHoldableObject.set_name(string value)
	{
		base.name = value;
	}

	// Token: 0x040003B3 RID: 947
	public CritterVisuals visuals;

	// Token: 0x040003B4 RID: 948
	public Collider bodyCollider;

	// Token: 0x040003B5 RID: 949
	[Header("Feedback")]
	public CrittersAnim heldAnimation;

	// Token: 0x040003B6 RID: 950
	public AudioClip grabbedHaptics;

	// Token: 0x040003B7 RID: 951
	public float grabbedHapticsStrength = 1f;

	// Token: 0x040003B8 RID: 952
	public GameObject grabbedFX;

	// Token: 0x040003B9 RID: 953
	private CrittersAnim _currentAnim;

	// Token: 0x040003BA RID: 954
	private float _currentAnimTime;

	// Token: 0x040003BB RID: 955
	private Transform _animRoot;

	// Token: 0x040003BC RID: 956
	private Vector3 _bodyScale;

	// Token: 0x040003BD RID: 957
	public MenagerieCritter.MenagerieCritterState currentState = MenagerieCritter.MenagerieCritterState.Displaying;

	// Token: 0x040003BE RID: 958
	private CritterConfiguration _critterConfiguration;

	// Token: 0x040003BF RID: 959
	private Menagerie.CritterData _critterData;

	// Token: 0x040003C0 RID: 960
	private MenagerieSlot _slot;

	// Token: 0x040003C1 RID: 961
	private List<GorillaGrabber> activeGrabbers = new List<GorillaGrabber>();

	// Token: 0x040003C2 RID: 962
	private GameObject heldBy;

	// Token: 0x040003C3 RID: 963
	private bool isHeld;

	// Token: 0x040003C4 RID: 964
	private bool isHeldLeftHand;

	// Token: 0x040003C5 RID: 965
	public Action<MenagerieCritter> OnReleased;

	// Token: 0x040003C6 RID: 966
	private KeyValueStringPair[] eyeScanData = new KeyValueStringPair[6];

	// Token: 0x0200007D RID: 125
	public enum MenagerieCritterState
	{
		// Token: 0x040003C9 RID: 969
		Donating,
		// Token: 0x040003CA RID: 970
		Displaying
	}
}
