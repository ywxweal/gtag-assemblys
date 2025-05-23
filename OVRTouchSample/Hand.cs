using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OVRTouchSample
{
	// Token: 0x02000C06 RID: 3078
	[RequireComponent(typeof(OVRGrabber))]
	public class Hand : MonoBehaviour
	{
		// Token: 0x06004C14 RID: 19476 RVA: 0x00168762 File Offset: 0x00166962
		private void Awake()
		{
			this.m_grabber = base.GetComponent<OVRGrabber>();
		}

		// Token: 0x06004C15 RID: 19477 RVA: 0x00168770 File Offset: 0x00166970
		private void Start()
		{
			this.m_showAfterInputFocusAcquired = new List<Renderer>();
			this.m_colliders = (from childCollider in base.GetComponentsInChildren<Collider>()
				where !childCollider.isTrigger
				select childCollider).ToArray<Collider>();
			this.CollisionEnable(false);
			this.m_animLayerIndexPoint = this.m_animator.GetLayerIndex("Point Layer");
			this.m_animLayerIndexThumb = this.m_animator.GetLayerIndex("Thumb Layer");
			this.m_animParamIndexFlex = Animator.StringToHash("Flex");
			this.m_animParamIndexPose = Animator.StringToHash("Pose");
			OVRManager.InputFocusAcquired += this.OnInputFocusAcquired;
			OVRManager.InputFocusLost += this.OnInputFocusLost;
		}

		// Token: 0x06004C16 RID: 19478 RVA: 0x00168832 File Offset: 0x00166A32
		private void OnDestroy()
		{
			OVRManager.InputFocusAcquired -= this.OnInputFocusAcquired;
			OVRManager.InputFocusLost -= this.OnInputFocusLost;
		}

		// Token: 0x06004C17 RID: 19479 RVA: 0x00168858 File Offset: 0x00166A58
		private void Update()
		{
			this.UpdateCapTouchStates();
			this.m_pointBlend = this.InputValueRateChange(this.m_isPointing, this.m_pointBlend);
			this.m_thumbsUpBlend = this.InputValueRateChange(this.m_isGivingThumbsUp, this.m_thumbsUpBlend);
			float num = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this.m_controller);
			bool flag = this.m_grabber.grabbedObject == null && num >= 0.9f;
			this.CollisionEnable(flag);
			this.UpdateAnimStates();
		}

		// Token: 0x06004C18 RID: 19480 RVA: 0x001688D7 File Offset: 0x00166AD7
		private void UpdateCapTouchStates()
		{
			this.m_isPointing = !OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, this.m_controller);
			this.m_isGivingThumbsUp = !OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, this.m_controller);
		}

		// Token: 0x06004C19 RID: 19481 RVA: 0x00168904 File Offset: 0x00166B04
		private void LateUpdate()
		{
			if (this.m_collisionEnabled && this.m_collisionScaleCurrent + Mathf.Epsilon < 1f)
			{
				this.m_collisionScaleCurrent = Mathf.Min(1f, this.m_collisionScaleCurrent + Time.deltaTime * 1f);
				for (int i = 0; i < this.m_colliders.Length; i++)
				{
					this.m_colliders[i].transform.localScale = new Vector3(this.m_collisionScaleCurrent, this.m_collisionScaleCurrent, this.m_collisionScaleCurrent);
				}
			}
		}

		// Token: 0x06004C1A RID: 19482 RVA: 0x0016898C File Offset: 0x00166B8C
		private void OnInputFocusLost()
		{
			if (base.gameObject.activeInHierarchy)
			{
				this.m_showAfterInputFocusAcquired.Clear();
				Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i].enabled)
					{
						componentsInChildren[i].enabled = false;
						this.m_showAfterInputFocusAcquired.Add(componentsInChildren[i]);
					}
				}
				this.CollisionEnable(false);
				this.m_restoreOnInputAcquired = true;
			}
		}

		// Token: 0x06004C1B RID: 19483 RVA: 0x001689F8 File Offset: 0x00166BF8
		private void OnInputFocusAcquired()
		{
			if (this.m_restoreOnInputAcquired)
			{
				for (int i = 0; i < this.m_showAfterInputFocusAcquired.Count; i++)
				{
					if (this.m_showAfterInputFocusAcquired[i])
					{
						this.m_showAfterInputFocusAcquired[i].enabled = true;
					}
				}
				this.m_showAfterInputFocusAcquired.Clear();
				this.m_restoreOnInputAcquired = false;
			}
		}

		// Token: 0x06004C1C RID: 19484 RVA: 0x00168A5C File Offset: 0x00166C5C
		private float InputValueRateChange(bool isDown, float value)
		{
			float num = Time.deltaTime * 20f;
			float num2 = (isDown ? 1f : (-1f));
			return Mathf.Clamp01(value + num * num2);
		}

		// Token: 0x06004C1D RID: 19485 RVA: 0x00168A90 File Offset: 0x00166C90
		private void UpdateAnimStates()
		{
			bool flag = this.m_grabber.grabbedObject != null;
			HandPose handPose = this.m_defaultGrabPose;
			if (flag)
			{
				HandPose component = this.m_grabber.grabbedObject.GetComponent<HandPose>();
				if (component != null)
				{
					handPose = component;
				}
			}
			HandPoseId poseId = handPose.PoseId;
			this.m_animator.SetInteger(this.m_animParamIndexPose, (int)poseId);
			float num = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this.m_controller);
			this.m_animator.SetFloat(this.m_animParamIndexFlex, num);
			float num2 = ((!flag || handPose.AllowPointing) ? this.m_pointBlend : 0f);
			this.m_animator.SetLayerWeight(this.m_animLayerIndexPoint, num2);
			float num3 = ((!flag || handPose.AllowThumbsUp) ? this.m_thumbsUpBlend : 0f);
			this.m_animator.SetLayerWeight(this.m_animLayerIndexThumb, num3);
			float num4 = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, this.m_controller);
			this.m_animator.SetFloat("Pinch", num4);
		}

		// Token: 0x06004C1E RID: 19486 RVA: 0x00168B8C File Offset: 0x00166D8C
		private void CollisionEnable(bool enabled)
		{
			if (this.m_collisionEnabled == enabled)
			{
				return;
			}
			this.m_collisionEnabled = enabled;
			if (enabled)
			{
				this.m_collisionScaleCurrent = 0.01f;
				for (int i = 0; i < this.m_colliders.Length; i++)
				{
					Collider collider = this.m_colliders[i];
					collider.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
					collider.enabled = true;
				}
				return;
			}
			this.m_collisionScaleCurrent = 1f;
			for (int j = 0; j < this.m_colliders.Length; j++)
			{
				Collider collider2 = this.m_colliders[j];
				collider2.enabled = false;
				collider2.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			}
		}

		// Token: 0x04004ED3 RID: 20179
		public const string ANIM_LAYER_NAME_POINT = "Point Layer";

		// Token: 0x04004ED4 RID: 20180
		public const string ANIM_LAYER_NAME_THUMB = "Thumb Layer";

		// Token: 0x04004ED5 RID: 20181
		public const string ANIM_PARAM_NAME_FLEX = "Flex";

		// Token: 0x04004ED6 RID: 20182
		public const string ANIM_PARAM_NAME_POSE = "Pose";

		// Token: 0x04004ED7 RID: 20183
		public const float THRESH_COLLISION_FLEX = 0.9f;

		// Token: 0x04004ED8 RID: 20184
		public const float INPUT_RATE_CHANGE = 20f;

		// Token: 0x04004ED9 RID: 20185
		public const float COLLIDER_SCALE_MIN = 0.01f;

		// Token: 0x04004EDA RID: 20186
		public const float COLLIDER_SCALE_MAX = 1f;

		// Token: 0x04004EDB RID: 20187
		public const float COLLIDER_SCALE_PER_SECOND = 1f;

		// Token: 0x04004EDC RID: 20188
		public const float TRIGGER_DEBOUNCE_TIME = 0.05f;

		// Token: 0x04004EDD RID: 20189
		public const float THUMB_DEBOUNCE_TIME = 0.15f;

		// Token: 0x04004EDE RID: 20190
		[SerializeField]
		private OVRInput.Controller m_controller;

		// Token: 0x04004EDF RID: 20191
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04004EE0 RID: 20192
		[SerializeField]
		private HandPose m_defaultGrabPose;

		// Token: 0x04004EE1 RID: 20193
		private Collider[] m_colliders;

		// Token: 0x04004EE2 RID: 20194
		private bool m_collisionEnabled = true;

		// Token: 0x04004EE3 RID: 20195
		private OVRGrabber m_grabber;

		// Token: 0x04004EE4 RID: 20196
		private List<Renderer> m_showAfterInputFocusAcquired;

		// Token: 0x04004EE5 RID: 20197
		private int m_animLayerIndexThumb = -1;

		// Token: 0x04004EE6 RID: 20198
		private int m_animLayerIndexPoint = -1;

		// Token: 0x04004EE7 RID: 20199
		private int m_animParamIndexFlex = -1;

		// Token: 0x04004EE8 RID: 20200
		private int m_animParamIndexPose = -1;

		// Token: 0x04004EE9 RID: 20201
		private bool m_isPointing;

		// Token: 0x04004EEA RID: 20202
		private bool m_isGivingThumbsUp;

		// Token: 0x04004EEB RID: 20203
		private float m_pointBlend;

		// Token: 0x04004EEC RID: 20204
		private float m_thumbsUpBlend;

		// Token: 0x04004EED RID: 20205
		private bool m_restoreOnInputAcquired;

		// Token: 0x04004EEE RID: 20206
		private float m_collisionScaleCurrent;
	}
}
