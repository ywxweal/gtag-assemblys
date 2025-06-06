﻿using System;
using Drawing;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000759 RID: 1881
public class GizmoRenderer : MonoBehaviour
{
	// Token: 0x06002EF8 RID: 12024 RVA: 0x000EB0AC File Offset: 0x000E92AC
	private void Update()
	{
		this.RenderGizmos();
	}

	// Token: 0x06002EF9 RID: 12025 RVA: 0x000EB0B4 File Offset: 0x000E92B4
	private unsafe void RenderGizmos()
	{
		if (this.renderMode == GizmoRenderer.RenderMode.Never)
		{
			return;
		}
		if (this.gizmos == null)
		{
			return;
		}
		int num = this.gizmos.Length;
		if (num == 0)
		{
			return;
		}
		CommandBuilder commandBuilder = *Draw.ingame;
		Transform transform = base.transform;
		for (int i = 0; i < num; i++)
		{
			GizmoRenderer.GizmoInfo gizmoInfo = this.gizmos[i];
			if (gizmoInfo.render)
			{
				Transform transform2 = (gizmoInfo.target ? gizmoInfo.target : transform);
				using (commandBuilder.InLocalSpace(transform2))
				{
					using (commandBuilder.WithLineWidth(gizmoInfo.lineWidth, false))
					{
						GizmoRenderer.gRenderFuncs[(int)gizmoInfo.type](commandBuilder, gizmoInfo);
					}
				}
			}
		}
	}

