using System;
using UnityEngine;

// Token: 0x020007BF RID: 1983
public class KIDAgeGateConfirmation : MonoBehaviour
{
	// Token: 0x170004F4 RID: 1268
	// (get) Token: 0x060030F6 RID: 12534 RVA: 0x000F0E9E File Offset: 0x000EF09E
	// (set) Token: 0x060030F7 RID: 12535 RVA: 0x000F0EA6 File Offset: 0x000EF0A6
	public KidAgeConfirmationResult Result { get; private set; }

	// Token: 0x060030F8 RID: 12536 RVA: 0x000F0EAF File Offset: 0x000EF0AF
	private void Start()
	{
		this.Result = KidAgeConfirmationResult.None;
	}

	// Token: 0x060030F9 RID: 12537 RVA: 0x000F0EB8 File Offset: 0x000EF0B8
	public void OnConfirm()
	{
		this.Result = KidAgeConfirmationResult.Confirm;
	}

	// Token: 0x060030FA RID: 12538 RVA: 0x000F0EC1 File Offset: 0x000EF0C1
	public void OnBack()
	{
		this.Result = KidAgeConfirmationResult.Back;
	}

	// Token: 0x060030FB RID: 12539 RVA: 0x000F0EAF File Offset: 0x000EF0AF
	public void Reset()
	{
		this.Result = KidAgeConfirmationResult.None;
	}
}
