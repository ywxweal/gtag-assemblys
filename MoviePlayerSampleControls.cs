using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200031C RID: 796
public class MoviePlayerSampleControls : MonoBehaviour
{
	// Token: 0x060012E9 RID: 4841 RVA: 0x0005994C File Offset: 0x00057B4C
	private void Start()
	{
		this.PlayPause.onButtonDown += this.OnPlayPauseClicked;
		this.FastForward.onButtonDown += this.OnFastForwardClicked;
		this.Rewind.onButtonDown += this.OnRewindClicked;
		this.ProgressBar.onValueChanged.AddListener(new UnityAction<float>(this.OnSeekBarMoved));
		this.PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Pause;
		this.FastForwardImage.buttonType = MediaPlayerImage.ButtonType.SkipForward;
		this.RewindImage.buttonType = MediaPlayerImage.ButtonType.SkipBack;
		this.SetVisible(false);
	}

	// Token: 0x060012EA RID: 4842 RVA: 0x000599E8 File Offset: 0x00057BE8
	private void OnPlayPauseClicked()
	{
		switch (this._state)
		{
		case MoviePlayerSampleControls.PlaybackState.Playing:
			this.Player.Pause();
			this.PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Play;
			this.FastForwardImage.buttonType = MediaPlayerImage.ButtonType.SkipForward;
			this.RewindImage.buttonType = MediaPlayerImage.ButtonType.SkipBack;
			this._state = MoviePlayerSampleControls.PlaybackState.Paused;
			return;
		case MoviePlayerSampleControls.PlaybackState.Paused:
			this.Player.Play();
			this.PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Pause;
			this.FastForwardImage.buttonType = MediaPlayerImage.ButtonType.FastForward;
			this.RewindImage.buttonType = MediaPlayerImage.ButtonType.Rewind;
			this._state = MoviePlayerSampleControls.PlaybackState.Playing;
			return;
		case MoviePlayerSampleControls.PlaybackState.Rewinding:
			this.Player.Play();
			this._state = MoviePlayerSampleControls.PlaybackState.Playing;
			this.PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Pause;
			return;
		case MoviePlayerSampleControls.PlaybackState.FastForwarding:
			this.Player.SetPlaybackSpeed(1f);
			this.PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Pause;
			this._state = MoviePlayerSampleControls.PlaybackState.Playing;
			return;
		default:
			return;
		}
	}

	// Token: 0x060012EB RID: 4843 RVA: 0x00059AC4 File Offset: 0x00057CC4
	private void OnFastForwardClicked()
	{
		switch (this._state)
		{
		case MoviePlayerSampleControls.PlaybackState.Playing:
			this.Player.SetPlaybackSpeed(2f);
			this.PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Play;
			this._state = MoviePlayerSampleControls.PlaybackState.FastForwarding;
			return;
		case MoviePlayerSampleControls.PlaybackState.Paused:
			this.Seek(this.Player.PlaybackPosition + 15000L);
			return;
		case MoviePlayerSampleControls.PlaybackState.Rewinding:
			this.Player.Play();
			this.Player.SetPlaybackSpeed(2f);
			this._state = MoviePlayerSampleControls.PlaybackState.FastForwarding;
			return;
		case MoviePlayerSampleControls.PlaybackState.FastForwarding:
			this.Player.SetPlaybackSpeed(1f);
			this._state = MoviePlayerSampleControls.PlaybackState.Playing;
			this.PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Pause;
			return;
		default:
			return;
		}
	}

	// Token: 0x060012EC RID: 4844 RVA: 0x00059B74 File Offset: 0x00057D74
	private void OnRewindClicked()
	{
		switch (this._state)
		{
		case MoviePlayerSampleControls.PlaybackState.Playing:
		case MoviePlayerSampleControls.PlaybackState.FastForwarding:
			this.Player.SetPlaybackSpeed(1f);
			this.Player.Pause();
			this._rewindStartPosition = this.Player.PlaybackPosition;
			this._rewindStartTime = Time.time;
			this.PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Play;
			this._state = MoviePlayerSampleControls.PlaybackState.Rewinding;
			return;
		case MoviePlayerSampleControls.PlaybackState.Paused:
			this.Seek(this.Player.PlaybackPosition - 15000L);
			return;
		case MoviePlayerSampleControls.PlaybackState.Rewinding:
			this.Player.Play();
			this.PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Pause;
			this._state = MoviePlayerSampleControls.PlaybackState.Playing;
			return;
		default:
			return;
		}
	}

	// Token: 0x060012ED RID: 4845 RVA: 0x00059C24 File Offset: 0x00057E24
	private void OnSeekBarMoved(float value)
	{
		long num = (long)(value * (float)this.Player.Duration);
		if (Mathf.Abs((float)(num - this.Player.PlaybackPosition)) > 200f)
		{
			this.Seek(num);
		}
	}

