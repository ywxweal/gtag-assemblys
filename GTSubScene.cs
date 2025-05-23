using System;
using UnityEngine;

// Token: 0x020009EA RID: 2538
public class GTSubScene : ScriptableObject
{
	// Token: 0x06003CAF RID: 15535 RVA: 0x00121308 File Offset: 0x0011F508
	public void SwitchToScene(int index)
	{
		this.scenes[index].LoadAsync();
	}

	// Token: 0x06003CB0 RID: 15536 RVA: 0x00121318 File Offset: 0x0011F518
	public void SwitchToScene(GTScene scene)
	{
		for (int i = 0; i < this.scenes.Length; i++)
		{
			GTScene gtscene = this.scenes[i];
			if (!(scene == gtscene))
			{
				gtscene.UnloadAsync();
			}
		}
		scene.LoadAsync();
	}

	// Token: 0x06003CB1 RID: 15537 RVA: 0x00121358 File Offset: 0x0011F558
	public void LoadAll()
	{
		for (int i = 0; i < this.scenes.Length; i++)
		{
			this.scenes[i].LoadAsync();
		}
	}

	// Token: 0x06003CB2 RID: 15538 RVA: 0x00121388 File Offset: 0x0011F588
	public void UnloadAll()
	{
		for (int i = 0; i < this.scenes.Length; i++)
		{
			this.scenes[i].UnloadAsync();
		}
	}

	// Token: 0x04004088 RID: 16520
	[DragDropScenes]
	public GTScene[] scenes = new GTScene[0];
}
