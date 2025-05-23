using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000985 RID: 2437
public class CompositeTriggerEvents : MonoBehaviour
{
	// Token: 0x170005CD RID: 1485
	// (get) Token: 0x06003A95 RID: 14997 RVA: 0x001187D2 File Offset: 0x001169D2
	private Dictionary<Collider, int> CollderMasks
	{
		get
		{
			return this.overlapMask;
		}
	}

	// Token: 0x1400006E RID: 110
	// (add) Token: 0x06003A96 RID: 14998 RVA: 0x001187DC File Offset: 0x001169DC
	// (remove) Token: 0x06003A97 RID: 14999 RVA: 0x00118814 File Offset: 0x00116A14
	public event CompositeTriggerEvents.TriggerEvent CompositeTriggerEnter;

	// Token: 0x1400006F RID: 111
	// (add) Token: 0x06003A98 RID: 15000 RVA: 0x0011884C File Offset: 0x00116A4C
	// (remove) Token: 0x06003A99 RID: 15001 RVA: 0x00118884 File Offset: 0x00116A84
	public event CompositeTriggerEvents.TriggerEvent CompositeTriggerExit;

	// Token: 0x06003A9A RID: 15002 RVA: 0x001188BC File Offset: 0x00116ABC
	private void Awake()
	{
		if (this.individualTriggerColliders.Count > 32)
		{
			Debug.LogError("The max number of triggers was exceeded in this composite trigger event sender on GameObject: " + base.gameObject.name + ".");
		}
		for (int i = 0; i < this.individualTriggerColliders.Count; i++)
		{
			TriggerEventNotifier triggerEventNotifier = this.individualTriggerColliders[i].gameObject.AddComponent<TriggerEventNotifier>();
			triggerEventNotifier.maskIndex = i;
			triggerEventNotifier.TriggerEnterEvent += this.TriggerEnterReceiver;
			triggerEventNotifier.TriggerExitEvent += this.TriggerExitReceiver;
			this.triggerEventNotifiers.Add(triggerEventNotifier);
		}
	}

	// Token: 0x06003A9B RID: 15003 RVA: 0x0011895C File Offset: 0x00116B5C
	private void OnDestroy()
	{
		for (int i = 0; i < this.triggerEventNotifiers.Count; i++)
		{
			if (this.triggerEventNotifiers[i] != null)
			{
				this.triggerEventNotifiers[i].TriggerEnterEvent -= this.TriggerEnterReceiver;
				this.triggerEventNotifiers[i].TriggerExitEvent -= this.TriggerExitReceiver;
			}
		}
	}

	// Token: 0x06003A9C RID: 15004 RVA: 0x001189D0 File Offset: 0x00116BD0
	public void TriggerEnterReceiver(TriggerEventNotifier notifier, Collider other)
	{
		int num;
		if (this.overlapMask.TryGetValue(other, out num))
		{
			num = this.SetMaskIndexTrue(num, notifier.maskIndex);
			this.overlapMask[other] = num;
			return;
		}
		int num2 = this.SetMaskIndexTrue(0, notifier.maskIndex);
		this.overlapMask.Add(other, num2);
		CompositeTriggerEvents.TriggerEvent compositeTriggerEnter = this.CompositeTriggerEnter;
		if (compositeTriggerEnter == null)
		{
			return;
		}
		compositeTriggerEnter(other);
	}

	// Token: 0x06003A9D RID: 15005 RVA: 0x00118A38 File Offset: 0x00116C38
	public void TriggerExitReceiver(TriggerEventNotifier notifier, Collider other)
	{
		int num;
		if (this.overlapMask.TryGetValue(other, out num))
		{
			num = this.SetMaskIndexFalse(num, notifier.maskIndex);
			if (num == 0)
			{
				this.overlapMask.Remove(other);
				CompositeTriggerEvents.TriggerEvent compositeTriggerExit = this.CompositeTriggerExit;
				if (compositeTriggerExit == null)
				{
					return;
				}
				compositeTriggerExit(other);
				return;
			}
			else
			{
				this.overlapMask[other] = num;
			}
		}
	}

	// Token: 0x06003A9E RID: 15006 RVA: 0x00118A94 File Offset: 0x00116C94
	public void ResetColliderMask(Collider other)
	{
		int num;
		if (this.overlapMask.TryGetValue(other, out num))
		{
			if (num != 0)
			{
				CompositeTriggerEvents.TriggerEvent compositeTriggerExit = this.CompositeTriggerExit;
				if (compositeTriggerExit != null)
				{
					compositeTriggerExit(other);
				}
			}
			this.overlapMask.Remove(other);
		}
	}

	// Token: 0x06003A9F RID: 15007 RVA: 0x00118AD3 File Offset: 0x00116CD3
	public void CompositeTriggerEnterReceiver(Collider other)
	{
		CompositeTriggerEvents.TriggerEvent compositeTriggerEnter = this.CompositeTriggerEnter;
		if (compositeTriggerEnter == null)
		{
			return;
		}
		compositeTriggerEnter(other);
	}

	// Token: 0x06003AA0 RID: 15008 RVA: 0x00118AE6 File Offset: 0x00116CE6
	public void CompositeTriggerExitReceiver(Collider other)
	{
		CompositeTriggerEvents.TriggerEvent compositeTriggerExit = this.CompositeTriggerExit;
		if (compositeTriggerExit == null)
		{
			return;
		}
		compositeTriggerExit(other);
	}

	// Token: 0x06003AA1 RID: 15009 RVA: 0x00118AF9 File Offset: 0x00116CF9
	private bool TestMaskIndex(int mask, int index)
	{
		return (mask & (1 << index)) != 0;
	}

	// Token: 0x06003AA2 RID: 15010 RVA: 0x00118B06 File Offset: 0x00116D06
	private int SetMaskIndexTrue(int mask, int index)
	{
		return mask | (1 << index);
	}

	// Token: 0x06003AA3 RID: 15011 RVA: 0x00118B10 File Offset: 0x00116D10
	private int SetMaskIndexFalse(int mask, int index)
	{
		return mask & ~(1 << index);
	}

	// Token: 0x06003AA4 RID: 15012 RVA: 0x00118B1C File Offset: 0x00116D1C
	private string MaskToString(int mask)
	{
		string text = "";
		for (int i = 31; i >= 0; i--)
		{
			text += (this.TestMaskIndex(mask, i) ? "1" : "0");
		}
		return text;
	}

	// Token: 0x04003F78 RID: 16248
	[SerializeField]
	private List<Collider> individualTriggerColliders = new List<Collider>();

	// Token: 0x04003F79 RID: 16249
	private List<TriggerEventNotifier> triggerEventNotifiers = new List<TriggerEventNotifier>();

	// Token: 0x04003F7A RID: 16250
	private Dictionary<Collider, int> overlapMask = new Dictionary<Collider, int>();

	// Token: 0x02000986 RID: 2438
	// (Invoke) Token: 0x06003AA7 RID: 15015
	public delegate void TriggerEvent(Collider collider);
}
