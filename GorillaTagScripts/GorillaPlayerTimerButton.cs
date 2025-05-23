using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000B14 RID: 2836
	public class GorillaPlayerTimerButton : MonoBehaviour
	{
		// Token: 0x060045B8 RID: 17848 RVA: 0x0014B89E File Offset: 0x00149A9E
		private void Awake()
		{
			this.materialProps = new MaterialPropertyBlock();
		}

		// Token: 0x060045B9 RID: 17849 RVA: 0x0014B8AB File Offset: 0x00149AAB
		private void Start()
		{
			this.TryInit();
		}

		// Token: 0x060045BA RID: 17850 RVA: 0x0014B8AB File Offset: 0x00149AAB
		private void OnEnable()
		{
			this.TryInit();
		}

		// Token: 0x060045BB RID: 17851 RVA: 0x0014B8B4 File Offset: 0x00149AB4
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

		// Token: 0x060045BC RID: 17852 RVA: 0x0014B930 File Offset: 0x00149B30
		private void OnDisable()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.OnTimerStopped.RemoveListener(new UnityAction<int, int>(this.OnTimerStopped));
				PlayerTimerManager.instance.OnLocalTimerStarted.RemoveListener(new UnityAction(this.OnLocalTimerStarted));
			}
			this.isInitialized = false;
		}

		// Token: 0x060045BD RID: 17853 RVA: 0x0014B987 File Offset: 0x00149B87
		private void OnLocalTimerStarted()
		{
			if (this.isBothStartAndStop)
			{
				this.isStartButton = false;
			}
		}

		// Token: 0x060045BE RID: 17854 RVA: 0x0014B998 File Offset: 0x00149B98
		private void OnTimerStopped(int actorNum, int timeDelta)
		{
			if (this.isBothStartAndStop && actorNum == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.isStartButton = true;
			}
		}

		// Token: 0x060045BF RID: 17855 RVA: 0x0014B9BC File Offset: 0x00149BBC
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

		// Token: 0x060045C0 RID: 17856 RVA: 0x0014BA7C File Offset: 0x00149C7C
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

		// Token: 0x04004854 RID: 18516
		private float lastTriggeredTime;

		// Token: 0x04004855 RID: 18517
		[SerializeField]
		private bool isStartButton;

		// Token: 0x04004856 RID: 18518
		[SerializeField]
		private bool isBothStartAndStop;

		// Token: 0x04004857 RID: 18519
		[SerializeField]
		private float debounceTime = 0.5f;

		// Token: 0x04004858 RID: 18520
		[SerializeField]
		private MeshRenderer mesh;

		// Token: 0x04004859 RID: 18521
		[SerializeField]
		private Color pressColor;

		// Token: 0x0400485A RID: 18522
		[SerializeField]
		private Color notPressedColor;

		// Token: 0x0400485B RID: 18523
		private MaterialPropertyBlock materialProps;

		// Token: 0x0400485C RID: 18524
		private bool isInitialized;
	}
}
