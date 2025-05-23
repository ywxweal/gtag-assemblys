using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002D6 RID: 726
public class DebugUIBuilder : MonoBehaviour
{
	// Token: 0x06001175 RID: 4469 RVA: 0x00054008 File Offset: 0x00052208
	public void Awake()
	{
		DebugUIBuilder.instance = this;
		this.menuOffset = base.transform.position;
		base.gameObject.SetActive(false);
		this.rig = Object.FindObjectOfType<OVRCameraRig>();
		for (int i = 0; i < this.toEnable.Count; i++)
		{
			this.toEnable[i].SetActive(false);
		}
		this.insertPositions = new Vector2[this.targetContentPanels.Length];
		for (int j = 0; j < this.insertPositions.Length; j++)
		{
			this.insertPositions[j].x = this.marginH;
			this.insertPositions[j].y = -this.marginV;
		}
		this.insertedElements = new List<RectTransform>[this.targetContentPanels.Length];
		for (int k = 0; k < this.insertedElements.Length; k++)
		{
			this.insertedElements[k] = new List<RectTransform>();
		}
		if (this.uiHelpersToInstantiate)
		{
			Object.Instantiate<GameObject>(this.uiHelpersToInstantiate);
		}
		this.lp = Object.FindObjectOfType<LaserPointer>();
		if (!this.lp)
		{
			Debug.LogError("Debug UI requires use of a LaserPointer and will not function without it. Add one to your scene, or assign the UIHelpers prefab to the DebugUIBuilder in the inspector.");
			return;
		}
		this.lp.laserBeamBehavior = this.laserBeamBehavior;
		if (!this.toEnable.Contains(this.lp.gameObject))
		{
			this.toEnable.Add(this.lp.gameObject);
		}
		base.GetComponent<OVRRaycaster>().pointer = this.lp.gameObject;
		this.lp.gameObject.SetActive(false);
	}

	// Token: 0x06001176 RID: 4470 RVA: 0x00054198 File Offset: 0x00052398
	public void Show()
	{
		this.Relayout();
		base.gameObject.SetActive(true);
		base.transform.position = this.rig.transform.TransformPoint(this.menuOffset);
		Vector3 eulerAngles = this.rig.transform.rotation.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		base.transform.eulerAngles = eulerAngles;
		if (this.reEnable == null || this.reEnable.Length < this.toDisable.Count)
		{
			this.reEnable = new bool[this.toDisable.Count];
		}
		this.reEnable.Initialize();
		int num = this.toDisable.Count;
		for (int i = 0; i < num; i++)
		{
			if (this.toDisable[i])
			{
				this.reEnable[i] = this.toDisable[i].activeSelf;
				this.toDisable[i].SetActive(false);
			}
		}
		num = this.toEnable.Count;
		for (int j = 0; j < num; j++)
		{
			this.toEnable[j].SetActive(true);
		}
		int num2 = this.targetContentPanels.Length;
		for (int k = 0; k < num2; k++)
		{
			this.targetContentPanels[k].gameObject.SetActive(this.insertedElements[k].Count > 0);
		}
	}

	// Token: 0x06001177 RID: 4471 RVA: 0x00054320 File Offset: 0x00052520
	public void Hide()
	{
		base.gameObject.SetActive(false);
		for (int i = 0; i < this.reEnable.Length; i++)
		{
			if (this.toDisable[i] && this.reEnable[i])
			{
				this.toDisable[i].SetActive(true);
			}
		}
		int count = this.toEnable.Count;
		for (int j = 0; j < count; j++)
		{
			this.toEnable[j].SetActive(false);
		}
	}

