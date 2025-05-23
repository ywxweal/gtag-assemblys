using System;

namespace BoingKit
{
	// Token: 0x02000E58 RID: 3672
	public struct Version : IEquatable<Version>
	{
		// Token: 0x170008F5 RID: 2293
		// (get) Token: 0x06005BDF RID: 23519 RVA: 0x001C4464 File Offset: 0x001C2664
		public readonly int MajorVersion { get; }

		// Token: 0x170008F6 RID: 2294
		// (get) Token: 0x06005BE0 RID: 23520 RVA: 0x001C446C File Offset: 0x001C266C
		public readonly int MinorVersion { get; }

		// Token: 0x170008F7 RID: 2295
		// (get) Token: 0x06005BE1 RID: 23521 RVA: 0x001C4474 File Offset: 0x001C2674
		public readonly int Revision { get; }

		// Token: 0x06005BE2 RID: 23522 RVA: 0x001C447C File Offset: 0x001C267C
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.MajorVersion.ToString(),
				".",
				this.MinorVersion.ToString(),
				".",
				this.Revision.ToString()
			});
		}

		// Token: 0x06005BE3 RID: 23523 RVA: 0x001C44D7 File Offset: 0x001C26D7
		public bool IsValid()
		{
			return this.MajorVersion >= 0 && this.MinorVersion >= 0 && this.Revision >= 0;
		}

		// Token: 0x06005BE4 RID: 23524 RVA: 0x001C44F9 File Offset: 0x001C26F9
		public Version(int majorVersion = -1, int minorVersion = -1, int revision = -1)
		{
			this.MajorVersion = majorVersion;
			this.MinorVersion = minorVersion;
			this.Revision = revision;
		}

		// Token: 0x06005BE5 RID: 23525 RVA: 0x001C4510 File Offset: 0x001C2710
		public static bool operator ==(Version lhs, Version rhs)
		{
			return lhs.IsValid() && rhs.IsValid() && (lhs.MajorVersion == rhs.MajorVersion && lhs.MinorVersion == rhs.MinorVersion) && lhs.Revision == rhs.Revision;
		}

		// Token: 0x06005BE6 RID: 23526 RVA: 0x001C4565 File Offset: 0x001C2765
		public static bool operator !=(Version lhs, Version rhs)
		{
			return !(lhs == rhs);
		}

		// Token: 0x06005BE7 RID: 23527 RVA: 0x001C4571 File Offset: 0x001C2771
		public override bool Equals(object obj)
		{
			return obj is Version && this.Equals((Version)obj);
		}

		// Token: 0x06005BE8 RID: 23528 RVA: 0x001C4589 File Offset: 0x001C2789
		public bool Equals(Version other)
		{
			return this.MajorVersion == other.MajorVersion && this.MinorVersion == other.MinorVersion && this.Revision == other.Revision;
		}

		// Token: 0x06005BE9 RID: 23529 RVA: 0x001C45BC File Offset: 0x001C27BC
		public override int GetHashCode()
		{
			return ((366299368 * -1521134295 + this.MajorVersion.GetHashCode()) * -1521134295 + this.MinorVersion.GetHashCode()) * -1521134295 + this.Revision.GetHashCode();
		}

		// Token: 0x04006015 RID: 24597
		public static readonly Version Invalid = new Version(-1, -1, -1);

		// Token: 0x04006016 RID: 24598
		public static readonly Version FirstTracked = new Version(1, 2, 33);

		// Token: 0x04006017 RID: 24599
		public static readonly Version LastUntracked = new Version(1, 2, 32);
	}
}
