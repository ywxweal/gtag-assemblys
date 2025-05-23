using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

// Token: 0x02000317 RID: 791
public class MoviePlayerSample : MonoBehaviour
{
	// Token: 0x17000224 RID: 548
	// (get) Token: 0x060012CE RID: 4814 RVA: 0x000590A6 File Offset: 0x000572A6
	// (set) Token: 0x060012CF RID: 4815 RVA: 0x000590AE File Offset: 0x000572AE
	public bool IsPlaying { get; private set; }

	// Token: 0x17000225 RID: 549
	// (get) Token: 0x060012D0 RID: 4816 RVA: 0x000590B7 File Offset: 0x000572B7
	// (set) Token: 0x060012D1 RID: 4817 RVA: 0x000590BF File Offset: 0x000572BF
	public long Duration { get; private set; }

	// Token: 0x17000226 RID: 550
	// (get) Token: 0x060012D2 RID: 4818 RVA: 0x000590C8 File Offset: 0x000572C8
	// (set) Token: 0x060012D3 RID: 4819 RVA: 0x000590D0 File Offset: 0x000572D0
	public long PlaybackPosition { get; private set; }

	// Token: 0x060012D4 RID: 4820 RVA: 0x000590DC File Offset: 0x000572DC
	private void Awake()
	{
		Debug.Log("MovieSample Awake");
		this.mediaRenderer = base.GetComponent<Renderer>();
		this.videoPlayer = base.GetComponent<VideoPlayer>();
		if (this.videoPlayer == null)
		{
			this.videoPlayer = base.gameObject.AddComponent<VideoPlayer>();
		}
		this.videoPlayer.isLooping = this.LoopVideo;
		this.overlay = base.GetComponent<OVROverlay>();
		if (this.overlay == null)
		{
			this.overlay = base.gameObject.AddComponent<OVROverlay>();
		}
		this.overlay.enabled = false;
		this.overlay.isExternalSurface = NativeVideoPlayer.IsAvailable;
		this.overlay.enabled = this.overlay.currentOverlayShape != OVROverlay.OverlayShape.Equirect || Application.platform == RuntimePlatform.Android;
	}

	// Token: 0x060012D5 RID: 4821 RVA: 0x000591A7 File Offset: 0x000573A7
	private bool IsLocalVideo(string movieName)
	{
		return !movieName.Contains("://");
	}

	// Token: 0x060012D6 RID: 4822 RVA: 0x000591B8 File Offset: 0x000573B8
	private void UpdateShapeAndStereo()
	{
		if (this.AutoDetectStereoLayout && this.overlay.isExternalSurface)
		{
			int videoWidth = NativeVideoPlayer.VideoWidth;
			int videoHeight = NativeVideoPlayer.VideoHeight;
			switch (NativeVideoPlayer.VideoStereoMode)
			{
			case NativeVideoPlayer.StereoMode.Unknown:
				if (videoWidth > videoHeight)
				{
					this.Stereo = MoviePlayerSample.VideoStereo.LeftRight;
				}
				else
				{
					this.Stereo = MoviePlayerSample.VideoStereo.TopBottom;
				}
				break;
			case NativeVideoPlayer.StereoMode.Mono:
				this.Stereo = MoviePlayerSample.VideoStereo.Mono;
				break;
			case NativeVideoPlayer.StereoMode.TopBottom:
				this.Stereo = MoviePlayerSample.VideoStereo.TopBottom;
				break;
			case NativeVideoPlayer.StereoMode.LeftRight:
				this.Stereo = MoviePlayerSample.VideoStereo.LeftRight;
				break;
			}
		}
		if (this.Shape != this._LastShape || this.Stereo != this._LastStereo || this.DisplayMono != this._LastDisplayMono)
		{
			Rect rect = new Rect(0f, 0f, 1f, 1f);
			switch (this.Shape)
			{
			case MoviePlayerSample.VideoShape._360:
				this.overlay.currentOverlayShape = OVROverlay.OverlayShape.Equirect;
				goto IL_0118;
			case MoviePlayerSample.VideoShape._180:
				this.overlay.currentOverlayShape = OVROverlay.OverlayShape.Equirect;
				rect = new Rect(0.25f, 0f, 0.5f, 1f);
				goto IL_0118;
			}
			this.overlay.currentOverlayShape = OVROverlay.OverlayShape.Quad;
			IL_0118:
			this.overlay.overrideTextureRectMatrix = true;
			this.overlay.invertTextureRects = false;
			Rect rect2 = new Rect(0f, 0f, 1f, 1f);
			Rect rect3 = new Rect(0f, 0f, 1f, 1f);
			switch (this.Stereo)
			{
			case MoviePlayerSample.VideoStereo.TopBottom:
				rect2 = new Rect(0f, 0.5f, 1f, 0.5f);
				rect3 = new Rect(0f, 0f, 1f, 0.5f);
				break;
			case MoviePlayerSample.VideoStereo.LeftRight:
				rect2 = new Rect(0f, 0f, 0.5f, 1f);
				rect3 = new Rect(0.5f, 0f, 0.5f, 1f);
				break;
			case MoviePlayerSample.VideoStereo.BottomTop:
				rect2 = new Rect(0f, 0f, 1f, 0.5f);
				rect3 = new Rect(0f, 0.5f, 1f, 0.5f);
				break;
			}
			this.overlay.SetSrcDestRects(rect2, this.DisplayMono ? rect2 : rect3, rect, rect);
			this._LastDisplayMono = this.DisplayMono;
			this._LastStereo = this.Stereo;
			this._LastShape = this.Shape;
		}
	}

