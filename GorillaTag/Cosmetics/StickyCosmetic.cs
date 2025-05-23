using System;
using GorillaExtensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DFA RID: 3578
	public class StickyCosmetic : MonoBehaviour
	{
		// Token: 0x06005892 RID: 22674 RVA: 0x001B3C73 File Offset: 0x001B1E73
		private void Start()
		{
			this.endRigidbody.isKinematic = false;
			this.endRigidbody.useGravity = false;
			this.UpdateState(StickyCosmetic.ObjectState.Idle);
		}

		// Token: 0x06005893 RID: 22675 RVA: 0x001B3C94 File Offset: 0x001B1E94
		public void Extend()
		{
			if (this.currentState == StickyCosmetic.ObjectState.Idle || this.currentState == StickyCosmetic.ObjectState.Extending)
			{
				this.UpdateState(StickyCosmetic.ObjectState.Extending);
			}
		}

		// Token: 0x06005894 RID: 22676 RVA: 0x001B3CAE File Offset: 0x001B1EAE
		public void Retract()
		{
			this.UpdateState(StickyCosmetic.ObjectState.Retracting);
		}

		// Token: 0x06005895 RID: 22677 RVA: 0x001B3CB8 File Offset: 0x001B1EB8
		private void Extend_Internal()
		{
			if (this.endRigidbody.isKinematic)
			{
				return;
			}
			this.rayLength = Mathf.Lerp(0f, this.maxObjectLength, this.blendShapeCosmetic.GetBlendValue() / this.blendShapeCosmetic.maxBlendShapeWeight);
			this.endRigidbody.MovePosition(this.startPosition.position + this.startPosition.forward * this.rayLength);
		}

		// Token: 0x06005896 RID: 22678 RVA: 0x001B3D34 File Offset: 0x001B1F34
		private void Retract_Internal()
		{
			this.endRigidbody.isKinematic = false;
			Vector3 vector = Vector3.MoveTowards(this.endRigidbody.position, this.startPosition.position, this.retractSpeed * Time.fixedDeltaTime);
			this.endRigidbody.MovePosition(vector);
		}

		// Token: 0x06005897 RID: 22679 RVA: 0x001B3D84 File Offset: 0x001B1F84
		private void FixedUpdate()
		{
			switch (this.currentState)
			{
			case StickyCosmetic.ObjectState.Extending:
			{
				if (Time.time - this.extendingStartedTime > this.retractAfterSecond)
				{
					this.UpdateState(StickyCosmetic.ObjectState.AutoRetract);
				}
				this.Extend_Internal();
				RaycastHit raycastHit;
				if (Physics.Raycast(this.rayOrigin.position, this.rayOrigin.forward, out raycastHit, this.rayLength, this.collisionLayers))
				{
					this.endRigidbody.isKinematic = true;
					this.endRigidbody.transform.parent = null;
					UnityEvent unityEvent = this.onStick;
					if (unityEvent != null)
					{
						unityEvent.Invoke();
					}
					this.UpdateState(StickyCosmetic.ObjectState.Stuck);
				}
				break;
			}
			case StickyCosmetic.ObjectState.Retracting:
				if (Vector3.Distance(this.endRigidbody.position, this.startPosition.position) <= 0.01f)
				{
					this.endRigidbody.position = this.startPosition.position;
					Transform transform = this.endRigidbody.transform;
					transform.parent = this.endPositionParent;
					transform.localRotation = quaternion.identity;
					transform.localScale = Vector3.one;
					if (this.lastState == StickyCosmetic.ObjectState.AutoUnstuck || this.lastState == StickyCosmetic.ObjectState.AutoRetract)
					{
						this.UpdateState(StickyCosmetic.ObjectState.JustRetracted);
					}
					else
					{
						this.UpdateState(StickyCosmetic.ObjectState.Idle);
					}
				}
				else
				{
					this.Retract_Internal();
				}
				break;
			case StickyCosmetic.ObjectState.Stuck:
				if (this.endRigidbody.isKinematic && (this.endRigidbody.position - this.startPosition.position).IsLongerThan(this.autoRetractThreshold))
				{
					this.UpdateState(StickyCosmetic.ObjectState.AutoUnstuck);
				}
				break;
			case StickyCosmetic.ObjectState.AutoUnstuck:
				this.UpdateState(StickyCosmetic.ObjectState.Retracting);
				break;
			case StickyCosmetic.ObjectState.AutoRetract:
				this.UpdateState(StickyCosmetic.ObjectState.Retracting);
				break;
			}
			Debug.DrawRay(this.rayOrigin.position, this.rayOrigin.forward * this.rayLength, Color.red);
		}

		// Token: 0x06005898 RID: 22680 RVA: 0x001B3F64 File Offset: 0x001B2164
		private void UpdateState(StickyCosmetic.ObjectState newState)
		{
			this.lastState = this.currentState;
			if (this.lastState == StickyCosmetic.ObjectState.Stuck && newState != this.currentState)
			{
				this.onUnstick.Invoke();
			}
			if (this.lastState != StickyCosmetic.ObjectState.Extending && newState == StickyCosmetic.ObjectState.Extending)
			{
				this.extendingStartedTime = Time.time;
			}
			this.currentState = newState;
		}

		// Token: 0x04005DDD RID: 24029
		[SerializeField]
		private UpdateBlendShapeCosmetic blendShapeCosmetic;

		// Token: 0x04005DDE RID: 24030
		[SerializeField]
		private LayerMask collisionLayers;

		// Token: 0x04005DDF RID: 24031
		[SerializeField]
		private Transform rayOrigin;

		// Token: 0x04005DE0 RID: 24032
		[SerializeField]
		private float maxObjectLength = 0.7f;

		// Token: 0x04005DE1 RID: 24033
		[SerializeField]
		private float autoRetractThreshold = 1f;

		// Token: 0x04005DE2 RID: 24034
		[SerializeField]
		private float retractSpeed = 5f;

		// Token: 0x04005DE3 RID: 24035
		[Tooltip("If extended but not stuck, retract automatically after X seconds")]
		[SerializeField]
		private float retractAfterSecond = 2f;

		// Token: 0x04005DE4 RID: 24036
		[SerializeField]
		private Transform startPosition;

		// Token: 0x04005DE5 RID: 24037
		[SerializeField]
		private Rigidbody endRigidbody;

		// Token: 0x04005DE6 RID: 24038
		[SerializeField]
		private Transform endPositionParent;

		// Token: 0x04005DE7 RID: 24039
		public UnityEvent onStick;

		// Token: 0x04005DE8 RID: 24040
		public UnityEvent onUnstick;

		// Token: 0x04005DE9 RID: 24041
		private StickyCosmetic.ObjectState currentState;

		// Token: 0x04005DEA RID: 24042
		private float rayLength;

		// Token: 0x04005DEB RID: 24043
		private bool stick;

		// Token: 0x04005DEC RID: 24044
		private StickyCosmetic.ObjectState lastState;

		// Token: 0x04005DED RID: 24045
		private float extendingStartedTime;

		// Token: 0x02000DFB RID: 3579
		private enum ObjectState
		{
			// Token: 0x04005DEF RID: 24047
			Extending,
			// Token: 0x04005DF0 RID: 24048
			Retracting,
			// Token: 0x04005DF1 RID: 24049
			Stuck,
			// Token: 0x04005DF2 RID: 24050
			JustRetracted,
			// Token: 0x04005DF3 RID: 24051
			Idle,
			// Token: 0x04005DF4 RID: 24052
			AutoUnstuck,
			// Token: 0x04005DF5 RID: 24053
			AutoRetract
		}
	}
}
