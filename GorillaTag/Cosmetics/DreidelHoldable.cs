using System;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DC3 RID: 3523
	public class DreidelHoldable : TransferrableObject
	{
		// Token: 0x06005753 RID: 22355 RVA: 0x001ACA38 File Offset: 0x001AAC38
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
				this._events.Activate += this.OnDreidelSpin;
			}
		}

		// Token: 0x06005754 RID: 22356 RVA: 0x001ACB08 File Offset: 0x001AAD08
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this._events != null)
			{
				this._events.Activate -= this.OnDreidelSpin;
				Object.Destroy(this._events);
				this._events = null;
			}
		}

		// Token: 0x06005755 RID: 22357 RVA: 0x001ACB60 File Offset: 0x001AAD60
		private void OnDreidelSpin(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			GorillaNot.IncrementRPCCall(info, "OnDreidelSpin");
			if (sender != target)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			Vector3 vector = (Vector3)args[0];
			Vector3 vector2 = (Vector3)args[1];
			float num = (float)args[2];
			double num2 = (double)args[6];
			float num3 = 10000f;
			if ((in vector).IsValid(in num3))
			{
				float num4 = 10000f;
				if ((in vector2).IsValid(in num4) && float.IsFinite(num) && double.IsFinite(num2))
				{
					bool flag = (bool)args[3];
					Dreidel.Side side = (Dreidel.Side)args[4];
					Dreidel.Variation variation = (Dreidel.Variation)args[5];
					this.StartSpinLocal(vector, vector2, num, flag, side, variation, num2);
					return;
				}
			}
		}

		// Token: 0x06005756 RID: 22358 RVA: 0x001ACC1F File Offset: 0x001AAE1F
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			base.OnGrab(pointGrabbed, grabbingHand);
			if (this.dreidelAnimation != null)
			{
				this.dreidelAnimation.TryCheckForSurfaces();
			}
		}

		// Token: 0x06005757 RID: 22359 RVA: 0x001ACC43 File Offset: 0x001AAE43
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			if (this.dreidelAnimation != null)
			{
				this.dreidelAnimation.TrySetIdle();
			}
			return true;
		}

		// Token: 0x06005758 RID: 22360 RVA: 0x001ACC6C File Offset: 0x001AAE6C
		public override void OnActivate()
		{
			base.OnActivate();
			Vector3 vector;
			Vector3 vector2;
			float num;
			Dreidel.Side side;
			Dreidel.Variation variation;
			double num2;
			if (this.dreidelAnimation != null && this.dreidelAnimation.TryGetSpinStartData(out vector, out vector2, out num, out side, out variation, out num2))
			{
				bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
				if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					object[] array = new object[]
					{
						vector,
						vector2,
						num,
						flag,
						(int)side,
						(int)variation,
						num2
					};
					this._events.Activate.RaiseAll(array);
					return;
				}
				this.StartSpinLocal(vector, vector2, num, flag, side, variation, num2);
			}
		}

		// Token: 0x06005759 RID: 22361 RVA: 0x001ACD4C File Offset: 0x001AAF4C
		private void StartSpinLocal(Vector3 surfacePoint, Vector3 surfaceNormal, float duration, bool counterClockwise, Dreidel.Side side, Dreidel.Variation variation, double startTime)
		{
			if (this.dreidelAnimation != null)
			{
				this.dreidelAnimation.SetSpinStartData(surfacePoint, surfaceNormal, duration, counterClockwise, side, variation, startTime);
				this.dreidelAnimation.Spin();
			}
		}

		// Token: 0x0600575A RID: 22362 RVA: 0x001ACD80 File Offset: 0x001AAF80
		public void DebugSpinDreidel()
		{
			Transform transform = GTPlayer.Instance.headCollider.transform;
			Vector3 vector = transform.position + transform.forward * 0.5f;
			float num = 2f;
			RaycastHit raycastHit;
			if (Physics.Raycast(vector, Vector3.down, out raycastHit, num, GTPlayer.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
			{
				Vector3 point = raycastHit.point;
				Vector3 normal = raycastHit.normal;
				float num2 = Random.Range(7f, 10f);
				Dreidel.Side side = (Dreidel.Side)Random.Range(0, 4);
				Dreidel.Variation variation = (Dreidel.Variation)Random.Range(0, 5);
				bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
				double num3 = (PhotonNetwork.InRoom ? PhotonNetwork.Time : (-1.0));
				if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					object[] array = new object[]
					{
						point,
						normal,
						num2,
						flag,
						(int)side,
						(int)variation,
						num3
					};
					this._events.Activate.RaiseAll(array);
					return;
				}
				this.StartSpinLocal(point, normal, num2, flag, side, variation, num3);
			}
		}

		// Token: 0x04005BDE RID: 23518
		[SerializeField]
		private Dreidel dreidelAnimation;

		// Token: 0x04005BDF RID: 23519
		private RubberDuckEvents _events;
	}
}
