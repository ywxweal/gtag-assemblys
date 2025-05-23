using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000321 RID: 801
public class DebugUISample : MonoBehaviour
{
	// Token: 0x060012FD RID: 4861 RVA: 0x0005A2C0 File Offset: 0x000584C0
	private void Start()
	{
		DebugUIBuilder.instance.AddButton("Button Pressed", new DebugUIBuilder.OnClick(this.LogButtonPressed), -1, 0, false);
		DebugUIBuilder.instance.AddLabel("Label", 0);
		RectTransform rectTransform = DebugUIBuilder.instance.AddSlider("Slider", 1f, 10f, new DebugUIBuilder.OnSlider(this.SliderPressed), true, 0);
		Text[] componentsInChildren = rectTransform.GetComponentsInChildren<Text>();
		this.sliderText = componentsInChildren[1];
		this.sliderText.text = rectTransform.GetComponentInChildren<Slider>().value.ToString();
		DebugUIBuilder.instance.AddDivider(0);
		DebugUIBuilder.instance.AddToggle("Toggle", new DebugUIBuilder.OnToggleValueChange(this.TogglePressed), 0);
		DebugUIBuilder.instance.AddRadio("Radio1", "group", delegate(Toggle t)
		{
			this.RadioPressed("Radio1", "group", t);
		}, 0);
		DebugUIBuilder.instance.AddRadio("Radio2", "group", delegate(Toggle t)
		{
			this.RadioPressed("Radio2", "group", t);
		}, 0);
		DebugUIBuilder.instance.AddLabel("Secondary Tab", 1);
		DebugUIBuilder.instance.AddDivider(1);
		DebugUIBuilder.instance.AddRadio("Side Radio 1", "group2", delegate(Toggle t)
		{
			this.RadioPressed("Side Radio 1", "group2", t);
		}, 1);
		DebugUIBuilder.instance.AddRadio("Side Radio 2", "group2", delegate(Toggle t)
		{
			this.RadioPressed("Side Radio 2", "group2", t);
		}, 1);
		DebugUIBuilder.instance.Show();
		this.inMenu = true;
	}

	// Token: 0x060012FE RID: 4862 RVA: 0x0005A434 File Offset: 0x00058634
	public void TogglePressed(Toggle t)
	{
		Debug.Log("Toggle pressed. Is on? " + t.isOn.ToString());
	}

	// Token: 0x060012FF RID: 4863 RVA: 0x0005A460 File Offset: 0x00058660
	public void RadioPressed(string radioLabel, string group, Toggle t)
	{
		Debug.Log(string.Concat(new string[]
		{
			"Radio value changed: ",
			radioLabel,
			", from group ",
			group,
			". New value: ",
			t.isOn.ToString()
		}));
	}

	// Token: 0x06001300 RID: 4864 RVA: 0x0005A4AE File Offset: 0x000586AE
	public void SliderPressed(float f)
	{
		Debug.Log("Slider: " + f.ToString());
		this.sliderText.text = f.ToString();
	}

	// Token: 0x06001301 RID: 4865 RVA: 0x0005A4D8 File Offset: 0x000586D8
	private void Update()
	{
		if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.Active) || OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.Active))
		{
			if (this.inMenu)
			{
				DebugUIBuilder.instance.Hide();
			}
			else
			{
				DebugUIBuilder.instance.Show();
			}
			this.inMenu = !this.inMenu;
		}
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x0005A530 File Offset: 0x00058730
	private void LogButtonPressed()
	{
		Debug.Log("Button pressed");
	}

	// Token: 0x04001529 RID: 5417
	private bool inMenu;

	// Token: 0x0400152A RID: 5418
	private Text sliderText;
}
