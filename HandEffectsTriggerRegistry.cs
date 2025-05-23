using System;
using System.Collections.Generic;
using TagEffects;
using UnityEngine;

// Token: 0x02000252 RID: 594
[DefaultExecutionOrder(10000)]
public class HandEffectsTriggerRegistry : MonoBehaviour
{
	// Token: 0x17000151 RID: 337
	// (get) Token: 0x06000D7D RID: 3453 RVA: 0x000461DB File Offset: 0x000443DB
	// (set) Token: 0x06000D7E RID: 3454 RVA: 0x000461E2 File Offset: 0x000443E2
	public static HandEffectsTriggerRegistry Instance { get; private set; }

	// Token: 0x17000152 RID: 338
	// (get) Token: 0x06000D7F RID: 3455 RVA: 0x000461EA File Offset: 0x000443EA
	// (set) Token: 0x06000D80 RID: 3456 RVA: 0x000461F1 File Offset: 0x000443F1
	public static bool HasInstance { get; private set; }

	// Token: 0x06000D81 RID: 3457 RVA: 0x000461F9 File Offset: 0x000443F9
	public static void FindInstance()
	{
		HandEffectsTriggerRegistry.Instance = Object.FindAnyObjectByType<HandEffectsTriggerRegistry>();
		HandEffectsTriggerRegistry.HasInstance = true;
	}

	// Token: 0x06000D82 RID: 3458 RVA: 0x0004620B File Offset: 0x0004440B
	private void Awake()
	{
		HandEffectsTriggerRegistry.Instance = this;
		HandEffectsTriggerRegistry.HasInstance = true;
	}

	// Token: 0x06000D83 RID: 3459 RVA: 0x00046219 File Offset: 0x00044419
	public void Register(IHandEffectsTrigger trigger)
	{
		if (this.triggers.Count < 30)
		{
			this.triggers.Add(trigger);
		}
	}

	// Token: 0x06000D84 RID: 3460 RVA: 0x00046236 File Offset: 0x00044436
	public void Unregister(IHandEffectsTrigger trigger)
	{
		this.triggers.Remove(trigger);
	}

	// Token: 0x06000D85 RID: 3461 RVA: 0x00046248 File Offset: 0x00044448
	private void Update()
	{
		int num = 0;
		for (int i = 0; i < this.triggers.Count; i++)
		{
			IHandEffectsTrigger handEffectsTrigger = this.triggers[i];
			int num2 = i * 30;
			for (int j = 0; j < this.triggers.Count; j++)
			{
				if (i != j && Time.time - this.triggerTimes[i] > 0.5f && Time.time - this.triggerTimes[j] > 0.5f)
				{
					IHandEffectsTrigger handEffectsTrigger2 = this.triggers[j];
					if (handEffectsTrigger.InTriggerZone(handEffectsTrigger2))
					{
						int num3 = 1 << num2 + j;
						num |= num3;
						if ((this.existingCollisionBits & num3) == 0)
						{
							handEffectsTrigger.OnTriggerEntered(handEffectsTrigger2);
							handEffectsTrigger2.OnTriggerEntered(handEffectsTrigger);
							this.triggerTimes[i] = (this.triggerTimes[j] = Time.time);
						}
					}
				}
			}
		}
		this.existingCollisionBits = num;
	}

	// Token: 0x04001112 RID: 4370
	private const int MAX_TRIGGERS = 30;

	// Token: 0x04001113 RID: 4371
	private const float COOLDOWN_TIME = 0.5f;

	// Token: 0x04001114 RID: 4372
	private List<IHandEffectsTrigger> triggers = new List<IHandEffectsTrigger>();

	// Token: 0x04001115 RID: 4373
	private float[] triggerTimes = new float[30];

	// Token: 0x04001116 RID: 4374
	private int existingCollisionBits;
}
