using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DE4 RID: 3556
	public class GlowBugsInJar : MonoBehaviour
	{
		// Token: 0x06005813 RID: 22547 RVA: 0x001B1EF0 File Offset: 0x001B00F0
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

		// Token: 0x06005814 RID: 22548 RVA: 0x001B1FC8 File Offset: 0x001B01C8
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.OnShakeEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06005815 RID: 22549 RVA: 0x001B2018 File Offset: 0x001B0218
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

		// Token: 0x06005816 RID: 22550 RVA: 0x001B2078 File Offset: 0x001B0278
		public void HandleOnShakeStart()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[] { true });
			}
			this.ShakeStartLocal();
		}

		// Token: 0x06005817 RID: 22551 RVA: 0x001B20D2 File Offset: 0x001B02D2
		private void ShakeStartLocal()
		{
			this.currentGlowAmount = 0f;
			this.shakeStarted = true;
			this.shakeTimer = 0f;
		}

		// Token: 0x06005818 RID: 22552 RVA: 0x001B20F4 File Offset: 0x001B02F4
		public void HandleOnShakeEnd()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[] { false });
			}
			this.ShakeEndLocal();
		}

		// Token: 0x06005819 RID: 22553 RVA: 0x001B214E File Offset: 0x001B034E
		private void ShakeEndLocal()
		{
			this.shakeStarted = false;
			this.shakeTimer = 0f;
		}

		// Token: 0x0600581A RID: 22554 RVA: 0x001B2164 File Offset: 0x001B0364
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

		// Token: 0x0600581B RID: 22555 RVA: 0x001B2230 File Offset: 0x001B0430
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

		// Token: 0x04005D4B RID: 23883
		[SerializeField]
		private TransferrableObject transferrableObject;

		// Token: 0x04005D4C RID: 23884
		[Space]
		[Tooltip("Time interval - every X seconds update the glow value")]
		[SerializeField]
		private float glowUpdateInterval = 2f;

		// Token: 0x04005D4D RID: 23885
		[Tooltip("step increment - increase the glow value one step for N amount")]
		[SerializeField]
		private float glowIncreaseStepAmount = 0.1f;

		// Token: 0x04005D4E RID: 23886
		[Tooltip("step decrement - decrease the glow value one step for N amount")]
		[SerializeField]
		private float glowDecreaseStepAmount = 0.2f;

		// Token: 0x04005D4F RID: 23887
		[Space]
		[SerializeField]
		private string shaderProperty = "_EmissionColor";

		// Token: 0x04005D50 RID: 23888
		[SerializeField]
		private Renderer[] renderers;

		// Token: 0x04005D51 RID: 23889
		private bool shakeStarted = true;

		// Token: 0x04005D52 RID: 23890
		private static int EmissionColor;

		// Token: 0x04005D53 RID: 23891
		private float currentGlowAmount;

		// Token: 0x04005D54 RID: 23892
		private float shakeTimer;

		// Token: 0x04005D55 RID: 23893
		private RubberDuckEvents _events;

		// Token: 0x04005D56 RID: 23894
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);
	}
}
