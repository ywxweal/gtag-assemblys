using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004C7 RID: 1223
public class MonkeBallResetGame : MonoBehaviour
{
	// Token: 0x06001DAE RID: 7598 RVA: 0x000908FC File Offset: 0x0008EAFC
	private void Awake()
	{
		this._resetButton.onPressButton.AddListener(new UnityAction(this.OnSelect));
		if (this._resetButton == null)
		{
			this._buttonOrigin = this._resetButton.transform.position;
		}
	}

	// Token: 0x06001DAF RID: 7599 RVA: 0x00090949 File Offset: 0x0008EB49
	private void Update()
	{
		if (this._cooldown)
		{
			this._cooldownTimer -= Time.deltaTime;
			if (this._cooldownTimer <= 0f)
			{
				this.ToggleButton(false, -1);
				this._cooldown = false;
			}
		}
	}

	// Token: 0x06001DB0 RID: 7600 RVA: 0x00090984 File Offset: 0x0008EB84
	public void ToggleReset(bool toggle, int teamId, bool force = false)
	{
		if (teamId < -1 || teamId >= this.teamMaterials.Length)
		{
			return;
		}
		if (toggle)
		{
			this.ToggleButton(true, teamId);
			this._cooldown = false;
			return;
		}
		if (force)
		{
			this.ToggleButton(false, -1);
			return;
		}
		this._cooldown = true;
		this._cooldownTimer = 3f;
	}

	// Token: 0x06001DB1 RID: 7601 RVA: 0x000909D4 File Offset: 0x0008EBD4
	private void ToggleButton(bool toggle, int teamId)
	{
		this._resetButton.enabled = toggle;
		this.allowedTeamId = teamId;
		if (!toggle || teamId == -1)
		{
			this.button.sharedMaterial = this.neutralMaterial;
			return;
		}
		this.button.sharedMaterial = this.teamMaterials[teamId];
	}

	// Token: 0x06001DB2 RID: 7602 RVA: 0x00090A20 File Offset: 0x0008EC20
	private void OnSelect()
	{
		MonkeBallGame.Instance.RequestResetGame();
	}

	// Token: 0x040020C7 RID: 8391
	[SerializeField]
	private GorillaPressableButton _resetButton;

	// Token: 0x040020C8 RID: 8392
	public Renderer button;

	// Token: 0x040020C9 RID: 8393
	public Vector3 buttonPressOffset;

	// Token: 0x040020CA RID: 8394
	private Vector3 _buttonOrigin = Vector3.zero;

	// Token: 0x040020CB RID: 8395
	[Space]
	public Material[] teamMaterials;

	// Token: 0x040020CC RID: 8396
	public Material neutralMaterial;

	// Token: 0x040020CD RID: 8397
	public int allowedTeamId = -1;

	// Token: 0x040020CE RID: 8398
	[SerializeField]
	private TextMeshPro _resetLabel;

	// Token: 0x040020CF RID: 8399
	private bool _cooldown;

	// Token: 0x040020D0 RID: 8400
	private float _cooldownTimer;
}
