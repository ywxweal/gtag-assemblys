using System;
using System.Collections;
using GorillaExtensions;
using GorillaGameModes;
using GorillaTagScripts.ModIO;
using GT_CustomMapSupportRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000B2A RID: 2858
	public class VirtualStumpReturnWatch : MonoBehaviour
	{
		// Token: 0x0600465B RID: 18011 RVA: 0x0014E688 File Offset: 0x0014C888
		private void Start()
		{
			if (this.returnButton != null)
			{
				this.returnButton.onStartPressingButton.AddListener(new UnityAction(this.OnStartedPressingButton));
				this.returnButton.onStopPressingButton.AddListener(new UnityAction(this.OnStoppedPressingButton));
				this.returnButton.onPressButton.AddListener(new UnityAction(this.OnButtonPressed));
			}
		}

		// Token: 0x0600465C RID: 18012 RVA: 0x0014E6F8 File Offset: 0x0014C8F8
		private void OnDestroy()
		{
			if (this.returnButton != null)
			{
				this.returnButton.onStartPressingButton.RemoveListener(new UnityAction(this.OnStartedPressingButton));
				this.returnButton.onStopPressingButton.RemoveListener(new UnityAction(this.OnStoppedPressingButton));
				this.returnButton.onPressButton.RemoveListener(new UnityAction(this.OnButtonPressed));
			}
		}

		// Token: 0x0600465D RID: 18013 RVA: 0x0014E768 File Offset: 0x0014C968
		public static void SetWatchProperties(VirtualStumpReturnWatchProps props)
		{
			VirtualStumpReturnWatch.currentCustomMapProps = props;
			VirtualStumpReturnWatch.currentCustomMapProps.holdDuration = Mathf.Clamp(VirtualStumpReturnWatch.currentCustomMapProps.holdDuration, 0.5f, 5f);
			VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Infection = Mathf.Clamp(VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Infection, 0.5f, 5f);
			VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Custom = Mathf.Clamp(VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Custom, 0.5f, 5f);
		}

		// Token: 0x0600465E RID: 18014 RVA: 0x0014E7E4 File Offset: 0x0014C9E4
		private float GetCurrentHoldDuration()
		{
			switch (GorillaGameManager.instance.GameType())
			{
			case GameModeType.Infection:
				if (VirtualStumpReturnWatch.currentCustomMapProps.infectionOverride)
				{
					return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Infection;
				}
				return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration;
			case GameModeType.Custom:
				if (VirtualStumpReturnWatch.currentCustomMapProps.customModeOverride)
				{
					return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Custom;
				}
				return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration;
			}
			return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration;
		}

		// Token: 0x0600465F RID: 18015 RVA: 0x0014E876 File Offset: 0x0014CA76
		private void OnStartedPressingButton()
		{
			this.startPressingButtonTime = Time.time;
			this.currentlyBeingPressed = true;
			this.returnButton.pressDuration = this.GetCurrentHoldDuration();
			this.ShowCountdownText();
			this.updateCountdownCoroutine = base.StartCoroutine(this.UpdateCountdownText());
		}

		// Token: 0x06004660 RID: 18016 RVA: 0x0014E8B3 File Offset: 0x0014CAB3
		private void OnStoppedPressingButton()
		{
			this.currentlyBeingPressed = false;
			this.HideCountdownText();
			if (this.updateCountdownCoroutine != null)
			{
				base.StopCoroutine(this.updateCountdownCoroutine);
				this.updateCountdownCoroutine = null;
			}
		}

		// Token: 0x06004661 RID: 18017 RVA: 0x0014E8E0 File Offset: 0x0014CAE0
		private void OnButtonPressed()
		{
			this.currentlyBeingPressed = false;
			if (ZoneManagement.IsInZone(GTZone.customMaps) && !CustomMapManager.IsLocalPlayerInVirtualStump())
			{
				bool flag = VirtualStumpReturnWatch.currentCustomMapProps.shouldTagPlayer;
				bool flag2 = VirtualStumpReturnWatch.currentCustomMapProps.shouldKickPlayer;
				switch (GorillaGameManager.instance.GameType())
				{
				case GameModeType.Infection:
					if (VirtualStumpReturnWatch.currentCustomMapProps.infectionOverride)
					{
						flag = VirtualStumpReturnWatch.currentCustomMapProps.shouldTagPlayer_Infection;
						flag2 = VirtualStumpReturnWatch.currentCustomMapProps.shouldKickPlayer_Infection;
					}
					break;
				case GameModeType.Custom:
					if (VirtualStumpReturnWatch.currentCustomMapProps.customModeOverride)
					{
						flag = VirtualStumpReturnWatch.currentCustomMapProps.shouldTagPlayer_CustomMode;
						flag2 = VirtualStumpReturnWatch.currentCustomMapProps.shouldKickPlayer_CustomMode;
					}
					break;
				}
				if (flag2 && NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.SessionIsPrivate)
				{
					NetworkSystem.Instance.ReturnToSinglePlayer();
				}
				else if (flag)
				{
					GameMode.ReportHit();
				}
				CustomMapManager.ReturnToVirtualStump();
			}
		}

		// Token: 0x06004662 RID: 18018 RVA: 0x0014E9D4 File Offset: 0x0014CBD4
		private void ShowCountdownText()
		{
			if (this.countdownText.IsNull())
			{
				return;
			}
			int num = 1 + Mathf.FloorToInt(this.GetCurrentHoldDuration());
			this.countdownText.text = num.ToString();
			this.countdownText.gameObject.SetActive(true);
			if (this.buttonText.IsNotNull())
			{
				this.buttonText.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004663 RID: 18019 RVA: 0x0014EA40 File Offset: 0x0014CC40
		private void HideCountdownText()
		{
			if (this.countdownText.IsNull())
			{
				return;
			}
			this.countdownText.text = "";
			this.countdownText.gameObject.SetActive(false);
			if (this.buttonText.IsNotNull())
			{
				this.buttonText.gameObject.SetActive(true);
			}
		}

		// Token: 0x06004664 RID: 18020 RVA: 0x0014EA9A File Offset: 0x0014CC9A
		private IEnumerator UpdateCountdownText()
		{
			while (this.currentlyBeingPressed)
			{
				if (this.countdownText.IsNull())
				{
					yield break;
				}
				float num = this.GetCurrentHoldDuration() - (Time.time - this.startPressingButtonTime);
				int num2 = 1 + Mathf.FloorToInt(num);
				this.countdownText.text = num2.ToString();
				yield return null;
			}
			yield break;
		}

		// Token: 0x040048F2 RID: 18674
		[SerializeField]
		private HeldButton returnButton;

		// Token: 0x040048F3 RID: 18675
		[SerializeField]
		private TMP_Text buttonText;

		// Token: 0x040048F4 RID: 18676
		[SerializeField]
		private TMP_Text countdownText;

		// Token: 0x040048F5 RID: 18677
		private static VirtualStumpReturnWatchProps currentCustomMapProps;

		// Token: 0x040048F6 RID: 18678
		private float startPressingButtonTime = -1f;

		// Token: 0x040048F7 RID: 18679
		private bool currentlyBeingPressed;

		// Token: 0x040048F8 RID: 18680
		private Coroutine updateCountdownCoroutine;
	}
}
