using System;
using System.Collections.Generic;
using System.Linq;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000425 RID: 1061
public class TransferrableItemSlotTransformOverride : MonoBehaviour, IGorillaSliceableSimple, ISpawnable
{
	// Token: 0x170002CF RID: 719
	// (get) Token: 0x060019E4 RID: 6628 RVA: 0x0007E4CF File Offset: 0x0007C6CF
	// (set) Token: 0x060019E5 RID: 6629 RVA: 0x0007E4D7 File Offset: 0x0007C6D7
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170002D0 RID: 720
	// (get) Token: 0x060019E6 RID: 6630 RVA: 0x0007E4E0 File Offset: 0x0007C6E0
	// (set) Token: 0x060019E7 RID: 6631 RVA: 0x0007E4E8 File Offset: 0x0007C6E8
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060019E8 RID: 6632 RVA: 0x0007E4F4 File Offset: 0x0007C6F4
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.defaultPosition = new SlotTransformOverride
		{
			positionState = TransferrableObject.PositionState.None
		};
		this.lastPosition = TransferrableObject.PositionState.None;
		foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
		{
			slotTransformOverride.Initialize(this, this.anchor);
		}
	}

	// Token: 0x060019E9 RID: 6633 RVA: 0x000023F4 File Offset: 0x000005F4
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060019EA RID: 6634 RVA: 0x0007E564 File Offset: 0x0007C764
	public void AddGripPosition(TransferrableObject.PositionState state, TransferrableObjectGripPosition togp)
	{
		foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
		{
			if (slotTransformOverride.positionState == state)
			{
				slotTransformOverride.AddSubGrabPoint(togp);
				return;
			}
		}
		SlotTransformOverride slotTransformOverride2 = new SlotTransformOverride
		{
			positionState = state
		};
		this.transformOverridesDeprecated.Add(slotTransformOverride2);
		slotTransformOverride2.AddSubGrabPoint(togp);
	}

	// Token: 0x060019EB RID: 6635 RVA: 0x00017251 File Offset: 0x00015451
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060019EC RID: 6636 RVA: 0x0001725A File Offset: 0x0001545A
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060019ED RID: 6637 RVA: 0x0007E5E4 File Offset: 0x0007C7E4
	public void SliceUpdate()
	{
		if (this.followingTransferrableObject == null)
		{
			return;
		}
		if (this.followingTransferrableObject.currentState != this.lastPosition)
		{
			SlotTransformOverride slotTransformOverride = this.transformOverridesDeprecated.Find((SlotTransformOverride x) => (x.positionState & this.followingTransferrableObject.currentState) > TransferrableObject.PositionState.None);
			if (slotTransformOverride != null && slotTransformOverride.positionState == TransferrableObject.PositionState.None)
			{
				slotTransformOverride = this.defaultPosition;
			}
		}
		this.lastPosition = this.followingTransferrableObject.currentState;
	}

	// Token: 0x060019EE RID: 6638 RVA: 0x0007E64E File Offset: 0x0007C84E
	private void Awake()
	{
		this.GenerateTransformFromPositionState();
	}

	// Token: 0x060019EF RID: 6639 RVA: 0x0007E658 File Offset: 0x0007C858
	public void GenerateTransformFromPositionState()
	{
		this.transformFromPosition = new Dictionary<TransferrableObject.PositionState, Transform>();
		if (this.transformOverridesDeprecated.Count > 0)
		{
			this.transformFromPosition[TransferrableObject.PositionState.None] = this.transformOverridesDeprecated[0].overrideTransform;
		}
		foreach (TransferrableObject.PositionState positionState in Enum.GetValues(typeof(TransferrableObject.PositionState)).Cast<TransferrableObject.PositionState>())
		{
			if (positionState == TransferrableObject.PositionState.None)
			{
				this.transformFromPosition[positionState] = null;
			}
			else
			{
				Transform transform = null;
				foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
				{
					if ((slotTransformOverride.positionState & positionState) != TransferrableObject.PositionState.None)
					{
						transform = slotTransformOverride.overrideTransform;
						break;
					}
				}
				this.transformFromPosition[positionState] = transform;
			}
		}
	}

	// Token: 0x060019F0 RID: 6640 RVA: 0x0007E754 File Offset: 0x0007C954
	[CanBeNull]
	public Transform GetTransformFromPositionState(TransferrableObject.PositionState currentState)
	{
		if (this.transformFromPosition == null)
		{
			this.GenerateTransformFromPositionState();
		}
		return this.transformFromPosition[currentState];
	}

	// Token: 0x060019F1 RID: 6641 RVA: 0x0007E770 File Offset: 0x0007C970
	public bool GetTransformFromPositionState(TransferrableObject.PositionState currentState, AdvancedItemState advancedItemState, Transform targetDockXf, out Matrix4x4 matrix4X4)
	{
		if (currentState != TransferrableObject.PositionState.None)
		{
			foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
			{
				if ((slotTransformOverride.positionState & currentState) != TransferrableObject.PositionState.None)
				{
					if (!slotTransformOverride.useAdvancedGrab)
					{
						matrix4X4 = slotTransformOverride.overrideTransformMatrix;
						return true;
					}
					if (advancedItemState.index >= slotTransformOverride.multiPoints.Count)
					{
						matrix4X4 = slotTransformOverride.overrideTransformMatrix;
						return true;
					}
					SubGrabPoint subGrabPoint = slotTransformOverride.multiPoints[advancedItemState.index];
					matrix4X4 = subGrabPoint.GetTransformFromPositionState(advancedItemState, slotTransformOverride, targetDockXf);
					return true;
				}
			}
			matrix4X4 = Matrix4x4.identity;
			return false;
		}
		if (this.transformOverridesDeprecated.Count > 0)
		{
			matrix4X4 = this.transformOverridesDeprecated[0].overrideTransformMatrix;
			return true;
		}
		matrix4X4 = Matrix4x4.identity;
		return false;
	}

	// Token: 0x060019F2 RID: 6642 RVA: 0x0007E874 File Offset: 0x0007CA74
	public AdvancedItemState GetAdvancedItemStateFromHand(TransferrableObject.PositionState currentState, Transform handTransform, Transform targetDock)
	{
		foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
		{
			if ((slotTransformOverride.positionState & currentState) != TransferrableObject.PositionState.None && slotTransformOverride.multiPoints.Count != 0)
			{
				SubGrabPoint subGrabPoint = slotTransformOverride.multiPoints[0];
				float num = float.PositiveInfinity;
				int num2 = -1;
				for (int i = 0; i < slotTransformOverride.multiPoints.Count; i++)
				{
					SubGrabPoint subGrabPoint2 = slotTransformOverride.multiPoints[i];
					if (!(subGrabPoint2.gripPoint == null))
					{
						float num3 = subGrabPoint2.EvaluateScore(base.transform, handTransform, targetDock);
						if (num3 < num)
						{
							subGrabPoint = subGrabPoint2;
							num = num3;
							num2 = i;
						}
					}
				}
				AdvancedItemState advancedItemStateFromHand = subGrabPoint.GetAdvancedItemStateFromHand(base.transform, handTransform, targetDock, slotTransformOverride);
				advancedItemStateFromHand.index = num2;
				return advancedItemStateFromHand;
			}
		}
		return new AdvancedItemState();
	}

	// Token: 0x060019F3 RID: 6643 RVA: 0x0007E974 File Offset: 0x0007CB74
	public void Edit()
	{
		if (TransferrableItemSlotTransformOverride.OnBringUpWindow != null)
		{
			TransferrableItemSlotTransformOverride.OnBringUpWindow(base.GetComponent<TransferrableObject>());
		}
	}

	// Token: 0x060019F5 RID: 6645 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04001CF7 RID: 7415
	[FormerlySerializedAs("transformOverridesList")]
	public List<SlotTransformOverride> transformOverridesDeprecated;

	// Token: 0x04001CF8 RID: 7416
	[SerializeReference]
	public List<SlotTransformOverride> transformOverrides;

	// Token: 0x04001CF9 RID: 7417
	private TransferrableObject.PositionState lastPosition;

	// Token: 0x04001CFA RID: 7418
	[Tooltip("(2024-08-20 MattO) For cosmetics this is almost always assigned to the TransferrableObject component in the same prefab and almost always belonging to the same gameobject as this Component.")]
	public TransferrableObject followingTransferrableObject;

	// Token: 0x04001CFB RID: 7419
	[Tooltip("(2024-08-20 MattO) This is filled in automatically by the cosmetic spawner.")]
	public SlotTransformOverride defaultPosition;

	// Token: 0x04001CFC RID: 7420
	[Obsolete("(2024-08-2024) This used to be assigned to `defaultPosition.overrideTransform` before, but was there ever an instance where it wasn't null? Keeping it serialized just in case there is a reason for it.")]
	public Transform defaultTransform;

	// Token: 0x04001CFD RID: 7421
	public Transform anchor;

	// Token: 0x04001D00 RID: 7424
	public Dictionary<TransferrableObject.PositionState, Transform> transformFromPosition;

	// Token: 0x04001D01 RID: 7425
	public static Action<TransferrableObject> OnBringUpWindow;
}
