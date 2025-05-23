using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag.Shared.Scripts;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DCC RID: 3532
	public class ThrowableHoldableCosmetic : TransferrableObject
	{
		// Token: 0x06005794 RID: 22420 RVA: 0x001ADE38 File Offset: 0x001AC038
		internal override void OnEnable()
		{
			base.OnEnable();
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = ((base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null));
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.OnThrowEvent;
			}
		}

		// Token: 0x06005795 RID: 22421 RVA: 0x001ADEFA File Offset: 0x001AC0FA
		protected override void Awake()
		{
			base.Awake();
			this.firecrackerProjectileHash = PoolUtils.GameObjHashCode(this.firecrackerProjectilePrefab);
			this.playersEffect = base.GetComponentInChildren<CosmeticEffectsOnPlayers>();
		}

		// Token: 0x06005796 RID: 22422 RVA: 0x001ADF1F File Offset: 0x001AC11F
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (!this.disableWhenThrown.gameObject.activeSelf)
			{
				return;
			}
			base.OnGrab(pointGrabbed, grabbingHand);
		}

		// Token: 0x06005797 RID: 22423 RVA: 0x001ADF3C File Offset: 0x001AC13C
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			if (VRRigCache.Instance.localRig.Rig != this.ownerRig)
			{
				return false;
			}
			Vector3 position = base.transform.position;
			Quaternion rotation = base.transform.rotation;
			Vector3 vector = ((releasingHand == EquipmentInteractor.instance.leftHand) ? GTPlayer.Instance.leftInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false) : GTPlayer.Instance.rightInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false));
			float scale = GTPlayer.Instance.scale;
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[] { position, rotation, vector, scale });
			}
			this.OnThrowLocal(position, rotation, vector, scale);
			return true;
		}

		// Token: 0x06005798 RID: 22424 RVA: 0x001AE048 File Offset: 0x001AC248
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this._events != null)
			{
				this._events.Activate -= this.OnThrowEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06005799 RID: 22425 RVA: 0x001AE0A0 File Offset: 0x001AC2A0
		private void OnThrowEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (args.Length != 4)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnThrowEvent");
			if (this.firecrackerCallLimiter.CheckCallTime(Time.time))
			{
				object obj = args[0];
				if (obj is Vector3)
				{
					Vector3 vector = (Vector3)obj;
					obj = args[1];
					if (obj is Quaternion)
					{
						Quaternion quaternion = (Quaternion)obj;
						obj = args[2];
						if (obj is Vector3)
						{
							Vector3 vector2 = (Vector3)obj;
							obj = args[3];
							if (obj is float)
							{
								float num = (float)obj;
								vector2 = this.targetRig.ClampVelocityRelativeToPlayerSafe(vector2, 40f);
								float num2 = num.ClampSafe(0.01f, 1f);
								if (!(in quaternion).IsValid())
								{
									return;
								}
								float num3 = 10000f;
								if (!(in vector).IsValid(in num3) || !this.targetRig.IsPositionInRange(vector, 4f))
								{
									return;
								}
								this.OnThrowLocal(vector, quaternion, vector2, num2);
								return;
							}
						}
					}
				}
			}
		}

		// Token: 0x0600579A RID: 22426 RVA: 0x001AE1AC File Offset: 0x001AC3AC
		private void OnThrowLocal(Vector3 startPos, Quaternion rotation, Vector3 velocity, float playerScale)
		{
			this.disableWhenThrown.SetActive(false);
			IProjectile component = ObjectPools.instance.Instantiate(this.firecrackerProjectileHash, true).GetComponent<IProjectile>();
			FirecrackerProjectile firecrackerProjectile = component as FirecrackerProjectile;
			if (firecrackerProjectile != null)
			{
				FirecrackerProjectile firecrackerProjectile2 = firecrackerProjectile;
				firecrackerProjectile2.OnHitComplete = (Action<FirecrackerProjectile>)Delegate.Combine(firecrackerProjectile2.OnHitComplete, new Action<FirecrackerProjectile>(this.HitComplete));
				FirecrackerProjectile firecrackerProjectile3 = firecrackerProjectile;
				firecrackerProjectile3.OnHitStart = (Action<FirecrackerProjectile, Vector3>)Delegate.Combine(firecrackerProjectile3.OnHitStart, new Action<FirecrackerProjectile, Vector3>(this.HitStart));
			}
			else
			{
				FartBagThrowable fartBagThrowable = component as FartBagThrowable;
				if (fartBagThrowable != null)
				{
					fartBagThrowable.OnDeflated += this.HitComplete;
					fartBagThrowable.ParentTransferable = this;
				}
			}
			component.Launch(startPos, rotation, velocity, playerScale);
		}

		// Token: 0x0600579B RID: 22427 RVA: 0x001AE25A File Offset: 0x001AC45A
		private void HitStart(FirecrackerProjectile firecracker, Vector3 contactPos)
		{
			if (firecracker == null)
			{
				return;
			}
			if (this.playersEffect == null)
			{
				return;
			}
			this.playersEffect.ApplyAllEffectsByDistance(contactPos);
		}

		// Token: 0x0600579C RID: 22428 RVA: 0x001AE284 File Offset: 0x001AC484
		private void HitComplete(IProjectile projectile)
		{
			if (projectile == null)
			{
				return;
			}
			this.disableWhenThrown.SetActive(true);
			FirecrackerProjectile firecrackerProjectile = projectile as FirecrackerProjectile;
			if (firecrackerProjectile != null)
			{
				FirecrackerProjectile firecrackerProjectile2 = firecrackerProjectile;
				firecrackerProjectile2.OnHitStart = (Action<FirecrackerProjectile, Vector3>)Delegate.Remove(firecrackerProjectile2.OnHitStart, new Action<FirecrackerProjectile, Vector3>(this.HitStart));
				FirecrackerProjectile firecrackerProjectile3 = firecrackerProjectile;
				firecrackerProjectile3.OnHitComplete = (Action<FirecrackerProjectile>)Delegate.Remove(firecrackerProjectile3.OnHitComplete, new Action<FirecrackerProjectile>(this.HitComplete));
				ObjectPools.instance.Destroy(firecrackerProjectile.gameObject);
				return;
			}
			FartBagThrowable fartBagThrowable = projectile as FartBagThrowable;
			if (fartBagThrowable != null)
			{
				fartBagThrowable.OnDeflated -= this.HitComplete;
				ObjectPools.instance.Destroy(fartBagThrowable.gameObject);
			}
		}

		// Token: 0x04005C34 RID: 23604
		[SerializeField]
		private GameObject firecrackerProjectilePrefab;

		// Token: 0x04005C35 RID: 23605
		[SerializeField]
		private GameObject disableWhenThrown;

		// Token: 0x04005C36 RID: 23606
		private CallLimiter firecrackerCallLimiter = new CallLimiter(10, 3f, 0.5f);

		// Token: 0x04005C37 RID: 23607
		private CosmeticEffectsOnPlayers playersEffect;

		// Token: 0x04005C38 RID: 23608
		private int firecrackerProjectileHash;

		// Token: 0x04005C39 RID: 23609
		private RubberDuckEvents _events;
	}
}
