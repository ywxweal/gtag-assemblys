using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020004FD RID: 1277
[CreateAssetMenu(fileName = "BuilderPieceSet01", menuName = "Gorilla Tag/Builder/PieceSet", order = 0)]
public class BuilderPieceSet : ScriptableObject
{
	// Token: 0x06001F22 RID: 7970 RVA: 0x0009AD5D File Offset: 0x00098F5D
	public int GetIntIdentifier()
	{
		return this.playfabID.GetStaticHash();
	}

	// Token: 0x06001F23 RID: 7971 RVA: 0x0009AD6C File Offset: 0x00098F6C
	public DateTime GetScheduleDateTime()
	{
		if (this.isScheduled)
		{
			try
			{
				return DateTime.Parse(this.scheduledDate, CultureInfo.InvariantCulture);
			}
			catch
			{
				return DateTime.MinValue;
			}
		}
		return DateTime.MinValue;
	}

	// Token: 0x0400229D RID: 8861
	[Tooltip("Display Name")]
	public string setName;

	// Token: 0x0400229E RID: 8862
	public GameObject displayModel;

	// Token: 0x0400229F RID: 8863
	[FormerlySerializedAs("uniqueId")]
	[Tooltip("Playfab ID")]
	public string playfabID;

	// Token: 0x040022A0 RID: 8864
	public string materialId;

	// Token: 0x040022A1 RID: 8865
	public bool isScheduled;

	// Token: 0x040022A2 RID: 8866
	public string scheduledDate = "1/1/0001 00:00:00";

	// Token: 0x040022A3 RID: 8867
	public List<BuilderPieceSet.BuilderPieceSubset> subsets;

	// Token: 0x020004FE RID: 1278
	public enum BuilderPieceCategory
	{
		// Token: 0x040022A5 RID: 8869
		FLAT,
		// Token: 0x040022A6 RID: 8870
		TALL,
		// Token: 0x040022A7 RID: 8871
		HALF_HEIGHT,
		// Token: 0x040022A8 RID: 8872
		BEAM,
		// Token: 0x040022A9 RID: 8873
		SLOPE,
		// Token: 0x040022AA RID: 8874
		OVERSIZED,
		// Token: 0x040022AB RID: 8875
		SPECIAL_DISPLAY,
		// Token: 0x040022AC RID: 8876
		FUNCTIONAL = 18,
		// Token: 0x040022AD RID: 8877
		DECORATIVE,
		// Token: 0x040022AE RID: 8878
		MISC
	}

	// Token: 0x020004FF RID: 1279
	[Serializable]
	public class BuilderPieceSubset
	{
		// Token: 0x040022AF RID: 8879
		public string subsetName;

		// Token: 0x040022B0 RID: 8880
		public BuilderPieceSet.BuilderPieceCategory pieceCategory;

		// Token: 0x040022B1 RID: 8881
		public List<BuilderPieceSet.PieceInfo> pieceInfos;
	}

	// Token: 0x02000500 RID: 1280
	[Serializable]
	public struct PieceInfo
	{
		// Token: 0x040022B2 RID: 8882
		public BuilderPiece piecePrefab;

		// Token: 0x040022B3 RID: 8883
		public bool overrideSetMaterial;

		// Token: 0x040022B4 RID: 8884
		public string[] pieceMaterialTypes;
	}
}
