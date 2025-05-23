using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BDE RID: 3038
	public class InteractableToolsInputRouter : MonoBehaviour
	{
		// Token: 0x17000751 RID: 1873
		// (get) Token: 0x06004AF6 RID: 19190 RVA: 0x00164508 File Offset: 0x00162708
		public static InteractableToolsInputRouter Instance
		{
			get
			{
				if (InteractableToolsInputRouter._instance == null)
				{
					InteractableToolsInputRouter[] array = Object.FindObjectsOfType<InteractableToolsInputRouter>();
					if (array.Length != 0)
					{
						InteractableToolsInputRouter._instance = array[0];
						for (int i = 1; i < array.Length; i++)
						{
							Object.Destroy(array[i].gameObject);
						}
					}
				}
				return InteractableToolsInputRouter._instance;
			}
		}

		// Token: 0x06004AF7 RID: 19191 RVA: 0x00164554 File Offset: 0x00162754
		public void RegisterInteractableTool(InteractableTool interactableTool)
		{
			if (interactableTool.IsRightHandedTool)
			{
				if (interactableTool.IsFarFieldTool)
				{
					this._rightHandFarTools.Add(interactableTool);
					return;
				}
				this._rightHandNearTools.Add(interactableTool);
				return;
			}
			else
			{
				if (interactableTool.IsFarFieldTool)
				{
					this._leftHandFarTools.Add(interactableTool);
					return;
				}
				this._leftHandNearTools.Add(interactableTool);
				return;
			}
		}

		// Token: 0x06004AF8 RID: 19192 RVA: 0x001645B0 File Offset: 0x001627B0
		public void UnregisterInteractableTool(InteractableTool interactableTool)
		{
			if (interactableTool.IsRightHandedTool)
			{
				if (interactableTool.IsFarFieldTool)
				{
					this._rightHandFarTools.Remove(interactableTool);
					return;
				}
				this._rightHandNearTools.Remove(interactableTool);
				return;
			}
			else
			{
				if (interactableTool.IsFarFieldTool)
				{
					this._leftHandFarTools.Remove(interactableTool);
					return;
				}
				this._leftHandNearTools.Remove(interactableTool);
				return;
			}
		}

		// Token: 0x06004AF9 RID: 19193 RVA: 0x0016460C File Offset: 0x0016280C
		private void Update()
		{
			if (!HandsManager.Instance.IsInitialized())
			{
				return;
			}
			bool flag = HandsManager.Instance.LeftHand.IsTracked && HandsManager.Instance.LeftHand.HandConfidence == OVRHand.TrackingConfidence.High;
			bool flag2 = HandsManager.Instance.RightHand.IsTracked && HandsManager.Instance.RightHand.HandConfidence == OVRHand.TrackingConfidence.High;
			bool isPointerPoseValid = HandsManager.Instance.LeftHand.IsPointerPoseValid;
			bool isPointerPoseValid2 = HandsManager.Instance.RightHand.IsPointerPoseValid;
			bool flag3 = this.UpdateToolsAndEnableState(this._leftHandNearTools, flag);
			this.UpdateToolsAndEnableState(this._leftHandFarTools, !flag3 && flag && isPointerPoseValid);
			bool flag4 = this.UpdateToolsAndEnableState(this._rightHandNearTools, flag2);
			this.UpdateToolsAndEnableState(this._rightHandFarTools, !flag4 && flag2 && isPointerPoseValid2);
		}

		// Token: 0x06004AFA RID: 19194 RVA: 0x001646E6 File Offset: 0x001628E6
		private bool UpdateToolsAndEnableState(HashSet<InteractableTool> tools, bool toolsAreEnabledThisFrame)
		{
			bool flag = this.UpdateTools(tools, !toolsAreEnabledThisFrame);
			this.ToggleToolsEnableState(tools, toolsAreEnabledThisFrame);
			return flag;
		}

		// Token: 0x06004AFB RID: 19195 RVA: 0x001646FC File Offset: 0x001628FC
		private bool UpdateTools(HashSet<InteractableTool> tools, bool resetCollisionData = false)
		{
			bool flag = false;
			foreach (InteractableTool interactableTool in tools)
			{
				List<InteractableCollisionInfo> nextIntersectingObjects = interactableTool.GetNextIntersectingObjects();
				if (nextIntersectingObjects.Count > 0 && !resetCollisionData)
				{
					if (!flag)
					{
						flag = nextIntersectingObjects.Count > 0;
					}
					interactableTool.UpdateCurrentCollisionsBasedOnDepth();
					if (interactableTool.IsFarFieldTool)
					{
						KeyValuePair<Interactable, InteractableCollisionInfo> firstCurrentCollisionInfo = interactableTool.GetFirstCurrentCollisionInfo();
						if (interactableTool.ToolInputState == ToolInputState.PrimaryInputUp)
						{
							firstCurrentCollisionInfo.Value.InteractableCollider = firstCurrentCollisionInfo.Key.ActionCollider;
							firstCurrentCollisionInfo.Value.CollisionDepth = InteractableCollisionDepth.Action;
						}
						else
						{
							firstCurrentCollisionInfo.Value.InteractableCollider = firstCurrentCollisionInfo.Key.ContactCollider;
							firstCurrentCollisionInfo.Value.CollisionDepth = InteractableCollisionDepth.Contact;
						}
						interactableTool.FocusOnInteractable(firstCurrentCollisionInfo.Key, firstCurrentCollisionInfo.Value.InteractableCollider);
					}
				}
				else
				{
					interactableTool.DeFocus();
					interactableTool.ClearAllCurrentCollisionInfos();
				}
				interactableTool.UpdateLatestCollisionData();
			}
			return flag;
		}

		// Token: 0x06004AFC RID: 19196 RVA: 0x00164810 File Offset: 0x00162A10
		private void ToggleToolsEnableState(HashSet<InteractableTool> tools, bool enableState)
		{
			foreach (InteractableTool interactableTool in tools)
			{
				if (interactableTool.EnableState != enableState)
				{
					interactableTool.EnableState = enableState;
				}
			}
		}

		// Token: 0x04004DA6 RID: 19878
		private static InteractableToolsInputRouter _instance;

		// Token: 0x04004DA7 RID: 19879
		private bool _leftPinch;

		// Token: 0x04004DA8 RID: 19880
		private bool _rightPinch;

		// Token: 0x04004DA9 RID: 19881
		private HashSet<InteractableTool> _leftHandNearTools = new HashSet<InteractableTool>();

		// Token: 0x04004DAA RID: 19882
		private HashSet<InteractableTool> _leftHandFarTools = new HashSet<InteractableTool>();

		// Token: 0x04004DAB RID: 19883
		private HashSet<InteractableTool> _rightHandNearTools = new HashSet<InteractableTool>();

		// Token: 0x04004DAC RID: 19884
		private HashSet<InteractableTool> _rightHandFarTools = new HashSet<InteractableTool>();
	}
}
