using System;
using GorillaGameModes;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000604 RID: 1540
public class GorillaGuardianEjectWatch : MonoBehaviour
{
	// Token: 0x0600261D RID: 9757 RVA: 0x000BC9D8 File Offset: 0x000BABD8
	private void Start()
	{
		if (this.ejectButton != null)
		{
			this.ejectButton.onPressButton.AddListener(new UnityAction(this.OnEjectButtonPressed));
		}
	}

	// Token: 0x0600261E RID: 9758 RVA: 0x000BCA04 File Offset: 0x000BAC04
	private void OnDestroy()
	{
		if (this.ejectButton != null)
		{
			this.ejectButton.onPressButton.RemoveListener(new UnityAction(this.OnEjectButtonPressed));
		}
	}

	// Token: 0x0600261F RID: 9759 RVA: 0x000BCA30 File Offset: 0x000BAC30
	private void OnEjectButtonPressed()
	{
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null)
		{
			gorillaGuardianManager.RequestEjectGuardian(NetworkSystem.Instance.LocalPlayer);
		}
	}

	// Token: 0x04002A9E RID: 10910
	[SerializeField]
	private HeldButton ejectButton;
}
