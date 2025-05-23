using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x02000835 RID: 2101
public class WarningScreens : MonoBehaviour
{
	// Token: 0x06003361 RID: 13153 RVA: 0x000FD531 File Offset: 0x000FB731
	private void Awake()
	{
		if (WarningScreens._activeReference == null)
		{
			WarningScreens._activeReference = this;
			return;
		}
		Debug.LogError("[WARNINGS] WarningScreens already exists. Destroying this instance.");
		Object.Destroy(this);
	}

	// Token: 0x06003362 RID: 13154 RVA: 0x000FD558 File Offset: 0x000FB758
	private async Task<WarningButtonResult> StartWarningScreenInternal(CancellationToken cancellationToken)
	{
		WarningScreens._closedMessageBox = false;
		WarningScreens._result = WarningButtonResult.CloseWarning;
		PlayerAgeGateWarningStatus? playerAgeGateWarningStatus = await WarningsServer.Instance.FetchPlayerData(cancellationToken);
		WarningButtonResult warningButtonResult;
		if (cancellationToken.IsCancellationRequested || playerAgeGateWarningStatus == null)
		{
			warningButtonResult = WarningButtonResult.None;
		}
		else
		{
			PlayerAgeGateWarningStatus value = playerAgeGateWarningStatus.Value;
			if (value.header.IsNullOrEmpty() || value.body.IsNullOrEmpty())
			{
				Debug.Log("[WARNINGS] Not showing warning screen.");
				warningButtonResult = WarningButtonResult.None;
			}
			else
			{
				this._messageBox.Header = value.header;
				this._messageBox.Body = value.body;
				this._messageBox.LeftButton = value.leftButtonText;
				this._messageBox.RightButton = value.rightButtonText;
				WarningScreens._leftButtonResult = value.leftButtonResult;
				WarningScreens._rightButtonResult = value.rightButtonResult;
				this._onLeftButtonPressedAction = value.onLeftButtonPressedAction;
				this._onRightButtonPressedAction = value.onRightButtonPressedAction;
				if (this._imageContainer && this._withImageText && this._noImageText)
				{
					this._imageContainer.SetActive(value.showImage);
					this._withImageText.text = value.body;
					this._withImageText.gameObject.SetActive(value.showImage);
					this._noImageText.gameObject.SetActive(!value.showImage);
				}
				this._messageBox.gameObject.SetActive(true);
				GameObject canvas = this._messageBox.GetCanvas();
				PrivateUIRoom.AddUI(canvas.transform);
				HandRayController.Instance.EnableHandRays();
				await WarningScreens.WaitForResponse(cancellationToken);
				HandRayController.Instance.DisableHandRays();
				PrivateUIRoom.RemoveUI(canvas.transform);
				this._messageBox.gameObject.SetActive(false);
				warningButtonResult = WarningScreens._result;
			}
		}
		return warningButtonResult;
	}

	// Token: 0x06003363 RID: 13155 RVA: 0x000FD5A4 File Offset: 0x000FB7A4
	private async Task<WarningButtonResult> StartOptInFollowUpScreenInternal(CancellationToken cancellationToken)
	{
		WarningScreens._closedMessageBox = false;
		WarningScreens._result = WarningButtonResult.CloseWarning;
		PlayerAgeGateWarningStatus? playerAgeGateWarningStatus = await WarningsServer.Instance.GetOptInFollowUpMessage(cancellationToken);
		WarningButtonResult warningButtonResult;
		if (cancellationToken.IsCancellationRequested || playerAgeGateWarningStatus == null)
		{
			warningButtonResult = WarningButtonResult.None;
		}
		else
		{
			Debug.Log("[KID::WARNING_SCREEN] Body: " + playerAgeGateWarningStatus.Value.body);
			this._messageBox.Header = playerAgeGateWarningStatus.Value.header;
			this._messageBox.Body = playerAgeGateWarningStatus.Value.body;
			this._messageBox.LeftButton = playerAgeGateWarningStatus.Value.leftButtonText;
			this._messageBox.RightButton = playerAgeGateWarningStatus.Value.rightButtonText;
			WarningScreens._leftButtonResult = playerAgeGateWarningStatus.Value.leftButtonResult;
			WarningScreens._rightButtonResult = playerAgeGateWarningStatus.Value.rightButtonResult;
			this._onLeftButtonPressedAction = playerAgeGateWarningStatus.Value.onLeftButtonPressedAction;
			this._onRightButtonPressedAction = playerAgeGateWarningStatus.Value.onRightButtonPressedAction;
			if (this._imageContainer && this._withImageText && this._noImageText)
			{
				this._imageContainer.SetActive(playerAgeGateWarningStatus.Value.showImage);
				this._withImageText.text = playerAgeGateWarningStatus.Value.body;
				this._withImageText.gameObject.SetActive(playerAgeGateWarningStatus.Value.showImage);
				this._noImageText.gameObject.SetActive(!playerAgeGateWarningStatus.Value.showImage);
			}
			this._messageBox.gameObject.SetActive(true);
			GameObject canvas = this._messageBox.GetCanvas();
			PrivateUIRoom.AddUI(canvas.transform);
			HandRayController.Instance.EnableHandRays();
			await WarningScreens.WaitForResponse(cancellationToken);
			HandRayController.Instance.DisableHandRays();
			PrivateUIRoom.RemoveUI(canvas.transform);
			this._messageBox.gameObject.SetActive(false);
			warningButtonResult = WarningScreens._result;
		}
		return warningButtonResult;
	}

