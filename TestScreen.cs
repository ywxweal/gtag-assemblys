using System;
using UnityEngine;

// Token: 0x02000271 RID: 625
public class TestScreen : ArcadeGame
{
	// Token: 0x06000E6D RID: 3693 RVA: 0x00045F91 File Offset: 0x00044191
	public override byte[] GetNetworkState()
	{
		return null;
	}

	// Token: 0x06000E6E RID: 3694 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void SetNetworkState(byte[] b)
	{
	}

	// Token: 0x06000E6F RID: 3695 RVA: 0x00048FC4 File Offset: 0x000471C4
	private int buttonToLightIndex(int player, ArcadeButtons button)
	{
		int num = 0;
		if (button <= ArcadeButtons.RIGHT)
		{
			switch (button)
			{
			case ArcadeButtons.GRAB:
				num = 0;
				break;
			case ArcadeButtons.UP:
				num = 1;
				break;
			case ArcadeButtons.GRAB | ArcadeButtons.UP:
				break;
			case ArcadeButtons.DOWN:
				num = 2;
				break;
			default:
				if (button != ArcadeButtons.LEFT)
				{
					if (button == ArcadeButtons.RIGHT)
					{
						num = 4;
					}
				}
				else
				{
					num = 3;
				}
				break;
			}
		}
		else if (button != ArcadeButtons.B0)
		{
			if (button != ArcadeButtons.B1)
			{
				if (button == ArcadeButtons.TRIGGER)
				{
					num = 7;
				}
			}
			else
			{
				num = 6;
			}
		}
		else
		{
			num = 5;
		}
		return (player * 8 + num) % this.lights.Length;
	}

	// Token: 0x06000E70 RID: 3696 RVA: 0x0004903B File Offset: 0x0004723B
	protected override void ButtonUp(int player, ArcadeButtons button)
	{
		this.lights[this.buttonToLightIndex(player, button)].color = Color.red;
	}

	// Token: 0x06000E71 RID: 3697 RVA: 0x00049056 File Offset: 0x00047256
	protected override void ButtonDown(int player, ArcadeButtons button)
	{
		this.lights[this.buttonToLightIndex(player, button)].color = Color.green;
	}

	// Token: 0x06000E72 RID: 3698 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void OnTimeout()
	{
	}

	// Token: 0x040011AB RID: 4523
	[SerializeField]
	private SpriteRenderer[] lights;

	// Token: 0x040011AC RID: 4524
	[SerializeField]
	private Transform dot;
}
