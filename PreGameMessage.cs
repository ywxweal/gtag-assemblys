using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x020007EF RID: 2031
public class PreGameMessage : MonoBehaviour
{
	// Token: 0x060031DC RID: 12764 RVA: 0x000F6896 File Offset: 0x000F4A96
	private void Awake()
	{
		this.controllerBehaviour = base.GetComponentInChildren<ControllerBehaviour>(true);
	}

	// Token: 0x060031DD RID: 12765 RVA: 0x000F68A5 File Offset: 0x000F4AA5
	private void OnEnable()
	{
		this.controllerBehaviour.OnAction += this.PostUpdate;
	}

	// Token: 0x060031DE RID: 12766 RVA: 0x000F68BE File Offset: 0x000F4ABE
	private void OnDisable()
	{
		this.controllerBehaviour.OnAction -= this.PostUpdate;
	}

	// Token: 0x060031DF RID: 12767 RVA: 0x000F68D8 File Offset: 0x000F4AD8
	public void ShowMessage(string messageTitle, string messageBody, string messageConfirmation, Action onConfirmationAction, float bodyFontSize = 0.5f, float buttonHideTimer = 0f)
	{
		this._alternativeAction = null;
		this._multiButtonRoot.SetActive(false);
		this._messageTitleTxt.text = messageTitle;
		this._messageBodyTxt.text = messageBody;
		this._messageConfirmationTxt.text = messageConfirmation;
		this._confirmationAction = onConfirmationAction;
		this._messageBodyTxt.fontSize = bodyFontSize;
		this._hasCompleted = false;
		if (this._confirmationAction == null)
		{
			this._confirmButtonRoot.SetActive(false);
		}
		else if (!string.IsNullOrEmpty(this._messageConfirmationTxt.text))
		{
			this._confirmButtonRoot.SetActive(true);
		}
		PrivateUIRoom.AddUI(this._uiParent.transform);
	}

	// Token: 0x060031E0 RID: 12768 RVA: 0x000F697C File Offset: 0x000F4B7C
	public void ShowMessage(string messageTitle, string messageBody, string messageConfirmationButton, string messageAlternativeButton, Action onConfirmationAction, Action onAlternativeAction, float bodyFontSize = 0.5f)
	{
		this._confirmButtonRoot.SetActive(false);
		this._messageTitleTxt.text = messageTitle;
		this._messageBodyTxt.text = messageBody;
		this._messageAlternativeConfirmationTxt.text = messageConfirmationButton;
		this._messageAlternativeButtonTxt.text = messageAlternativeButton;
		this._confirmationAction = onConfirmationAction;
		this._alternativeAction = onAlternativeAction;
		this._messageBodyTxt.fontSize = bodyFontSize;
		this._hasCompleted = false;
		if (this._confirmationAction == null || this._alternativeAction == null)
		{
			Debug.LogError("[KID] Trying to show a mesasge with multiple buttons, but one or both callbacks are null");
			this._multiButtonRoot.SetActive(false);
		}
		else if (!string.IsNullOrEmpty(this._messageAlternativeConfirmationTxt.text) && !string.IsNullOrEmpty(this._messageAlternativeButtonTxt.text))
		{
			this._multiButtonRoot.SetActive(true);
		}
		PrivateUIRoom.AddUI(this._uiParent.transform);
	}

	// Token: 0x060031E1 RID: 12769 RVA: 0x000F6A54 File Offset: 0x000F4C54
	public async Task ShowMessageWithAwait(string messageTitle, string messageBody, string messageConfirmation, Action onConfirmationAction, float bodyFontSize = 0.5f, float buttonHideTimer = 0f)
	{
		this._alternativeAction = null;
		this._multiButtonRoot.SetActive(false);
		this._messageTitleTxt.text = messageTitle;
		this._messageBodyTxt.text = messageBody;
		this._messageConfirmationTxt.text = messageConfirmation;
		this._confirmationAction = onConfirmationAction;
		this._messageBodyTxt.fontSize = bodyFontSize;
		this._hasCompleted = false;
		if (this._confirmationAction == null)
		{
			this._confirmButtonRoot.SetActive(false);
		}
		else if (!string.IsNullOrEmpty(this._messageConfirmationTxt.text))
		{
			this._confirmButtonRoot.SetActive(true);
		}
		PrivateUIRoom.AddUI(this._uiParent.transform);
		await this.WaitForCompletion();
	}

	// Token: 0x060031E2 RID: 12770 RVA: 0x000F6AC4 File Offset: 0x000F4CC4
	public void UpdateMessage(string newMessageBody, string newConfirmButton)
	{
		this._messageBodyTxt.text = newMessageBody;
		this._messageConfirmationTxt.text = newConfirmButton;
		if (string.IsNullOrEmpty(this._messageConfirmationTxt.text))
		{
			this._confirmButtonRoot.SetActive(false);
			return;
		}
		if (this._confirmationAction != null)
		{
			this._confirmButtonRoot.SetActive(true);
		}
	}

