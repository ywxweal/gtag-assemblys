using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000735 RID: 1845
public class CustomMapsTerminalButton : GorillaPressableButton
{
	// Token: 0x06002E23 RID: 11811 RVA: 0x000E69BC File Offset: 0x000E4BBC
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

	// Token: 0x06002E24 RID: 11812 RVA: 0x000E6A41 File Offset: 0x000E4C41
	public override void Start()
	{
		base.Start();
		this.ResetButtonColor();
	}

	// Token: 0x06002E25 RID: 11813 RVA: 0x000E6A50 File Offset: 0x000E4C50
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

	// Token: 0x06002E26 RID: 11814 RVA: 0x000E6B0F File Offset: 0x000E4D0F
	private void ResetButtonColor()
	{
		if (this.buttonRenderer != null)
		{
			this.buttonRenderer.material = this.unpressedMaterial;
		}
	}

	// Token: 0x06002E27 RID: 11815 RVA: 0x000E6B30 File Offset: 0x000E4D30
	private IEnumerator PressButtonColorUpdate()
	{
		this.isOn = true;
		this.UpdateColor();
		yield return new WaitForSeconds(this.debounceTime);
		this.isOn = false;
		this.UpdateColor();
		yield break;
	}

	// Token: 0x0400348E RID: 13454
	public CustomMapsTerminalButton.ModIOKeyboardBindings modIOBinding;

	// Token: 0x0400348F RID: 13455
	private float pressedTime;

	// Token: 0x02000736 RID: 1846
	public enum ModIOKeyboardBindings
	{
		// Token: 0x04003491 RID: 13457
		zero,
		// Token: 0x04003492 RID: 13458
		one,
		// Token: 0x04003493 RID: 13459
		two,
		// Token: 0x04003494 RID: 13460
		three,
		// Token: 0x04003495 RID: 13461
		four,
		// Token: 0x04003496 RID: 13462
		five,
		// Token: 0x04003497 RID: 13463
		six,
		// Token: 0x04003498 RID: 13464
		seven,
		// Token: 0x04003499 RID: 13465
		eight,
		// Token: 0x0400349A RID: 13466
		nine,
		// Token: 0x0400349B RID: 13467
		up,
		// Token: 0x0400349C RID: 13468
		down,
		// Token: 0x0400349D RID: 13469
		delete,
		// Token: 0x0400349E RID: 13470
		enter,
		// Token: 0x0400349F RID: 13471
		option1,
		// Token: 0x040034A0 RID: 13472
		option2,
		// Token: 0x040034A1 RID: 13473
		option3,
		// Token: 0x040034A2 RID: 13474
		A,
		// Token: 0x040034A3 RID: 13475
		B,
		// Token: 0x040034A4 RID: 13476
		C,
		// Token: 0x040034A5 RID: 13477
		D,
		// Token: 0x040034A6 RID: 13478
		E,
		// Token: 0x040034A7 RID: 13479
		F,
		// Token: 0x040034A8 RID: 13480
		G,
		// Token: 0x040034A9 RID: 13481
		H,
		// Token: 0x040034AA RID: 13482
		I,
		// Token: 0x040034AB RID: 13483
		J,
		// Token: 0x040034AC RID: 13484
		K,
		// Token: 0x040034AD RID: 13485
		L,
		// Token: 0x040034AE RID: 13486
		M,
		// Token: 0x040034AF RID: 13487
		N,
		// Token: 0x040034B0 RID: 13488
		O,
		// Token: 0x040034B1 RID: 13489
		P,
		// Token: 0x040034B2 RID: 13490
		Q,
		// Token: 0x040034B3 RID: 13491
		R,
		// Token: 0x040034B4 RID: 13492
		S,
		// Token: 0x040034B5 RID: 13493
		T,
		// Token: 0x040034B6 RID: 13494
		U,
		// Token: 0x040034B7 RID: 13495
		V,
		// Token: 0x040034B8 RID: 13496
		W,
		// Token: 0x040034B9 RID: 13497
		X,
		// Token: 0x040034BA RID: 13498
		Y,
		// Token: 0x040034BB RID: 13499
		Z,
		// Token: 0x040034BC RID: 13500
		at,
		// Token: 0x040034BD RID: 13501
		dash,
		// Token: 0x040034BE RID: 13502
		period,
		// Token: 0x040034BF RID: 13503
		underscore,
		// Token: 0x040034C0 RID: 13504
		plus,
		// Token: 0x040034C1 RID: 13505
		space,
		// Token: 0x040034C2 RID: 13506
		goback,
		// Token: 0x040034C3 RID: 13507
		left,
		// Token: 0x040034C4 RID: 13508
		right,
		// Token: 0x040034C5 RID: 13509
		option4,
		// Token: 0x040034C6 RID: 13510
		sort,
		// Token: 0x040034C7 RID: 13511
		sub,
		// Token: 0x040034C8 RID: 13512
		map
	}
}
