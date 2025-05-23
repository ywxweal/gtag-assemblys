using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000199 RID: 409
public class RadioButtonGroupWearable : MonoBehaviour, ISpawnable
{
	// Token: 0x170000FD RID: 253
	// (get) Token: 0x06000A16 RID: 2582 RVA: 0x000352E5 File Offset: 0x000334E5
	// (set) Token: 0x06000A17 RID: 2583 RVA: 0x000352ED File Offset: 0x000334ED
	public bool IsSpawned { get; set; }

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x06000A18 RID: 2584 RVA: 0x000352F6 File Offset: 0x000334F6
	// (set) Token: 0x06000A19 RID: 2585 RVA: 0x000352FE File Offset: 0x000334FE
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06000A1A RID: 2586 RVA: 0x00035308 File Offset: 0x00033508
	private void Start()
	{
		this.stateBitsWriteInfo = VRRig.WearablePackedStatesBitWriteInfos[(int)this.assignedSlot];
		if (!this.ownerRig.isLocal)
		{
			GorillaPressableButton[] array = this.buttons;
			for (int i = 0; i < array.Length; i++)
			{
				Collider component = array[i].GetComponent<Collider>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
		}
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x00035366 File Offset: 0x00033566
	private void OnEnable()
	{
		this.SharedRefreshState();
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x0003536E File Offset: 0x0003356E
	private int GetCurrentState()
	{
		return GTBitOps.ReadBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo.index, this.stateBitsWriteInfo.valueMask);
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x00035396 File Offset: 0x00033596
	private void Update()
	{
		if (this.ownerRig.isLocal)
		{
			return;
		}
		if (this.lastReportedState != this.GetCurrentState())
		{
			this.SharedRefreshState();
		}
	}

	// Token: 0x06000A1E RID: 2590 RVA: 0x000353BC File Offset: 0x000335BC
	public void SharedRefreshState()
	{
		int currentState = this.GetCurrentState();
		int num = (this.AllowSelectNone ? (currentState - 1) : currentState);
		for (int i = 0; i < this.buttons.Length; i++)
		{
			this.buttons[i].isOn = num == i;
			this.buttons[i].UpdateColor();
		}
		if (this.lastReportedState != currentState)
		{
			this.lastReportedState = currentState;
			this.OnSelectionChanged.Invoke(currentState);
		}
	}

	// Token: 0x06000A1F RID: 2591 RVA: 0x0003542C File Offset: 0x0003362C
	public void OnPress(GorillaPressableButton button)
	{
		int currentState = this.GetCurrentState();
		int num = Array.IndexOf<GorillaPressableButton>(this.buttons, button);
		if (this.AllowSelectNone)
		{
			num++;
		}
		int num2 = num;
		if (this.AllowSelectNone && num == currentState)
		{
			num2 = 0;
		}
		this.ownerRig.WearablePackedStates = GTBitOps.WriteBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo, num2);
		this.SharedRefreshState();
	}

	// Token: 0x06000A20 RID: 2592 RVA: 0x00035491 File Offset: 0x00033691
	public void OnSpawn(VRRig rig)
	{
		this.ownerRig = rig;
	}

	// Token: 0x06000A21 RID: 2593 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnDespawn()
	{
	}

	// Token: 0x04000C32 RID: 3122
	[SerializeField]
	private bool AllowSelectNone = true;

	// Token: 0x04000C33 RID: 3123
	[SerializeField]
	private GorillaPressableButton[] buttons;

	// Token: 0x04000C34 RID: 3124
	[SerializeField]
	private UnityEvent<int> OnSelectionChanged;

	// Token: 0x04000C35 RID: 3125
	[Tooltip("This is to determine what bit to change in VRRig.WearablesPackedStates.")]
	[SerializeField]
	private VRRig.WearablePackedStateSlots assignedSlot = VRRig.WearablePackedStateSlots.Pants1;

	// Token: 0x04000C36 RID: 3126
	private int lastReportedState;

	// Token: 0x04000C37 RID: 3127
	private VRRig ownerRig;

	// Token: 0x04000C38 RID: 3128
	private GTBitOps.BitWriteInfo stateBitsWriteInfo;
}
