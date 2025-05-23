using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200008C RID: 140
public class GameModeSelectorButtonLayout : MonoBehaviour
{
	// Token: 0x06000396 RID: 918 RVA: 0x0001640B File Offset: 0x0001460B
	private void OnEnable()
	{
		this.SetupButtons();
		NetworkSystem.Instance.OnJoinedRoomEvent += this.SetupButtons;
	}

	// Token: 0x06000397 RID: 919 RVA: 0x00016429 File Offset: 0x00014629
	private void OnDisable()
	{
		NetworkSystem.Instance.OnJoinedRoomEvent -= this.SetupButtons;
	}

	// Token: 0x06000398 RID: 920 RVA: 0x00016444 File Offset: 0x00014644
	private async void SetupButtons()
	{
		int count = 0;
		while (GorillaComputer.instance == null)
		{
			await Task.Delay(100);
		}
		bool flag = GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone != this.zone;
		foreach (GameModeType gameModeType in GameMode.GameModeZoneMapping.GetModesForZone(this.zone, NetworkSystem.Instance.SessionIsPrivate))
		{
			if (count == this.currentButtons.Count)
			{
				this.currentButtons.Add(Object.Instantiate<ModeSelectButton>(this.pf_button, base.transform));
			}
			ModeSelectButton modeSelectButton = this.currentButtons[count];
			modeSelectButton.transform.localPosition = new Vector3((float)count * -0.15f, 0f, 0f);
			modeSelectButton.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
			modeSelectButton.WarningScreen = this.warningScreen;
			modeSelectButton.SetInfo(gameModeType.ToString(), GameMode.GameModeZoneMapping.GetModeName(gameModeType), GameMode.GameModeZoneMapping.IsNew(gameModeType), GameMode.GameModeZoneMapping.GetCountdown(gameModeType));
			modeSelectButton.gameObject.SetActive(true);
			count++;
			flag |= GorillaComputer.instance.currentGameMode.Value.ToUpper() == gameModeType.ToString().ToUpper();
		}
		for (int i = count; i < this.currentButtons.Count; i++)
		{
			this.currentButtons[i].gameObject.SetActive(false);
		}
		if (!flag)
		{
			GorillaComputer.instance.SetGameModeWithoutButton(this.currentButtons[0].gameMode);
		}
	}

	// Token: 0x04000413 RID: 1043
	[SerializeField]
	private ModeSelectButton pf_button;

	// Token: 0x04000414 RID: 1044
	[SerializeField]
	private GTZone zone;

	// Token: 0x04000415 RID: 1045
	[SerializeField]
	private PartyGameModeWarning warningScreen;

	// Token: 0x04000416 RID: 1046
	private List<ModeSelectButton> currentButtons = new List<ModeSelectButton>();
}
