using System;
using UnityEngine;

// Token: 0x02000311 RID: 785
public static class NativeVideoPlayer
{
	// Token: 0x17000219 RID: 537
	// (get) Token: 0x060012B3 RID: 4787 RVA: 0x00057E64 File Offset: 0x00056064
	private static IntPtr VideoPlayerClass
	{
		get
		{
			if (NativeVideoPlayer._VideoPlayerClass == null)
			{
				try
				{
					IntPtr intPtr = AndroidJNI.FindClass("com/oculus/videoplayer/NativeVideoPlayer");
					if (intPtr != IntPtr.Zero)
					{
						NativeVideoPlayer._VideoPlayerClass = new IntPtr?(AndroidJNI.NewGlobalRef(intPtr));
						AndroidJNI.DeleteLocalRef(intPtr);
					}
					else
					{
						Debug.LogError("Failed to find NativeVideoPlayer class");
						NativeVideoPlayer._VideoPlayerClass = new IntPtr?(IntPtr.Zero);
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("Failed to find NativeVideoPlayer class");
					Debug.LogException(ex);
					NativeVideoPlayer._VideoPlayerClass = new IntPtr?(IntPtr.Zero);
				}
			}
			return NativeVideoPlayer._VideoPlayerClass.GetValueOrDefault();
		}
	}

	// Token: 0x1700021A RID: 538
	// (get) Token: 0x060012B4 RID: 4788 RVA: 0x00057F04 File Offset: 0x00056104
	private static IntPtr Activity
	{
		get
		{
			if (NativeVideoPlayer._Activity == null)
			{
				try
				{
					IntPtr intPtr = AndroidJNI.FindClass("com/unity3d/player/UnityPlayer");
					IntPtr staticFieldID = AndroidJNI.GetStaticFieldID(intPtr, "currentActivity", "Landroid/app/Activity;");
					IntPtr staticObjectField = AndroidJNI.GetStaticObjectField(intPtr, staticFieldID);
					NativeVideoPlayer._Activity = new IntPtr?(AndroidJNI.NewGlobalRef(staticObjectField));
					AndroidJNI.DeleteLocalRef(staticObjectField);
					AndroidJNI.DeleteLocalRef(intPtr);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					NativeVideoPlayer._Activity = new IntPtr?(IntPtr.Zero);
				}
			}
			return NativeVideoPlayer._Activity.GetValueOrDefault();
		}
	}

	// Token: 0x1700021B RID: 539
	// (get) Token: 0x060012B5 RID: 4789 RVA: 0x00002076 File Offset: 0x00000276
	public static bool IsAvailable
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700021C RID: 540
	// (get) Token: 0x060012B6 RID: 4790 RVA: 0x00057F90 File Offset: 0x00056190
	public static bool IsPlaying
	{
		get
		{
			if (NativeVideoPlayer.getIsPlayingMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getIsPlayingMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getIsPlaying", "()Z");
			}
			return AndroidJNI.CallStaticBooleanMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getIsPlayingMethodId, NativeVideoPlayer.EmptyParams);
		}
	}