	// Token: 0x060012D7 RID: 4823 RVA: 0x00059434 File Offset: 0x00057634
	private IEnumerator Start()
	{
		if (this.mediaRenderer.material == null)
		{
			Debug.LogError("No material for movie surface");
			yield break;
		}
		yield return new WaitForSeconds(1f);
		if (!string.IsNullOrEmpty(this.MovieName))
		{
			if (this.IsLocalVideo(this.MovieName))
			{
				this.Play(Application.streamingAssetsPath + "/" + this.MovieName, null);
			}
			else
			{
				this.Play(this.MovieName, this.DrmLicenseUrl);
			}
		}
		yield break;
	}

	// Token: 0x060012D8 RID: 4824 RVA: 0x00059444 File Offset: 0x00057644
	public void Play(string moviePath, string drmLicencesUrl)
	{
		if (moviePath != string.Empty)
		{
			Debug.Log("Playing Video: " + moviePath);
			if (this.overlay.isExternalSurface)
			{
				OVROverlay.ExternalSurfaceObjectCreated externalSurfaceObjectCreated = delegate
				{
					Debug.Log("Playing ExoPlayer with SurfaceObject");
					NativeVideoPlayer.PlayVideo(moviePath, drmLicencesUrl, this.overlay.externalSurfaceObject);
					NativeVideoPlayer.SetLooping(this.LoopVideo);
				};
				if (this.overlay.externalSurfaceObject == IntPtr.Zero)
				{
					this.overlay.externalSurfaceObjectCreated = externalSurfaceObjectCreated;
				}
				else
				{
					externalSurfaceObjectCreated();
				}
			}
			else
			{
				Debug.Log("Playing Unity VideoPlayer");
				this.videoPlayer.url = moviePath;
				this.videoPlayer.Prepare();
				this.videoPlayer.Play();
			}
			Debug.Log("MovieSample Start");
			this.IsPlaying = true;
			return;
		}
		Debug.LogError("No media file name provided");
	}

	// Token: 0x060012D9 RID: 4825 RVA: 0x0005952A File Offset: 0x0005772A
	public void Play()
	{
		if (this.overlay.isExternalSurface)
		{
			NativeVideoPlayer.Play();
		}
		else
		{
			this.videoPlayer.Play();
		}
		this.IsPlaying = true;
	}

	// Token: 0x060012DA RID: 4826 RVA: 0x00059552 File Offset: 0x00057752
	public void Pause()
	{
		if (this.overlay.isExternalSurface)
		{
			NativeVideoPlayer.Pause();
		}
		else
		{
			this.videoPlayer.Pause();
		}
		this.IsPlaying = false;
	}

	// Token: 0x060012DB RID: 4827 RVA: 0x0005957C File Offset: 0x0005777C
	public void SeekTo(long position)
	{
		long num = Math.Max(0L, Math.Min(this.Duration, position));
		if (this.overlay.isExternalSurface)
		{
			NativeVideoPlayer.PlaybackPosition = num;
			return;
		}
		this.videoPlayer.time = (double)num / 1000.0;
	}

