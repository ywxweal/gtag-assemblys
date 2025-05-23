using System;
using UnityEngine;

// Token: 0x020007BF RID: 1983
public class KIDAgeGateConfirmation : MonoBehaviour
{
	// Token: 0x170004F4 RID: 1268
	// (get) Token: 0x060030F5 RID: 12533 RVA: 0x000F0DFA File Offset: 0x000EEFFA
	// (set) Token: 0x060030F6 RID: 12534 RVA: 0x000F0E02 File Offset: 0x000EF002
	public KidAgeConfirmationResult Result { get; private set; }

	// Token: 0x060030F7 RID: 12535 RVA: 0x000F0E0B File Offset: 0x000EF00B
	private void Start()
	{
		this.Result = KidAgeConfirmationResult.None;
	}

	// Token: 0x060030F8 RID: 12536 RVA: 0x000F0E14 File Offset: 0x000EF014
	public void OnConfirm()
	{
		this.Result = KidAgeConfirmationResult.Confirm;
	}

	// Token: 0x060030F9 RID: 12537 RVA: 0x000F0E1D File Offset: 0x000EF01D
	public void OnBack()
	{
		this.Result = KidAgeConfirmationResult.Back;
	}

	// Token: 0x060030FA RID: 12538 RVA: 0x000F0E0B File Offset: 0x000EF00B
	public void Reset()
	{
		this.Result = KidAgeConfirmationResult.None;
	}
}
