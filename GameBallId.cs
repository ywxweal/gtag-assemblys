using System;

// Token: 0x020004B2 RID: 1202
public struct GameBallId
{
	// Token: 0x06001D00 RID: 7424 RVA: 0x0008C7B9 File Offset: 0x0008A9B9
	public GameBallId(int index)
	{
		this.index = index;
	}

	// Token: 0x06001D01 RID: 7425 RVA: 0x0008C7C2 File Offset: 0x0008A9C2
	public bool IsValid()
	{
		return this.index != -1;
	}

	// Token: 0x06001D02 RID: 7426 RVA: 0x0008C7D0 File Offset: 0x0008A9D0
	public static bool operator ==(GameBallId obj1, GameBallId obj2)
	{
		return obj1.index == obj2.index;
	}

	// Token: 0x06001D03 RID: 7427 RVA: 0x0008C7E0 File Offset: 0x0008A9E0
	public static bool operator !=(GameBallId obj1, GameBallId obj2)
	{
		return obj1.index != obj2.index;
	}

	// Token: 0x06001D04 RID: 7428 RVA: 0x0008C7F4 File Offset: 0x0008A9F4
	public override bool Equals(object obj)
	{
		GameBallId gameBallId = (GameBallId)obj;
		return this.index == gameBallId.index;
	}

	// Token: 0x06001D05 RID: 7429 RVA: 0x0008C816 File Offset: 0x0008AA16
	public override int GetHashCode()
	{
		return this.index.GetHashCode();
	}

	// Token: 0x04002040 RID: 8256
	public static GameBallId Invalid = new GameBallId(-1);

	// Token: 0x04002041 RID: 8257
	public int index;
}
