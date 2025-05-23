using System;
using System.Collections;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DF7 RID: 3575
	public class SnakeInCanHoldable : TransferrableObject
	{
		// Token: 0x0600587C RID: 22652 RVA: 0x001B3522 File Offset: 0x001B1722
		protected override void Awake()
		{
			base.Awake();
			this.topRigPosition = this.topRigObject.transform.position;
		}

		// Token: 0x0600587D RID: 22653 RVA: 0x001B3540 File Offset: 0x001B1740
		internal override void OnEnable()
		{
			base.OnEnable();
			this.disableObjectBeforeTrigger.SetActive(false);
			if (this.compressedPoint != null)
			{
				this.topRigObject.transform.position = this.compressedPoint.position;
			}
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
				this._events.Activate += this.OnEnableObject;
			}
		}

		// Token: 0x0600587E RID: 22654 RVA: 0x001B3638 File Offset: 0x001B1838
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this._events != null)
			{
				this._events.Activate -= this.OnEnableObject;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x0600587F RID: 22655 RVA: 0x001B3690 File Offset: 0x001B1890
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
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[] { false });
			}
			this.EnableObjectLocal(false);
			return true;
		}

		// Token: 0x06005880 RID: 22656 RVA: 0x001B3718 File Offset: 0x001B1918
		private void OnEnableObject(int sender, int target, object[] arg, PhotonMessageInfoWrapped info)
		{
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			if (arg.Length != 1 || !(arg[0] is bool))
			{
				return;
			}
			if (sender != target)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnEnableObject");
			if (!this.snakeInCanCallLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			bool flag = (bool)arg[0];
			this.EnableObjectLocal(flag);
		}

		// Token: 0x06005881 RID: 22657 RVA: 0x001B3784 File Offset: 0x001B1984
		private void EnableObjectLocal(bool enable)
		{
			this.disableObjectBeforeTrigger.SetActive(enable);
			if (!enable)
			{
				if (this.compressedPoint != null)
				{
					this.topRigObject.transform.position = this.compressedPoint.position;
				}
				return;
			}
			if (this.stretchedPoint != null)
			{
				base.StartCoroutine(this.SmoothTransition());
				return;
			}
			this.topRigObject.transform.position = this.topRigPosition;
		}

		// Token: 0x06005882 RID: 22658 RVA: 0x001B37FC File Offset: 0x001B19FC
		private IEnumerator SmoothTransition()
		{
			while (Vector3.Distance(this.topRigObject.transform.position, this.stretchedPoint.position) > 0.01f)
			{
				this.topRigObject.transform.position = Vector3.MoveTowards(this.topRigObject.transform.position, this.stretchedPoint.position, this.jumpSpeed * Time.deltaTime);
				yield return null;
			}
			this.topRigObject.transform.position = this.stretchedPoint.position;
			yield break;
		}

		// Token: 0x06005883 RID: 22659 RVA: 0x001B380B File Offset: 0x001B1A0B
		public void OnButtonPressed()
		{
			this.EnableObjectLocal(true);
		}

		// Token: 0x04005DCB RID: 24011
		[SerializeField]
		private float jumpSpeed;

		// Token: 0x04005DCC RID: 24012
		[SerializeField]
		private Transform stretchedPoint;

		// Token: 0x04005DCD RID: 24013
		[SerializeField]
		private Transform compressedPoint;

		// Token: 0x04005DCE RID: 24014
		[SerializeField]
		private GameObject topRigObject;

		// Token: 0x04005DCF RID: 24015
		[SerializeField]
		private GameObject disableObjectBeforeTrigger;

		// Token: 0x04005DD0 RID: 24016
		private CallLimiter snakeInCanCallLimiter = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x04005DD1 RID: 24017
		private Vector3 topRigPosition;

		// Token: 0x04005DD2 RID: 24018
		private Vector3 originalTopRigPosition;

		// Token: 0x04005DD3 RID: 24019
		private RubberDuckEvents _events;
	}
}
