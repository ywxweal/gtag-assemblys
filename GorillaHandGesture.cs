using System;
using UnityEngine;

// Token: 0x02000181 RID: 385
[CreateAssetMenu(fileName = "New Hand Gesture", menuName = "Gorilla/Hand Gesture")]
public class GorillaHandGesture : ScriptableObject
{
	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x06000982 RID: 2434 RVA: 0x0003347C File Offset: 0x0003167C
	// (set) Token: 0x06000983 RID: 2435 RVA: 0x0003348B File Offset: 0x0003168B
	public GestureHandNode hand
	{
		get
		{
			return (GestureHandNode)this.nodes[0];
		}
		set
		{
			this.nodes[0] = value;
		}
	}

	// Token: 0x170000E2 RID: 226
	// (get) Token: 0x06000984 RID: 2436 RVA: 0x00033496 File Offset: 0x00031696
	// (set) Token: 0x06000985 RID: 2437 RVA: 0x000334A0 File Offset: 0x000316A0
	public GestureNode palm
	{
		get
		{
			return this.nodes[1];
		}
		set
		{
			this.nodes[1] = value;
		}
	}

	// Token: 0x170000E3 RID: 227
	// (get) Token: 0x06000986 RID: 2438 RVA: 0x000334AB File Offset: 0x000316AB
	// (set) Token: 0x06000987 RID: 2439 RVA: 0x000334B5 File Offset: 0x000316B5
	public GestureNode wrist
	{
		get
		{
			return this.nodes[2];
		}
		set
		{
			this.nodes[2] = value;
		}
	}

	// Token: 0x170000E4 RID: 228
	// (get) Token: 0x06000988 RID: 2440 RVA: 0x000334C0 File Offset: 0x000316C0
	// (set) Token: 0x06000989 RID: 2441 RVA: 0x000334CA File Offset: 0x000316CA
	public GestureNode digits
	{
		get
		{
			return this.nodes[3];
		}
		set
		{
			this.nodes[3] = value;
		}
	}

	// Token: 0x170000E5 RID: 229
	// (get) Token: 0x0600098A RID: 2442 RVA: 0x000334D5 File Offset: 0x000316D5
	// (set) Token: 0x0600098B RID: 2443 RVA: 0x000334E4 File Offset: 0x000316E4
	public GestureDigitNode thumb
	{
		get
		{
			return (GestureDigitNode)this.nodes[4];
		}
		set
		{
			this.nodes[4] = value;
		}
	}

	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x0600098C RID: 2444 RVA: 0x000334EF File Offset: 0x000316EF
	// (set) Token: 0x0600098D RID: 2445 RVA: 0x000334FE File Offset: 0x000316FE
	public GestureDigitNode index
	{
		get
		{
			return (GestureDigitNode)this.nodes[5];
		}
		set
		{
			this.nodes[5] = value;
		}
	}

	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x0600098E RID: 2446 RVA: 0x00033509 File Offset: 0x00031709
	// (set) Token: 0x0600098F RID: 2447 RVA: 0x00033518 File Offset: 0x00031718
	public GestureDigitNode middle
	{
		get
		{
			return (GestureDigitNode)this.nodes[6];
		}
		set
		{
			this.nodes[6] = value;
		}
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x00033523 File Offset: 0x00031723
	private static GestureNode[] InitNodes()
	{
		return new GestureNode[]
		{
			new GestureHandNode(),
			new GestureNode(),
			new GestureNode(),
			new GestureNode(),
			new GestureDigitNode(),
			new GestureDigitNode(),
			new GestureDigitNode()
		};
	}

	// Token: 0x04000BA0 RID: 2976
	public bool track = true;

	// Token: 0x04000BA1 RID: 2977
	public GestureNode[] nodes = GorillaHandGesture.InitNodes();
}
