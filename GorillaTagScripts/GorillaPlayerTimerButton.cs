using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000B14 RID: 2836
	public class GorillaPlayerTimerButton : MonoBehaviour
	{
		// Token: 0x060045B7 RID: 17847 RVA: 0x0014B7C6 File Offset: 0x001499C6
		private void Awake()
		{
			this.materialProps = new MaterialPropertyBlock();
		}

		// Token: 0x060045B8 RID: 17848 RVA: 0x0014B7D3 File Offset: 0x001499D3
		private void Start()
		{
			this.TryInit();
		}

		// Token: 0x060045B9 RID: 17849 RVA: 0x0014B7D3 File Offset: 0x001499D3
		private void OnEnable()
		{
			this.TryInit();
		}

		// Token: 0x060045BA RID: 17850 RVA: 0x0014B7DC File Offset: 0x001499DC
		private void TryInit()
		{
			if (this.isInitialized)
			{
				return;
			}
			if (PlayerTimerManager.instance == null)
			{
				return;
			}
			PlayerTimerManager.instance.OnTimerStopped.AddListener(new UnityAction<int, int>(this.OnTimerStopped));
			PlayerTimerManager.instance.OnLocalTimerStarted.AddListener(new UnityAction(this.OnLocalTimerStarted));
			if (this.isBothStartAndStop)
			{
				this.isStartButton = !PlayerTimerManager.instance.IsLocalTimerStarted();
			}
			this.isInitialized = true;
		}

		// Token: 0x060045BB RID: 17851 RVA: 0x0014B858 File Offset: 0x00149A58
		private void OnDisable()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.OnTimerStopped.RemoveListener(new UnityAction<int, int>(this.OnTimerStopped));
				PlayerTimerManager.instance.OnLocalTimerStarted.RemoveListener(new UnityAction(this.OnLocalTimerStarted));
			}
			this.isInitialized = false;
		}

		// Token: 0x060045BC RID: 17852 RVA: 0x0014B8AF File Offset: 0x00149AAF
		private void OnLocalTimerStarted()
		{
			if (this.isBothStartAndStop)
			{
				this.isStartButton = false;
			}
		}

		// Token: 0x060045BD RID: 17853 RVA: 0x0014B8C0 File Offset: 0x00149AC0
		private void OnTimerStopped(int actorNum, int timeDelta)
		{
			if (this.isBothStartAndStop && actorNum == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.isStartButton = true;
			}
		}

		// Token: 0x060045BE RID: 17854 RVA: 0x0014B8E4 File Offset: 0x00149AE4
		private void OnTriggerEnter(Collider other)
		{
			if (!base.enabled)
			{
				return;
			}
			GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (componentInParent == null)
			{
				return;
			}
			if (Time.time < this.lastTriggeredTime + this.debounceTime)
			{
				return;
			}
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			this.mesh.GetPropertyBlock(this.materialProps);
			this.materialProps.SetColor("_BaseColor", this.pressColor);
			this.mesh.SetPropertyBlock(this.materialProps);
			PlayerTimerManager.instance.RequestTimerToggle(this.isStartButton);
			this.lastTriggeredTime = Time.time;
		}

		// Token: 0x060045BF RID: 17855 RVA: 0x0014B9A4 File Offset: 0x00149BA4
		private void OnTriggerExit(Collider other)
		{
			if (!base.enabled)
			{
				return;
			}
			if (other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
			{
				return;
			}
			this.mesh.GetPropertyBlock(this.materialProps);
			this.materialProps.SetColor("_BaseColor", this.notPressedColor);
			this.mesh.SetPropertyBlock(this.materialProps);
		}

		// Token: 0x04004853 RID: 18515
		private float lastTriggeredTime;

		// Token: 0x04004854 RID: 18516
		[SerializeField]
		private bool isStartButton;

		// Token: 0x04004855 RID: 18517
		[SerializeField]
		private bool isBothStartAndStop;

		// Token: 0x04004856 RID: 18518
		[SerializeField]
		private float debounceTime = 0.5f;

		// Token: 0x04004857 RID: 18519
		[SerializeField]
		private MeshRenderer mesh;

		// Token: 0x04004858 RID: 18520
		[SerializeField]
		private Color pressColor;

		// Token: 0x04004859 RID: 18521
		[SerializeField]
		private Color notPressedColor;

		// Token: 0x0400485A RID: 18522
		private MaterialPropertyBlock materialProps;

		// Token: 0x0400485B RID: 18523
		private bool isInitialized;
	}
}