	// Token: 0x060012DC RID: 4828 RVA: 0x000595C8 File Offset: 0x000577C8
	private void Update()
	{
		this.UpdateShapeAndStereo();
		if (!this.overlay.isExternalSurface)
		{
			Texture texture = ((this.videoPlayer.texture != null) ? this.videoPlayer.texture : Texture2D.blackTexture);
			if (this.overlay.enabled)
			{
				if (this.overlay.textures[0] != texture)
				{
					this.overlay.enabled = false;
					this.overlay.textures[0] = texture;
					this.overlay.enabled = true;
				}
			}
			else
			{
				this.mediaRenderer.material.mainTexture = texture;
				this.mediaRenderer.material.SetVector("_SrcRectLeft", this.overlay.srcRectLeft.ToVector());
				this.mediaRenderer.material.SetVector("_SrcRectRight", this.overlay.srcRectRight.ToVector());
			}
			this.IsPlaying = this.videoPlayer.isPlaying;
			this.PlaybackPosition = (long)(this.videoPlayer.time * 1000.0);
			this.Duration = (long)(this.videoPlayer.length * 1000.0);
			return;
		}
		NativeVideoPlayer.SetListenerRotation(Camera.main.transform.rotation);
		this.IsPlaying = NativeVideoPlayer.IsPlaying;
		this.PlaybackPosition = NativeVideoPlayer.PlaybackPosition;
		this.Duration = NativeVideoPlayer.Duration;
		if (this.IsPlaying && (int)OVRManager.display.displayFrequency != 60)
		{
			OVRManager.display.displayFrequency = 60f;
			return;
		}
		if (!this.IsPlaying && (int)OVRManager.display.displayFrequency != 72)
		{
			OVRManager.display.displayFrequency = 72f;
		}
	}

	// Token: 0x060012DD RID: 4829 RVA: 0x00059785 File Offset: 0x00057985
	public void SetPlaybackSpeed(float speed)
	{
		speed = Mathf.Max(0f, speed);
		if (this.overlay.isExternalSurface)
		{
			NativeVideoPlayer.SetPlaybackSpeed(speed);
			return;
		}
		this.videoPlayer.playbackSpeed = speed;
	}

	// Token: 0x060012DE RID: 4830 RVA: 0x000597B4 File Offset: 0x000579B4
	public void Stop()
	{
		if (this.overlay.isExternalSurface)
		{
			NativeVideoPlayer.Stop();
		}
		else
		{
			this.videoPlayer.Stop();
		}
		this.IsPlaying = false;
	}

	// Token: 0x060012DF RID: 4831 RVA: 0x000597DC File Offset: 0x000579DC
	private void OnApplicationPause(bool appWasPaused)
	{
		Debug.Log("OnApplicationPause: " + appWasPaused.ToString());
		if (appWasPaused)
		{
			this.videoPausedBeforeAppPause = !this.IsPlaying;
		}
		if (!this.videoPausedBeforeAppPause)
		{
			if (appWasPaused)
			{
				this.Pause();
				return;
			}
			this.Play();
		}
	}

	// Token: 0x040014E2 RID: 5346
	private bool videoPausedBeforeAppPause;

	// Token: 0x040014E3 RID: 5347
	private VideoPlayer videoPlayer;

	// Token: 0x040014E4 RID: 5348
	private OVROverlay overlay;

	// Token: 0x040014E5 RID: 5349
	private Renderer mediaRenderer;

	// Token: 0x040014E9 RID: 5353
	private RenderTexture copyTexture;

	// Token: 0x040014EA RID: 5354
	private Material externalTex2DMaterial;

	// Token: 0x040014EB RID: 5355
	public string MovieName;

	// Token: 0x040014EC RID: 5356
	public string DrmLicenseUrl;

	// Token: 0x040014ED RID: 5357
	public bool LoopVideo;

	// Token: 0x040014EE RID: 5358
	public MoviePlayerSample.VideoShape Shape;

	// Token: 0x040014EF RID: 5359
	public MoviePlayerSample.VideoStereo Stereo;

	// Token: 0x040014F0 RID: 5360
	public bool AutoDetectStereoLayout;

	// Token: 0x040014F1 RID: 5361
	public bool DisplayMono;

	// Token: 0x040014F2 RID: 5362
	private MoviePlayerSample.VideoShape _LastShape = (MoviePlayerSample.VideoShape)(-1);

	// Token: 0x040014F3 RID: 5363
	private MoviePlayerSample.VideoStereo _LastStereo = (MoviePlayerSample.VideoStereo)(-1);

	// Token: 0x040014F4 RID: 5364
	private bool _LastDisplayMono;

	// Token: 0x02000318 RID: 792
	public enum VideoShape
	{
		// Token: 0x040014F6 RID: 5366
		_360,
		// Token: 0x040014F7 RID: 5367
		_180,
		// Token: 0x040014F8 RID: 5368
		Quad
	}

	// Token: 0x02000319 RID: 793
	public enum VideoStereo
	{
		// Token: 0x040014FA RID: 5370
		Mono,
		// Token: 0x040014FB RID: 5371
		TopBottom,
		// Token: 0x040014FC RID: 5372
		LeftRight,
		// Token: 0x040014FD RID: 5373
		BottomTop
	}
}