	// Token: 0x060031E3 RID: 12771 RVA: 0x000F6B1C File Offset: 0x000F4D1C
	public void CloseMessage()
	{
		PrivateUIRoom.RemoveUI(this._uiParent.transform);
	}

	// Token: 0x060031E4 RID: 12772 RVA: 0x000F6B30 File Offset: 0x000F4D30
	private async Task WaitForCompletion()
	{
		do
		{
			await Task.Yield();
		}
		while (!this._hasCompleted);
	}

	// Token: 0x060031E5 RID: 12773 RVA: 0x000F6B74 File Offset: 0x000F4D74
	private void PostUpdate()
	{
		bool isLeftStick = this.controllerBehaviour.IsLeftStick;
		bool isRightStick = this.controllerBehaviour.IsRightStick;
		bool buttonDown = this.controllerBehaviour.ButtonDown;
		if (this._multiButtonRoot.activeInHierarchy)
		{
			if (isLeftStick)
			{
				this.progress += Time.deltaTime / this.holdTime;
				this.progressBarL.transform.localScale = new Vector3(0f, 1f, 1f);
				this.progressBarR.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
				this.progressBarR.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
				if (this.progress >= 1f)
				{
					this.OnConfirmedPressed();
					return;
				}
			}
			else if (isRightStick)
			{
				this.progress += Time.deltaTime / this.holdTime;
				this.progressBarR.transform.localScale = new Vector3(0f, 1f, 1f);
				this.progressBarL.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
				this.progressBarL.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
				if (this.progress >= 1f)
				{
					this.OnAlternativePressed();
					return;
				}
			}
			else
			{
				this.progress = 0f;
				this.progressBarR.transform.localScale = new Vector3(0f, 1f, 1f);
				this.progressBarL.transform.localScale = new Vector3(0f, 1f, 1f);
				this.progressBarL.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
			}
			return;
		}
		if (this._confirmButtonRoot.activeInHierarchy)
		{
			if (buttonDown)
			{
				this.progress += Time.deltaTime / this.holdTime;
				this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
				this.progressBar.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
				if (this.progress >= 1f)
				{
					this.OnConfirmedPressed();
					return;
				}
			}
			else
			{
				this.progress = 0f;
				this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
				this.progressBar.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
			}
			return;
		}
	}

	// Token: 0x060031E6 RID: 12774 RVA: 0x000F6E4E File Offset: 0x000F504E
	private void OnConfirmedPressed()
	{
		PrivateUIRoom.RemoveUI(this._uiParent.transform);
		this._hasCompleted = true;
		Action confirmationAction = this._confirmationAction;
		if (confirmationAction == null)
		{
			return;
		}
		confirmationAction();
	}

	// Token: 0x060031E7 RID: 12775 RVA: 0x000F6E77 File Offset: 0x000F5077
	private void OnAlternativePressed()
	{
		PrivateUIRoom.RemoveUI(this._uiParent.transform);
		this._hasCompleted = true;
		Action alternativeAction = this._alternativeAction;
		if (alternativeAction == null)
		{
			return;
		}
		alternativeAction();
	}

	// Token: 0x04003894 RID: 14484
	[SerializeField]
	private GameObject _uiParent;

	// Token: 0x04003895 RID: 14485
	[SerializeField]
	private TMP_Text _messageTitleTxt;

	// Token: 0x04003896 RID: 14486
	[SerializeField]
	private TMP_Text _messageBodyTxt;

	// Token: 0x04003897 RID: 14487
	[SerializeField]
	private GameObject _confirmButtonRoot;

	// Token: 0x04003898 RID: 14488
	[SerializeField]
	private GameObject _multiButtonRoot;

	// Token: 0x04003899 RID: 14489
	[SerializeField]
	private TMP_Text _messageConfirmationTxt;

	// Token: 0x0400389A RID: 14490
	[SerializeField]
	private TMP_Text _messageAlternativeConfirmationTxt;

	// Token: 0x0400389B RID: 14491
	[SerializeField]
	private TMP_Text _messageAlternativeButtonTxt;

	// Token: 0x0400389C RID: 14492
	private Action _confirmationAction;

	// Token: 0x0400389D RID: 14493
	private Action _alternativeAction;

	// Token: 0x0400389E RID: 14494
	private bool _hasCompleted;

	// Token: 0x0400389F RID: 14495
	private float progress;

	// Token: 0x040038A0 RID: 14496
	[SerializeField]
	private float holdTime;

	// Token: 0x040038A1 RID: 14497
	[SerializeField]
	private LineRenderer progressBar;

	// Token: 0x040038A2 RID: 14498
	[SerializeField]
	private LineRenderer progressBarL;

	// Token: 0x040038A3 RID: 14499
	[SerializeField]
	private LineRenderer progressBarR;

	// Token: 0x040038A4 RID: 14500
	private ControllerBehaviour controllerBehaviour;
}
