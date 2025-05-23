using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000557 RID: 1367
public class FortuneTellerButton : GorillaPressableButton
{
	// Token: 0x06002119 RID: 8473 RVA: 0x000A633D File Offset: 0x000A453D
	public void Awake()
	{
		this.startingPos = base.transform.localPosition;
	}

	// Token: 0x0600211A RID: 8474 RVA: 0x000A6350 File Offset: 0x000A4550
	public override void ButtonActivation()
	{
		this.PressButtonUpdate();
	}

	// Token: 0x0600211B RID: 8475 RVA: 0x000A6358 File Offset: 0x000A4558
	public void PressButtonUpdate()
	{
		if (this.pressTime != 0f)
		{
			return;
		}
		base.transform.localPosition = this.startingPos + this.pressedOffset;
		this.buttonRenderer.material = this.pressedMaterial;
		this.pressTime = Time.time;
		base.StartCoroutine(this.<PressButtonUpdate>g__ButtonColorUpdate_Local|6_0());
	}

	// Token: 0x0600211D RID: 8477 RVA: 0x000A63E5 File Offset: 0x000A45E5
	[CompilerGenerated]
	private IEnumerator <PressButtonUpdate>g__ButtonColorUpdate_Local|6_0()
	{
		yield return new WaitForSeconds(this.durationPressed);
		if (this.pressTime != 0f && Time.time > this.durationPressed + this.pressTime)
		{
			base.transform.localPosition = this.startingPos;
			this.buttonRenderer.material = this.unpressedMaterial;
			this.pressTime = 0f;
		}
		yield break;
	}

	// Token: 0x0400254B RID: 9547
	[SerializeField]
	private float durationPressed = 0.25f;

	// Token: 0x0400254C RID: 9548
	[SerializeField]
	private Vector3 pressedOffset = new Vector3(0f, 0f, 0.1f);

	// Token: 0x0400254D RID: 9549
	private float pressTime;

	// Token: 0x0400254E RID: 9550
	private Vector3 startingPos;
}
