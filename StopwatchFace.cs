using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006B5 RID: 1717
public class StopwatchFace : MonoBehaviour
{
	// Token: 0x1700042F RID: 1071
	// (get) Token: 0x06002AD8 RID: 10968 RVA: 0x000D2581 File Offset: 0x000D0781
	public bool watchActive
	{
		get
		{
			return this._watchActive;
		}
	}

	// Token: 0x17000430 RID: 1072
	// (get) Token: 0x06002AD9 RID: 10969 RVA: 0x000D2589 File Offset: 0x000D0789
	public int millisElapsed
	{
		get
		{
			return this._millisElapsed;
		}
	}

	// Token: 0x17000431 RID: 1073
	// (get) Token: 0x06002ADA RID: 10970 RVA: 0x000D2591 File Offset: 0x000D0791
	public Vector3Int digitsMmSsMs
	{
		get
		{
			return StopwatchFace.ParseDigits(TimeSpan.FromMilliseconds((double)this._millisElapsed));
		}
	}

	// Token: 0x06002ADB RID: 10971 RVA: 0x000D25A4 File Offset: 0x000D07A4
	public void SetMillisElapsed(int millis, bool updateFace = true)
	{
		this._millisElapsed = millis;
		if (!updateFace)
		{
			return;
		}
		this.UpdateText();
		this.UpdateHand();
	}

	// Token: 0x06002ADC RID: 10972 RVA: 0x000D25BD File Offset: 0x000D07BD
	private void Awake()
	{
		this._lerpToZero = new LerpTask<int>();
		this._lerpToZero.onLerp = new Action<int, int, float>(this.OnLerpToZero);
		this._lerpToZero.onLerpEnd = new Action(this.OnLerpEnd);
	}

	// Token: 0x06002ADD RID: 10973 RVA: 0x000D25F8 File Offset: 0x000D07F8
	private void OnLerpToZero(int a, int b, float t)
	{
		this._millisElapsed = Mathf.FloorToInt(Mathf.Lerp((float)a, (float)b, t * t));
		this.UpdateText();
		this.UpdateHand();
	}

	// Token: 0x06002ADE RID: 10974 RVA: 0x000D261D File Offset: 0x000D081D
	private void OnLerpEnd()
	{
		this.WatchReset(false);
	}

	// Token: 0x06002ADF RID: 10975 RVA: 0x000D261D File Offset: 0x000D081D
	private void OnEnable()
	{
		this.WatchReset(false);
	}

	// Token: 0x06002AE0 RID: 10976 RVA: 0x000D261D File Offset: 0x000D081D
	private void OnDisable()
	{
		this.WatchReset(false);
	}

	// Token: 0x06002AE1 RID: 10977 RVA: 0x000D2628 File Offset: 0x000D0828
	private void Update()
	{
		if (this._lerpToZero.active)
		{
			this._lerpToZero.Update();
			return;
		}
		if (this._watchActive)
		{
			this._millisElapsed += Mathf.FloorToInt(Time.deltaTime * 1000f);
			this.UpdateText();
			this.UpdateHand();
		}
	}

	// Token: 0x06002AE2 RID: 10978 RVA: 0x000D2680 File Offset: 0x000D0880
	private static Vector3Int ParseDigits(TimeSpan time)
	{
		int num = (int)time.TotalMinutes % 100;
		double num2 = 60.0 * (time.TotalMinutes - (double)num);
		int num3 = (int)num2;
		int num4 = (int)(100.0 * (num2 - (double)num3));
		num = Math.Clamp(num, 0, 99);
		num3 = Math.Clamp(num3, 0, 59);
		num4 = Math.Clamp(num4, 0, 99);
		return new Vector3Int(num, num3, num4);
	}

	// Token: 0x06002AE3 RID: 10979 RVA: 0x000D26E8 File Offset: 0x000D08E8
	private void UpdateText()
	{
		Vector3Int vector3Int = StopwatchFace.ParseDigits(TimeSpan.FromMilliseconds((double)this._millisElapsed));
		string text = vector3Int.x.ToString("D2");
		string text2 = vector3Int.y.ToString("D2");
		string text3 = vector3Int.z.ToString("D2");
		this._text.text = string.Concat(new string[] { text, ":", text2, ":", text3 });
	}

	// Token: 0x06002AE4 RID: 10980 RVA: 0x000D277C File Offset: 0x000D097C
	private void UpdateHand()
	{
		float num = (float)(this._millisElapsed % 60000) / 60000f * 360f;
		this._hand.localEulerAngles = new Vector3(0f, 0f, num);
	}

	// Token: 0x06002AE5 RID: 10981 RVA: 0x000D27BE File Offset: 0x000D09BE
	public void WatchToggle()
	{
		if (!this._watchActive)
		{
			this.WatchStart();
			return;
		}
		this.WatchStop();
	}

	// Token: 0x06002AE6 RID: 10982 RVA: 0x000D27D5 File Offset: 0x000D09D5
	public void WatchStart()
	{
		if (this._lerpToZero.active)
		{
			return;
		}
		this._watchActive = true;
	}

	// Token: 0x06002AE7 RID: 10983 RVA: 0x000D27EC File Offset: 0x000D09EC
	public void WatchStop()
	{
		if (this._lerpToZero.active)
		{
			return;
		}
		this._watchActive = false;
	}

	// Token: 0x06002AE8 RID: 10984 RVA: 0x000D2803 File Offset: 0x000D0A03
	public void WatchReset()
	{
		this.WatchReset(true);
	}

	// Token: 0x06002AE9 RID: 10985 RVA: 0x000D280C File Offset: 0x000D0A0C
	public void WatchReset(bool doLerp)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (doLerp)
		{
			if (!this._lerpToZero.active)
			{
				this._lerpToZero.Start(this._millisElapsed % 60000, 0, 0.36f);
				return;
			}
		}
		else
		{
			this._watchActive = false;
			this._millisElapsed = 0;
			this.UpdateText();
			this.UpdateHand();
		}
	}

	// Token: 0x04002FC8 RID: 12232
	[SerializeField]
	private Transform _hand;

	// Token: 0x04002FC9 RID: 12233
	[SerializeField]
	private Text _text;

	// Token: 0x04002FCA RID: 12234
	[Space]
	[SerializeField]
	private StopwatchCosmetic _cosmetic;

	// Token: 0x04002FCB RID: 12235
	[Space]
	[SerializeField]
	private AudioClip _audioClick;

	// Token: 0x04002FCC RID: 12236
	[SerializeField]
	private AudioClip _audioReset;

	// Token: 0x04002FCD RID: 12237
	[SerializeField]
	private AudioClip _audioTick;

	// Token: 0x04002FCE RID: 12238
	[Space]
	[NonSerialized]
	private int _millisElapsed;

	// Token: 0x04002FCF RID: 12239
	[NonSerialized]
	private bool _watchActive;

	// Token: 0x04002FD0 RID: 12240
	[NonSerialized]
	private LerpTask<int> _lerpToZero;
}