	// Token: 0x06001178 RID: 4472 RVA: 0x000543A8 File Offset: 0x000525A8
	private void StackedRelayout()
	{
		for (int i = 0; i < this.targetContentPanels.Length; i++)
		{
			RectTransform component = this.targetContentPanels[i].GetComponent<RectTransform>();
			List<RectTransform> list = this.insertedElements[i];
			int count = list.Count;
			float num = this.marginH;
			float num2 = -this.marginV;
			float num3 = 0f;
			for (int j = 0; j < count; j++)
			{
				RectTransform rectTransform = list[j];
				rectTransform.anchoredPosition = new Vector2(num, num2);
				if (this.isHorizontal)
				{
					num += rectTransform.rect.width + this.elementSpacing;
				}
				else
				{
					num2 -= rectTransform.rect.height + this.elementSpacing;
				}
				num3 = Mathf.Max(rectTransform.rect.width + 2f * this.marginH, num3);
			}
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num3);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -num2 + this.marginV);
		}
	}

	// Token: 0x06001179 RID: 4473 RVA: 0x000544B8 File Offset: 0x000526B8
	private void PanelCentricRelayout()
	{
		if (!this.isHorizontal)
		{
			Debug.Log("Error:Panel Centeric relayout is implemented only for horizontal panels");
			return;
		}
		for (int i = 0; i < this.targetContentPanels.Length; i++)
		{
			RectTransform component = this.targetContentPanels[i].GetComponent<RectTransform>();
			List<RectTransform> list = this.insertedElements[i];
			int count = list.Count;
			float num = this.marginH;
			float num2 = -this.marginV;
			float num3 = num;
			for (int j = 0; j < count; j++)
			{
				RectTransform rectTransform = list[j];
				num3 += rectTransform.rect.width + this.elementSpacing;
			}
			num3 -= this.elementSpacing;
			num3 += this.marginH;
			float num4 = num3;
			num = -0.5f * num4;
			num2 = -this.marginV;
			for (int k = 0; k < count; k++)
			{
				RectTransform rectTransform2 = list[k];
				if (k == 0)
				{
					num += this.marginH;
				}
				num += 0.5f * rectTransform2.rect.width;
				rectTransform2.anchoredPosition = new Vector2(num, num2);
				num += rectTransform2.rect.width * 0.5f + this.elementSpacing;
				num3 = Mathf.Max(rectTransform2.rect.width + 2f * this.marginH, num3);
			}
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num3);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -num2 + this.marginV);
		}
	}

	// Token: 0x0600117A RID: 4474 RVA: 0x00054643 File Offset: 0x00052843
	private void Relayout()
	{
		if (this.usePanelCentricRelayout)
		{
			this.PanelCentricRelayout();
			return;
		}
		this.StackedRelayout();
	}

	// Token: 0x0600117B RID: 4475 RVA: 0x0005465C File Offset: 0x0005285C
	private void AddRect(RectTransform r, int targetCanvas)
	{
		if (targetCanvas > this.targetContentPanels.Length)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Attempted to add debug panel to canvas ",
				targetCanvas.ToString(),
				", but only ",
				this.targetContentPanels.Length.ToString(),
				" panels were provided. Fix in the inspector or pass a lower value for target canvas."
			}));
			return;
		}
		r.transform.SetParent(this.targetContentPanels[targetCanvas], false);
		this.insertedElements[targetCanvas].Add(r);
		if (base.gameObject.activeInHierarchy)
		{
			this.Relayout();
		}
	}

	// Token: 0x0600117C RID: 4476 RVA: 0x000546F0 File Offset: 0x000528F0
	public RectTransform AddButton(string label, DebugUIBuilder.OnClick handler = null, int buttonIndex = -1, int targetCanvas = 0, bool highResolutionText = false)
	{
		RectTransform rectTransform;
		if (buttonIndex == -1)
		{
			rectTransform = Object.Instantiate<RectTransform>(this.buttonPrefab).GetComponent<RectTransform>();
		}
		else
		{
			rectTransform = Object.Instantiate<RectTransform>(this.additionalButtonPrefab[buttonIndex]).GetComponent<RectTransform>();
		}
		Button componentInChildren = rectTransform.GetComponentInChildren<Button>();
		if (handler != null)
		{
			componentInChildren.onClick.AddListener(delegate
			{
				handler();
			});
		}
		if (highResolutionText)
		{
			((TextMeshProUGUI)rectTransform.GetComponentsInChildren(typeof(TextMeshProUGUI), true)[0]).text = label;
		}
		else
		{
			((Text)rectTransform.GetComponentsInChildren(typeof(Text), true)[0]).text = label;
		}
		this.AddRect(rectTransform, targetCanvas);
		return rectTransform;
	}

	// Token: 0x0600117D RID: 4477 RVA: 0x000547A8 File Offset: 0x000529A8
	public RectTransform AddLabel(string label, int targetCanvas = 0)
	{
		RectTransform component = Object.Instantiate<RectTransform>(this.labelPrefab).GetComponent<RectTransform>();
		component.GetComponent<Text>().text = label;
		this.AddRect(component, targetCanvas);
		return component;
	}

	// Token: 0x0600117E RID: 4478 RVA: 0x000547DC File Offset: 0x000529DC
	public RectTransform AddSlider(string label, float min, float max, DebugUIBuilder.OnSlider onValueChanged, bool wholeNumbersOnly = false, int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate<RectTransform>(this.sliderPrefab);
		Slider componentInChildren = rectTransform.GetComponentInChildren<Slider>();
		componentInChildren.minValue = min;
		componentInChildren.maxValue = max;
		componentInChildren.onValueChanged.AddListener(delegate(float f)
		{
			onValueChanged(f);
		});
		componentInChildren.wholeNumbers = wholeNumbersOnly;
		this.AddRect(rectTransform, targetCanvas);
		return rectTransform;
	}

	// Token: 0x0600117F RID: 4479 RVA: 0x00054840 File Offset: 0x00052A40
	public RectTransform AddDivider(int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate<RectTransform>(this.dividerPrefab);
		this.AddRect(rectTransform, targetCanvas);
		return rectTransform;
	}

	// Token: 0x06001180 RID: 4480 RVA: 0x00054864 File Offset: 0x00052A64
	public RectTransform AddToggle(string label, DebugUIBuilder.OnToggleValueChange onValueChanged, int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate<RectTransform>(this.togglePrefab);
		this.AddRect(rectTransform, targetCanvas);
		rectTransform.GetComponentInChildren<Text>().text = label;
		Toggle t = rectTransform.GetComponentInChildren<Toggle>();
		t.onValueChanged.AddListener(delegate
		{
			onValueChanged(t);
		});
		return rectTransform;
	}

	// Token: 0x06001181 RID: 4481 RVA: 0x000548C8 File Offset: 0x00052AC8
	public RectTransform AddToggle(string label, DebugUIBuilder.OnToggleValueChange onValueChanged, bool defaultValue, int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate<RectTransform>(this.togglePrefab);
		this.AddRect(rectTransform, targetCanvas);
		rectTransform.GetComponentInChildren<Text>().text = label;
		Toggle t = rectTransform.GetComponentInChildren<Toggle>();
		t.isOn = defaultValue;
		t.onValueChanged.AddListener(delegate
		{
			onValueChanged(t);
		});
		return rectTransform;
	}

	// Token: 0x06001182 RID: 4482 RVA: 0x00054938 File Offset: 0x00052B38
	public RectTransform AddRadio(string label, string group, DebugUIBuilder.OnToggleValueChange handler, int targetCanvas = 0)
	{
		RectTransform rectTransform = Object.Instantiate<RectTransform>(this.radioPrefab);
		this.AddRect(rectTransform, targetCanvas);
		rectTransform.GetComponentInChildren<Text>().text = label;
		Toggle tb = rectTransform.GetComponentInChildren<Toggle>();
		if (group == null)
		{
			group = "default";
		}
		bool flag = false;
		ToggleGroup toggleGroup;
		if (!this.radioGroups.ContainsKey(group))
		{
			toggleGroup = tb.gameObject.AddComponent<ToggleGroup>();
			this.radioGroups[group] = toggleGroup;
			flag = true;
		}
		else
		{
			toggleGroup = this.radioGroups[group];
		}
		tb.group = toggleGroup;
		tb.isOn = flag;
		tb.onValueChanged.AddListener(delegate
		{
			handler(tb);
		});
		return rectTransform;
	}

	// Token: 0x06001183 RID: 4483 RVA: 0x00054A00 File Offset: 0x00052C00
	public RectTransform AddTextField(string label, int targetCanvas = 0)
	{
		RectTransform component = Object.Instantiate<RectTransform>(this.textPrefab).GetComponent<RectTransform>();
		component.GetComponentInChildren<InputField>().text = label;
		this.AddRect(component, targetCanvas);
		return component;
	}

	// Token: 0x06001184 RID: 4484 RVA: 0x00054A33 File Offset: 0x00052C33
	public void ToggleLaserPointer(bool isOn)
	{
		if (this.lp)
		{
			if (isOn)
			{
				this.lp.enabled = true;
				return;
			}
			this.lp.enabled = false;
		}
	}

	// Token: 0x040013AC RID: 5036
	public const int DEBUG_PANE_CENTER = 0;

	// Token: 0x040013AD RID: 5037
	public const int DEBUG_PANE_RIGHT = 1;

	// Token: 0x040013AE RID: 5038
	public const int DEBUG_PANE_LEFT = 2;

	// Token: 0x040013AF RID: 5039
	[SerializeField]
	private RectTransform buttonPrefab;

	// Token: 0x040013B0 RID: 5040
	[SerializeField]
	private RectTransform[] additionalButtonPrefab;

	// Token: 0x040013B1 RID: 5041
	[SerializeField]
	private RectTransform labelPrefab;

	// Token: 0x040013B2 RID: 5042
	[SerializeField]
	private RectTransform sliderPrefab;

	// Token: 0x040013B3 RID: 5043
	[SerializeField]
	private RectTransform dividerPrefab;

	// Token: 0x040013B4 RID: 5044
	[SerializeField]
	private RectTransform togglePrefab;

	// Token: 0x040013B5 RID: 5045
	[SerializeField]
	private RectTransform radioPrefab;

	// Token: 0x040013B6 RID: 5046
	[SerializeField]
	private RectTransform textPrefab;

	// Token: 0x040013B7 RID: 5047
	[SerializeField]
	private GameObject uiHelpersToInstantiate;

	// Token: 0x040013B8 RID: 5048
	[SerializeField]
	private Transform[] targetContentPanels;

	// Token: 0x040013B9 RID: 5049
	private bool[] reEnable;

	// Token: 0x040013BA RID: 5050
	[SerializeField]
	private List<GameObject> toEnable;

	// Token: 0x040013BB RID: 5051
	[SerializeField]
	private List<GameObject> toDisable;

	// Token: 0x040013BC RID: 5052
	public static DebugUIBuilder instance;

	// Token: 0x040013BD RID: 5053
	public float elementSpacing = 16f;

	// Token: 0x040013BE RID: 5054
	public float marginH = 16f;

	// Token: 0x040013BF RID: 5055
	public float marginV = 16f;

	// Token: 0x040013C0 RID: 5056
	private Vector2[] insertPositions;

	// Token: 0x040013C1 RID: 5057
	private List<RectTransform>[] insertedElements;

	// Token: 0x040013C2 RID: 5058
	private Vector3 menuOffset;

	// Token: 0x040013C3 RID: 5059
	private OVRCameraRig rig;

	// Token: 0x040013C4 RID: 5060
	private Dictionary<string, ToggleGroup> radioGroups = new Dictionary<string, ToggleGroup>();

	// Token: 0x040013C5 RID: 5061
	private LaserPointer lp;

	// Token: 0x040013C6 RID: 5062
	private LineRenderer lr;

	// Token: 0x040013C7 RID: 5063
	public LaserPointer.LaserBeamBehavior laserBeamBehavior;

	// Token: 0x040013C8 RID: 5064
	public bool isHorizontal;

	// Token: 0x040013C9 RID: 5065
	public bool usePanelCentricRelayout;

	// Token: 0x020002D7 RID: 727
	// (Invoke) Token: 0x06001187 RID: 4487
	public delegate void OnClick();

	// Token: 0x020002D8 RID: 728
	// (Invoke) Token: 0x0600118B RID: 4491
	public delegate void OnToggleValueChange(Toggle t);

	// Token: 0x020002D9 RID: 729
	// (Invoke) Token: 0x0600118F RID: 4495
	public delegate void OnSlider(float f);

	// Token: 0x020002DA RID: 730
	// (Invoke) Token: 0x06001193 RID: 4499
	public delegate bool ActiveUpdate();
}
