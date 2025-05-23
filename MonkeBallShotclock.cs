using System;
using TMPro;
using UnityEngine;

// Token: 0x020004CA RID: 1226
public class MonkeBallShotclock : MonoBehaviour
{
	// Token: 0x06001DC0 RID: 7616 RVA: 0x00090BC8 File Offset: 0x0008EDC8
	private void Update()
	{
		if (this._time >= 0f)
		{
			this._time -= Time.deltaTime;
			this.UpdateTimeText(this._time);
			if (this._time < 0f)
			{
				this.SetBackboard(this.neutralMaterial);
			}
		}
	}

	// Token: 0x06001DC1 RID: 7617 RVA: 0x00090C1C File Offset: 0x0008EE1C
	public void SetTime(int teamId, float time)
	{
		this._time = time;
		if (teamId == -1)
		{
			this._time = 0f;
			this.SetBackboard(this.neutralMaterial);
		}
		else if (teamId >= 0 && teamId < this.teamMaterials.Length)
		{
			this.SetBackboard(this.teamMaterials[teamId]);
		}
		this.UpdateTimeText(time);
	}

	// Token: 0x06001DC2 RID: 7618 RVA: 0x00090C71 File Offset: 0x0008EE71
	private void SetBackboard(Material teamMaterial)
	{
		if (this.backboard != null)
		{
			this.backboard.material = teamMaterial;
		}
	}

	// Token: 0x06001DC3 RID: 7619 RVA: 0x00090C90 File Offset: 0x0008EE90
	private void UpdateTimeText(float time)
	{
		int num = Mathf.CeilToInt(time);
		if (this._timeInt != num)
		{
			this._timeInt = num;
			this.timeRemainingLabel.text = this._timeInt.ToString("#00");
		}
	}

	// Token: 0x040020E0 RID: 8416
	public Renderer backboard;

	// Token: 0x040020E1 RID: 8417
	public Material[] teamMaterials;

	// Token: 0x040020E2 RID: 8418
	public Material neutralMaterial;

	// Token: 0x040020E3 RID: 8419
	public TextMeshPro timeRemainingLabel;

	// Token: 0x040020E4 RID: 8420
	private float _time;

	// Token: 0x040020E5 RID: 8421
	private int _timeInt = -1;
}
