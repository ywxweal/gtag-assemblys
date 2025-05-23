using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000341 RID: 833
public class PassthroughStyler : MonoBehaviour
{
	// Token: 0x060013A4 RID: 5028 RVA: 0x0005F81C File Offset: 0x0005DA1C
	private void Start()
	{
		GrabObject grabObject;
		if (base.TryGetComponent<GrabObject>(out grabObject))
		{
			GrabObject grabObject2 = grabObject;
			grabObject2.GrabbedObjectDelegate = (GrabObject.GrabbedObject)Delegate.Combine(grabObject2.GrabbedObjectDelegate, new GrabObject.GrabbedObject(this.Grab));
			GrabObject grabObject3 = grabObject;
			grabObject3.ReleasedObjectDelegate = (GrabObject.ReleasedObject)Delegate.Combine(grabObject3.ReleasedObjectDelegate, new GrabObject.ReleasedObject(this.Release));
			GrabObject grabObject4 = grabObject;
			grabObject4.CursorPositionDelegate = (GrabObject.SetCursorPosition)Delegate.Combine(grabObject4.CursorPositionDelegate, new GrabObject.SetCursorPosition(this.Cursor));
		}
		this._savedColor = new Color(1f, 1f, 1f, 0f);
		this.ShowFullMenu(false);
		this._mainCanvas.interactable = false;
		this._passthroughColorLut = new OVRPassthroughColorLut(this._colorLutTexture, true);
		if (!OVRManager.GetPassthroughCapabilities().SupportsColorPassthrough && this._objectsToHideForColorPassthrough != null)
		{
			for (int i = 0; i < this._objectsToHideForColorPassthrough.Length; i++)
			{
				this._objectsToHideForColorPassthrough[i].SetActive(false);
			}
		}
	}

	// Token: 0x060013A5 RID: 5029 RVA: 0x0005F912 File Offset: 0x0005DB12
	private void Update()
	{
		if (this._controllerHand == OVRInput.Controller.None)
		{
			return;
		}
		if (this._settingColor)
		{
			this.GetColorFromWheel();
		}
	}

	// Token: 0x060013A6 RID: 5030 RVA: 0x0005F92B File Offset: 0x0005DB2B
	public void OnBrightnessChanged(float newValue)
	{
		this._savedBrightness = newValue;
		this.UpdateBrighnessContrastSaturation();
	}

	// Token: 0x060013A7 RID: 5031 RVA: 0x0005F93A File Offset: 0x0005DB3A
	public void OnContrastChanged(float newValue)
	{
		this._savedContrast = newValue;
		this.UpdateBrighnessContrastSaturation();
	}

	// Token: 0x060013A8 RID: 5032 RVA: 0x0005F949 File Offset: 0x0005DB49
	public void OnSaturationChanged(float newValue)
	{
		this._savedSaturation = newValue;
		this.UpdateBrighnessContrastSaturation();
	}

	// Token: 0x060013A9 RID: 5033 RVA: 0x0005F958 File Offset: 0x0005DB58
	public void OnAlphaChanged(float newValue)
	{
		this._savedColor = new Color(this._savedColor.r, this._savedColor.g, this._savedColor.b, newValue);
		this._passthroughLayer.edgeColor = this._savedColor;
	}

	// Token: 0x060013AA RID: 5034 RVA: 0x0005F998 File Offset: 0x0005DB98
	public void OnBlendChange(float newValue)
	{
		this._savedBlend = newValue;
		this._passthroughLayer.SetColorLut(this._passthroughColorLut, this._savedBlend);
	}

	// Token: 0x060013AB RID: 5035 RVA: 0x0005F9B8 File Offset: 0x0005DBB8
	public void DoColorDrag(bool doDrag)
	{
		this._settingColor = doDrag;
	}

	// Token: 0x060013AC RID: 5036 RVA: 0x0005F9C1 File Offset: 0x0005DBC1
	public void SetPassthroughStyleToColorAdjustment(bool isOn)
	{
		if (isOn)
		{
			this.SetPassthroughStyle(OVRPassthroughLayer.ColorMapEditorType.ColorAdjustment);
		}
	}

