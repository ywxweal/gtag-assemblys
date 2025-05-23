using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020000E9 RID: 233
public class SnowballMaker : MonoBehaviour
{
	// Token: 0x17000074 RID: 116
	// (get) Token: 0x060005D8 RID: 1496 RVA: 0x00021D57 File Offset: 0x0001FF57
	// (set) Token: 0x060005D9 RID: 1497 RVA: 0x00021D5E File Offset: 0x0001FF5E
	public static SnowballMaker leftHandInstance { get; private set; }

	// Token: 0x17000075 RID: 117
	// (get) Token: 0x060005DA RID: 1498 RVA: 0x00021D66 File Offset: 0x0001FF66
	// (set) Token: 0x060005DB RID: 1499 RVA: 0x00021D6D File Offset: 0x0001FF6D
	public static SnowballMaker rightHandInstance { get; private set; }

	// Token: 0x17000076 RID: 118
	// (get) Token: 0x060005DC RID: 1500 RVA: 0x00021D75 File Offset: 0x0001FF75
	// (set) Token: 0x060005DD RID: 1501 RVA: 0x00021D7D File Offset: 0x0001FF7D
	public SnowballThrowable[] snowballs { get; private set; }

	// Token: 0x060005DE RID: 1502 RVA: 0x00021D88 File Offset: 0x0001FF88
	private void Awake()
	{
		if (this.isLeftHand)
		{
			if (SnowballMaker.leftHandInstance == null)
			{
				SnowballMaker.leftHandInstance = this;
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
		else if (SnowballMaker.rightHandInstance == null)
		{
			SnowballMaker.rightHandInstance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		if (this.handTransform == null)
		{
			this.handTransform = base.transform;
		}
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x00021DF9 File Offset: 0x0001FFF9
	internal void SetupThrowables(SnowballThrowable[] newThrowables)
	{
		this.snowballs = newThrowables;
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00021E04 File Offset: 0x00020004
	protected void LateUpdate()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (!CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			return;
		}
		if (this.snowballs == null)
		{
			return;
		}
		if (BuilderPieceInteractor.instance != null && BuilderPieceInteractor.instance.BlockSnowballCreation())
		{
			return;
		}
		if (!GTPlayer.hasInstance || !EquipmentInteractor.hasInstance || !GorillaTagger.hasInstance || !GorillaTagger.Instance.offlineVRRig || this.snowballs.Length == 0)
		{
			return;
		}
		GTPlayer instance = GTPlayer.Instance;
		int num = (this.isLeftHand ? instance.leftHandMaterialTouchIndex : instance.rightHandMaterialTouchIndex);
		if (num == 0)
		{
			if (Time.time > this.lastGroundContactTime + this.snowballCreationCooldownTime)
			{
				this.requiresFreshMaterialContact = false;
			}
			return;
		}
		this.lastGroundContactTime = Time.time;
		EquipmentInteractor instance2 = EquipmentInteractor.instance;
		bool flag = (this.isLeftHand ? instance2.leftHandHeldEquipment : instance2.rightHandHeldEquipment) != null;
		bool flag2 = (this.isLeftHand ? instance2.isLeftGrabbing : instance2.isRightGrabbing);
		bool flag3 = false;
		if (flag2 && !this.requiresFreshMaterialContact)
		{
			int num2 = -1;
			for (int i = 0; i < this.snowballs.Length; i++)
			{
				if (this.snowballs[i].gameObject.activeSelf)
				{
					num2 = i;
					break;
				}
			}
			SnowballThrowable snowballThrowable = ((num2 > -1) ? this.snowballs[num2] : null);
			GrowingSnowballThrowable growingSnowballThrowable = snowballThrowable as GrowingSnowballThrowable;
			bool flag4 = (this.isLeftHand ? (!ConnectedControllerHandler.Instance.RightValid) : (!ConnectedControllerHandler.Instance.LeftValid));
			if (growingSnowballThrowable != null && (!GrowingSnowballThrowable.twoHandedSnowballGrowing || flag4 || flag3))
			{
				if (snowballThrowable.matDataIndexes.Contains(num))
				{
					growingSnowballThrowable.IncreaseSize(1);
					GorillaTagger.Instance.StartVibration(this.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
					this.requiresFreshMaterialContact = true;
					return;
				}
			}
			else if (!flag)
			{
				foreach (SnowballThrowable snowballThrowable2 in this.snowballs)
				{
					Transform transform = snowballThrowable2.transform;
					if (snowballThrowable2.matDataIndexes.Contains(num))
					{
						Transform transform2 = this.handTransform;
						snowballThrowable2.SetSnowballActiveLocal(true);
						snowballThrowable2.velocityEstimator = this.velocityEstimator;
						transform.position = transform2.position;
						transform.rotation = transform2.rotation;
						GorillaTagger.Instance.StartVibration(this.isLeftHand, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
						this.requiresFreshMaterialContact = true;
						return;
					}
				}
			}
		}
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x000220A4 File Offset: 0x000202A4
	public bool TryCreateSnowball(int materialIndex, out SnowballThrowable result)
	{
		foreach (SnowballThrowable snowballThrowable in this.snowballs)
		{
			if (snowballThrowable.matDataIndexes.Contains(materialIndex))
			{
				Transform transform = snowballThrowable.transform;
				Transform transform2 = this.handTransform;
				snowballThrowable.SetSnowballActiveLocal(true);
				snowballThrowable.velocityEstimator = this.velocityEstimator;
				transform.position = transform2.position;
				transform.rotation = transform2.rotation;
				GorillaTagger.Instance.StartVibration(this.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				result = snowballThrowable;
				return true;
			}
		}
		result = null;
		return false;
	}

	// Token: 0x040006DC RID: 1756
	public bool isLeftHand;

	// Token: 0x040006DE RID: 1758
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x040006DF RID: 1759
	private float snowballCreationCooldownTime = 0.1f;

	// Token: 0x040006E0 RID: 1760
	private float lastGroundContactTime;

	// Token: 0x040006E1 RID: 1761
	private bool requiresFreshMaterialContact;

	// Token: 0x040006E2 RID: 1762
	[SerializeField]
	private Transform handTransform;
}
