using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020007F2 RID: 2034
public class PressAndHoldWithProgessBar : MonoBehaviour, IBuildValidation
{
	// Token: 0x17000513 RID: 1299
	// (get) Token: 0x060031ED RID: 12781 RVA: 0x000F70EE File Offset: 0x000F52EE
	// (set) Token: 0x060031EE RID: 12782 RVA: 0x000F70F6 File Offset: 0x000F52F6
	public PressAndHoldWithProgessBar.PressAndHoldEvent onHoldComplete
	{
		get
		{
			return this.m_OnHoldComplete;
		}
		set
		{
			this.m_OnHoldComplete = value;
		}
	}

	// Token: 0x060031EF RID: 12783 RVA: 0x000F70FF File Offset: 0x000F52FF
	private void Awake()
	{
		this.controllerBehaviour = base.GetComponentInChildren<ControllerBehaviour>(true);
	}

	// Token: 0x060031F0 RID: 12784 RVA: 0x000F710E File Offset: 0x000F530E
	private void OnEnable()
	{
		this.progressBarFill.rectTransform.localScale = new Vector3(0f, 1f, 1f);
	}

	// Token: 0x060031F1 RID: 12785 RVA: 0x000F7134 File Offset: 0x000F5334
	protected void Update()
	{
		if (this.controllerBehaviour.ButtonDown && this._progressBarContainer.activeInHierarchy)
		{
			this.progress += Time.deltaTime / this.holdTime;
			this.progress = Mathf.Clamp01(this.progress);
		}
		else
		{
			this.progress = 0f;
		}
		if ((double)this.progress == 1.0)
		{
			this.m_OnHoldComplete.Invoke();
			this.progress = 0f;
		}
		this.progressBarFill.rectTransform.localScale = new Vector3(this.progress, 1f, 1f);
	}

	// Token: 0x060031F2 RID: 12786 RVA: 0x000F71DF File Offset: 0x000F53DF
	public bool BuildValidationCheck()
	{
		if (this._progressBarContainer == null)
		{
			Debug.LogError("[KID] Object [_progressBarContainer] is NULL. Must be assigned in editor");
			return false;
		}
		return true;
	}

	// Token: 0x040038B2 RID: 14514
	[SerializeField]
	private PressAndHoldWithProgessBar.PressAndHoldEvent m_OnHoldComplete = new PressAndHoldWithProgessBar.PressAndHoldEvent();

	// Token: 0x040038B3 RID: 14515
	[SerializeField]
	private GameObject _progressBarContainer;

	// Token: 0x040038B4 RID: 14516
	[SerializeField]
	private float holdTime = 5f;

	// Token: 0x040038B5 RID: 14517
	[SerializeField]
	private Image progressBarFill;

	// Token: 0x040038B6 RID: 14518
	private float progress;

	// Token: 0x040038B7 RID: 14519
	private ControllerBehaviour controllerBehaviour;

	// Token: 0x020007F3 RID: 2035
	[Serializable]
	public class PressAndHoldEvent : UnityEvent
	{
	}
}