	// Token: 0x060013AD RID: 5037 RVA: 0x0005F9CD File Offset: 0x0005DBCD
	public void SetPassthroughStyleToColorLut(bool isOn)
	{
		if (isOn)
		{
			this.SetPassthroughStyle(OVRPassthroughLayer.ColorMapEditorType.ColorLut);
		}
	}

	// Token: 0x060013AE RID: 5038 RVA: 0x0005F9DC File Offset: 0x0005DBDC
	private void Grab(OVRInput.Controller grabHand)
	{
		this._controllerHand = grabHand;
		this.ShowFullMenu(true);
		if (this._mainCanvas)
		{
			this._mainCanvas.interactable = true;
		}
		if (this._fade != null)
		{
			base.StopCoroutine(this._fade);
		}
		this._fade = this.FadeToCurrentStyle(0.2f);
		base.StartCoroutine(this._fade);
	}

	// Token: 0x060013AF RID: 5039 RVA: 0x0005FA44 File Offset: 0x0005DC44
	private void Release()
	{
		this._controllerHand = OVRInput.Controller.None;
		this.ShowFullMenu(false);
		if (this._mainCanvas)
		{
			this._mainCanvas.interactable = false;
		}
		if (this._fade != null)
		{
			base.StopCoroutine(this._fade);
		}
		this._fade = this.FadeToDefaultPassthrough(0.2f);
		base.StartCoroutine(this._fade);
	}

	// Token: 0x060013B0 RID: 5040 RVA: 0x0005FAAA File Offset: 0x0005DCAA
	private IEnumerator FadeToCurrentStyle(float fadeTime)
	{
		this._passthroughLayer.edgeRenderingEnabled = true;
		yield return this.FadeTo(1f, fadeTime);
		yield break;
	}

	// Token: 0x060013B1 RID: 5041 RVA: 0x0005FAC0 File Offset: 0x0005DCC0
	private IEnumerator FadeToDefaultPassthrough(float fadeTime)
	{
		yield return this.FadeTo(0f, fadeTime);
		this._passthroughLayer.edgeRenderingEnabled = false;
		yield break;
	}

	// Token: 0x060013B2 RID: 5042 RVA: 0x0005FAD6 File Offset: 0x0005DCD6
	private IEnumerator FadeTo(float styleValueMultiplier, float duration)
	{
		float timer = 0f;
		float brightness = this._passthroughLayer.colorMapEditorBrightness;
		float contrast = this._passthroughLayer.colorMapEditorContrast;
		float saturation = this._passthroughLayer.colorMapEditorSaturation;
		Color edgeCol = this._passthroughLayer.edgeColor;
		float blend = this._savedBlend;
		while (timer <= duration)
		{
			timer += Time.deltaTime;
			float num = Mathf.Clamp01(timer / duration);
			if (this._currentStyle == OVRPassthroughLayer.ColorMapEditorType.ColorLut)
			{
				this._passthroughLayer.SetColorLut(this._passthroughColorLut, Mathf.Lerp(blend, this._savedBlend * styleValueMultiplier, num));
			}
			else
			{
				this._passthroughLayer.SetBrightnessContrastSaturation(Mathf.Lerp(brightness, this._savedBrightness * styleValueMultiplier, num), Mathf.Lerp(contrast, this._savedContrast * styleValueMultiplier, num), Mathf.Lerp(saturation, this._savedSaturation * styleValueMultiplier, num));
			}
			this._passthroughLayer.edgeColor = Color.Lerp(edgeCol, new Color(this._savedColor.r, this._savedColor.g, this._savedColor.b, this._savedColor.a * styleValueMultiplier), num);
			yield return null;
		}
		yield break;
	}

	// Token: 0x060013B3 RID: 5043 RVA: 0x0005FAF3 File Offset: 0x0005DCF3
	private void UpdateBrighnessContrastSaturation()
	{
		this._passthroughLayer.SetBrightnessContrastSaturation(this._savedBrightness, this._savedContrast, this._savedSaturation);
	}

