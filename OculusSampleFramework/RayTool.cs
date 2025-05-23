using System;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BE9 RID: 3049
	public class RayTool : InteractableTool
	{
		// Token: 0x1700076A RID: 1898
		// (get) Token: 0x06004B40 RID: 19264 RVA: 0x00047642 File Offset: 0x00045842
		public override InteractableToolTags ToolTags
		{
			get
			{
				return InteractableToolTags.Ray;
			}
		}

		// Token: 0x1700076B RID: 1899
		// (get) Token: 0x06004B41 RID: 19265 RVA: 0x00165375 File Offset: 0x00163575
		public override ToolInputState ToolInputState
		{
			get
			{
				if (this._pinchStateModule.PinchDownOnFocusedObject)
				{
					return ToolInputState.PrimaryInputDown;
				}
				if (this._pinchStateModule.PinchSteadyOnFocusedObject)
				{
					return ToolInputState.PrimaryInputDownStay;
				}
				if (this._pinchStateModule.PinchUpAndDownOnFocusedObject)
				{
					return ToolInputState.PrimaryInputUp;
				}
				return ToolInputState.Inactive;
			}
		}

		// Token: 0x1700076C RID: 1900
		// (get) Token: 0x06004B42 RID: 19266 RVA: 0x00047642 File Offset: 0x00045842
		public override bool IsFarFieldTool
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700076D RID: 1901
		// (get) Token: 0x06004B43 RID: 19267 RVA: 0x001653A5 File Offset: 0x001635A5
		// (set) Token: 0x06004B44 RID: 19268 RVA: 0x001653B2 File Offset: 0x001635B2
		public override bool EnableState
		{
			get
			{
				return this._rayToolView.EnableState;
			}
			set
			{
				this._rayToolView.EnableState = value;
			}
		}

		// Token: 0x06004B45 RID: 19269 RVA: 0x001653C0 File Offset: 0x001635C0
		public override void Initialize()
		{
			InteractableToolsInputRouter.Instance.RegisterInteractableTool(this);
			this._rayToolView.InteractableTool = this;
			this._coneAngleReleaseDegrees = this._coneAngleDegrees * 1.2f;
			this._initialized = true;
		}

		// Token: 0x06004B46 RID: 19270 RVA: 0x001653F2 File Offset: 0x001635F2
		private void OnDestroy()
		{
			if (InteractableToolsInputRouter.Instance != null)
			{
				InteractableToolsInputRouter.Instance.UnregisterInteractableTool(this);
			}
		}

		// Token: 0x06004B47 RID: 19271 RVA: 0x0016540C File Offset: 0x0016360C
		private void Update()
		{
			if (!HandsManager.Instance || !HandsManager.Instance.IsInitialized() || !this._initialized)
			{
				return;
			}
			OVRHand ovrhand = (base.IsRightHandedTool ? HandsManager.Instance.RightHand : HandsManager.Instance.LeftHand);
			Transform pointerPose = ovrhand.PointerPose;
			base.transform.position = pointerPose.position;
			base.transform.rotation = pointerPose.rotation;
			Vector3 interactionPosition = base.InteractionPosition;
			Vector3 position = base.transform.position;
			base.Velocity = (position - interactionPosition) / Time.deltaTime;
			base.InteractionPosition = position;
			this._pinchStateModule.UpdateState(ovrhand, this._focusedInteractable);
			this._rayToolView.ToolActivateState = this._pinchStateModule.PinchSteadyOnFocusedObject || this._pinchStateModule.PinchDownOnFocusedObject;
		}

		// Token: 0x06004B48 RID: 19272 RVA: 0x001654EB File Offset: 0x001636EB
		private Vector3 GetRayCastOrigin()
		{
			return base.transform.position + 0.8f * base.transform.forward;
		}

		// Token: 0x06004B49 RID: 19273 RVA: 0x00165514 File Offset: 0x00163714
		public override List<InteractableCollisionInfo> GetNextIntersectingObjects()
		{
			if (!this._initialized)
			{
				return this._currentIntersectingObjects;
			}
			if (this._currInteractableCastedAgainst != null && this.HasRayReleasedInteractable(this._currInteractableCastedAgainst))
			{
				this._currInteractableCastedAgainst = null;
			}
			if (this._currInteractableCastedAgainst == null)
			{
				this._currentIntersectingObjects.Clear();
				this._currInteractableCastedAgainst = this.FindTargetInteractable();
				if (this._currInteractableCastedAgainst != null)
				{
					int num = Physics.OverlapSphereNonAlloc(this._currInteractableCastedAgainst.transform.position, 0.01f, this._collidersOverlapped);
					for (int i = 0; i < num; i++)
					{
						ColliderZone component = this._collidersOverlapped[i].GetComponent<ColliderZone>();
						if (component != null)
						{
							Interactable parentInteractable = component.ParentInteractable;
							if (!(parentInteractable == null) && !(parentInteractable != this._currInteractableCastedAgainst))
							{
								InteractableCollisionInfo interactableCollisionInfo = new InteractableCollisionInfo(component, component.CollisionDepth, this);
								this._currentIntersectingObjects.Add(interactableCollisionInfo);
							}
						}
					}
					if (this._currentIntersectingObjects.Count == 0)
					{
						this._currInteractableCastedAgainst = null;
					}
				}
			}
			return this._currentIntersectingObjects;
		}

		// Token: 0x06004B4A RID: 19274 RVA: 0x00165620 File Offset: 0x00163820
		private bool HasRayReleasedInteractable(Interactable focusedInteractable)
		{
			Vector3 position = base.transform.position;
			Vector3 forward = base.transform.forward;
			float num = Mathf.Cos(this._coneAngleReleaseDegrees * 0.017453292f);
			Vector3 vector = focusedInteractable.transform.position - position;
			vector.Normalize();
			return Vector3.Dot(vector, forward) < num;
		}

		// Token: 0x06004B4B RID: 19275 RVA: 0x0016567C File Offset: 0x0016387C
		private Interactable FindTargetInteractable()
		{
			Vector3 rayCastOrigin = this.GetRayCastOrigin();
			Vector3 forward = base.transform.forward;
			Interactable interactable = this.FindPrimaryRaycastHit(rayCastOrigin, forward);
			if (interactable == null)
			{
				interactable = this.FindInteractableViaConeTest(rayCastOrigin, forward);
			}
			return interactable;
		}

		// Token: 0x06004B4C RID: 19276 RVA: 0x001656BC File Offset: 0x001638BC
		private Interactable FindPrimaryRaycastHit(Vector3 rayOrigin, Vector3 rayDirection)
		{
			Interactable interactable = null;
			int num = Physics.RaycastNonAlloc(new Ray(rayOrigin, rayDirection), this._primaryHits, float.PositiveInfinity);
			float num2 = 0f;
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this._primaryHits[i];
				ColliderZone component = raycastHit.transform.GetComponent<ColliderZone>();
				if (component != null)
				{
					Interactable parentInteractable = component.ParentInteractable;
					if (!(parentInteractable == null) && (parentInteractable.ValidToolTagsMask & (int)this.ToolTags) != 0)
					{
						float magnitude = (parentInteractable.transform.position - rayOrigin).magnitude;
						if (interactable == null || magnitude < num2)
						{
							interactable = parentInteractable;
							num2 = magnitude;
						}
					}
				}
			}
			return interactable;
		}

		// Token: 0x06004B4D RID: 19277 RVA: 0x0016576C File Offset: 0x0016396C
		private Interactable FindInteractableViaConeTest(Vector3 rayOrigin, Vector3 rayDirection)
		{
			Interactable interactable = null;
			float num = 0f;
			float num2 = Mathf.Cos(this._coneAngleDegrees * 0.017453292f);
			float num3 = Mathf.Tan(0.017453292f * this._coneAngleDegrees * 0.5f) * this._farFieldMaxDistance;
			int num4 = Physics.OverlapBoxNonAlloc(rayOrigin + rayDirection * this._farFieldMaxDistance * 0.5f, new Vector3(num3, num3, this._farFieldMaxDistance * 0.5f), this._secondaryOverlapResults, base.transform.rotation);
			for (int i = 0; i < num4; i++)
			{
				ColliderZone component = this._secondaryOverlapResults[i].GetComponent<ColliderZone>();
				if (component != null)
				{
					Interactable parentInteractable = component.ParentInteractable;
					if (!(parentInteractable == null) && (parentInteractable.ValidToolTagsMask & (int)this.ToolTags) != 0)
					{
						Vector3 vector = parentInteractable.transform.position - rayOrigin;
						float magnitude = vector.magnitude;
						vector /= magnitude;
						if (Vector3.Dot(vector, rayDirection) >= num2 && (interactable == null || magnitude < num))
						{
							interactable = parentInteractable;
							num = magnitude;
						}
					}
				}
			}
			return interactable;
		}

		// Token: 0x06004B4E RID: 19278 RVA: 0x0016588F File Offset: 0x00163A8F
		public override void FocusOnInteractable(Interactable focusedInteractable, ColliderZone colliderZone)
		{
			this._rayToolView.SetFocusedInteractable(focusedInteractable);
			this._focusedInteractable = focusedInteractable;
		}

		// Token: 0x06004B4F RID: 19279 RVA: 0x001658A4 File Offset: 0x00163AA4
		public override void DeFocus()
		{
			this._rayToolView.SetFocusedInteractable(null);
			this._focusedInteractable = null;
		}

		// Token: 0x04004DDE RID: 19934
		private const float MINIMUM_RAY_CAST_DISTANCE = 0.8f;

		// Token: 0x04004DDF RID: 19935
		private const float COLLIDER_RADIUS = 0.01f;

		// Token: 0x04004DE0 RID: 19936
		private const int NUM_MAX_PRIMARY_HITS = 10;

		// Token: 0x04004DE1 RID: 19937
		private const int NUM_MAX_SECONDARY_HITS = 25;

		// Token: 0x04004DE2 RID: 19938
		private const int NUM_COLLIDERS_TO_TEST = 20;

		// Token: 0x04004DE3 RID: 19939
		[SerializeField]
		private RayToolView _rayToolView;

		// Token: 0x04004DE4 RID: 19940
		[Range(0f, 45f)]
		[SerializeField]
		private float _coneAngleDegrees = 20f;

		// Token: 0x04004DE5 RID: 19941
		[SerializeField]
		private float _farFieldMaxDistance = 5f;

		// Token: 0x04004DE6 RID: 19942
		private PinchStateModule _pinchStateModule = new PinchStateModule();

		// Token: 0x04004DE7 RID: 19943
		private Interactable _focusedInteractable;

		// Token: 0x04004DE8 RID: 19944
		private Collider[] _collidersOverlapped = new Collider[20];

		// Token: 0x04004DE9 RID: 19945
		private Interactable _currInteractableCastedAgainst;

		// Token: 0x04004DEA RID: 19946
		private float _coneAngleReleaseDegrees;

		// Token: 0x04004DEB RID: 19947
		private RaycastHit[] _primaryHits = new RaycastHit[10];

		// Token: 0x04004DEC RID: 19948
		private Collider[] _secondaryOverlapResults = new Collider[25];

		// Token: 0x04004DED RID: 19949
		private bool _initialized;
	}
}