	// Token: 0x060012EE RID: 4846 RVA: 0x00059C62 File Offset: 0x00057E62
	private void Seek(long pos)
	{
		this._didSeek = true;
		this._seekPreviousPosition = this.Player.PlaybackPosition;
		this.Player.SeekTo(pos);
	}

	// Token: 0x060012EF RID: 4847 RVA: 0x00059C88 File Offset: 0x00057E88
	private void Update()
	{
		if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.Active) || OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.Active) || OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger, OVRInput.Controller.Active))
		{
			this._lastButtonTime = Time.time;
			if (!this._isVisible)
			{
				this.SetVisible(true);
			}
		}
		if (OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
		{
			this.InputModule.rayTransform = this.LeftHand.transform;
			this.GazePointer.rayTransform = this.LeftHand.transform;
		}
		else
		{
			this.InputModule.rayTransform = this.RightHand.transform;
			this.GazePointer.rayTransform = this.RightHand.transform;
		}
		if (OVRInput.Get(OVRInput.Button.Back, OVRInput.Controller.Active) && this._isVisible)
		{
			this.SetVisible(false);
		}
		if (this._state == MoviePlayerSampleControls.PlaybackState.Rewinding)
		{
			this.ProgressBar.value = Mathf.Clamp01(((float)this._rewindStartPosition - 1000f * (Time.time - this._rewindStartTime)) / (float)this.Player.Duration);
		}
		if (this._isVisible && this._state == MoviePlayerSampleControls.PlaybackState.Playing && Time.time - this._lastButtonTime > this.TimeoutTime)
		{
			this.SetVisible(false);
		}
		if (this._isVisible && (!this._didSeek || Mathf.Abs((float)(this._seekPreviousPosition - this.Player.PlaybackPosition)) > 50f))
		{
			this._didSeek = false;
			if (this.Player.Duration > 0L)
			{
				this.ProgressBar.value = (float)((double)this.Player.PlaybackPosition / (double)this.Player.Duration);
				return;
			}
			this.ProgressBar.value = 0f;
		}
	}

	// Token: 0x060012F0 RID: 4848 RVA: 0x00059E48 File Offset: 0x00058048
	private void SetVisible(bool visible)
	{
		this.Canvas.enabled = visible;
		this._isVisible = visible;
		this.Player.DisplayMono = visible;
		this.LeftHand.SetActive(visible);
		this.RightHand.SetActive(visible);
		Debug.Log("Controls Visible: " + visible.ToString());
	}

	// Token: 0x04001504 RID: 5380
	public MoviePlayerSample Player;

	// Token: 0x04001505 RID: 5381
	public OVRInputModule InputModule;

	// Token: 0x04001506 RID: 5382
	public OVRGazePointer GazePointer;

	// Token: 0x04001507 RID: 5383
	public GameObject LeftHand;

	// Token: 0x04001508 RID: 5384
	public GameObject RightHand;

	// Token: 0x04001509 RID: 5385
	public Canvas Canvas;

	// Token: 0x0400150A RID: 5386
	public ButtonDownListener PlayPause;

	// Token: 0x0400150B RID: 5387
	public MediaPlayerImage PlayPauseImage;

	// Token: 0x0400150C RID: 5388
	public Slider ProgressBar;

	// Token: 0x0400150D RID: 5389
	public ButtonDownListener FastForward;

	// Token: 0x0400150E RID: 5390
	public MediaPlayerImage FastForwardImage;

	// Token: 0x0400150F RID: 5391
	public ButtonDownListener Rewind;

	// Token: 0x04001510 RID: 5392
	public MediaPlayerImage RewindImage;

	// Token: 0x04001511 RID: 5393
	public float TimeoutTime = 10f;

	// Token: 0x04001512 RID: 5394
	private bool _isVisible;

	// Token: 0x04001513 RID: 5395
	private float _lastButtonTime;

	// Token: 0x04001514 RID: 5396
	private bool _didSeek;

	// Token: 0x04001515 RID: 5397
	private long _seekPreviousPosition;

	// Token: 0x04001516 RID: 5398
	private long _rewindStartPosition;

	// Token: 0x04001517 RID: 5399
	private float _rewindStartTime;

	// Token: 0x04001518 RID: 5400
	private MoviePlayerSampleControls.PlaybackState _state;

	// Token: 0x0200031D RID: 797
	private enum PlaybackState
	{
		// Token: 0x0400151A RID: 5402
		Playing,
		// Token: 0x0400151B RID: 5403
		Paused,
		// Token: 0x0400151C RID: 5404
		Rewinding,
		// Token: 0x0400151D RID: 5405
		FastForwarding
	}
}