	// Token: 0x060013B4 RID: 5044 RVA: 0x0005FB14 File Offset: 0x0005DD14
	private void ShowFullMenu(bool doShow)
	{
		GameObject[] compactObjects = this._compactObjects;
		for (int i = 0; i < compactObjects.Length; i++)
		{
			compactObjects[i].SetActive(doShow);
		}
	}

	// Token: 0x060013B5 RID: 5045 RVA: 0x0005FB3F File Offset: 0x0005DD3F
	private void Cursor(Vector3 cP)
	{
		this._cursorPosition = cP;
	}

	// Token: 0x060013B6 RID: 5046 RVA: 0x0005FB48 File Offset: 0x0005DD48
	private void GetColorFromWheel()
	{
		Vector3 vector = this._colorWheel.transform.InverseTransformPoint(this._cursorPosition);
		Vector2 vector2 = new Vector2(vector.x / this._colorWheel.sizeDelta.x + 0.5f, vector.y / this._colorWheel.sizeDelta.y + 0.5f);
		Debug.Log("Sanctuary: " + vector2.x.ToString() + ", " + vector2.y.ToString());
		Color color = Color.black;
		if ((double)vector2.x < 1.0 && vector2.x > 0f && (double)vector2.y < 1.0 && vector2.y > 0f)
		{
			int num = Mathf.RoundToInt(vector2.x * (float)this._colorTexture.width);
			int num2 = Mathf.RoundToInt(vector2.y * (float)this._colorTexture.height);
			color = this._colorTexture.GetPixel(num, num2);
		}
		this._savedColor = new Color(color.r, color.g, color.b, this._savedColor.a);
		this._passthroughLayer.edgeColor = this._savedColor;
	}

	// Token: 0x060013B7 RID: 5047 RVA: 0x0005FC98 File Offset: 0x0005DE98
	private void SetPassthroughStyle(OVRPassthroughLayer.ColorMapEditorType passthroughStyle)
	{
		this._currentStyle = passthroughStyle;
		if (this._currentStyle == OVRPassthroughLayer.ColorMapEditorType.ColorLut)
		{
			this._passthroughLayer.SetColorLut(this._passthroughColorLut, this._savedBlend);
			return;
		}
		this.UpdateBrighnessContrastSaturation();
	}

	// Token: 0x040015D8 RID: 5592
	private const float FadeDuration = 0.2f;

	// Token: 0x040015D9 RID: 5593
	[SerializeField]
	private OVRInput.Controller _controllerHand;

	// Token: 0x040015DA RID: 5594
	[SerializeField]
	private OVRPassthroughLayer _passthroughLayer;

	// Token: 0x040015DB RID: 5595
	[SerializeField]
	private RectTransform _colorWheel;

	// Token: 0x040015DC RID: 5596
	[SerializeField]
	private Texture2D _colorTexture;

	// Token: 0x040015DD RID: 5597
	[SerializeField]
	private Texture2D _colorLutTexture;

	// Token: 0x040015DE RID: 5598
	[SerializeField]
	private CanvasGroup _mainCanvas;

	// Token: 0x040015DF RID: 5599
	[SerializeField]
	private GameObject[] _compactObjects;

	// Token: 0x040015E0 RID: 5600
	[SerializeField]
	private GameObject[] _objectsToHideForColorPassthrough;

	// Token: 0x040015E1 RID: 5601
	private Vector3 _cursorPosition = Vector3.zero;

	// Token: 0x040015E2 RID: 5602
	private bool _settingColor;

	// Token: 0x040015E3 RID: 5603
	private Color _savedColor = Color.white;

	// Token: 0x040015E4 RID: 5604
	private float _savedBrightness;

	// Token: 0x040015E5 RID: 5605
	private float _savedContrast;

	// Token: 0x040015E6 RID: 5606
	private float _savedSaturation;

	// Token: 0x040015E7 RID: 5607
	private OVRPassthroughLayer.ColorMapEditorType _currentStyle = OVRPassthroughLayer.ColorMapEditorType.ColorAdjustment;

	// Token: 0x040015E8 RID: 5608
	private float _savedBlend = 1f;

	// Token: 0x040015E9 RID: 5609
	private OVRPassthroughColorLut _passthroughColorLut;

	// Token: 0x040015EA RID: 5610
	private IEnumerator _fade;
}
