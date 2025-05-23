using System;
using System.Collections.Generic;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DC4 RID: 3524
	public class MedusaEyeLantern : MonoBehaviour
	{
		// Token: 0x0600575C RID: 22364 RVA: 0x001ACED8 File Offset: 0x001AB0D8
		private void Awake()
		{
			foreach (MedusaEyeLantern.EyeState eyeState in this.allStates)
			{
				this.allStatesDict.Add(eyeState.eyeState, eyeState);
			}
		}

		// Token: 0x0600575D RID: 22365 RVA: 0x001ACF10 File Offset: 0x001AB110
		private void OnDestroy()
		{
			this.allStatesDict.Clear();
		}

		// Token: 0x0600575E RID: 22366 RVA: 0x001ACF1D File Offset: 0x001AB11D
		private void Start()
		{
			if (this.rotatingObjectTransform == null)
			{
				this.rotatingObjectTransform = base.transform;
			}
			this.initialRotation = this.rotatingObjectTransform.localRotation;
			this.SwitchState(MedusaEyeLantern.State.DORMANT);
		}

		// Token: 0x0600575F RID: 22367 RVA: 0x001ACF54 File Offset: 0x001AB154
		private void Update()
		{
			if (!this.transferableParent.InHand() && this.currentState != MedusaEyeLantern.State.DORMANT)
			{
				this.SwitchState(MedusaEyeLantern.State.DORMANT);
			}
			if (!this.transferableParent.InHand())
			{
				return;
			}
			this.UpdateState();
			if (this.velocityTracker == null || this.rotatingObjectTransform == null)
			{
				return;
			}
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			Vector3 vector = new Vector3(averageVelocity.x, 0f, averageVelocity.z);
			float magnitude = vector.magnitude;
			Vector3 normalized = vector.normalized;
			float num = Mathf.Clamp(-normalized.z, -1f, 1f) * this.maxRotationAngle * (magnitude * this.rotationSpeedMultiplier);
			float num2 = Mathf.Clamp(normalized.x, -1f, 1f) * this.maxRotationAngle * (magnitude * this.rotationSpeedMultiplier);
			this.targetRotation = this.initialRotation * Quaternion.Euler(num, 0f, num2);
			if (magnitude > this.sloshVelocityThreshold)
			{
				this.SwitchState(MedusaEyeLantern.State.SLOSHING);
			}
			if ((double)magnitude < 0.01)
			{
				this.targetRotation = this.initialRotation;
			}
			if (!this.EyeIsLockedOn())
			{
				this.rotatingObjectTransform.localRotation = Quaternion.Slerp(this.rotatingObjectTransform.localRotation, this.targetRotation, Time.deltaTime * this.rotationSmoothing);
			}
		}

		// Token: 0x06005760 RID: 22368 RVA: 0x001AD0B2 File Offset: 0x001AB2B2
		public void HandleOnNoOneInRange()
		{
			this.SwitchState(MedusaEyeLantern.State.DORMANT);
			this.rotatingObjectTransform.localRotation = this.initialRotation;
		}

		// Token: 0x06005761 RID: 22369 RVA: 0x001AD0CC File Offset: 0x001AB2CC
		public void HandleOnNewPlayerDetected(VRRig target, float distance)
		{
			this.targetRig = target;
			if (this.currentState != MedusaEyeLantern.State.SLOSHING)
			{
				this.SwitchState(MedusaEyeLantern.State.TRACKING);
			}
		}

		// Token: 0x06005762 RID: 22370 RVA: 0x001AD0E4 File Offset: 0x001AB2E4
		private void Sloshing()
		{
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			Vector3 vector = new Vector3(averageVelocity.x, 0f, averageVelocity.z);
			if ((double)vector.magnitude < 0.01)
			{
				this.SwitchState(MedusaEyeLantern.State.DORMANT);
			}
		}

		// Token: 0x06005763 RID: 22371 RVA: 0x001AD138 File Offset: 0x001AB338
		private void FaceTarget()
		{
			if (this.targetRig == null || this.rotatingObjectTransform == null)
			{
				return;
			}
			Vector3 vector = this.targetRig.tagSound.transform.position - this.rotatingObjectTransform.position;
			Vector3 normalized = new Vector3(vector.x, 0f, vector.z).normalized;
			Debug.DrawRay(this.targetRig.tagSound.transform.position, this.targetRig.tagSound.transform.up, Color.red);
			Debug.DrawRay(this.rotatingObjectTransform.position, normalized, Color.green);
			Debug.DrawRay(this.rotatingObjectTransform.position, this.rotatingObjectTransform.forward, Color.blue);
			if (normalized.sqrMagnitude > 0.001f)
			{
				Quaternion quaternion = Quaternion.LookRotation(-normalized, Vector3.up);
				if (Quaternion.Angle(this.rotatingObjectTransform.rotation, quaternion) < 180f - this.targetHeadAngleThreshold && this.currentState == MedusaEyeLantern.State.TRACKING)
				{
					this.warmUpStarted = Time.time;
					this.SwitchState(MedusaEyeLantern.State.WARMUP);
					return;
				}
				this.rotatingObjectTransform.rotation = Quaternion.RotateTowards(this.rotatingObjectTransform.rotation, quaternion, this.lookAtTargetSpeed * Time.deltaTime);
			}
		}

		// Token: 0x06005764 RID: 22372 RVA: 0x001AD294 File Offset: 0x001AB494
		private bool IsTargetLookingAtEye()
		{
			if (this.targetRig == null || this.rotatingObjectTransform == null)
			{
				return false;
			}
			Transform transform = this.targetRig.tagSound.transform;
			Vector3 normalized = (this.rotatingObjectTransform.position - this.rotatingObjectTransform.forward * this.faceDistanceOffset - transform.position).normalized;
			float num = Mathf.Acos(Mathf.Clamp(Vector3.Dot(transform.up.normalized, normalized), -1f, 1f)) * 57.29578f;
			Debug.DrawRay(transform.position, transform.up * 0.3f, Color.magenta);
			Debug.DrawRay(transform.position, normalized * 0.3f, Color.yellow);
			return num < this.lookAtEyeAngleThreshold;
		}

		// Token: 0x06005765 RID: 22373 RVA: 0x001AD37C File Offset: 0x001AB57C
		private void UpdateState()
		{
			switch (this.currentState)
			{
			case MedusaEyeLantern.State.SLOSHING:
				this.Sloshing();
				return;
			case MedusaEyeLantern.State.DORMANT:
				this.warmUpStarted = float.PositiveInfinity;
				this.petrificationStarted = float.PositiveInfinity;
				if (this.targetRig != null && Vector3.Distance(base.transform.position, this.targetRig.transform.position) < this.distanceChecker.distanceThreshold)
				{
					this.SwitchState(MedusaEyeLantern.State.TRACKING);
					return;
				}
				break;
			case MedusaEyeLantern.State.TRACKING:
				this.FaceTarget();
				return;
			case MedusaEyeLantern.State.WARMUP:
				this.FaceTarget();
				if (Time.time - this.warmUpStarted > this.warmUpProgressTime)
				{
					this.SwitchState(MedusaEyeLantern.State.PRIMING);
					this.warmUpStarted = float.PositiveInfinity;
					return;
				}
				break;
			case MedusaEyeLantern.State.PRIMING:
				this.FaceTarget();
				if (this.IsTargetLookingAtEye())
				{
					this.petrificationStarted = Time.time;
					UnityEvent<VRRig> onPetrification = this.OnPetrification;
					if (onPetrification != null)
					{
						onPetrification.Invoke(this.targetRig);
					}
					this.SwitchState(MedusaEyeLantern.State.PETRIFICATION);
					return;
				}
				break;
			case MedusaEyeLantern.State.PETRIFICATION:
				if (Time.time - this.petrificationStarted > this.petrificationCooldown)
				{
					this.SwitchState(MedusaEyeLantern.State.DORMANT);
					this.petrificationStarted = float.PositiveInfinity;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06005766 RID: 22374 RVA: 0x001AD4A8 File Offset: 0x001AB6A8
		private void SwitchState(MedusaEyeLantern.State newState)
		{
			this.lastState = this.currentState;
			this.currentState = newState;
			MedusaEyeLantern.EyeState eyeState;
			if (this.lastState != this.currentState && this.allStatesDict.TryGetValue(newState, out eyeState))
			{
				this.PlayStateEffect(eyeState);
			}
			MedusaEyeLantern.EyeState eyeState2;
			if (this.lastState != this.currentState && this.allStatesDict.TryGetValue(this.lastState, out eyeState2))
			{
				this.StopStateEffect(eyeState2);
			}
		}

		// Token: 0x06005767 RID: 22375 RVA: 0x001AD518 File Offset: 0x001AB718
		private void PlayStateEffect(MedusaEyeLantern.EyeState state)
		{
			if (state == null)
			{
				return;
			}
			if (state.particle != null)
			{
				state.particle.Play();
			}
			if (state.audioClip && this.audioSource != null)
			{
				this.audioSource.GTPlayOneShot(state.audioClip, 1f);
			}
			if (this.meshRenderer != null && state.material != null)
			{
				Material[] materials = this.meshRenderer.materials;
				materials[1] = state.material;
				this.meshRenderer.materials = materials;
			}
		}

		// Token: 0x06005768 RID: 22376 RVA: 0x001AD5B0 File Offset: 0x001AB7B0
		private void StopStateEffect(MedusaEyeLantern.EyeState state)
		{
			if (state == null)
			{
				return;
			}
			if (state.particle != null)
			{
				state.particle.Stop();
			}
		}

		// Token: 0x06005769 RID: 22377 RVA: 0x001AD5CF File Offset: 0x001AB7CF
		private bool EyeIsLockedOn()
		{
			return this.currentState == MedusaEyeLantern.State.TRACKING || this.currentState == MedusaEyeLantern.State.WARMUP || this.currentState == MedusaEyeLantern.State.PRIMING;
		}

		// Token: 0x04005BE0 RID: 23520
		[SerializeField]
		private DistanceCheckerCosmetic distanceChecker;

		// Token: 0x04005BE1 RID: 23521
		[SerializeField]
		private TransferrableObject transferableParent;

		// Token: 0x04005BE2 RID: 23522
		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		// Token: 0x04005BE3 RID: 23523
		[SerializeField]
		private Transform rotatingObjectTransform;

		// Token: 0x04005BE4 RID: 23524
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04005BE5 RID: 23525
		[SerializeField]
		private SkinnedMeshRenderer meshRenderer;

		// Token: 0x04005BE6 RID: 23526
		[Space]
		[Header("Rotation Settings")]
		[SerializeField]
		private float maxRotationAngle = 50f;

		// Token: 0x04005BE7 RID: 23527
		[SerializeField]
		private float sloshVelocityThreshold = 1f;

		// Token: 0x04005BE8 RID: 23528
		[SerializeField]
		private float rotationSmoothing = 10f;

		// Token: 0x04005BE9 RID: 23529
		[SerializeField]
		private float rotationSpeedMultiplier = 5f;

		// Token: 0x04005BEA RID: 23530
		[Space]
		[Header("Target Tracking Settings")]
		[SerializeField]
		private float lookAtEyeAngleThreshold = 90f;

		// Token: 0x04005BEB RID: 23531
		[SerializeField]
		private float targetHeadAngleThreshold = 5f;

		// Token: 0x04005BEC RID: 23532
		[SerializeField]
		private float lookAtTargetSpeed = 5f;

		// Token: 0x04005BED RID: 23533
		[SerializeField]
		private float warmUpProgressTime = 3f;

		// Token: 0x04005BEE RID: 23534
		[SerializeField]
		private float petrificationCooldown = 5f;

		// Token: 0x04005BEF RID: 23535
		[SerializeField]
		private float faceDistanceOffset = 0.2f;

		// Token: 0x04005BF0 RID: 23536
		[Space]
		[Header("Eye State Settings")]
		public MedusaEyeLantern.EyeState[] allStates = new MedusaEyeLantern.EyeState[0];

		// Token: 0x04005BF1 RID: 23537
		public UnityEvent<VRRig> OnPetrification;

		// Token: 0x04005BF2 RID: 23538
		private Quaternion initialRotation;

		// Token: 0x04005BF3 RID: 23539
		private Quaternion targetRotation;

		// Token: 0x04005BF4 RID: 23540
		private MedusaEyeLantern.State currentState;

		// Token: 0x04005BF5 RID: 23541
		private MedusaEyeLantern.State lastState;

		// Token: 0x04005BF6 RID: 23542
		private float warmUpStarted = float.PositiveInfinity;

		// Token: 0x04005BF7 RID: 23543
		private float petrificationStarted = float.PositiveInfinity;

		// Token: 0x04005BF8 RID: 23544
		private Dictionary<MedusaEyeLantern.State, MedusaEyeLantern.EyeState> allStatesDict = new Dictionary<MedusaEyeLantern.State, MedusaEyeLantern.EyeState>();

		// Token: 0x04005BF9 RID: 23545
		private VRRig targetRig;

		// Token: 0x02000DC5 RID: 3525
		[Serializable]
		public class EyeState
		{
			// Token: 0x04005BFA RID: 23546
			public MedusaEyeLantern.State eyeState;

			// Token: 0x04005BFB RID: 23547
			public ParticleSystem particle;

			// Token: 0x04005BFC RID: 23548
			public Material material;

			// Token: 0x04005BFD RID: 23549
			public AudioClip audioClip;
		}

		// Token: 0x02000DC6 RID: 3526
		public enum State
		{
			// Token: 0x04005BFF RID: 23551
			SLOSHING,
			// Token: 0x04005C00 RID: 23552
			DORMANT,
			// Token: 0x04005C01 RID: 23553
			TRACKING,
			// Token: 0x04005C02 RID: 23554
			WARMUP,
			// Token: 0x04005C03 RID: 23555
			PRIMING,
			// Token: 0x04005C04 RID: 23556
			PETRIFICATION
		}
	}
}
