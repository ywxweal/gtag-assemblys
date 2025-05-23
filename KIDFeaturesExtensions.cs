using System;

// Token: 0x020007B1 RID: 1969
public static class KIDFeaturesExtensions
{
	// Token: 0x060030C3 RID: 12483 RVA: 0x000EFBCC File Offset: 0x000EDDCC
	public static string ToStandardisedString(this EKIDFeatures feature)
	{
		switch (feature)
		{
		case EKIDFeatures.Multiplayer:
			return "multiplayer";
		case EKIDFeatures.Custom_Nametags:
			return "custom-username";
		case EKIDFeatures.Voice_Chat:
			return "voice-chat";
		case EKIDFeatures.Mods:
			return "mods";
		case EKIDFeatures.Groups:
			return "join-groups";
		default:
			return feature.ToString();
		}
	}

	// Token: 0x060030C4 RID: 12484 RVA: 0x000EFC20 File Offset: 0x000EDE20
	public static EKIDFeatures? FromString(string name)
	{
		string text = name.ToLower();
		if (text == "voice-chat")
		{
			return new EKIDFeatures?(EKIDFeatures.Voice_Chat);
		}
		if (text == "custom-username")
		{
			return new EKIDFeatures?(EKIDFeatures.Custom_Nametags);
		}
		if (text == "multiplayer")
		{
			return new EKIDFeatures?(EKIDFeatures.Multiplayer);
		}
		if (text == "mods")
		{
			return new EKIDFeatures?(EKIDFeatures.Mods);
		}
		if (!(text == "join-groups"))
		{
			return null;
		}
		return new EKIDFeatures?(EKIDFeatures.Groups);
	}

	// Token: 0x060030C5 RID: 12485 RVA: 0x000EFCA4 File Offset: 0x000EDEA4
	public static bool TryGetFromString(string name, out EKIDFeatures result)
	{
		EKIDFeatures? ekidfeatures = KIDFeaturesExtensions.FromString(name);
		if (ekidfeatures != null)
		{
			result = ekidfeatures.Value;
			return true;
		}
		result = EKIDFeatures.Voice_Chat;
		return false;
	}
}
