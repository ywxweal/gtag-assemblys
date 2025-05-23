using System;
using Photon.Pun;

// Token: 0x0200073A RID: 1850
public class CustomMapsTerminalToggleButton : CustomMapsTerminalButton
{
	// Token: 0x06002E35 RID: 11829 RVA: 0x000E6C2C File Offset: 0x000E4E2C
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, isLeftHand, 0.1f);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { 66, isLeftHand, 0.1f });
		}
		if (CustomMapsTerminal.IsDriver)
		{
			GameEvents.OnModIOKeyboardButtonPressedEvent.Invoke(this.modIOBinding);
		}
	}

	// Token: 0x06002E36 RID: 11830 RVA: 0x000E6CE5 File Offset: 0x000E4EE5
	public void SetButtonStatus(bool isPressed)
	{
		this.isOn = isPressed;
		this.UpdateColor();
	}
}
