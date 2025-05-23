using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DF9 RID: 3577
	public class SprayCanCosmeticNetworked : MonoBehaviour
	{
		// Token: 0x0600588C RID: 22668 RVA: 0x001B39E8 File Offset: 0x001B1BE8
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
				this._events.Activate += this.OnShakeEvent;
			}
		}

		// Token: 0x0600588D RID: 22669 RVA: 0x001B3AB0 File Offset: 0x001B1CB0
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.OnShakeEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x0600588E RID: 22670 RVA: 0x001B3B00 File Offset: 0x001B1D00
		private void OnShakeEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnShakeEvent");
			NetPlayer sender2 = info.Sender;
			VRRig myOnlineRig = this.transferrableObject.myOnlineRig;
			if (sender2 != ((myOnlineRig != null) ? myOnlineRig.creator : null))
			{
				return;
			}
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			object obj = args[0];
			if (!(obj is bool))
			{
				return;
			}
			bool flag = (bool)obj;
			if (flag)
			{
				UnityEvent handleOnShakeStart = this.HandleOnShakeStart;
				if (handleOnShakeStart == null)
				{
					return;
				}
				handleOnShakeStart.Invoke();
				return;
			}
			else
			{
				UnityEvent handleOnShakeEnd = this.HandleOnShakeEnd;
				if (handleOnShakeEnd == null)
				{
					return;
				}
				handleOnShakeEnd.Invoke();
				return;
			}
		}

		// Token: 0x0600588F RID: 22671 RVA: 0x001B3B8C File Offset: 0x001B1D8C
		public void OnShakeStart()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[] { true });
			}
			UnityEvent handleOnShakeStart = this.HandleOnShakeStart;
			if (handleOnShakeStart == null)
			{
				return;
			}
			handleOnShakeStart.Invoke();
		}

		// Token: 0x06005890 RID: 22672 RVA: 0x001B3BF0 File Offset: 0x001B1DF0
		public void OnShakeEnd()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[] { false });
			}
			UnityEvent handleOnShakeEnd = this.HandleOnShakeEnd;
			if (handleOnShakeEnd == null)
			{
				return;
			}
			handleOnShakeEnd.Invoke();
		}

		// Token: 0x04005DD8 RID: 24024
		[SerializeField]
		private TransferrableObject transferrableObject;

		// Token: 0x04005DD9 RID: 24025
		private RubberDuckEvents _events;

		// Token: 0x04005DDA RID: 24026
		private CallLimiter callLimiter = new CallLimiter(10, 1f, 0.5f);

		// Token: 0x04005DDB RID: 24027
		public UnityEvent HandleOnShakeStart;

		// Token: 0x04005DDC RID: 24028
		public UnityEvent HandleOnShakeEnd;
	}
}
