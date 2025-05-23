using System;
using UnityEngine;

// Token: 0x0200008E RID: 142
[CreateAssetMenu(fileName = "New Game Mode Selector Button Layout Data", menuName = "Game Settings/Game Mode Selector Button Layout Data", order = 1)]
public class GameModeSelectorButtonLayoutData : ScriptableObject
{
	// Token: 0x1700003A RID: 58
	// (get) Token: 0x0600039C RID: 924 RVA: 0x0001676E File Offset: 0x0001496E
	public ModeSelectButtonInfoData[] Info
	{
		get
		{
			return this.info;
		}
	}

	// Token: 0x0400041C RID: 1052
	[SerializeField]
	private ModeSelectButtonInfoData[] info;
}
