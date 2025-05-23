using System;
using ExitGames.Client.Photon;
using Photon.Voice;
using Photon.Voice.Unity;
using POpusCodec.Enums;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000C0B RID: 3083
	[CreateAssetMenu(fileName = "VoiceSettings", menuName = "Gorilla Tag/VoiceSettings")]
	public class SO_NetworkVoiceSettings : ScriptableObject
	{
		// Token: 0x04004EFB RID: 20219
		[Header("Voice settings")]
		public bool AutoConnectAndJoin = true;

		// Token: 0x04004EFC RID: 20220
		public bool AutoLeaveAndDisconnect = true;

		// Token: 0x04004EFD RID: 20221
		public bool WorkInOfflineMode = true;

		// Token: 0x04004EFE RID: 20222
		public DebugLevel LogLevel = DebugLevel.ERROR;

		// Token: 0x04004EFF RID: 20223
		public DebugLevel GlobalRecordersLogLevel = DebugLevel.INFO;

		// Token: 0x04004F00 RID: 20224
		public DebugLevel GlobalSpeakersLogLevel = DebugLevel.INFO;

		// Token: 0x04004F01 RID: 20225
		public bool CreateSpeakerIfNotFound;

		// Token: 0x04004F02 RID: 20226
		public int UpdateInterval = 50;

		// Token: 0x04004F03 RID: 20227
		public bool SupportLogger;

		// Token: 0x04004F04 RID: 20228
		public int BackgroundTimeout = 60000;

		// Token: 0x04004F05 RID: 20229
		[Header("Recorder Settings")]
		public bool RecordOnlyWhenEnabled;

		// Token: 0x04004F06 RID: 20230
		public bool RecordOnlyWhenJoined = true;

		// Token: 0x04004F07 RID: 20231
		public bool StopRecordingWhenPaused;

		// Token: 0x04004F08 RID: 20232
		public bool TransmitEnabled = true;

		// Token: 0x04004F09 RID: 20233
		public bool AutoStart = true;

		// Token: 0x04004F0A RID: 20234
		public bool Encrypt;

		// Token: 0x04004F0B RID: 20235
		public byte InterestGroup;

		// Token: 0x04004F0C RID: 20236
		public bool DebugEcho;

		// Token: 0x04004F0D RID: 20237
		public bool ReliableMode;

		// Token: 0x04004F0E RID: 20238
		[Header("Recorder Codec Parameters")]
		public OpusCodec.FrameDuration FrameDuration = OpusCodec.FrameDuration.Frame60ms;

		// Token: 0x04004F0F RID: 20239
		public SamplingRate SamplingRate = SamplingRate.Sampling16000;

		// Token: 0x04004F10 RID: 20240
		[Range(6000f, 510000f)]
		public int Bitrate = 20000;

		// Token: 0x04004F11 RID: 20241
		[Header("Recorder Audio Source Settings")]
		public Recorder.InputSourceType InputSourceType;

		// Token: 0x04004F12 RID: 20242
		public Recorder.MicType MicrophoneType;

		// Token: 0x04004F13 RID: 20243
		public bool UseFallback = true;

		// Token: 0x04004F14 RID: 20244
		public bool Detect = true;

		// Token: 0x04004F15 RID: 20245
		[Range(0f, 1f)]
		public float Threshold = 0.07f;

		// Token: 0x04004F16 RID: 20246
		public int Delay = 500;
	}
}
