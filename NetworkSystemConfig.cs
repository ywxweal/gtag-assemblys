using System;
using UnityEngine;

// Token: 0x020002A9 RID: 681
[Serializable]
public struct NetworkSystemConfig
{
	// Token: 0x170001A9 RID: 425
	// (get) Token: 0x06000FEF RID: 4079 RVA: 0x0004F1F9 File Offset: 0x0004D3F9
	public static string AppVersion
	{
		get
		{
			return NetworkSystemConfig.prependCode + "." + NetworkSystemConfig.AppVersionStripped;
		}
	}

	// Token: 0x170001AA RID: 426
	// (get) Token: 0x06000FF0 RID: 4080 RVA: 0x0004F210 File Offset: 0x0004D410
	public static string AppVersionStripped
	{
		get
		{
			return string.Concat(new string[]
			{
				NetworkSystemConfig.gameVersionType,
				".",
				NetworkSystemConfig.majorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion2.ToString()
			});
		}
	}

	// Token: 0x170001AB RID: 427
	// (get) Token: 0x06000FF1 RID: 4081 RVA: 0x0004F270 File Offset: 0x0004D470
	public static string BundleVersion
	{
		get
		{
			return string.Concat(new string[]
			{
				NetworkSystemConfig.majorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion2.ToString()
			});
		}
	}

	// Token: 0x170001AC RID: 428
	// (get) Token: 0x06000FF2 RID: 4082 RVA: 0x0004F2BF File Offset: 0x0004D4BF
	public static string GameVersionType
	{
		get
		{
			return NetworkSystemConfig.gameVersionType;
		}
	}

	// Token: 0x170001AD RID: 429
	// (get) Token: 0x06000FF3 RID: 4083 RVA: 0x0004F2C6 File Offset: 0x0004D4C6
	public static int GameMajorVersion
	{
		get
		{
			return NetworkSystemConfig.majorVersion;
		}
	}

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x06000FF4 RID: 4084 RVA: 0x0004F2CD File Offset: 0x0004D4CD
	public static int GameMinorVersion
	{
		get
		{
			return NetworkSystemConfig.minorVersion;
		}
	}

	// Token: 0x170001AF RID: 431
	// (get) Token: 0x06000FF5 RID: 4085 RVA: 0x0004F2D4 File Offset: 0x0004D4D4
	public static int GameMinorVersion2
	{
		get
		{
			return NetworkSystemConfig.minorVersion2;
		}
	}

	// Token: 0x040012B6 RID: 4790
	[HideInInspector]
	public int MaxPlayerCount;

	// Token: 0x040012B7 RID: 4791
	private static string gameVersionType = "live1";

	// Token: 0x040012B8 RID: 4792
	public static string prependCode = "BuildingSharingPlayingPrepend36987";

	// Token: 0x040012B9 RID: 4793
	public static int majorVersion = 1;

	// Token: 0x040012BA RID: 4794
	public static int minorVersion = 1;

	// Token: 0x040012BB RID: 4795
	public static int minorVersion2 = 110;
}
