﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BCD RID: 3021
	public class ButtonController : Interactable
	{
		// Token: 0x17000734 RID: 1844
		// (get) Token: 0x06004A97 RID: 19095 RVA: 0x001634A2 File Offset: 0x001616A2
		public override int ValidToolTagsMask
		{
			get
			{
				return this._toolTagsMask;
			}
		}

		// Token: 0x17000735 RID: 1845
		// (get) Token: 0x06004A98 RID: 19096 RVA: 0x001634AA File Offset: 0x001616AA
		public Vector3 LocalButtonDirection
		{
			get
			{
				return this._localButtonDirection;
			}
		}

		// Token: 0x17000736 RID: 1846
		// (get) Token: 0x06004A99 RID: 19097 RVA: 0x001634B2 File Offset: 0x001616B2
		// (set) Token: 0x06004A9A RID: 19098 RVA: 0x001634BA File Offset: 0x001616BA
		public InteractableState CurrentButtonState { get; private set; }

		// Token: 0x06004A9B RID: 19099 RVA: 0x001634C4 File Offset: 0x001616C4
		protected override void Awake()
		{
			base.Awake();
			foreach (InteractableToolTags interactableToolTags in this._allValidToolsTags)
			{
				this._toolTagsMask |= (int)interactableToolTags;
			}
			this._proximityZoneCollider = this._proximityZone.GetComponent<ColliderZone>();
			this._contactZoneCollider = this._contactZone.GetComponent<ColliderZone>();
			this._actionZoneCollider = this._actionZone.GetComponent<ColliderZone>();
		}

		// Token: 0x06004A9C RID: 19100 RVA: 0x00163534 File Offset: 0x00161734
		private void FireInteractionEventsOnDepth(InteractableCollisionDepth oldDepth, InteractableTool collidingTool, InteractionType interactionType)
		{
			switch (oldDepth)
			{
			case InteractableCollisionDepth.Proximity:
				this.OnProximityZoneEvent(new ColliderZoneArgs(base.ProximityCollider, (float)Time.frameCount, collidingTool, interactionType));
				return;
			case InteractableCollisionDepth.Contact:
				this.OnContactZoneEvent(new ColliderZoneArgs(base.ContactCollider, (float)Time.frameCount, collidingTool, interactionType));
				return;
			case InteractableCollisionDepth.Action:
				this.OnActionZoneEvent(new ColliderZoneArgs(base.ActionCollider, (float)Time.frameCount, collidingTool, interactionType));
				return;
			default:
				return;
			}
		}

		// Token: 0x06004A9D RID: 19101 RVA: 0x001635A4 File Offset: 0x001617A4
		public override void UpdateCollisionDepth(InteractableTool interactableTool, InteractableCollisionDepth oldCollisionDepth, InteractableCollisionDepth newCollisionDepth)
		{
			bool isFarFieldTool = interactableTool.IsFarFieldTool;
			if (!isFarFieldTool && !this._allowMultipleNearFieldInteraction && this._toolToState.Keys.Count > 0 && !this._toolToState.ContainsKey(interactableTool))
			{
				return;
			}
			InteractableState currentButtonState = this.CurrentButtonState;
			Vector3 vector = base.transform.TransformDirection(this._localButtonDirection);
			bool flag = this.IsValidContact(interactableTool, vector) || interactableTool.IsFarFieldTool;
			bool flag2 = newCollisionDepth >= InteractableCollisionDepth.Proximity;
			bool flag3 = newCollisionDepth == InteractableCollisionDepth.Contact;
			bool flag4 = newCollisionDepth == InteractableCollisionDepth.Action;
			bool flag5 = oldCollisionDepth != newCollisionDepth;
			if (flag5)
			{
				this.FireInteractionEventsOnDepth(oldCollisionDepth, interactableTool, InteractionType.Exit);
				this.FireInteractionEventsOnDepth(newCollisionDepth, interactableTool, InteractionType.Enter);
			}
			else
			{
				this.FireInteractionEventsOnDepth(newCollisionDepth, interactableTool, InteractionType.Stay);
			}
			InteractableState interactableState = currentButtonState;
			if (interactableTool.IsFarFieldTool)
			{
				interactableState = (flag3 ? InteractableState.ContactState : (flag4 ? InteractableState.ActionState : InteractableState.Default));
			}
			else
			{
				Plane plane = new Plane(-vector, this._buttonPlaneCenter.position);
				bool flag6 = !this._makeSureToolIsOnPositiveSide || plane.GetSide(interactableTool.InteractionPosition);
				interactableState = this.GetUpcomingStateNearField(currentButtonState, newCollisionDepth, flag4, flag3, flag2, flag, flag6);
			}
			if (interactableState != InteractableState.Default)
			{
				this._toolToState[interactableTool] = interactableState;
			}
			else
			{
				this._toolToState.Remove(interactableTool);
			}
			if (isFarFieldTool || this._allowMultipleNearFieldInteraction)
			{
				foreach (InteractableState interactableState2 in this._toolToState.Values)
				{
					if (interactableState < interactableState2)
					{
						interactableState = interactableState2;
					}
				}
			}
			if (currentButtonState != interactableState)
			{
				this.CurrentButtonState = interactableState;
				InteractionType interactionType = ((!flag5) ? InteractionType.Stay : ((newCollisionDepth == InteractableCollisionDepth.None) ? InteractionType.Exit : InteractionType.Enter));
				ColliderZone colliderZone;
				switch (this.CurrentButtonState)
				{
				case InteractableState.ProximityState:
					colliderZone = base.ProximityCollider;
					break;
				case InteractableState.ContactState:
					colliderZone = base.ContactCollider;
					break;
				case InteractableState.ActionState:
					colliderZone = base.ActionCollider;
					break;
				default:
					colliderZone = null;
					break;
				}
				Interactable.InteractableStateArgsEvent interactableStateChanged = this.InteractableStateChanged;
				if (interactableStateChanged == null)
				{
					return;
				}
				interactableStateChanged.Invoke(new InteractableStateArgs(this, interactableTool, this.CurrentButtonState, currentButtonState, new ColliderZoneArgs(colliderZone, (float)Time.frameCount, interactableTool, interactionType)));
			}
		}

		// Token: 0x06004A9E RID: 19102 RVA: 0x001637CC File Offset: 0x001619CC
		private InteractableState GetUpcomingStateNearField(InteractableState oldState, InteractableCollisionDepth newCollisionDepth, bool toolIsInActionZone, bool toolIsInContactZone, bool toolIsInProximity, bool validContact, bool onPositiveSideOfInteractable)
		{
			InteractableState interactableState = oldState;
			switch (oldState)
			{
			case InteractableState.Default:
				if (validContact && onPositiveSideOfInteractable && newCollisionDepth > InteractableCollisionDepth.Proximity)
				{
					interactableState = ((newCollisionDepth == InteractableCollisionDepth.Action) ? InteractableState.ActionState : InteractableState.ContactState);
				}
				else if (toolIsInProximity)
				{
					interactableState = InteractableState.ProximityState;
				}
				break;
			case InteractableState.ProximityState:
				if (newCollisionDepth < InteractableCollisionDepth.Proximity)
				{
					interactableState = InteractableState.Default;
				}
				else if (validContact && onPositiveSideOfInteractable && newCollisionDepth > InteractableCollisionDepth.Proximity)
				{
					interactableState = ((newCollisionDepth == InteractableCollisionDepth.Action) ? InteractableState.ActionState : InteractableState.ContactState);
				}
				break;
			case InteractableState.ContactState:
				if (newCollisionDepth < InteractableCollisionDepth.Contact)
				{
					interactableState = (toolIsInProximity ? InteractableState.ProximityState : InteractableState.Default);
				}
				else if (toolIsInActionZone && validContact && onPositiveSideOfInteractable)
				{
					interactableState = InteractableState.ActionState;
				}
				break;
			case InteractableState.ActionState:
				if (!toolIsInActionZone)
				{
					if (toolIsInContactZone)
					{
						interactableState = InteractableState.ContactState;
					}
					else if (toolIsInProximity)
					{
						interactableState = InteractableState.ProximityState;
					}
					else
					{
						interactableState = InteractableState.Default;
					}
				}
				break;
			}
			return interactableState;
		}

		// Token: 0x06004A9F RID: 19103 RVA: 0x00163864 File Offset: 0x00161A64
		public void ForceResetButton()
		{
			InteractableState currentButtonState = this.CurrentButtonState;
			this.CurrentButtonState = InteractableState.Default;
			Interactable.InteractableStateArgsEvent interactableStateChanged = this.InteractableStateChanged;
			if (interactableStateChanged == null)
			{
				return;
			}
			interactableStateChanged.Invoke(new InteractableStateArgs(this, null, this.CurrentButtonState, currentButtonState, new ColliderZoneArgs(base.ContactCollider, (float)Time.frameCount, null, InteractionType.Exit)));
		}

		// Token: 0x06004AA0 RID: 19104 RVA: 0x001638B0 File Offset: 0x00161AB0
		private bool IsValidContact(InteractableTool collidingTool, Vector3 buttonDirection)
		{
			if (this._contactTests == null || collidingTool.IsFarFieldTool)
			{
				return true;
			}
			ButtonController.ContactTest[] contactTests = this._contactTests;
			for (int i = 0; i < contactTests.Length; i++)
			{
				if (contactTests[i] == ButtonController.ContactTest.BackwardsPress)
				{
					if (!this.PassEntryTest(collidingTool, buttonDirection))
					{
						return false;
					}
				}
				else if (!this.PassPerpTest(collidingTool, buttonDirection))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004AA1 RID: 19105 RVA: 0x00163904 File Offset: 0x00161B04
		private bool PassEntryTest(InteractableTool collidingTool, Vector3 buttonDirection)
		{
			return Vector3.Dot(collidingTool.Velocity.normalized, buttonDirection) >= 0.8f;
		}

		// Token: 0x06004AA2 RID: 19106 RVA: 0x00163930 File Offset: 0x00161B30
		private bool PassPerpTest(InteractableTool collidingTool, Vector3 buttonDirection)
		{
			Vector3 vector = collidingTool.ToolTransform.right;
			if (collidingTool.IsRightHandedTool)
			{
				vector = -vector;
			}
			return Vector3.Dot(vector, buttonDirection) >= 0.5f;
		}

		// Token: 0x04004D51 RID: 19793
		private const float ENTRY_DOT_THRESHOLD = 0.8f;

		// Token: 0x04004D52 RID: 19794
		private const float PERP_DOT_THRESHOLD = 0.5f;

		// Token: 0x04004D53 RID: 19795
		[SerializeField]
		private GameObject _proximityZone;

		// Token: 0x04004D54 RID: 19796
		[SerializeField]
		private GameObject _contactZone;

		// Token: 0x04004D55 RID: 19797
		[SerializeField]
		private GameObject _actionZone;

		// Token: 0x04004D56 RID: 19798
		[SerializeField]
		private ButtonController.ContactTest[] _contactTests;

		// Token: 0x04004D57 RID: 19799
		[SerializeField]
		private Transform _buttonPlaneCenter;

		// Token: 0x04004D58 RID: 19800
		[SerializeField]
		private bool _makeSureToolIsOnPositiveSide = true;

		// Token: 0x04004D59 RID: 19801
		[SerializeField]
		private Vector3 _localButtonDirection = Vector3.down;

		// Token: 0x04004D5A RID: 19802
		[SerializeField]
		private InteractableToolTags[] _allValidToolsTags = new InteractableToolTags[] { InteractableToolTags.All };

		// Token: 0x04004D5B RID: 19803
		private int _toolTagsMask;

		// Token: 0x04004D5C RID: 19804
		[SerializeField]
		private bool _allowMultipleNearFieldInteraction;

		// Token: 0x04004D5E RID: 19806
		private Dictionary<InteractableTool, InteractableState> _toolToState = new Dictionary<InteractableTool, InteractableState>();

		// Token: 0x02000BCE RID: 3022
		public enum ContactTest
		{
			// Token: 0x04004D60 RID: 19808
			PerpenTest,
			// Token: 0x04004D61 RID: 19809
			BackwardsPress
		}
	}
}
