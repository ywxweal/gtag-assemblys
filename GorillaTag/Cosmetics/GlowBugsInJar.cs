using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DE4 RID: 3556
	public class GlowBugsInJar : MonoBehaviour
	{
		// Token: 0x06005814 RID: 22548 RVA: 0x001B1FC8 File Offset: 0x001B01C8
		private void OnEnable()
		{
			this.shakeStarted = false;
			this.UpdateGlow(0f);
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

		// Token: 0x06005815 RID: 22549 RVA: 0x001B20A0 File Offset: 0x001B02A0
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.OnShakeEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06005816 RID: 22550 RVA: 0x001B20F0 File Offset: 0x001B02F0
		private void OnShakeEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnShakeEvent");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			if (args != null && args.Length == 1)
			{
				object obj = args[0];
				if (obj is bool)
				{
					bool flag = (bool)obj;
					if (flag)
					{
						this.ShakeStartLocal();
						return;
					}
					this.ShakeEndLocal();
					return;
				}
			}
		}

		// Token: 0x06005817 RID: 22551 RVA: 0x001B2150 File Offset: 0x001B0350
		public void HandleOnShakeStart()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[] { true });
			}
			this.ShakeStartLocal();
		}

		// Token: 0x06005818 RID: 22552 RVA: 0x001B21AA File Offset: 0x001B03AA
		private void ShakeStartLocal()
		{
			this.currentGlowAmount = 0f;
			this.shakeStarted = true;
			this.shakeTimer = 0f;
		}

		// Token: 0x06005819 RID: 22553 RVA: 0x001B21CC File Offset: 0x001B03CC
		public void HandleOnShakeEnd()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[] { false });
			}
			this.ShakeEndLocal();
		}

		// Token: 0x0600581A RID: 22554 RVA: 0x001B2226 File Offset: 0x001B0426
		private void ShakeEndLocal()
		{
			this.shakeStarted = false;
			this.shakeTimer = 0f;
		}

		// Token: 0x0600581B RID: 22555 RVA: 0x001B223C File Offset: 0x001B043C
		public void Update()
		{
			if (this.shakeStarted)
			{
				this.shakeTimer += 1f;
				if (this.shakeTimer >= this.glowUpdateInterval && this.currentGlowAmount < 1f)
				{
					this.currentGlowAmount += this.glowIncreaseStepAmount;
					this.UpdateGlow(this.currentGlowAmount);
					this.shakeTimer = 0f;
					return;
				}
			}
			else
			{
				this.shakeTimer += 1f;
				if (this.shakeTimer >= this.glowUpdateInterval && this.currentGlowAmount > 0f)
				{
					this.currentGlowAmount -= this.glowDecreaseStepAmount;
					this.UpdateGlow(this.currentGlowAmount);
					this.shakeTimer = 0f;
				}
			}
		}

		// Token: 0x0600581C RID: 22556 RVA: 0x001B2308 File Offset: 0x001B0508
		private void UpdateGlow(float value)
		{
			if (this.renderers.Length != 0)
			{
				for (int i = 0; i < this.renderers.Length; i++)
				{
					Material material = this.renderers[i].material;
					Color color = material.GetColor(this.shaderProperty);
					color.a = value;
					material.SetColor(this.shaderProperty, color);
					material.EnableKeyword("_EMISSION");
				}
			}
		}

		// Token: 0x04005D4C RID: 23884
		[SerializeField]
		private TransferrableObject transferrableObject;

		// Token: 0x04005D4D RID: 23885
		[Space]
		[Tooltip("Time interval - every X seconds update the glow value")]
		[SerializeField]
		private float glowUpdateInterval = 2f;

		// Token: 0x04005D4E RID: 23886
		[Tooltip("step increment - increase the glow value one step for N amount")]
		[SerializeField]
		private float glowIncreaseStepAmount = 0.1f;

		// Token: 0x04005D4F RID: 23887
		[Tooltip("step decrement - decrease the glow value one step for N amount")]
		[SerializeField]
		private float glowDecreaseStepAmount = 0.2f;

		// Token: 0x04005D50 RID: 23888
		[Space]
		[SerializeField]
		private string shaderProperty = "_EmissionColor";

		// Token: 0x04005D51 RID: 23889
		[SerializeField]
		private Renderer[] renderers;

		// Token: 0x04005D52 RID: 23890
		private bool shakeStarted = true;

		// Token: 0x04005D53 RID: 23891
		private static int EmissionColor;

		// Token: 0x04005D54 RID: 23892
		private float currentGlowAmount;

		// Token: 0x04005D55 RID: 23893
		private float shakeTimer;

		// Token: 0x04005D56 RID: 23894
		private RubberDuckEvents _events;

		// Token: 0x04005D57 RID: 23895
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);
	}
}
