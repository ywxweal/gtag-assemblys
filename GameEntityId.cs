using System;

// Token: 0x02000563 RID: 1379
public struct GameEntityId
{
	// Token: 0x0600217D RID: 8573 RVA: 0x000A75D2 File Offset: 0x000A57D2
	public bool IsValid()
	{
		return this.index != -1;
	}

	// Token: 0x0600217E RID: 8574 RVA: 0x000A75E0 File Offset: 0x000A57E0
	public int GetNetId()
	{
		return GameEntityManager.instance.GetNetIdFromEntityId(this);
	}

	// Token: 0x0600217F RID: 8575 RVA: 0x000A75F4 File Offset: 0x000A57F4
	public static bool operator ==(GameEntityId obj1, GameEntityId obj2)
	{
		return obj1.index == obj2.index;
	}

	// Token: 0x06002180 RID: 8576 RVA: 0x000A7604 File Offset: 0x000A5804
	public static bool operator !=(GameEntityId obj1, GameEntityId obj2)
	{
		return obj1.index != obj2.index;
	}

	// Token: 0x06002181 RID: 8577 RVA: 0x000A7618 File Offset: 0x000A5818
	public override bool Equals(object obj)
	{
		GameEntityId gameEntityId = (GameEntityId)obj;
		return this.index == gameEntityId.index;
	}

	// Token: 0x06002182 RID: 8578 RVA: 0x000A763A File Offset: 0x000A583A
	public override int GetHashCode()
	{
		return this.index.GetHashCode();
	}

	// Token: 0x040025B5 RID: 9653
	public static GameEntityId Invalid = new GameEntityId
	{
		index = -1
	};

	// Token: 0x040025B6 RID: 9654
	public int index;
}
