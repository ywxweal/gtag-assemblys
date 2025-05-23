using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E39 RID: 3641
	public class GizmosUtil
	{
		// Token: 0x06005B03 RID: 23299 RVA: 0x001BB197 File Offset: 0x001B9397
		public static void DrawLine(Vector3 v0, Vector3 v1, Color color)
		{
			Gizmos.color = color;
			Gizmos.DrawLine(v0, v1);
		}

		// Token: 0x06005B04 RID: 23300 RVA: 0x001BB1A8 File Offset: 0x001B93A8
		public static void DrawLines(Vector3[] aVert, Color color)
		{
			Gizmos.color = color;
			for (int i = 0; i < aVert.Length; i += 2)
			{
				Gizmos.DrawLine(aVert[i], aVert[i + 1]);
			}
		}

		// Token: 0x06005B05 RID: 23301 RVA: 0x001BB1E0 File Offset: 0x001B93E0
		public static void DrawLineStrip(Vector3[] aVert, Color color)
		{
			Gizmos.color = color;
			for (int i = 0; i < aVert.Length; i++)
			{
				Gizmos.DrawLine(aVert[i], aVert[i + 1]);
			}
		}

		// Token: 0x06005B06 RID: 23302 RVA: 0x001BB218 File Offset: 0x001B9418
		public static void DrawBox(Vector3 center, Quaternion rotation, Vector3 dimensions, Color color, GizmosUtil.Style style = GizmosUtil.Style.FlatShaded)
		{
			if (dimensions.x < MathUtil.Epsilon || dimensions.y < MathUtil.Epsilon || dimensions.z < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			if (style != GizmosUtil.Style.Wireframe)
			{
				if (style - GizmosUtil.Style.FlatShaded <= 1)
				{
					mesh = PrimitiveMeshFactory.BoxFlatShaded();
				}
			}
			else
			{
				mesh = PrimitiveMeshFactory.BoxWireframe();
			}
			if (mesh == null)
			{
				return;
			}
			Gizmos.color = color;
			if (style == GizmosUtil.Style.Wireframe)
			{
				Gizmos.DrawWireMesh(mesh, center, rotation, dimensions);
				return;
			}
			Gizmos.DrawMesh(mesh, center, rotation, dimensions);
		}

		// Token: 0x06005B07 RID: 23303 RVA: 0x001BB294 File Offset: 0x001B9494
		public static void DrawCylinder(Vector3 center, Quaternion rotation, float height, float radius, int numSegments, Color color, GizmosUtil.Style style = GizmosUtil.Style.SmoothShaded)
		{
			if (height < MathUtil.Epsilon || radius < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			switch (style)
			{
			case GizmosUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.CylinderWireframe(numSegments);
				break;
			case GizmosUtil.Style.FlatShaded:
				mesh = PrimitiveMeshFactory.CylinderFlatShaded(numSegments);
				break;
			case GizmosUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.CylinderSmoothShaded(numSegments);
				break;
			}
			if (mesh == null)
			{
				return;
			}
			Gizmos.color = color;
			if (style == GizmosUtil.Style.Wireframe)
			{
				Gizmos.DrawWireMesh(mesh, center, rotation, new Vector3(radius, height, radius));
				return;
			}
			Gizmos.DrawMesh(mesh, center, rotation, new Vector3(radius, height, radius));
		}

		// Token: 0x06005B08 RID: 23304 RVA: 0x001BB31C File Offset: 0x001B951C
		public static void DrawCylinder(Vector3 point0, Vector3 point1, float radius, int numSegments, Color color, GizmosUtil.Style style = GizmosUtil.Style.SmoothShaded)
		{
			Vector3 vector = point1 - point0;
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return;
			}
			vector.Normalize();
			Vector3 vector2 = 0.5f * (point0 + point1);
			Quaternion quaternion = Quaternion.LookRotation(Vector3.Normalize(Vector3.Cross((Vector3.Dot(vector.normalized, Vector3.up) < 0.5f) ? Vector3.up : Vector3.forward, vector)), vector);
			GizmosUtil.DrawCylinder(vector2, quaternion, magnitude, radius, numSegments, color, style);
		}

		// Token: 0x06005B09 RID: 23305 RVA: 0x001BB3A0 File Offset: 0x001B95A0
		public static void DrawSphere(Vector3 center, Quaternion rotation, float radius, int latSegments, int longSegments, Color color, GizmosUtil.Style style = GizmosUtil.Style.SmoothShaded)
		{
			if (radius < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			switch (style)
			{
			case GizmosUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.SphereWireframe(latSegments, longSegments);
				break;
			case GizmosUtil.Style.FlatShaded:
				mesh = PrimitiveMeshFactory.SphereFlatShaded(latSegments, longSegments);
				break;
			case GizmosUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.SphereSmoothShaded(latSegments, longSegments);
				break;
			}
			if (mesh == null)
			{
				return;
			}
			Gizmos.color = color;
			if (style == GizmosUtil.Style.Wireframe)
			{
				Gizmos.DrawWireMesh(mesh, center, rotation, new Vector3(radius, radius, radius));
				return;
			}
			Gizmos.DrawMesh(mesh, center, rotation, new Vector3(radius, radius, radius));
		}

		// Token: 0x06005B0A RID: 23306 RVA: 0x001BB422 File Offset: 0x001B9622
		public static void DrawSphere(Vector3 center, float radius, int latSegments, int longSegments, Color color, GizmosUtil.Style style = GizmosUtil.Style.SmoothShaded)
		{
			GizmosUtil.DrawSphere(center, Quaternion.identity, radius, latSegments, longSegments, color, style);
		}

		// Token: 0x06005B0B RID: 23307 RVA: 0x001BB438 File Offset: 0x001B9638
		public static void DrawCapsule(Vector3 center, Quaternion rotation, float height, float radius, int latSegmentsPerCap, int longSegmentsPerCap, Color color, GizmosUtil.Style style = GizmosUtil.Style.SmoothShaded)
		{
			if (height < MathUtil.Epsilon || radius < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			Mesh mesh2 = null;
			switch (style)
			{
			case GizmosUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.CapsuleWireframe(latSegmentsPerCap, longSegmentsPerCap, true, true, false);
				mesh2 = PrimitiveMeshFactory.CapsuleWireframe(latSegmentsPerCap, longSegmentsPerCap, false, false, true);
				break;
			case GizmosUtil.Style.FlatShaded:
				mesh = PrimitiveMeshFactory.CapsuleFlatShaded(latSegmentsPerCap, longSegmentsPerCap, true, true, false);
				mesh2 = PrimitiveMeshFactory.CapsuleFlatShaded(latSegmentsPerCap, longSegmentsPerCap, false, false, true);
				break;
			case GizmosUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.CapsuleSmoothShaded(latSegmentsPerCap, longSegmentsPerCap, true, true, false);
				mesh2 = PrimitiveMeshFactory.CapsuleSmoothShaded(latSegmentsPerCap, longSegmentsPerCap, false, false, true);
				break;
			}
			if (mesh == null || mesh2 == null)
			{
				return;
			}
			Vector3 vector = rotation * Vector3.up;
			Vector3 vector2 = 0.5f * (height - radius) * vector;
			Vector3 vector3 = center + vector2;
			Vector3 vector4 = center - vector2;
			Quaternion quaternion = Quaternion.AngleAxis(180f, vector) * rotation;
			Gizmos.color = color;
			if (style == GizmosUtil.Style.Wireframe)
			{
				Gizmos.DrawWireMesh(mesh, vector3, rotation, new Vector3(radius, radius, radius));
				Gizmos.DrawWireMesh(mesh, vector4, quaternion, new Vector3(-radius, -radius, radius));
				Gizmos.DrawWireMesh(mesh2, center, rotation, new Vector3(radius, height, radius));
				return;
			}
			Gizmos.DrawMesh(mesh, vector3, rotation, new Vector3(radius, radius, radius));
			Gizmos.DrawMesh(mesh, vector4, quaternion, new Vector3(-radius, -radius, radius));
			Gizmos.DrawMesh(mesh2, center, rotation, new Vector3(radius, height, radius));
		}

		// Token: 0x06005B0C RID: 23308 RVA: 0x001BB58C File Offset: 0x001B978C
		public static void DrawCapsule(Vector3 point0, Vector3 point1, float radius, int latSegmentsPerCap, int longSegmentsPerCap, Color color, GizmosUtil.Style style = GizmosUtil.Style.SmoothShaded)
		{
			Vector3 vector = point1 - point0;
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return;
			}
			vector.Normalize();
			Vector3 vector2 = 0.5f * (point0 + point1);
			Quaternion quaternion = Quaternion.LookRotation(Vector3.Normalize(Vector3.Cross((Vector3.Dot(vector.normalized, Vector3.up) < 0.5f) ? Vector3.up : Vector3.forward, vector)), vector);
			GizmosUtil.DrawCapsule(vector2, quaternion, magnitude, radius, latSegmentsPerCap, longSegmentsPerCap, color, style);
		}

		// Token: 0x06005B0D RID: 23309 RVA: 0x001BB610 File Offset: 0x001B9810
		public static void DrawCone(Vector3 baseCenter, Quaternion rotation, float height, float radius, int numSegments, Color color, GizmosUtil.Style style = GizmosUtil.Style.FlatShaded)
		{
			if (height < MathUtil.Epsilon || radius < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			switch (style)
			{
			case GizmosUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.ConeWireframe(numSegments);
				break;
			case GizmosUtil.Style.FlatShaded:
				mesh = PrimitiveMeshFactory.ConeFlatShaded(numSegments);
				break;
			case GizmosUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.ConeSmoothShaded(numSegments);
				break;
			}
			if (mesh == null)
			{
				return;
			}
			Gizmos.color = color;
			if (style == GizmosUtil.Style.Wireframe)
			{
				Gizmos.DrawWireMesh(mesh, baseCenter, rotation, new Vector3(radius, height, radius));
				return;
			}
			Gizmos.DrawMesh(mesh, baseCenter, rotation, new Vector3(radius, height, radius));
		}

		// Token: 0x06005B0E RID: 23310 RVA: 0x001BB698 File Offset: 0x001B9898
		public static void DrawCone(Vector3 baseCenter, Vector3 top, float radius, int numSegments, Color color, GizmosUtil.Style style = GizmosUtil.Style.FlatShaded)
		{
			Vector3 vector = top - baseCenter;
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return;
			}
			vector.Normalize();
			Quaternion quaternion = Quaternion.LookRotation(Vector3.Normalize(Vector3.Cross((Vector3.Dot(vector, Vector3.up) < 0.5f) ? Vector3.up : Vector3.forward, vector)), vector);
			GizmosUtil.DrawCone(baseCenter, quaternion, magnitude, radius, numSegments, color, style);
		}

		// Token: 0x06005B0F RID: 23311 RVA: 0x001BB704 File Offset: 0x001B9904
		public static void DrawArrow(Vector3 from, Vector3 to, float coneRadius, float coneHeight, int numSegments, float stemThickness, Color color, GizmosUtil.Style style = GizmosUtil.Style.FlatShaded)
		{
			Vector3 vector = to - from;
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return;
			}
			vector.Normalize();
			Quaternion quaternion = Quaternion.LookRotation(Vector3.Normalize(Vector3.Cross((Vector3.Dot(vector, Vector3.up) < 0.5f) ? Vector3.up : Vector3.forward, vector)), vector);
			GizmosUtil.DrawCone(to - coneHeight * vector, quaternion, coneHeight, coneRadius, numSegments, color, style);
			if (stemThickness <= 0f)
			{
				if (style != GizmosUtil.Style.Wireframe)
				{
					to -= coneHeight * vector;
				}
				GizmosUtil.DrawLine(from, to, color);
				return;
			}
			if (coneHeight < magnitude)
			{
				to -= coneHeight * vector;
				GizmosUtil.DrawCylinder(from, to, 0.5f * stemThickness, numSegments, color, style);
			}
		}

		// Token: 0x06005B10 RID: 23312 RVA: 0x001BB7CA File Offset: 0x001B99CA
		public static void DrawArrow(Vector3 from, Vector3 to, float size, Color color, GizmosUtil.Style style = GizmosUtil.Style.FlatShaded)
		{
			GizmosUtil.DrawArrow(from, to, 0.5f * size, size, 8, 0f, color, style);
		}

		// Token: 0x02000E3A RID: 3642
		public enum Style
		{
			// Token: 0x04005EF9 RID: 24313
			Wireframe,
			// Token: 0x04005EFA RID: 24314
			FlatShaded,
			// Token: 0x04005EFB RID: 24315
			SmoothShaded
		}
	}
}
