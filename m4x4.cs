using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x0200076C RID: 1900
[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 64)]
public struct m4x4
{
	// Token: 0x06002F5A RID: 12122 RVA: 0x000EBFA0 File Offset: 0x000EA1A0
	public m4x4(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33)
	{
		this = default(m4x4);
		this.m00 = m00;
		this.m01 = m01;
		this.m02 = m02;
		this.m03 = m03;
		this.m10 = m10;
		this.m11 = m11;
		this.m12 = m12;
		this.m13 = m13;
		this.m20 = m20;
		this.m21 = m21;
		this.m22 = m22;
		this.m23 = m23;
		this.m30 = m30;
		this.m31 = m31;
		this.m32 = m32;
		this.m33 = m33;
	}

	// Token: 0x06002F5B RID: 12123 RVA: 0x000EC031 File Offset: 0x000EA231
	public m4x4(Vector4 row0, Vector4 row1, Vector4 row2, Vector4 row3)
	{
		this = default(m4x4);
		this.r0 = row0;
		this.r1 = row1;
		this.r2 = row2;
		this.r3 = row3;
	}

	// Token: 0x06002F5C RID: 12124 RVA: 0x000EC058 File Offset: 0x000EA258
	public void Clear()
	{
		this.m00 = 0f;
		this.m01 = 0f;
		this.m02 = 0f;
		this.m03 = 0f;
		this.m10 = 0f;
		this.m11 = 0f;
		this.m12 = 0f;
		this.m13 = 0f;
		this.m20 = 0f;
		this.m21 = 0f;
		this.m22 = 0f;
		this.m23 = 0f;
		this.m30 = 0f;
		this.m31 = 0f;
		this.m32 = 0f;
		this.m33 = 0f;
	}

	// Token: 0x06002F5D RID: 12125 RVA: 0x000EC115 File Offset: 0x000EA315
	public void SetRow0(ref Vector4 v)
	{
		this.m00 = v.x;
		this.m01 = v.y;
		this.m02 = v.z;
		this.m03 = v.w;
	}

	// Token: 0x06002F5E RID: 12126 RVA: 0x000EC147 File Offset: 0x000EA347
	public void SetRow1(ref Vector4 v)
	{
		this.m10 = v.x;
		this.m11 = v.y;
		this.m12 = v.z;
		this.m13 = v.w;
	}

	// Token: 0x06002F5F RID: 12127 RVA: 0x000EC179 File Offset: 0x000EA379
	public void SetRow2(ref Vector4 v)
	{
		this.m20 = v.x;
		this.m21 = v.y;
		this.m22 = v.z;
		this.m23 = v.w;
	}

	// Token: 0x06002F60 RID: 12128 RVA: 0x000EC1AB File Offset: 0x000EA3AB
	public void SetRow3(ref Vector4 v)
	{
		this.m30 = v.x;
		this.m31 = v.y;
		this.m32 = v.z;
		this.m33 = v.w;
	}

	// Token: 0x06002F61 RID: 12129 RVA: 0x000EC1E0 File Offset: 0x000EA3E0
	public void Transpose()
	{
		float num = this.m01;
		float num2 = this.m02;
		float num3 = this.m03;
		float num4 = this.m10;
		float num5 = this.m12;
		float num6 = this.m13;
		float num7 = this.m20;
		float num8 = this.m21;
		float num9 = this.m23;
		float num10 = this.m30;
		float num11 = this.m31;
		float num12 = this.m32;
		this.m01 = num4;
		this.m02 = num7;
		this.m03 = num10;
		this.m10 = num;
		this.m12 = num8;
		this.m13 = num11;
		this.m20 = num2;
		this.m21 = num5;
		this.m23 = num12;
		this.m30 = num3;
		this.m31 = num6;
		this.m32 = num9;
	}

	// Token: 0x06002F62 RID: 12130 RVA: 0x000EC2A5 File Offset: 0x000EA4A5
	public void Set(ref Vector4 row0, ref Vector4 row1, ref Vector4 row2, ref Vector4 row3)
	{
		this.r0 = row0;
		this.r1 = row1;
		this.r2 = row2;
		this.r3 = row3;
	}

	// Token: 0x06002F63 RID: 12131 RVA: 0x000EC2D8 File Offset: 0x000EA4D8
	public void SetTransposed(ref Vector4 row0, ref Vector4 row1, ref Vector4 row2, ref Vector4 row3)
	{
		this.m00 = row0.x;
		this.m01 = row1.x;
		this.m02 = row2.x;
		this.m03 = row3.x;
		this.m10 = row0.y;
		this.m11 = row1.y;
		this.m12 = row2.y;
		this.m13 = row3.y;
		this.m20 = row0.z;
		this.m21 = row1.z;
		this.m22 = row2.z;
		this.m23 = row3.z;
		this.m30 = row0.w;
		this.m31 = row1.w;
		this.m32 = row2.w;
		this.m33 = row3.w;
	}

