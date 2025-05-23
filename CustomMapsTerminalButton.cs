using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000735 RID: 1845
public class CustomMapsTerminalButton : GorillaPressableButton
{
	// Token: 0x06002E24 RID: 11812 RVA: 0x000E6A60 File Offset: 0x000E4C60
	public static string BindingToString(CustomMapsTerminalButton.ModIOKeyboardBindings binding)
	{
		if (binding < CustomMapsTerminalButton.ModIOKeyboardBindings.up || (binding > CustomMapsTerminalButton.ModIOKeyboardBindings.option3 && binding < CustomMapsTerminalButton.ModIOKeyboardBindings.at))
		{
			if (binding >= CustomMapsTerminalButton.ModIOKeyboardBindings.up)
			{
				return binding.ToString();
			}
			int num = (int)binding;
			return num.ToString();
		}
		else
		{
			switch (binding)
			{
			case CustomMapsTerminalButton.ModIOKeyboardBindings.at:
				return "@";
			case CustomMapsTerminalButton.ModIOKeyboardBindings.dash:
				return "-";
			case CustomMapsTerminalButton.ModIOKeyboardBindings.period:
				return ".";
			case CustomMapsTerminalButton.ModIOKeyboardBindings.underscore:
				return "_";
			case CustomMapsTerminalButton.ModIOKeyboardBindings.plus:
				return "+";
			case CustomMapsTerminalButton.ModIOKeyboardBindings.space:
				return " ";
			default:
				return "";
			}
		}
	}

	// Token: 0x06002E25 RID: 11813 RVA: 0x000E6AE5 File Offset: 0x000E4CE5
	public override void Start()
	{
		base.Start();
		this.ResetButtonColor();
	}

	// Token: 0x06002E26 RID: 11814 RVA: 0x000E6AF4 File Offset: 0x000E4CF4
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		GameEvents.OnModIOKeyboardButtonPressedEvent.Invoke(this.modIOBinding);
		base.StartCoroutine(this.PressButtonColorUpdate());
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, isLeftHand, 0.1f);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { 66, isLeftHand, 0.1f });
		}
	}

	// Token: 0x06002E27 RID: 11815 RVA: 0x000E6BB3 File Offset: 0x000E4DB3
	private void ResetButtonColor()
	{
		if (this.buttonRenderer != null)
		{
			this.buttonRenderer.material = this.unpressedMaterial;
		}
	}

	// Token: 0x06002E28 RID: 11816 RVA: 0x000E6BD4 File Offset: 0x000E4DD4
	private IEnumerator PressButtonColorUpdate()
	{
		this.isOn = true;
		this.UpdateColor();
		yield return new WaitForSeconds(this.debounceTime);
		this.isOn = false;
		this.UpdateColor();
		yield break;
	}

	// Token: 0x04003490 RID: 13456
	public CustomMapsTerminalButton.ModIOKeyboardBindings modIOBinding;

	// Token: 0x04003491 RID: 13457
	private float pressedTime;

	// Token: 0x02000736 RID: 1846
	public enum ModIOKeyboardBindings
	{
		// Token: 0x04003493 RID: 13459
		zero,
		// Token: 0x04003494 RID: 13460
		one,
		// Token: 0x04003495 RID: 13461
		two,
		// Token: 0x04003496 RID: 13462
		three,
		// Token: 0x04003497 RID: 13463
		four,
		// Token: 0x04003498 RID: 13464
		five,
		// Token: 0x04003499 RID: 13465
		six,
		// Token: 0x0400349A RID: 13466
		seven,
		// Token: 0x0400349B RID: 13467
		eight,
		// Token: 0x0400349C RID: 13468
		nine,
		// Token: 0x0400349D RID: 13469
		up,
		// Token: 0x0400349E RID: 13470
		down,
		// Token: 0x0400349F RID: 13471
		delete,
		// Token: 0x040034A0 RID: 13472
		enter,
		// Token: 0x040034A1 RID: 13473
		option1,
		// Token: 0x040034A2 RID: 13474
		option2,
		// Token: 0x040034A3 RID: 13475
		option3,
		// Token: 0x040034A4 RID: 13476
		A,
		// Token: 0x040034A5 RID: 13477
		B,
		// Token: 0x040034A6 RID: 13478
		C,
		// Token: 0x040034A7 RID: 13479
		D,
		// Token: 0x040034A8 RID: 13480
		E,
		// Token: 0x040034A9 RID: 13481
		F,
		// Token: 0x040034AA RID: 13482
		G,
		// Token: 0x040034AB RID: 13483
		H,
		// Token: 0x040034AC RID: 13484
		I,
		// Token: 0x040034AD RID: 13485
		J,
		// Token: 0x040034AE RID: 13486
		K,
		// Token: 0x040034AF RID: 13487
		L,
		// Token: 0x040034B0 RID: 13488
		M,
		// Token: 0x040034B1 RID: 13489
		N,
		// Token: 0x040034B2 RID: 13490
		O,
		// Token: 0x040034B3 RID: 13491
		P,
		// Token: 0x040034B4 RID: 13492
		Q,
		// Token: 0x040034B5 RID: 13493
		R,
		// Token: 0x040034B6 RID: 13494
		S,
		// Token: 0x040034B7 RID: 13495
		T,
		// Token: 0x040034B8 RID: 13496
		U,
		// Token: 0x040034B9 RID: 13497
		V,
		// Token: 0x040034BA RID: 13498
		W,
		// Token: 0x040034BB RID: 13499
		X,
		// Token: 0x040034BC RID: 13500
		Y,
		// Token: 0x040034BD RID: 13501
		Z,
		// Token: 0x040034BE RID: 13502
		at,
		// Token: 0x040034BF RID: 13503
		dash,
		// Token: 0x040034C0 RID: 13504
		period,
		// Token: 0x040034C1 RID: 13505
		underscore,
		// Token: 0x040034C2 RID: 13506
		plus,
		// Token: 0x040034C3 RID: 13507
		space,
		// Token: 0x040034C4 RID: 13508
		goback,
		// Token: 0x040034C5 RID: 13509
		left,
		// Token: 0x040034C6 RID: 13510
		right,
		// Token: 0x040034C7 RID: 13511
		option4,
		// Token: 0x040034C8 RID: 13512
		sort,
		// Token: 0x040034C9 RID: 13513
		sub,
		// Token: 0x040034CA RID: 13514
		map
	}
}
