using System;
using Unity.Mathematics;

// Token: 0x020001A2 RID: 418
public static class CosmeticIDUtils
{
	// Token: 0x06000A56 RID: 2646 RVA: 0x00036154 File Offset: 0x00034354
	public static int PlayFabIdToIndexInCategory(string playFabIdString)
	{
		if (playFabIdString == null || playFabIdString.Length < 6)
		{
			throw new ArgumentException("PlayFabIdToIndexInCategory: playFabId cannot be null or less than 6 characters.");
		}
		if (playFabIdString[0] != 'L' || playFabIdString[playFabIdString.Length - 1] != '.')
		{
			throw new ArgumentException("PlayFabIdToIndexInCategory: playFabId must start with 'L' and end with '.', instead got " + playFabIdString + ".");
		}
		int num = playFabIdString.Length - 2;
		int num2 = 0;
		for (int i = 2; i <= num; i++)
		{
			char c = playFabIdString[i];
			if (c < 'A' || c > 'Z')
			{
				throw new ArgumentException("String must contain only uppercase letters A-Z.");
			}
			int num3 = (int)(playFabIdString[i] - 'A');
			num2 += num3 * (int)math.pow(26f, (float)(num - i));
		}
		return num2;
	}
}
