﻿using System;
using System.IO;
using UnityEngine;

// Token: 0x020004F3 RID: 1267
[Serializable]
public struct SnapBounds
{
	// Token: 0x06001E9F RID: 7839 RVA: 0x0009564A File Offset: 0x0009384A
	public SnapBounds(Vector2Int min, Vector2Int max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x06001EA0 RID: 7840 RVA: 0x0009565A File Offset: 0x0009385A
	public SnapBounds(int minX, int minY, int maxX, int maxY)
	{
		this.min = new Vector2Int(minX, minY);
		this.max = new Vector2Int(maxX, maxY);
	}

	// Token: 0x06001EA1 RID: 7841 RVA: 0x00095677 File Offset: 0x00093877
	public void Clear()
	{
		this.min = new Vector2Int(int.MinValue, int.MinValue);
		this.max = new Vector2Int(int.MinValue, int.MinValue);
	}

	// Token: 0x06001EA2 RID: 7842 RVA: 0x000956A4 File Offset: 0x000938A4
	public void Write(BinaryWriter writer)
	{
		writer.Write(this.min.x);
		writer.Write(this.min.y);
		writer.Write(this.max.x);
		writer.Write(this.max.y);
	}

	// Token: 0x06001EA3 RID: 7843 RVA: 0x000956F8 File Offset: 0x000938F8
	public void Read(BinaryReader reader)
	{
		this.min.x = reader.ReadInt32();
		this.min.y = reader.ReadInt32();
		this.max.x = reader.ReadInt32();
		this.max.y = reader.ReadInt32();
	}

	// Token: 0x040021EE RID: 8686
	public Vector2Int min;

	// Token: 0x040021EF RID: 8687
	public Vector2Int max;
}
