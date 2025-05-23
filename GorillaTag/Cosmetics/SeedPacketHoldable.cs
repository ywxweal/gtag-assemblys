using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DCA RID: 3530
	[RequireComponent(typeof(TransferrableObject))]
	public class SeedPacketHoldable : MonoBehaviour
	{
		// Token: 0x0600577B RID: 22395 RVA: 0x001AD932 File Offset: 0x001ABB32
		private void Awake()
		{
			this.transferrableObject = base.GetComponent<TransferrableObject>();
			this.flowerEffectHash = PoolUtils.GameObjHashCode(this.flowerEffectPrefab);
		}

		// Token: 0x0600577C RID: 22396 RVA: 0x001AD954 File Offset: 0x001ABB54
		private void OnEnable()
		{
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
				this._events.Activate += this.SyncTriggerEffect;
			}
		}

		// Token: 0x0600577D RID: 22397 RVA: 0x001ADA1C File Offset: 0x001ABC1C
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.SyncTriggerEffect;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x0600577E RID: 22398 RVA: 0x001ADA6B File Offset: 0x001ABC6B
		private void OnDestroy()
		{
			this.pooledObjects.Clear();
		}

		// Token: 0x0600577F RID: 22399 RVA: 0x001ADA78 File Offset: 0x001ABC78
		private void Update()
		{
			if (!this.transferrableObject.InHand())
			{
				return;
			}
			if (!this.isPouring && Vector3.Angle(base.transform.up, Vector3.down) <= this.pouringAngle)
			{
				this.StartPouring();
				RaycastHit raycastHit;
				if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, this.pouringRaycastDistance, this.raycastLayerMask))
				{
					this.hitPoint = raycastHit.point;
					base.Invoke("SpawnEffect", raycastHit.distance * this.placeEffectDelayMultiplier);
				}
			}
			if (this.isPouring && Time.time - this.pouringStartedTime >= this.cooldown)
			{
				this.isPouring = false;
			}
		}

		// Token: 0x06005780 RID: 22400 RVA: 0x001ADB31 File Offset: 0x001ABD31
		private void StartPouring()
		{
			if (this.particles)
			{
				this.particles.Play();
			}
			this.isPouring = true;
			this.pouringStartedTime = Time.time;
		}

		// Token: 0x06005781 RID: 22401 RVA: 0x001ADB60 File Offset: 0x001ABD60
		private void SpawnEffect()
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(this.flowerEffectHash, true);
			gameObject.transform.position = this.hitPoint;
			OnTriggerEventsHandlerCosmetic onTriggerEventsHandlerCosmetic;
			if (gameObject.TryGetComponent<OnTriggerEventsHandlerCosmetic>(out onTriggerEventsHandlerCosmetic))
			{
				this.pooledObjects.Add(onTriggerEventsHandlerCosmetic);
				onTriggerEventsHandlerCosmetic.onTriggerEntered.AddListener(new UnityAction<OnTriggerEventsHandlerCosmetic>(this.SyncTriggerEffectForOthers));
			}
		}

		// Token: 0x06005782 RID: 22402 RVA: 0x001ADBBC File Offset: 0x001ABDBC
		private void SyncTriggerEffectForOthers(OnTriggerEventsHandlerCosmetic onTriggerEventsHandlerCosmeticTriggerEvent)
		{
			int num = this.pooledObjects.IndexOf(onTriggerEventsHandlerCosmeticTriggerEvent);
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[] { num });
			}
		}

		// Token: 0x06005783 RID: 22403 RVA: 0x001ADC20 File Offset: 0x001ABE20
		private void SyncTriggerEffect(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (args.Length != 1)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "SyncTriggerEffect");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			int num = (int)args[0];
			if (num < 0 && num >= this.pooledObjects.Count)
			{
				return;
			}
			this.pooledObjects[num].ToggleEffects();
		}

		// Token: 0x04005C19 RID: 23577
		[SerializeField]
		private float cooldown;

		// Token: 0x04005C1A RID: 23578
		[SerializeField]
		private ParticleSystem particles;

		// Token: 0x04005C1B RID: 23579
		[SerializeField]
		private float pouringAngle;

		// Token: 0x04005C1C RID: 23580
		[SerializeField]
		private float pouringRaycastDistance = 5f;

		// Token: 0x04005C1D RID: 23581
		[SerializeField]
		private LayerMask raycastLayerMask;

		// Token: 0x04005C1E RID: 23582
		[SerializeField]
		private float placeEffectDelayMultiplier = 10f;

		// Token: 0x04005C1F RID: 23583
		[SerializeField]
		private GameObject flowerEffectPrefab;

		// Token: 0x04005C20 RID: 23584
		private List<OnTriggerEventsHandlerCosmetic> pooledObjects = new List<OnTriggerEventsHandlerCosmetic>();

		// Token: 0x04005C21 RID: 23585
		private CallLimiter callLimiter = new CallLimiter(10, 3f, 0.5f);

		// Token: 0x04005C22 RID: 23586
		private int flowerEffectHash;

		// Token: 0x04005C23 RID: 23587
		private Vector3 hitPoint;

		// Token: 0x04005C24 RID: 23588
		private TransferrableObject transferrableObject;

		// Token: 0x04005C25 RID: 23589
		private bool isPouring = true;

		// Token: 0x04005C26 RID: 23590
		private float pouringStartedTime;

		// Token: 0x04005C27 RID: 23591
		private RubberDuckEvents _events;
	}
}