	// Token: 0x1700021D RID: 541
	// (get) Token: 0x060012B7 RID: 4791 RVA: 0x00057FD0 File Offset: 0x000561D0
	public static NativeVideoPlayer.PlabackState CurrentPlaybackState
	{
		get
		{
			if (NativeVideoPlayer.getCurrentPlaybackStateMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getCurrentPlaybackStateMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getCurrentPlaybackState", "()I");
			}
			return (NativeVideoPlayer.PlabackState)AndroidJNI.CallStaticIntMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getCurrentPlaybackStateMethodId, NativeVideoPlayer.EmptyParams);
		}
	}

	// Token: 0x1700021E RID: 542
	// (get) Token: 0x060012B8 RID: 4792 RVA: 0x00058010 File Offset: 0x00056210
	public static long Duration
	{
		get
		{
			if (NativeVideoPlayer.getDurationMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getDurationMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getDuration", "()J");
			}
			return AndroidJNI.CallStaticLongMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getDurationMethodId, NativeVideoPlayer.EmptyParams);
		}
	}

	// Token: 0x1700021F RID: 543
	// (get) Token: 0x060012B9 RID: 4793 RVA: 0x00058050 File Offset: 0x00056250
	public static NativeVideoPlayer.StereoMode VideoStereoMode
	{
		get
		{
			if (NativeVideoPlayer.getStereoModeMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getStereoModeMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getStereoMode", "()I");
			}
			return (NativeVideoPlayer.StereoMode)AndroidJNI.CallStaticIntMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getStereoModeMethodId, NativeVideoPlayer.EmptyParams);
		}
	}

	// Token: 0x17000220 RID: 544
	// (get) Token: 0x060012BA RID: 4794 RVA: 0x00058090 File Offset: 0x00056290
	public static int VideoWidth
	{
		get
		{
			if (NativeVideoPlayer.getWidthMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getWidthMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getWidth", "()I");
			}
			return AndroidJNI.CallStaticIntMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getWidthMethodId, NativeVideoPlayer.EmptyParams);
		}
	}

	// Token: 0x17000221 RID: 545
	// (get) Token: 0x060012BB RID: 4795 RVA: 0x000580D0 File Offset: 0x000562D0
	public static int VideoHeight
	{
		get
		{
			if (NativeVideoPlayer.getHeightMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getHeightMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getHeight", "()I");
			}
			return AndroidJNI.CallStaticIntMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getHeightMethodId, NativeVideoPlayer.EmptyParams);
		}
	}

	// Token: 0x17000222 RID: 546
	// (get) Token: 0x060012BC RID: 4796 RVA: 0x00058110 File Offset: 0x00056310
	// (set) Token: 0x060012BD RID: 4797 RVA: 0x00058150 File Offset: 0x00056350
	public static long PlaybackPosition
	{
		get
		{
			if (NativeVideoPlayer.getPlaybackPositionMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getPlaybackPositionMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getPlaybackPosition", "()J");
			}
			return AndroidJNI.CallStaticLongMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getPlaybackPositionMethodId, NativeVideoPlayer.EmptyParams);
		}
		set
		{
			if (NativeVideoPlayer.setPlaybackPositionMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.setPlaybackPositionMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "setPlaybackPosition", "(J)V");
				NativeVideoPlayer.setPlaybackPositionParams = new jvalue[1];
			}
			NativeVideoPlayer.setPlaybackPositionParams[0].j = value;
			AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.setPlaybackPositionMethodId, NativeVideoPlayer.setPlaybackPositionParams);
		}
	}

	// Token: 0x060012BE RID: 4798 RVA: 0x000581B8 File Offset: 0x000563B8
	public static void PlayVideo(string path, string drmLicenseUrl, IntPtr surfaceObj)
	{
		if (NativeVideoPlayer.playVideoMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.playVideoMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "playVideo", "(Landroid/content/Context;Ljava/lang/String;Ljava/lang/String;Landroid/view/Surface;)V");
			NativeVideoPlayer.playVideoParams = new jvalue[4];
		}
		IntPtr intPtr = AndroidJNI.NewStringUTF(path);
		IntPtr intPtr2 = AndroidJNI.NewStringUTF(drmLicenseUrl);
		NativeVideoPlayer.playVideoParams[0].l = NativeVideoPlayer.Activity;
		NativeVideoPlayer.playVideoParams[1].l = intPtr;
		NativeVideoPlayer.playVideoParams[2].l = intPtr2;
		NativeVideoPlayer.playVideoParams[3].l = surfaceObj;
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.playVideoMethodId, NativeVideoPlayer.playVideoParams);
		AndroidJNI.DeleteLocalRef(intPtr);
		AndroidJNI.DeleteLocalRef(intPtr2);
	}

	// Token: 0x060012BF RID: 4799 RVA: 0x00058270 File Offset: 0x00056470
	public static void Stop()
	{
		if (NativeVideoPlayer.stopMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.stopMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "stop", "()V");
		}
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.stopMethodId, NativeVideoPlayer.EmptyParams);
	}

	// Token: 0x060012C0 RID: 4800 RVA: 0x000582B0 File Offset: 0x000564B0
	public static void Play()
	{
		if (NativeVideoPlayer.resumeMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.resumeMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "resume", "()V");
		}
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.resumeMethodId, NativeVideoPlayer.EmptyParams);
	}

	// Token: 0x060012C1 RID: 4801 RVA: 0x000582F0 File Offset: 0x000564F0
	public static void Pause()
	{
		if (NativeVideoPlayer.pauseMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.pauseMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "pause", "()V");
		}
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.pauseMethodId, NativeVideoPlayer.EmptyParams);
	}

	// Token: 0x060012C2 RID: 4802 RVA: 0x00058330 File Offset: 0x00056530
	public static void SetPlaybackSpeed(float speed)
	{
		if (NativeVideoPlayer.setPlaybackSpeedMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.setPlaybackSpeedMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "setPlaybackSpeed", "(F)V");
			NativeVideoPlayer.setPlaybackSpeedParams = new jvalue[1];
		}
		NativeVideoPlayer.setPlaybackSpeedParams[0].f = speed;
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.setPlaybackSpeedMethodId, NativeVideoPlayer.setPlaybackSpeedParams);
	}

	// Token: 0x060012C3 RID: 4803 RVA: 0x00058398 File Offset: 0x00056598
	public static void SetLooping(bool looping)
	{
		if (NativeVideoPlayer.setLoopingMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.setLoopingMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "setLooping", "(Z)V");
			NativeVideoPlayer.setLoopingParams = new jvalue[1];
		}
		NativeVideoPlayer.setLoopingParams[0].z = looping;
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.setLoopingMethodId, NativeVideoPlayer.setLoopingParams);
	}

	// Token: 0x060012C4 RID: 4804 RVA: 0x00058400 File Offset: 0x00056600
	public static void SetListenerRotation(Quaternion rotation)
	{
		if (NativeVideoPlayer.setListenerRotationQuaternionMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.setListenerRotationQuaternionMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "setListenerRotationQuaternion", "(FFFF)V");
			NativeVideoPlayer.setListenerRotationQuaternionParams = new jvalue[4];
		}
		NativeVideoPlayer.setListenerRotationQuaternionParams[0].f = rotation.x;
		NativeVideoPlayer.setListenerRotationQuaternionParams[1].f = rotation.y;
		NativeVideoPlayer.setListenerRotationQuaternionParams[2].f = rotation.z;
		NativeVideoPlayer.setListenerRotationQuaternionParams[3].f = rotation.w;
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.setListenerRotationQuaternionMethodId, NativeVideoPlayer.setListenerRotationQuaternionParams);
	}

	// Token: 0x040014B5 RID: 5301
	private static IntPtr? _Activity;

	// Token: 0x040014B6 RID: 5302
	private static IntPtr? _VideoPlayerClass;

	// Token: 0x040014B7 RID: 5303
	private static readonly jvalue[] EmptyParams = new jvalue[0];

	// Token: 0x040014B8 RID: 5304
	private static IntPtr getIsPlayingMethodId;

	// Token: 0x040014B9 RID: 5305
	private static IntPtr getCurrentPlaybackStateMethodId;

	// Token: 0x040014BA RID: 5306
	private static IntPtr getDurationMethodId;

	// Token: 0x040014BB RID: 5307
	private static IntPtr getStereoModeMethodId;

	// Token: 0x040014BC RID: 5308
	private static IntPtr getWidthMethodId;

	// Token: 0x040014BD RID: 5309
	private static IntPtr getHeightMethodId;

	// Token: 0x040014BE RID: 5310
	private static IntPtr getPlaybackPositionMethodId;

	// Token: 0x040014BF RID: 5311
	private static IntPtr setPlaybackPositionMethodId;

	// Token: 0x040014C0 RID: 5312
	private static jvalue[] setPlaybackPositionParams;

	// Token: 0x040014C1 RID: 5313
	private static IntPtr playVideoMethodId;

	// Token: 0x040014C2 RID: 5314
	private static jvalue[] playVideoParams;

	// Token: 0x040014C3 RID: 5315
	private static IntPtr stopMethodId;

	// Token: 0x040014C4 RID: 5316
	private static IntPtr resumeMethodId;

	// Token: 0x040014C5 RID: 5317
	private static IntPtr pauseMethodId;

	// Token: 0x040014C6 RID: 5318
	private static IntPtr setPlaybackSpeedMethodId;

	// Token: 0x040014C7 RID: 5319
	private static jvalue[] setPlaybackSpeedParams;

	// Token: 0x040014C8 RID: 5320
	private static IntPtr setLoopingMethodId;

	// Token: 0x040014C9 RID: 5321
	private static jvalue[] setLoopingParams;

	// Token: 0x040014CA RID: 5322
	private static IntPtr setListenerRotationQuaternionMethodId;

	// Token: 0x040014CB RID: 5323
	private static jvalue[] setListenerRotationQuaternionParams;

	// Token: 0x02000312 RID: 786
	public enum PlabackState
	{
		// Token: 0x040014CD RID: 5325
		Idle = 1,
		// Token: 0x040014CE RID: 5326
		Preparing,
		// Token: 0x040014CF RID: 5327
		Buffering,
		// Token: 0x040014D0 RID: 5328
		Ready,
		// Token: 0x040014D1 RID: 5329
		Ended
	}

	// Token: 0x02000313 RID: 787
	public enum StereoMode
	{
		// Token: 0x040014D3 RID: 5331
		Unknown = -1,
		// Token: 0x040014D4 RID: 5332
		Mono,
		// Token: 0x040014D5 RID: 5333
		TopBottom,
		// Token: 0x040014D6 RID: 5334
		LeftRight,
		// Token: 0x040014D7 RID: 5335
		Mesh
	}
}
