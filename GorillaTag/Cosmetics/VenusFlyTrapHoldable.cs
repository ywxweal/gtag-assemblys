using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DFE RID: 3582
	[RequireComponent(typeof(TransferrableObject))]
	public class VenusFlyTrapHoldable : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x170008D0 RID: 2256
		// (get) Token: 0x060058A4 RID: 22692 RVA: 0x001B40B3 File Offset: 0x001B22B3
		// (set) Token: 0x060058A5 RID: 22693 RVA: 0x001B40BB File Offset: 0x001B22BB
		public bool TickRunning { get; set; }

		// Token: 0x060058A6 RID: 22694 RVA: 0x001B40C4 File Offset: 0x001B22C4
		private void Awake()
		{
			this.transferrableObject = base.GetComponent<TransferrableObject>();
		}

		// Token: 0x060058A7 RID: 22695 RVA: 0x001B40D4 File Offset: 0x001B22D4
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
			this.triggerEventNotifier.TriggerEnterEvent += this.TriggerEntered;
			this.state = VenusFlyTrapHoldable.VenusState.Open;
			this.localRotA = this.lipA.transform.localRotation;
			this.localRotB = this.lipB.transform.localRotation;
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = ((this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? (this.transferrableObject.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null));
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.OnTriggerEvent;
			}
		}

		// Token: 0x060058A8 RID: 22696 RVA: 0x001B41EC File Offset: 0x001B23EC
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
			this.triggerEventNotifier.TriggerEnterEvent -= this.TriggerEntered;
			if (this._events != null)
			{
				this._events.Activate -= this.OnTriggerEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x060058A9 RID: 22697 RVA: 0x001B4258 File Offset: 0x001B2458
		public void Tick()
		{
			if (this.transferrableObject.InHand() && this.audioSource && !this.audioSource.isPlaying && this.flyLoopingAudio != null)
			{
				this.audioSource.clip = this.flyLoopingAudio;
				this.audioSource.GTPlay();
			}
			if (!this.transferrableObject.InHand() && this.audioSource && this.audioSource.isPlaying)
			{
				this.audioSource.GTStop();
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Open)
			{
				return;
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Closed && Time.time - this.closedStartedTime >= this.closedDuration)
			{
				this.UpdateState(VenusFlyTrapHoldable.VenusState.Opening);
				if (this.audioSource && this.openingAudio != null)
				{
					this.audioSource.GTPlayOneShot(this.openingAudio, 1f);
				}
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Closing)
			{
				this.SmoothRotation(true);
				return;
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Opening)
			{
				this.SmoothRotation(false);
			}
		}

		// Token: 0x060058AA RID: 22698 RVA: 0x001B4368 File Offset: 0x001B2568
		private void SmoothRotation(bool isClosing)
		{
			if (isClosing)
			{
				Quaternion quaternion = Quaternion.Euler(this.targetRotationB);
				this.lipB.transform.localRotation = Quaternion.Lerp(this.lipB.transform.localRotation, quaternion, Time.deltaTime * this.speed);
				Quaternion quaternion2 = Quaternion.Euler(this.targetRotationA);
				this.lipA.transform.localRotation = Quaternion.Lerp(this.lipA.transform.localRotation, quaternion2, Time.deltaTime * this.speed);
				if (Quaternion.Angle(this.lipB.transform.localRotation, quaternion) < 1f && Quaternion.Angle(this.lipA.transform.localRotation, quaternion2) < 1f)
				{
					this.lipB.transform.localRotation = quaternion;
					this.lipA.transform.localRotation = quaternion2;
					this.UpdateState(VenusFlyTrapHoldable.VenusState.Closed);
					return;
				}
			}
			else
			{
				this.lipB.transform.localRotation = Quaternion.Lerp(this.lipB.transform.localRotation, this.localRotB, Time.deltaTime * this.speed / 2f);
				this.lipA.transform.localRotation = Quaternion.Lerp(this.lipA.transform.localRotation, this.localRotA, Time.deltaTime * this.speed / 2f);
				if (Quaternion.Angle(this.lipB.transform.localRotation, this.localRotB) < 1f && Quaternion.Angle(this.lipA.transform.localRotation, this.localRotA) < 1f)
				{
					this.lipB.transform.localRotation = this.localRotB;
					this.lipA.transform.localRotation = this.localRotA;
					this.UpdateState(VenusFlyTrapHoldable.VenusState.Open);
				}
			}
		}

		// Token: 0x060058AB RID: 22699 RVA: 0x001B4552 File Offset: 0x001B2752
		private void UpdateState(VenusFlyTrapHoldable.VenusState newState)
		{
			this.state = newState;
			if (this.state == VenusFlyTrapHoldable.VenusState.Closed)
			{
				this.closedStartedTime = Time.time;
			}
		}

		// Token: 0x060058AC RID: 22700 RVA: 0x001B4570 File Offset: 0x001B2770
		private void TriggerEntered(TriggerEventNotifier notifier, Collider other)
		{
			if (this.state != VenusFlyTrapHoldable.VenusState.Open)
			{
				return;
			}
			if (!other.gameObject.IsOnLayer(this.layers))
			{
				return;
			}
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(Array.Empty<object>());
			}
			this.OnTriggerLocal();
			GorillaTriggerColliderHandIndicator componentInChildren = other.GetComponentInChildren<GorillaTriggerColliderHandIndicator>();
			if (componentInChildren == null)
			{
				return;
			}
			GorillaTagger.Instance.StartVibration(componentInChildren.isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x060058AD RID: 22701 RVA: 0x001B460B File Offset: 0x001B280B
		private void OnTriggerEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnTriggerEvent");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			this.OnTriggerLocal();
		}

		// Token: 0x060058AE RID: 22702 RVA: 0x001B4637 File Offset: 0x001B2837
		private void OnTriggerLocal()
		{
			this.UpdateState(VenusFlyTrapHoldable.VenusState.Closing);
			if (this.audioSource && this.closingAudio != null)
			{
				this.audioSource.GTPlayOneShot(this.closingAudio, 1f);
			}
		}

		// Token: 0x04005E03 RID: 24067
		[SerializeField]
		private GameObject lipA;

		// Token: 0x04005E04 RID: 24068
		[SerializeField]
		private GameObject lipB;

		// Token: 0x04005E05 RID: 24069
		[SerializeField]
		private Vector3 targetRotationA;

		// Token: 0x04005E06 RID: 24070
		[SerializeField]
		private Vector3 targetRotationB;

		// Token: 0x04005E07 RID: 24071
		[SerializeField]
		private float closedDuration = 3f;

		// Token: 0x04005E08 RID: 24072
		[SerializeField]
		private float speed = 2f;

		// Token: 0x04005E09 RID: 24073
		[SerializeField]
		private UnityLayer layers;

		// Token: 0x04005E0A RID: 24074
		[SerializeField]
		private TriggerEventNotifier triggerEventNotifier;

		// Token: 0x04005E0B RID: 24075
		[SerializeField]
		private float hapticStrength = 0.5f;

		// Token: 0x04005E0C RID: 24076
		[SerializeField]
		private float hapticDuration = 0.1f;

		// Token: 0x04005E0D RID: 24077
		[SerializeField]
		private GameObject bug;

		// Token: 0x04005E0E RID: 24078
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04005E0F RID: 24079
		[SerializeField]
		private AudioClip closingAudio;

		// Token: 0x04005E10 RID: 24080
		[SerializeField]
		private AudioClip openingAudio;

		// Token: 0x04005E11 RID: 24081
		[SerializeField]
		private AudioClip flyLoopingAudio;

		// Token: 0x04005E12 RID: 24082
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x04005E13 RID: 24083
		private float closedStartedTime;

		// Token: 0x04005E14 RID: 24084
		private VenusFlyTrapHoldable.VenusState state;

		// Token: 0x04005E15 RID: 24085
		private Quaternion localRotA;

		// Token: 0x04005E16 RID: 24086
		private Quaternion localRotB;

		// Token: 0x04005E17 RID: 24087
		private RubberDuckEvents _events;

		// Token: 0x04005E18 RID: 24088
		private TransferrableObject transferrableObject;

		// Token: 0x02000DFF RID: 3583
		private enum VenusState
		{
			// Token: 0x04005E1B RID: 24091
			Closed,
			// Token: 0x04005E1C RID: 24092
			Open,
			// Token: 0x04005E1D RID: 24093
			Closing,
			// Token: 0x04005E1E RID: 24094
			Opening
		}
	}
}