	// Token: 0x06002EFA RID: 12026 RVA: 0x000EB1A0 File Offset: 0x000E93A0
	private static void RenderPlaneWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WirePlane(gizmo.center, gizmo.rotation, gizmo.size.xz, gizmo.color);
	}

	// Token: 0x06002EFB RID: 12027 RVA: 0x000EB1C6 File Offset: 0x000E93C6
	private static void RenderPlaneSolid(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.SolidPlane(gizmo.center, gizmo.rotation, gizmo.size.xz, gizmo.color);
	}

	// Token: 0x06002EFC RID: 12028 RVA: 0x000EB1EC File Offset: 0x000E93EC
	private static void RenderGridWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WireGrid(gizmo.center, gizmo.rotation, gizmo.gridCells, gizmo.size.xz, gizmo.color);
	}

	// Token: 0x06002EFD RID: 12029 RVA: 0x000EB218 File Offset: 0x000E9418
	private static void RenderBoxWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WireBox(gizmo.center, gizmo.rotation, gizmo.size, gizmo.color);
	}

	// Token: 0x06002EFE RID: 12030 RVA: 0x000EB239 File Offset: 0x000E9439
	private static void RenderBoxSolid(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.SolidBox(gizmo.center, gizmo.rotation, gizmo.size, gizmo.color);
	}

	// Token: 0x06002EFF RID: 12031 RVA: 0x000EB25A File Offset: 0x000E945A
	private static void RenderSphereWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WireSphere(gizmo.center, gizmo.radius * 0.5f, gizmo.color);
	}

	// Token: 0x06002F00 RID: 12032 RVA: 0x000EB27C File Offset: 0x000E947C
	private static void RenderSphereSolid(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		Matrix4x4 matrix4x = Matrix4x4.TRS(gizmo.center, quaternion.identity, new float3(gizmo.radius));
		using (draw.WithMatrix(matrix4x))
		{
			draw.SolidMesh(GizmoRenderer.gSphereMesh, gizmo.color);
		}
	}

	// Token: 0x06002F01 RID: 12033 RVA: 0x000EB2F0 File Offset: 0x000E94F0
	private static void RenderLabel3D(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.Label3D(gizmo.center, gizmo.rotation, gizmo.text, gizmo.textSize * 0.1f, GizmoRenderer.gLabelAligns[(int)gizmo.textAlign], gizmo.color);
	}

	// Token: 0x06002F02 RID: 12034 RVA: 0x000EB32D File Offset: 0x000E952D
	private static void RenderLabel2D(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.Label2D(gizmo.center, gizmo.text, gizmo.textSize * gizmo.textPPU, GizmoRenderer.gLabelAligns[(int)gizmo.textAlign], gizmo.color);
	}

	// Token: 0x06002F03 RID: 12035 RVA: 0x000EB367 File Offset: 0x000E9567
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitializeOnLoad()
	{
		GizmoRenderer.gSphereMesh = Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");
	}

	// Token: 0x06002F04 RID: 12036 RVA: 0x000EB378 File Offset: 0x000E9578
	private static Color GetRandomColor()
	{
		Color color = Color.HSVToRGB((float)(DateTime.UtcNow.Ticks % 65536L) / 65535f, 1f, 1f, true);
		color.a = 1f;
		return color;
	}

	// Token: 0x04003564 RID: 13668
	public GizmoRenderer.RenderMode renderMode = GizmoRenderer.RenderMode.Always;

	// Token: 0x04003565 RID: 13669
	public bool includeInBuild;

	// Token: 0x04003566 RID: 13670
	public GizmoRenderer.GizmoInfo[] gizmos = new GizmoRenderer.GizmoInfo[0];

	// Token: 0x04003567 RID: 13671
	private static readonly Action<CommandBuilder, GizmoRenderer.GizmoInfo>[] gRenderFuncs = new Action<CommandBuilder, GizmoRenderer.GizmoInfo>[]
	{
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderBoxWire),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderBoxSolid),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderSphereWire),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderSphereSolid),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderLabel3D),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderLabel2D),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderGridWire),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderPlaneSolid),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderPlaneWire)
	};

	// Token: 0x04003568 RID: 13672
	private static readonly LabelAlignment[] gLabelAligns = new LabelAlignment[]
	{
		LabelAlignment.Center,
		LabelAlignment.MiddleRight,
		LabelAlignment.MiddleLeft,
		LabelAlignment.BottomCenter,
		LabelAlignment.BottomRight,
		LabelAlignment.BottomLeft,
		LabelAlignment.TopRight,
		LabelAlignment.TopLeft,
		LabelAlignment.TopCenter
	};

	// Token: 0x04003569 RID: 13673
	private static Mesh gSphereMesh;

	// Token: 0x0200075A RID: 1882
	[Serializable]
	public class GizmoInfo
	{
		// Token: 0x0400356A RID: 13674
		public bool render = true;

		// Token: 0x0400356B RID: 13675
		public GizmoRenderer.GizmoType type;

		// Token: 0x0400356C RID: 13676
		public Color color = GizmoRenderer.GetRandomColor();

		// Token: 0x0400356D RID: 13677
		public uint lineWidth = 1U;

		// Token: 0x0400356E RID: 13678
		[Space]
		public Transform target;

		// Token: 0x0400356F RID: 13679
		[Space]
		public float3 center = float3.zero;

		// Token: 0x04003570 RID: 13680
		public float3 size = Vector3.one;

		// Token: 0x04003571 RID: 13681
		public float radius = 1f;

		// Token: 0x04003572 RID: 13682
		public quaternion rotation = quaternion.identity;

		// Token: 0x04003573 RID: 13683
		[Space]
		public string text = string.Empty;

		// Token: 0x04003574 RID: 13684
		public float textSize = 4f;

		// Token: 0x04003575 RID: 13685
		public GizmoRenderer.TextAlign textAlign;

		// Token: 0x04003576 RID: 13686
		public uint textPPU = 24U;

		// Token: 0x04003577 RID: 13687
		[Space]
		public int2 gridCells = new int2(4);
	}

	// Token: 0x0200075B RID: 1883
	[Flags]
	public enum RenderMode : uint
	{
		// Token: 0x04003579 RID: 13689
		Never = 0U,
		// Token: 0x0400357A RID: 13690
		InEditor = 1U,
		// Token: 0x0400357B RID: 13691
		InBuild = 2U,
		// Token: 0x0400357C RID: 13692
		Always = 3U
	}

	// Token: 0x0200075C RID: 1884
	public enum GizmoType : uint
	{
		// Token: 0x0400357E RID: 13694
		BoxWire,
		// Token: 0x0400357F RID: 13695
		BoxSolid,
		// Token: 0x04003580 RID: 13696
		SphereWire,
		// Token: 0x04003581 RID: 13697
		SphereSolid,
		// Token: 0x04003582 RID: 13698
		Label3D,
		// Token: 0x04003583 RID: 13699
		Label2D,
		// Token: 0x04003584 RID: 13700
		GridWire,
		// Token: 0x04003585 RID: 13701
		PlaneSolid,
		// Token: 0x04003586 RID: 13702
		PlaneWire
	}

	// Token: 0x0200075D RID: 1885
	public enum TextAlign : uint
	{
		// Token: 0x04003588 RID: 13704
		Center,
		// Token: 0x04003589 RID: 13705
		MiddleRight,
		// Token: 0x0400358A RID: 13706
		MiddleLeft,
		// Token: 0x0400358B RID: 13707
		BottomCenter,
		// Token: 0x0400358C RID: 13708
		BottomRight,
		// Token: 0x0400358D RID: 13709
		BottomLeft,
		// Token: 0x0400358E RID: 13710
		TopRight,
		// Token: 0x0400358F RID: 13711
		TopLeft,
		// Token: 0x04003590 RID: 13712
		TopCenter
	}
}
