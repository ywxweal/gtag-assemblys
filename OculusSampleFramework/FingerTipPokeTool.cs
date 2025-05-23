using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BDF RID: 3039
	public class FingerTipPokeTool : InteractableTool
	{
		// Token: 0x17000752 RID: 1874
		// (get) Token: 0x06004AFE RID: 19198 RVA: 0x00110F07 File Offset: 0x0010F107
		public override InteractableToolTags ToolTags
		{
			get
			{
				return InteractableToolTags.Poke;
			}
		}

		// Token: 0x17000753 RID: 1875
		// (get) Token: 0x06004AFF RID: 19199 RVA: 0x00002076 File Offset: 0x00000276
		public override ToolInputState ToolInputState
		{
			get
			{
				return ToolInputState.Inactive;
			}
		}

		// Token: 0x17000754 RID: 1876
		// (get) Token: 0x06004B00 RID: 19200 RVA: 0x00002076 File Offset: 0x00000276
		public override bool IsFarFieldTool
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000755 RID: 1877
		// (get) Token: 0x06004B01 RID: 19201 RVA: 0x0016489C File Offset: 0x00162A9C
		// (set) Token: 0x06004B02 RID: 19202 RVA: 0x001648AE File Offset: 0x00162AAE
		public override bool EnableState
		{
			get
			{
				return this._fingerTipPokeToolView.gameObject.activeSelf;
			}
			set
			{
				this._fingerTipPokeToolView.gameObject.SetActive(value);
			}
		}

		// Token: 0x06004B03 RID: 19203 RVA: 0x001648C4 File Offset: 0x00162AC4
		public override void Initialize()
		{
			InteractableToolsInputRouter.Instance.RegisterInteractableTool(this);
			this._fingerTipPokeToolView.InteractableTool = this;
			this._velocityFrames = new Vector3[10];
			Array.Clear(this._velocityFrames, 0, 10);
			base.StartCoroutine(this.AttachTriggerLogic());
		}

		// Token: 0x06004B04 RID: 19204 RVA: 0x00164910 File Offset: 0x00162B10
		private IEnumerator AttachTriggerLogic()
		{
			while (!HandsManager.Instance || !HandsManager.Instance.IsInitialized())
			{
				yield return null;
			}
			OVRSkeleton ovrskeleton = (base.IsRightHandedTool ? HandsManager.Instance.RightHandSkeleton : HandsManager.Instance.LeftHandSkeleton);
			OVRSkeleton.BoneId boneId;
			switch (this._fingerToFollow)
			{
			case OVRPlugin.HandFinger.Thumb:
				boneId = OVRSkeleton.BoneId.Hand_Thumb3;
				break;
			case OVRPlugin.HandFinger.Index:
				boneId = OVRSkeleton.BoneId.Hand_Index3;
				break;
			case OVRPlugin.HandFinger.Middle:
				boneId = OVRSkeleton.BoneId.Hand_Middle3;
				break;
			case OVRPlugin.HandFinger.Ring:
				boneId = OVRSkeleton.BoneId.Hand_Ring3;
				break;
			default:
				boneId = OVRSkeleton.BoneId.Hand_Pinky3;
				break;
			}
			List<BoneCapsuleTriggerLogic> list = new List<BoneCapsuleTriggerLogic>();
			List<OVRBoneCapsule> capsulesPerBone = HandsManager.GetCapsulesPerBone(ovrskeleton, boneId);
			foreach (OVRBoneCapsule ovrboneCapsule in capsulesPerBone)
			{
				BoneCapsuleTriggerLogic boneCapsuleTriggerLogic = ovrboneCapsule.CapsuleRigidbody.gameObject.AddComponent<BoneCapsuleTriggerLogic>();
				ovrboneCapsule.CapsuleCollider.isTrigger = true;
				boneCapsuleTriggerLogic.ToolTags = this.ToolTags;
				list.Add(boneCapsuleTriggerLogic);
			}
			this._boneCapsuleTriggerLogic = list.ToArray();
			if (capsulesPerBone.Count > 0)
			{
				this._capsuleToTrack = capsulesPerBone[0];
			}
			this._isInitialized = true;
			yield break;
		}

		// Token: 0x06004B05 RID: 19205 RVA: 0x00164920 File Offset: 0x00162B20
		private void Update()
		{
			if (!HandsManager.Instance || !HandsManager.Instance.IsInitialized() || !this._isInitialized || this._capsuleToTrack == null)
			{
				return;
			}
			float handScale = (base.IsRightHandedTool ? HandsManager.Instance.RightHand : HandsManager.Instance.LeftHand).HandScale;
			Transform transform = this._capsuleToTrack.CapsuleCollider.transform;
			Vector3 right = transform.right;
			Vector3 vector = transform.position + this._capsuleToTrack.CapsuleCollider.height * 0.5f * right;
			Vector3 vector2 = handScale * this._fingerTipPokeToolView.SphereRadius * right;
			Vector3 vector3 = vector + vector2;
			base.transform.position = vector3;
			base.transform.rotation = transform.rotation;
			base.InteractionPosition = vector;
			this.UpdateAverageVelocity();
			this.CheckAndUpdateScale();
		}

		// Token: 0x06004B06 RID: 19206 RVA: 0x00164A08 File Offset: 0x00162C08
		private void UpdateAverageVelocity()
		{
			Vector3 position = this._position;
			Vector3 position2 = base.transform.position;
			Vector3 vector = (position2 - position) / Time.deltaTime;
			this._position = position2;
			this._velocityFrames[this._currVelocityFrame] = vector;
			this._currVelocityFrame = (this._currVelocityFrame + 1) % 10;
			base.Velocity = Vector3.zero;
			if (!this._sampledMaxFramesAlready && this._currVelocityFrame == 9)
			{
				this._sampledMaxFramesAlready = true;
			}
			int num = (this._sampledMaxFramesAlready ? 10 : (this._currVelocityFrame + 1));
			for (int i = 0; i < num; i++)
			{
				base.Velocity += this._velocityFrames[i];
			}
			base.Velocity /= (float)num;
		}

		// Token: 0x06004B07 RID: 19207 RVA: 0x00164AE0 File Offset: 0x00162CE0
		private void CheckAndUpdateScale()
		{
			float num = (base.IsRightHandedTool ? HandsManager.Instance.RightHand.HandScale : HandsManager.Instance.LeftHand.HandScale);
			if (Mathf.Abs(num - this._lastScale) > Mathf.Epsilon)
			{
				base.transform.localScale = new Vector3(num, num, num);
				this._lastScale = num;
			}
		}

		// Token: 0x06004B08 RID: 19208 RVA: 0x00164B44 File Offset: 0x00162D44
		public override List<InteractableCollisionInfo> GetNextIntersectingObjects()
		{
			this._currentIntersectingObjects.Clear();
			BoneCapsuleTriggerLogic[] boneCapsuleTriggerLogic = this._boneCapsuleTriggerLogic;
			for (int i = 0; i < boneCapsuleTriggerLogic.Length; i++)
			{
				foreach (ColliderZone colliderZone in boneCapsuleTriggerLogic[i].CollidersTouchingUs)
				{
					this._currentIntersectingObjects.Add(new InteractableCollisionInfo(colliderZone, colliderZone.CollisionDepth, this));
				}
			}
			return this._currentIntersectingObjects;
		}

		// Token: 0x06004B09 RID: 19209 RVA: 0x000023F4 File Offset: 0x000005F4
		public override void FocusOnInteractable(Interactable focusedInteractable, ColliderZone colliderZone)
		{
		}

		// Token: 0x06004B0A RID: 19210 RVA: 0x000023F4 File Offset: 0x000005F4
		public override void DeFocus()
		{
		}

		// Token: 0x04004DAD RID: 19885
		private const int NUM_VELOCITY_FRAMES = 10;

		// Token: 0x04004DAE RID: 19886
		[SerializeField]
		private FingerTipPokeToolView _fingerTipPokeToolView;

		// Token: 0x04004DAF RID: 19887
		[SerializeField]
		private OVRPlugin.HandFinger _fingerToFollow = OVRPlugin.HandFinger.Index;

		// Token: 0x04004DB0 RID: 19888
		private Vector3[] _velocityFrames;

		// Token: 0x04004DB1 RID: 19889
		private int _currVelocityFrame;

		// Token: 0x04004DB2 RID: 19890
		private bool _sampledMaxFramesAlready;

		// Token: 0x04004DB3 RID: 19891
		private Vector3 _position;

		// Token: 0x04004DB4 RID: 19892
		private BoneCapsuleTriggerLogic[] _boneCapsuleTriggerLogic;

		// Token: 0x04004DB5 RID: 19893
		private float _lastScale = 1f;

		// Token: 0x04004DB6 RID: 19894
		private bool _isInitialized;

		// Token: 0x04004DB7 RID: 19895
		private OVRBoneCapsule _capsuleToTrack;
	}
}