	// Token: 0x06002F64 RID: 12132 RVA: 0x000EC3AC File Offset: 0x000EA5AC
	public void Set(ref Matrix4x4 x)
	{
		this.m00 = x.m00;
		this.m01 = x.m01;
		this.m02 = x.m02;
		this.m03 = x.m03;
		this.m10 = x.m10;
		this.m11 = x.m11;
		this.m12 = x.m12;
		this.m13 = x.m13;
		this.m20 = x.m20;
		this.m21 = x.m21;
		this.m22 = x.m22;
		this.m23 = x.m23;
		this.m30 = x.m30;
		this.m31 = x.m31;
		this.m32 = x.m32;
		this.m33 = x.m33;
	}

	// Token: 0x06002F65 RID: 12133 RVA: 0x000EC47C File Offset: 0x000EA67C
	public void SetTransposed(ref Matrix4x4 x)
	{
		this.m00 = x.m00;
		this.m01 = x.m10;
		this.m02 = x.m20;
		this.m03 = x.m30;
		this.m10 = x.m01;
		this.m11 = x.m11;
		this.m12 = x.m21;
		this.m13 = x.m31;
		this.m20 = x.m02;
		this.m21 = x.m12;
		this.m22 = x.m22;
		this.m23 = x.m32;
		this.m30 = x.m03;
		this.m31 = x.m13;
		this.m32 = x.m23;
		this.m33 = x.m33;
	}

	// Token: 0x06002F66 RID: 12134 RVA: 0x000EC54C File Offset: 0x000EA74C
	public void Push(ref Matrix4x4 x)
	{
		x.m00 = this.m00;
		x.m01 = this.m01;
		x.m02 = this.m02;
		x.m03 = this.m03;
		x.m10 = this.m10;
		x.m11 = this.m11;
		x.m12 = this.m12;
		x.m13 = this.m13;
		x.m20 = this.m20;
		x.m21 = this.m21;
		x.m22 = this.m22;
		x.m23 = this.m23;
		x.m30 = this.m30;
		x.m31 = this.m31;
		x.m32 = this.m32;
		x.m33 = this.m33;
	}

	// Token: 0x06002F67 RID: 12135 RVA: 0x000EC61C File Offset: 0x000EA81C
	public void PushTransposed(ref Matrix4x4 x)
	{
		x.m00 = this.m00;
		x.m01 = this.m10;
		x.m02 = this.m20;
		x.m03 = this.m30;
		x.m10 = this.m01;
		x.m11 = this.m11;
		x.m12 = this.m21;
		x.m13 = this.m31;
		x.m20 = this.m02;
		x.m21 = this.m12;
		x.m22 = this.m22;
		x.m23 = this.m32;
		x.m30 = this.m03;
		x.m31 = this.m13;
		x.m32 = this.m23;
		x.m33 = this.m33;
	}

	// Token: 0x06002F68 RID: 12136 RVA: 0x000EC6E9 File Offset: 0x000EA8E9
	public static ref m4x4 From(ref Matrix4x4 src)
	{
		return Unsafe.As<Matrix4x4, m4x4>(ref src);
	}

	// Token: 0x040035BB RID: 13755
	[FixedBuffer(typeof(float), 16)]
	[NonSerialized]
	[FieldOffset(0)]
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public m4x4.<data_f>e__FixedBuffer data_f;

	// Token: 0x040035BC RID: 13756
	[FixedBuffer(typeof(int), 16)]
	[NonSerialized]
	[FieldOffset(0)]
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public m4x4.<data_i>e__FixedBuffer data_i;

	// Token: 0x040035BD RID: 13757
	[FixedBuffer(typeof(ushort), 32)]
	[NonSerialized]
	[FieldOffset(0)]
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	public m4x4.<data_h>e__FixedBuffer data_h;

	// Token: 0x040035BE RID: 13758
	[NonSerialized]
	[FieldOffset(0)]
	public Vector4 r0;

	// Token: 0x040035BF RID: 13759
	[NonSerialized]
	[FieldOffset(16)]
	public Vector4 r1;

	// Token: 0x040035C0 RID: 13760
	[NonSerialized]
	[FieldOffset(32)]
	public Vector4 r2;

	// Token: 0x040035C1 RID: 13761
	[NonSerialized]
	[FieldOffset(48)]
	public Vector4 r3;

