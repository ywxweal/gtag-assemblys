using System;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DCD RID: 3533
	public class ThrowablePickupableCosmetic : TransferrableObject
	{
		// Token: 0x0600579E RID: 22430 RVA: 0x001AE34C File Offset: 0x001AC54C
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
				else
				{
					Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.OnReleaseEvent;
			}
		}

		// Token: 0x0600579F RID: 22431 RVA: 0x001AE41C File Offset: 0x001AC61C
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this._events != null)
			{
				this._events.Activate -= this.OnReleaseEvent;
				Object.Destroy(this._events);
				this._events = null;
			}
			if (this.pickupableVariant.enabled)
			{
				this.pickupableVariant.DelayedPickup();
			}
		}

		// Token: 0x060057A0 RID: 22432 RVA: 0x001AE48C File Offset: 0x001AC68C
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (VRRigCache.Instance.localRig.Rig != this.ownerRig)
			{
				return;
			}
			if (this.pickupableVariant.enabled)
			{
				if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					this._events.Activate.RaiseOthers(new object[] { false });
				}
				base.transform.position = this.pickupableVariant.transform.position;
				base.transform.rotation = this.pickupableVariant.transform.rotation;
				this.pickupableVariant.Pickup();
				if (grabbingHand == EquipmentInteractor.instance.leftHand && this.currentState == TransferrableObject.PositionState.OnLeftArm)
				{
					this.canAutoGrabLeft = false;
					this.interpState = TransferrableObject.InterpolateState.None;
					this.currentState = TransferrableObject.PositionState.InRightHand;
				}
				else if (grabbingHand == EquipmentInteractor.instance.rightHand && this.currentState == TransferrableObject.PositionState.OnRightArm)
				{
					this.canAutoGrabRight = false;
					this.interpState = TransferrableObject.InterpolateState.None;
					this.currentState = TransferrableObject.PositionState.InLeftHand;
				}
			}
			UnityEvent onGrabFromDockPosition = this.OnGrabFromDockPosition;
			if (onGrabFromDockPosition != null)
			{
				onGrabFromDockPosition.Invoke();
			}
			base.OnGrab(pointGrabbed, grabbingHand);
		}

		// Token: 0x060057A1 RID: 22433 RVA: 0x001AE5D0 File Offset: 0x001AC7D0
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
			if (this.DistanceToDock() > this.returnToDockDistanceThreshold)
			{
				Vector3 position = base.transform.position;
				Vector3 vector = ((releasingHand == EquipmentInteractor.instance.leftHand) ? GTPlayer.Instance.leftInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false) : GTPlayer.Instance.rightInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false));
				float scale = GTPlayer.Instance.scale;
				if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					this._events.Activate.RaiseOthers(new object[] { true, position, vector, scale });
				}
				this.OnReleaseEventLocal(position, vector, scale);
			}
			else
			{
				UnityEvent onReturnToDockPosition = this.OnReturnToDockPosition;
				if (onReturnToDockPosition != null)
				{
					onReturnToDockPosition.Invoke();
				}
			}
			return true;
		}

		// Token: 0x060057A2 RID: 22434 RVA: 0x001AE6F4 File Offset: 0x001AC8F4
		private void OnReleaseEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnReleaseEvent");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			object obj = args[0];
			if (obj is bool)
			{
				bool flag = (bool)obj;
				if (flag)
				{
					obj = args[1];
					if (obj is Vector3)
					{
						Vector3 vector = (Vector3)obj;
						obj = args[2];
						if (obj is Vector3)
						{
							Vector3 vector2 = (Vector3)obj;
							obj = args[3];
							if (obj is float)
							{
								float num = (float)obj;
								Vector3 position = base.transform.position;
								Vector3 vector3 = base.transform.forward;
								(ref position).SetValueSafe(in vector);
								if (this.ownerRig.IsPositionInRange(position, 4f))
								{
									vector3 = this.ownerRig.ClampVelocityRelativeToPlayerSafe(vector2, 50f);
									float num2 = num.ClampSafe(0.01f, 1f);
									this.OnReleaseEventLocal(position, vector3, num2);
									return;
								}
								return;
							}
						}
					}
					return;
				}
				this.pickupableVariant.Pickup();
				return;
			}
		}

		// Token: 0x060057A3 RID: 22435 RVA: 0x001AE80D File Offset: 0x001ACA0D
		private void OnReleaseEventLocal(Vector3 startPosition, Vector3 releaseVelocity, float playerScale)
		{
			this.pickupableVariant.Release(this, startPosition, releaseVelocity, playerScale);
		}

		// Token: 0x060057A4 RID: 22436 RVA: 0x001AE820 File Offset: 0x001ACA20
		private float DistanceToDock()
		{
			float num = 0f;
			if (this.currentState == TransferrableObject.PositionState.OnRightShoulder)
			{
				num = Vector3.Distance(this.ownerRig.myBodyDockPositions.rightBackTransform.position, base.transform.position);
			}
			else if (this.currentState == TransferrableObject.PositionState.OnLeftShoulder)
			{
				num = Vector3.Distance(this.ownerRig.myBodyDockPositions.leftBackTransform.position, base.transform.position);
			}
			else if (this.currentState == TransferrableObject.PositionState.OnRightArm)
			{
				num = Vector3.Distance(this.ownerRig.myBodyDockPositions.rightArmTransform.position, base.transform.position);
			}
			else if (this.currentState == TransferrableObject.PositionState.OnLeftArm)
			{
				num = Vector3.Distance(this.ownerRig.myBodyDockPositions.leftArmTransform.position, base.transform.position);
			}
			else if (this.currentState == TransferrableObject.PositionState.OnChest)
			{
				num = Vector3.Distance(this.ownerRig.myBodyDockPositions.chestTransform.position, base.transform.position);
			}
			return num;
		}

		// Token: 0x04005C3A RID: 23610
		[SerializeField]
		private PickupableVariant pickupableVariant;

		// Token: 0x04005C3B RID: 23611
		[SerializeField]
		private float returnToDockDistanceThreshold = 0.7f;

		// Token: 0x04005C3C RID: 23612
		public UnityEvent OnReturnToDockPosition;

		// Token: 0x04005C3D RID: 23613
		public UnityEvent OnGrabFromDockPosition;

		// Token: 0x04005C3E RID: 23614
		private RubberDuckEvents _events;

		// Token: 0x04005C3F RID: 23615
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);
	}
}