	// Token: 0x06003364 RID: 13156 RVA: 0x000FD5F0 File Offset: 0x000FB7F0
	public static async Task<WarningButtonResult> StartWarningScreen(CancellationToken cancellationToken)
	{
		return await WarningScreens._activeReference.StartWarningScreenInternal(cancellationToken);
	}

	// Token: 0x06003365 RID: 13157 RVA: 0x000FD634 File Offset: 0x000FB834
	public static async Task<WarningButtonResult> StartOptInFollowUpScreen(CancellationToken cancellationToken)
	{
		return await WarningScreens._activeReference.StartOptInFollowUpScreenInternal(cancellationToken);
	}

	// Token: 0x06003366 RID: 13158 RVA: 0x000FD678 File Offset: 0x000FB878
	private static async Task WaitForResponse(CancellationToken cancellationToken)
	{
		while (!WarningScreens._closedMessageBox)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			await Task.Yield();
		}
	}

	// Token: 0x06003367 RID: 13159 RVA: 0x000FD6BB File Offset: 0x000FB8BB
	public static void OnLeftButtonClicked()
	{
		WarningScreens._result = WarningScreens._leftButtonResult;
		WarningScreens._closedMessageBox = true;
		WarningScreens activeReference = WarningScreens._activeReference;
		if (activeReference == null)
		{
			return;
		}
		Action onLeftButtonPressedAction = activeReference._onLeftButtonPressedAction;
		if (onLeftButtonPressedAction == null)
		{
			return;
		}
		onLeftButtonPressedAction();
	}

	// Token: 0x06003368 RID: 13160 RVA: 0x000FD6E6 File Offset: 0x000FB8E6
	public static void OnRightButtonClicked()
	{
		WarningScreens._result = WarningScreens._rightButtonResult;
		WarningScreens._closedMessageBox = true;
		WarningScreens activeReference = WarningScreens._activeReference;
		if (activeReference == null)
		{
			return;
		}
		Action onRightButtonPressedAction = activeReference._onRightButtonPressedAction;
		if (onRightButtonPressedAction == null)
		{
			return;
		}
		onRightButtonPressedAction();
	}

	// Token: 0x04003A31 RID: 14897
	private static WarningScreens _activeReference;

	// Token: 0x04003A32 RID: 14898
	[SerializeField]
	private MessageBox _messageBox;

	// Token: 0x04003A33 RID: 14899
	[SerializeField]
	private GameObject _imageContainer;

	// Token: 0x04003A34 RID: 14900
	[SerializeField]
	private TMP_Text _withImageText;

	// Token: 0x04003A35 RID: 14901
	[SerializeField]
	private TMP_Text _noImageText;

	// Token: 0x04003A36 RID: 14902
	private Action _onLeftButtonPressedAction;

	// Token: 0x04003A37 RID: 14903
	private Action _onRightButtonPressedAction;

	// Token: 0x04003A38 RID: 14904
	private static WarningButtonResult _result;

	// Token: 0x04003A39 RID: 14905
	private static WarningButtonResult _leftButtonResult;

	// Token: 0x04003A3A RID: 14906
	private static WarningButtonResult _rightButtonResult;

	// Token: 0x04003A3B RID: 14907
	private static bool _closedMessageBox;
}
