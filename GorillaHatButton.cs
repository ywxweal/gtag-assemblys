using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200060E RID: 1550
[Obsolete("This class is obsolete and will be removed in a future version. (MattO 2024-02-26) It doesn't appear to be used anywhere.")]
public class GorillaHatButton : MonoBehaviour
{
	// Token: 0x06002685 RID: 9861 RVA: 0x000BE5EC File Offset: 0x000BC7EC
	public void Update()
	{
		if (this.testPress)
		{
			this.testPress = false;
			if (this.touchTime + this.debounceTime < Time.time)
			{
				this.touchTime = Time.time;
				this.isOn = !this.isOn;
				this.buttonParent.PressButton(this.isOn, this.buttonType, this.cosmeticName);
			}
		}
	}

	// Token: 0x06002686 RID: 9862 RVA: 0x000BE654 File Offset: 0x000BC854
	private void OnTriggerEnter(Collider collider)
	{
		if (this.touchTime + this.debounceTime < Time.time && collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			this.touchTime = Time.time;
			GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
			this.isOn = !this.isOn;
			this.buttonParent.PressButton(this.isOn, this.buttonType, this.cosmeticName);
			if (component != null)
			{
				GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			}
		}
	}

	// Token: 0x06002687 RID: 9863 RVA: 0x000BE6F4 File Offset: 0x000BC8F4
	public void UpdateColor()
	{
		if (this.isOn)
		{
			base.GetComponent<MeshRenderer>().material = this.onMaterial;
			this.myText.text = this.onText;
			return;
		}
		base.GetComponent<MeshRenderer>().material = this.offMaterial;
		this.myText.text = this.offText;
	}

	// Token: 0x04002AE3 RID: 10979
	public GorillaHatButtonParent buttonParent;

	// Token: 0x04002AE4 RID: 10980
	public GorillaHatButton.HatButtonType buttonType;

	// Token: 0x04002AE5 RID: 10981
	public bool isOn;

	// Token: 0x04002AE6 RID: 10982
	public Material offMaterial;

	// Token: 0x04002AE7 RID: 10983
	public Material onMaterial;

	// Token: 0x04002AE8 RID: 10984
	public string offText;

	// Token: 0x04002AE9 RID: 10985
	public string onText;

	// Token: 0x04002AEA RID: 10986
	public Text myText;

	// Token: 0x04002AEB RID: 10987
	public float debounceTime = 0.25f;

	// Token: 0x04002AEC RID: 10988
	public float touchTime;

	// Token: 0x04002AED RID: 10989
	public string cosmeticName;

	// Token: 0x04002AEE RID: 10990
	public bool testPress;

	// Token: 0x0200060F RID: 1551
	public enum HatButtonType
	{
		// Token: 0x04002AF0 RID: 10992
		Hat,
		// Token: 0x04002AF1 RID: 10993
		Face,
		// Token: 0x04002AF2 RID: 10994
		Badge
	}
}