	// Token: 0x040035C2 RID: 13762
	[NonSerialized]
	[FieldOffset(0)]
	public float m00;

	// Token: 0x040035C3 RID: 13763
	[NonSerialized]
	[FieldOffset(4)]
	public float m01;

	// Token: 0x040035C4 RID: 13764
	[NonSerialized]
	[FieldOffset(8)]
	public float m02;

	// Token: 0x040035C5 RID: 13765
	[NonSerialized]
	[FieldOffset(12)]
	public float m03;

	// Token: 0x040035C6 RID: 13766
	[NonSerialized]
	[FieldOffset(16)]
	public float m10;

	// Token: 0x040035C7 RID: 13767
	[NonSerialized]
	[FieldOffset(20)]
	public float m11;

	// Token: 0x040035C8 RID: 13768
	[NonSerialized]
	[FieldOffset(24)]
	public float m12;

	// Token: 0x040035C9 RID: 13769
	[NonSerialized]
	[FieldOffset(28)]
	public float m13;

	// Token: 0x040035CA RID: 13770
	[NonSerialized]
	[FieldOffset(32)]
	public float m20;

	// Token: 0x040035CB RID: 13771
	[NonSerialized]
	[FieldOffset(36)]
	public float m21;

	// Token: 0x040035CC RID: 13772
	[NonSerialized]
	[FieldOffset(40)]
	public float m22;

	// Token: 0x040035CD RID: 13773
	[NonSerialized]
	[FieldOffset(44)]
	public float m23;

	// Token: 0x040035CE RID: 13774
	[NonSerialized]
	[FieldOffset(48)]
	public float m30;

	// Token: 0x040035CF RID: 13775
	[NonSerialized]
	[FieldOffset(52)]
	public float m31;

	// Token: 0x040035D0 RID: 13776
	[NonSerialized]
	[FieldOffset(56)]
	public float m32;

	// Token: 0x040035D1 RID: 13777
	[NonSerialized]
	[FieldOffset(60)]
	public float m33;

	// Token: 0x040035D2 RID: 13778
	[HideInInspector]
	[FieldOffset(0)]
	public int i00;

	// Token: 0x040035D3 RID: 13779
	[HideInInspector]
	[FieldOffset(4)]
	public int i01;

	// Token: 0x040035D4 RID: 13780
	[HideInInspector]
	[FieldOffset(8)]
	public int i02;

	// Token: 0x040035D5 RID: 13781
	[HideInInspector]
	[FieldOffset(12)]
	public int i03;

	// Token: 0x040035D6 RID: 13782
	[HideInInspector]
	[FieldOffset(16)]
	public int i10;

	// Token: 0x040035D7 RID: 13783
	[HideInInspector]
	[FieldOffset(20)]
	public int i11;

	// Token: 0x040035D8 RID: 13784
	[HideInInspector]
	[FieldOffset(24)]
	public int i12;

	// Token: 0x040035D9 RID: 13785
	[HideInInspector]
	[FieldOffset(28)]
	public int i13;

	// Token: 0x040035DA RID: 13786
	[HideInInspector]
	[FieldOffset(32)]
	public int i20;

	// Token: 0x040035DB RID: 13787
	[HideInInspector]
	[FieldOffset(36)]
	public int i21;

	// Token: 0x040035DC RID: 13788
	[HideInInspector]
	[FieldOffset(40)]
	public int i22;

	// Token: 0x040035DD RID: 13789
	[HideInInspector]
	[FieldOffset(44)]
	public int i23;

	// Token: 0x040035DE RID: 13790
	[HideInInspector]
	[FieldOffset(48)]
	public int i30;

	// Token: 0x040035DF RID: 13791
	[HideInInspector]
	[FieldOffset(52)]
	public int i31;

	// Token: 0x040035E0 RID: 13792
	[HideInInspector]
	[FieldOffset(56)]
	public int i32;

	// Token: 0x040035E1 RID: 13793
	[HideInInspector]
	[FieldOffset(60)]
	public int i33;

	// Token: 0x040035E2 RID: 13794
	[NonSerialized]
	[FieldOffset(0)]
	public ushort h00_a;

	// Token: 0x040035E3 RID: 13795
	[NonSerialized]
	[FieldOffset(2)]
	public ushort h00_b;

	// Token: 0x040035E4 RID: 13796
	[NonSerialized]
	[FieldOffset(4)]
	public ushort h01_a;

	// Token: 0x040035E5 RID: 13797
	[NonSerialized]
	[FieldOffset(6)]
	public ushort h01_b;

	// Token: 0x040035E6 RID: 13798
	[NonSerialized]
	[FieldOffset(8)]
	public ushort h02_a;

