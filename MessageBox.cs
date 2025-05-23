using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020007E9 RID: 2025
public class MessageBox : MonoBehaviour
{
	// Token: 0x1700050D RID: 1293
	// (get) Token: 0x060031B9 RID: 12729 RVA: 0x000F5BD3 File Offset: 0x000F3DD3
	// (set) Token: 0x060031BA RID: 12730 RVA: 0x000F5BDB File Offset: 0x000F3DDB
	public MessageBoxResult Result { get; private set; }

	// Token: 0x1700050E RID: 1294
	// (get) Token: 0x060031BB RID: 12731 RVA: 0x000F5BE4 File Offset: 0x000F3DE4
	// (set) Token: 0x060031BC RID: 12732 RVA: 0x000F5BF1 File Offset: 0x000F3DF1
	public string Header
	{
		get
		{
			return this._headerText.text;
		}
		set
		{
			this._headerText.text = value;
			this._headerText.gameObject.SetActive(!string.IsNullOrEmpty(value));
		}
	}

	// Token: 0x1700050F RID: 1295
	// (get) Token: 0x060031BD RID: 12733 RVA: 0x000F5C18 File Offset: 0x000F3E18
	// (set) Token: 0x060031BE RID: 12734 RVA: 0x000F5C25 File Offset: 0x000F3E25
	public string Body
	{
		get
		{
			return this._bodyText.text;
		}
		set
		{
			this._bodyText.text = value;
		}
	}

	// Token: 0x17000510 RID: 1296
	// (get) Token: 0x060031BF RID: 12735 RVA: 0x000F5C33 File Offset: 0x000F3E33
	// (set) Token: 0x060031C0 RID: 12736 RVA: 0x000F5C40 File Offset: 0x000F3E40
	public string LeftButton
	{
		get
		{
			return this._leftButtonText.text;
		}
		set
		{
			this._leftButtonText.text = value;
			this._leftButton.SetActive(!string.IsNullOrEmpty(value));
			if (string.IsNullOrEmpty(value))
			{
				RectTransform component = this._rightButton.GetComponent<RectTransform>();
				component.anchorMin = new Vector2(0.5f, 0.5f);
				component.anchorMax = new Vector2(0.5f, 0.5f);
				component.pivot = new Vector2(0.5f, 0.5f);
				component.anchoredPosition = Vector3.zero;
				return;
			}
			RectTransform component2 = this._rightButton.GetComponent<RectTransform>();
			component2.anchorMin = new Vector2(1f, 0.5f);
			component2.anchorMax = new Vector2(1f, 0.5f);
			component2.pivot = new Vector2(1f, 0.5f);
			component2.anchoredPosition = Vector3.zero;
		}
	}

	// Token: 0x17000511 RID: 1297
	// (get) Token: 0x060031C1 RID: 12737 RVA: 0x000F5D28 File Offset: 0x000F3F28
	// (set) Token: 0x060031C2 RID: 12738 RVA: 0x000F5D38 File Offset: 0x000F3F38
	public string RightButton
	{
		get
		{
			return this._rightButtonText.text;
		}
		set
		{
			this._rightButtonText.text = value;
			this._rightButton.SetActive(!string.IsNullOrEmpty(value));
			if (string.IsNullOrEmpty(value))
			{
				RectTransform component = this._leftButton.GetComponent<RectTransform>();
				component.anchorMin = new Vector2(0.5f, 0.5f);
				component.anchorMax = new Vector2(0.5f, 0.5f);
				component.pivot = new Vector2(0.5f, 0.5f);
				component.anchoredPosition3D = Vector3.zero;
				return;
			}
			RectTransform component2 = this._leftButton.GetComponent<RectTransform>();
			component2.anchorMin = new Vector2(0f, 0.5f);
			component2.anchorMax = new Vector2(0f, 0.5f);
			component2.pivot = new Vector2(0f, 0.5f);
			component2.anchoredPosition3D = Vector3.zero;
		}
	}

	// Token: 0x060031C3 RID: 12739 RVA: 0x000F5E16 File Offset: 0x000F4016
	private void Start()
	{
		this.Result = MessageBoxResult.None;
	}

	// Token: 0x060031C4 RID: 12740 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Update()
	{
	}

	// Token: 0x060031C5 RID: 12741 RVA: 0x000F5E1F File Offset: 0x000F401F
	public void OnClickLeftButton()
	{
		this.Result = MessageBoxResult.Left;
		this._leftButtonCallback.Invoke();
	}

	// Token: 0x060031C6 RID: 12742 RVA: 0x000F5E33 File Offset: 0x000F4033
	public void OnClickRightButton()
	{
		this.Result = MessageBoxResult.Right;
		this._rightButtonCallback.Invoke();
	}

	// Token: 0x060031C7 RID: 12743 RVA: 0x000F5E47 File Offset: 0x000F4047
	public GameObject GetCanvas()
	{
		return base.GetComponentInChildren<Canvas>(true).gameObject;
	}

	// Token: 0x04003878 RID: 14456
	[SerializeField]
	private TMP_Text _headerText;

	// Token: 0x04003879 RID: 14457
	[SerializeField]
	private TMP_Text _bodyText;

	// Token: 0x0400387A RID: 14458
	[SerializeField]
	private TMP_Text _leftButtonText;

	// Token: 0x0400387B RID: 14459
	[SerializeField]
	private TMP_Text _rightButtonText;

	// Token: 0x0400387C RID: 14460
	[SerializeField]
	private GameObject _leftButton;

	// Token: 0x0400387D RID: 14461
	[SerializeField]
	private GameObject _rightButton;

	// Token: 0x0400387F RID: 14463
	[SerializeField]
	private UnityEvent _leftButtonCallback = new UnityEvent();

	// Token: 0x04003880 RID: 14464
	[SerializeField]
	private UnityEvent _rightButtonCallback = new UnityEvent();
}
