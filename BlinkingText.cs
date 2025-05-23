using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000C8 RID: 200
public class BlinkingText : MonoBehaviour
{
	// Token: 0x060004EF RID: 1263 RVA: 0x0001C8B0 File Offset: 0x0001AAB0
	private void Awake()
	{
		this.textComponent = base.GetComponent<Text>();
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x0001C8C0 File Offset: 0x0001AAC0
	private void Update()
	{
		if (this.isOn && Time.time > this.lastTime + this.cycleTime * this.dutyCycle)
		{
			this.isOn = false;
			this.textComponent.enabled = false;
			return;
		}
		if (!this.isOn && Time.time > this.lastTime + this.cycleTime)
		{
			this.lastTime = Time.time;
			this.isOn = true;
			this.textComponent.enabled = true;
		}
	}

	// Token: 0x040005D1 RID: 1489
	public float cycleTime;

	// Token: 0x040005D2 RID: 1490
	public float dutyCycle;

	// Token: 0x040005D3 RID: 1491
	private bool isOn;

	// Token: 0x040005D4 RID: 1492
	private float lastTime;

	// Token: 0x040005D5 RID: 1493
	private Text textComponent;
}