	// Token: 0x040035E7 RID: 13799
	[NonSerialized]
	[FieldOffset(10)]
	public ushort h02_b;

	// Token: 0x040035E8 RID: 13800
	[NonSerialized]
	[FieldOffset(12)]
	public ushort h03_a;

	// Token: 0x040035E9 RID: 13801
	[NonSerialized]
	[FieldOffset(14)]
	public ushort h03_b;

	// Token: 0x040035EA RID: 13802
	[NonSerialized]
	[FieldOffset(16)]
	public ushort h10_a;

	// Token: 0x040035EB RID: 13803
	[NonSerialized]
	[FieldOffset(18)]
	public ushort h10_b;

	// Token: 0x040035EC RID: 13804
	[NonSerialized]
	[FieldOffset(20)]
	public ushort h11_a;

	// Token: 0x040035ED RID: 13805
	[NonSerialized]
	[FieldOffset(22)]
	public ushort h11_b;

	// Token: 0x040035EE RID: 13806
	[NonSerialized]
	[FieldOffset(24)]
	public ushort h12_a;

	// Token: 0x040035EF RID: 13807
	[NonSerialized]
	[FieldOffset(26)]
	public ushort h12_b;

	// Token: 0x040035F0 RID: 13808
	[NonSerialized]
	[FieldOffset(28)]
	public ushort h13_a;

	// Token: 0x040035F1 RID: 13809
	[NonSerialized]
	[FieldOffset(30)]
	public ushort h13_b;

	// Token: 0x040035F2 RID: 13810
	[NonSerialized]
	[FieldOffset(32)]
	public ushort h20_a;

	// Token: 0x040035F3 RID: 13811
	[NonSerialized]
	[FieldOffset(34)]
	public ushort h20_b;

	// Token: 0x040035F4 RID: 13812
	[NonSerialized]
	[FieldOffset(36)]
	public ushort h21_a;

	// Token: 0x040035F5 RID: 13813
	[NonSerialized]
	[FieldOffset(38)]
	public ushort h21_b;

	// Token: 0x040035F6 RID: 13814
	[NonSerialized]
	[FieldOffset(40)]
	public ushort h22_a;

	// Token: 0x040035F7 RID: 13815
	[NonSerialized]
	[FieldOffset(42)]
	public ushort h22_b;

	// Token: 0x040035F8 RID: 13816
	[NonSerialized]
	[FieldOffset(44)]
	public ushort h23_a;

	// Token: 0x040035F9 RID: 13817
	[NonSerialized]
	[FieldOffset(46)]
	public ushort h23_b;

	// Token: 0x040035FA RID: 13818
	[NonSerialized]
	[FieldOffset(48)]
	public ushort h30_a;

	// Token: 0x040035FB RID: 13819
	[NonSerialized]
	[FieldOffset(50)]
	public ushort h30_b;

	// Token: 0x040035FC RID: 13820
	[NonSerialized]
	[FieldOffset(52)]
	public ushort h31_a;

	// Token: 0x040035FD RID: 13821
	[NonSerialized]
	[FieldOffset(54)]
	public ushort h31_b;

	// Token: 0x040035FE RID: 13822
	[NonSerialized]
	[FieldOffset(56)]
	public ushort h32_a;

	// Token: 0x040035FF RID: 13823
	[NonSerialized]
	[FieldOffset(58)]
	public ushort h32_b;

	// Token: 0x04003600 RID: 13824
	[NonSerialized]
	[FieldOffset(60)]
	public ushort h33_a;

	// Token: 0x04003601 RID: 13825
	[NonSerialized]
	[FieldOffset(62)]
	public ushort h33_b;

	// Token: 0x0200076D RID: 1901
	[CompilerGenerated]
	[UnsafeValueType]
	[StructLayout(LayoutKind.Sequential, Size = 64)]
	public struct <data_f>e__FixedBuffer
	{
		// Token: 0x04003602 RID: 13826
		public float FixedElementField;
	}

	// Token: 0x0200076E RID: 1902
	[CompilerGenerated]
	[UnsafeValueType]
	[StructLayout(LayoutKind.Sequential, Size = 64)]
	public struct <data_h>e__FixedBuffer
	{
		// Token: 0x04003603 RID: 13827
		public ushort FixedElementField;
	}

	// Token: 0x0200076F RID: 1903
	[CompilerGenerated]
	[UnsafeValueType]
	[StructLayout(LayoutKind.Sequential, Size = 64)]
	public struct <data_i>e__FixedBuffer
	{
		// Token: 0x04003604 RID: 13828
		public int FixedElementField;
	}
}
