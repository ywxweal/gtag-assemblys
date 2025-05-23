using System;
using TMPro;
using UnityEngine;

// Token: 0x0200047B RID: 1147
public class GorillaDebugUI : MonoBehaviour
{
	// Token: 0x04001F44 RID: 8004
	private readonly float Delay = 0.5f;

	// Token: 0x04001F45 RID: 8005
	public GameObject parentCanvas;

	// Token: 0x04001F46 RID: 8006
	public GameObject rayInteractorLeft;

	// Token: 0x04001F47 RID: 8007
	public GameObject rayInteractorRight;

	// Token: 0x04001F48 RID: 8008
	[SerializeField]
	private TMP_Dropdown playfabIdDropdown;

	// Token: 0x04001F49 RID: 8009
	[SerializeField]
	private TMP_Dropdown roomIdDropdown;

	// Token: 0x04001F4A RID: 8010
	[SerializeField]
	private TMP_Dropdown locationDropdown;

	// Token: 0x04001F4B RID: 8011
	[SerializeField]
	private TMP_Dropdown playerNameDropdown;

	// Token: 0x04001F4C RID: 8012
	[SerializeField]
	private TMP_Dropdown gameModeDropdown;

	// Token: 0x04001F4D RID: 8013
	[SerializeField]
	private TMP_Dropdown timeOfDayDropdown;

	// Token: 0x04001F4E RID: 8014
	[SerializeField]
	private TMP_Text networkStateTextBox;

	// Token: 0x04001F4F RID: 8015
	[SerializeField]
	private TMP_Text gameModeTextBox;

	// Token: 0x04001F50 RID: 8016
	[SerializeField]
	private TMP_Text currentRoomTextBox;

	// Token: 0x04001F51 RID: 8017
	[SerializeField]
	private TMP_Text playerCountTextBox;

	// Token: 0x04001F52 RID: 8018
	[SerializeField]
	private TMP_Text roomVisibilityTextBox;

	// Token: 0x04001F53 RID: 8019
	[SerializeField]
	private TMP_Text timeMultiplierTextBox;

	// Token: 0x04001F54 RID: 8020
	[SerializeField]
	private TMP_Text versionTextBox;
}
