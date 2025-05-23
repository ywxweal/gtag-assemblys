using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DDF RID: 3551
	public class FartBagThrowable : MonoBehaviour, IProjectile
	{
		// Token: 0x170008C4 RID: 2244
		// (get) Token: 0x060057FB RID: 22523 RVA: 0x001B1649 File Offset: 0x001AF849
		// (set) Token: 0x060057FC RID: 22524 RVA: 0x001B1651 File Offset: 0x001AF851
		public TransferrableObject ParentTransferable { get; set; }

		// Token: 0x1400009A RID: 154
		// (add) Token: 0x060057FD RID: 22525 RVA: 0x001B165C File Offset: 0x001AF85C
		// (remove) Token: 0x060057FE RID: 22526 RVA: 0x001B1694 File Offset: 0x001AF894
		public event Action<IProjectile> OnDeflated;

		// Token: 0x060057FF RID: 22527 RVA: 0x001B16CC File Offset: 0x001AF8CC
		private void OnEnable()
		{
			this.placedOnFloor = false;
			this.deflated = false;
			this.handContactPoint = Vector3.negativeInfinity;
			this.handNormalVector = Vector3.zero;
			this.timeCreated = float.PositiveInfinity;
			this.placedOnFloorTime = float.PositiveInfinity;
			if (this.updateBlendShapeCosmetic)
			{
				this.updateBlendShapeCosmetic.ResetBlend();
			}
		}

		// Token: 0x06005800 RID: 22528 RVA: 0x001B172B File Offset: 0x001AF92B
		private void Update()
		{
			if (Time.time - this.timeCreated > this.forceDestroyAfterSec)
			{
				this.DeflateLocal();
			}
		}

		// Token: 0x06005801 RID: 22529 RVA: 0x001B1748 File Offset: 0x001AF948
		public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float scale)
		{
			base.transform.position = startPosition;
			base.transform.rotation = startRotation;
			base.transform.localScale = Vector3.one * scale;
			this.rigidbody.velocity = velocity;
			this.timeCreated = Time.time;
			this.InitialPhotonEvent();
		}

		// Token: 0x06005802 RID: 22530 RVA: 0x001B17A4 File Offset: 0x001AF9A4
		private void InitialPhotonEvent()
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			if (this.ParentTransferable)
			{
				NetPlayer netPlayer = ((this.ParentTransferable.myOnlineRig != null) ? this.ParentTransferable.myOnlineRig.creator : ((this.ParentTransferable.myRig != null) ? (this.ParentTransferable.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null));
				if (this._events != null && netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.DeflateEvent;
			}
		}

		// Token: 0x06005803 RID: 22531 RVA: 0x001B1878 File Offset: 0x001AFA78
		private void OnTriggerEnter(Collider other)
		{
			if ((this.handLayerMask.value & (1 << other.gameObject.layer)) != 0)
			{
				if (!this.placedOnFloor)
				{
					return;
				}
				this.handContactPoint = other.ClosestPoint(base.transform.position);
				this.handNormalVector = (this.handContactPoint - base.transform.position).normalized;
				if (Time.time - this.placedOnFloorTime > 0.3f)
				{
					this.Deflate();
				}
			}
		}

		// Token: 0x06005804 RID: 22532 RVA: 0x001B1900 File Offset: 0x001AFB00
		private void OnCollisionEnter(Collision other)
		{
			if ((this.floorLayerMask.value & (1 << other.gameObject.layer)) != 0)
			{
				this.placedOnFloor = true;
				this.placedOnFloorTime = Time.time;
				Vector3 normal = other.contacts[0].normal;
				base.transform.position = other.contacts[0].point + normal * this.placementOffset;
				Quaternion quaternion = Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.forward, normal).normalized, normal);
				base.transform.rotation = quaternion;
			}
		}

		// Token: 0x06005805 RID: 22533 RVA: 0x001B19A8 File Offset: 0x001AFBA8
		private void Deflate()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[] { this.handContactPoint, this.handNormalVector });
			}
			this.DeflateLocal();
		}

		// Token: 0x06005806 RID: 22534 RVA: 0x001B1A18 File Offset: 0x001AFC18
		private void DeflateEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (args.Length != 2)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "DeflateEvent");
			if (this.callLimiter.CheckCallTime(Time.time))
			{
				object obj = args[0];
				if (obj is Vector3)
				{
					Vector3 vector = (Vector3)obj;
					obj = args[1];
					if (obj is Vector3)
					{
						Vector3 vector2 = (Vector3)obj;
						float num = 10000f;
						if (!(in vector2).IsValid(in num))
						{
							return;
						}
						num = 10000f;
						if (!(in vector).IsValid(in num) || !this.ParentTransferable.targetRig.IsPositionInRange(vector, 4f))
						{
							return;
						}
						this.handNormalVector = vector2;
						this.handContactPoint = vector;
						this.DeflateLocal();
						return;
					}
				}
			}
		}

		// Token: 0x06005807 RID: 22535 RVA: 0x001B1AC8 File Offset: 0x001AFCC8
		private void DeflateLocal()
		{
			if (this.deflated)
			{
				return;
			}
			GameObject gameObject = ObjectPools.instance.Instantiate(this.deflationEffect, this.handContactPoint, true);
			gameObject.transform.up = this.handNormalVector;
			gameObject.transform.position = base.transform.position;
			SoundBankPlayer componentInChildren = gameObject.GetComponentInChildren<SoundBankPlayer>();
			if (componentInChildren.soundBank)
			{
				componentInChildren.Play();
			}
			this.placedOnFloor = false;
			this.timeCreated = float.PositiveInfinity;
			if (this.updateBlendShapeCosmetic)
			{
				this.updateBlendShapeCosmetic.FullyBlend();
			}
			this.deflated = true;
			base.Invoke("DisableObject", this.destroyWhenDeflateDelay);
		}

		// Token: 0x06005808 RID: 22536 RVA: 0x001B1B77 File Offset: 0x001AFD77
		private void DisableObject()
		{
			Action<IProjectile> onDeflated = this.OnDeflated;
			if (onDeflated != null)
			{
				onDeflated(this);
			}
			this.deflated = false;
		}

		// Token: 0x06005809 RID: 22537 RVA: 0x001B1B94 File Offset: 0x001AFD94
		private void OnDestroy()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.DeflateEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x04005D24 RID: 23844
		[SerializeField]
		private GameObject deflationEffect;

		// Token: 0x04005D25 RID: 23845
		[SerializeField]
		private float destroyWhenDeflateDelay = 3f;

		// Token: 0x04005D26 RID: 23846
		[SerializeField]
		private float forceDestroyAfterSec = 10f;

		// Token: 0x04005D27 RID: 23847
		[SerializeField]
		private float placementOffset = 0.2f;

		// Token: 0x04005D28 RID: 23848
		[SerializeField]
		private UpdateBlendShapeCosmetic updateBlendShapeCosmetic;

		// Token: 0x04005D29 RID: 23849
		[SerializeField]
		private LayerMask floorLayerMask;

		// Token: 0x04005D2A RID: 23850
		[SerializeField]
		private LayerMask handLayerMask;

		// Token: 0x04005D2B RID: 23851
		[SerializeField]
		private Rigidbody rigidbody;

		// Token: 0x04005D2C RID: 23852
		private bool placedOnFloor;

		// Token: 0x04005D2D RID: 23853
		private float placedOnFloorTime;

		// Token: 0x04005D2E RID: 23854
		private float timeCreated;

		// Token: 0x04005D2F RID: 23855
		private bool deflated;

		// Token: 0x04005D30 RID: 23856
		private Vector3 handContactPoint;

		// Token: 0x04005D31 RID: 23857
		private Vector3 handNormalVector;

		// Token: 0x04005D32 RID: 23858
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x04005D35 RID: 23861
		private RubberDuckEvents _events;
	}
}
