using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020006C8 RID: 1736
public static class UberShader
{
	// Token: 0x1700043B RID: 1083
	// (get) Token: 0x06002B40 RID: 11072 RVA: 0x000D4959 File Offset: 0x000D2B59
	public static Material ReferenceMaterial
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceMaterial;
		}
	}

	// Token: 0x1700043C RID: 1084
	// (get) Token: 0x06002B41 RID: 11073 RVA: 0x000D4965 File Offset: 0x000D2B65
	public static Shader ReferenceShader
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceShader;
		}
	}

	// Token: 0x1700043D RID: 1085
	// (get) Token: 0x06002B42 RID: 11074 RVA: 0x000D4971 File Offset: 0x000D2B71
	public static Material ReferenceMaterialNonSRP
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceMaterialNonSRP;
		}
	}

	// Token: 0x1700043E RID: 1086
	// (get) Token: 0x06002B43 RID: 11075 RVA: 0x000D497D File Offset: 0x000D2B7D
	public static Shader ReferenceShaderNonSRP
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceShaderNonSRP;
		}
	}

	// Token: 0x1700043F RID: 1087
	// (get) Token: 0x06002B44 RID: 11076 RVA: 0x000D4989 File Offset: 0x000D2B89
	public static UberShaderProperty[] AllProperties
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kProperties;
		}
	}

	// Token: 0x06002B45 RID: 11077 RVA: 0x000D4998 File Offset: 0x000D2B98
	public static bool IsAnimated(Material m)
	{
		if (m == null)
		{
			return false;
		}
		if ((double)UberShader.UvShiftToggle.GetValue<float>(m) <= 0.5)
		{
			return false;
		}
		Vector2 value = UberShader.UvShiftRate.GetValue<Vector2>(m);
		return value.x > 0f || value.y > 0f;
	}

	// Token: 0x06002B46 RID: 11078 RVA: 0x000D49F3 File Offset: 0x000D2BF3
	private static UberShaderProperty GetProperty(int i)
	{
		UberShader.InitDependencies();
		return UberShader.kProperties[i];
	}

	// Token: 0x06002B47 RID: 11079 RVA: 0x000D49F3 File Offset: 0x000D2BF3
	private static UberShaderProperty GetProperty(int i, string expectedName)
	{
		UberShader.InitDependencies();
		return UberShader.kProperties[i];
	}

	// Token: 0x06002B48 RID: 11080 RVA: 0x000D4A04 File Offset: 0x000D2C04
	private static void InitDependencies()
	{
		if (UberShader.gInitialized)
		{
			return;
		}
		UberShader.kReferenceShader = Shader.Find("GorillaTag/UberShader");
		UberShader.kReferenceMaterial = new Material(UberShader.kReferenceShader);
		UberShader.kReferenceShaderNonSRP = Shader.Find("GorillaTag/UberShaderNonSRP");
		UberShader.kReferenceMaterialNonSRP = new Material(UberShader.kReferenceShaderNonSRP);
		UberShader.kProperties = UberShader.EnumerateAllProperties(UberShader.kReferenceShader);
		UberShader.gInitialized = true;
	}

	// Token: 0x06002B49 RID: 11081 RVA: 0x000D4965 File Offset: 0x000D2B65
	public static Shader GetShader()
	{
		UberShader.InitDependencies();
		return UberShader.kReferenceShader;
	}

	// Token: 0x06002B4A RID: 11082 RVA: 0x000D4A6C File Offset: 0x000D2C6C
	private static UberShaderProperty[] EnumerateAllProperties(Shader uberShader)
	{
		int propertyCount = uberShader.GetPropertyCount();
		UberShaderProperty[] array = new UberShaderProperty[propertyCount];
		for (int i = 0; i < propertyCount; i++)
		{
			UberShaderProperty uberShaderProperty = new UberShaderProperty
			{
				index = i,
				flags = uberShader.GetPropertyFlags(i),
				type = uberShader.GetPropertyType(i),
				nameID = uberShader.GetPropertyNameId(i),
				name = uberShader.GetPropertyName(i),
				attributes = uberShader.GetPropertyAttributes(i)
			};
			if (uberShaderProperty.type == ShaderPropertyType.Range)
			{
				uberShaderProperty.rangeLimits = uberShader.GetPropertyRangeLimits(uberShaderProperty.index);
			}
			string[] attributes = uberShaderProperty.attributes;
			if (attributes != null && attributes.Length != 0)
			{
				foreach (string text in attributes)
				{
					if (!string.IsNullOrWhiteSpace(text))
					{
						bool flag = text.StartsWith("Toggle(");
						uberShaderProperty.isKeywordToggle = flag;
						if (flag)
						{
							string text2 = text.Split('(', StringSplitOptions.RemoveEmptyEntries)[1].RemoveEnd(")", StringComparison.InvariantCulture);
							uberShaderProperty.keyword = text2;
						}
					}
				}
			}
			array[i] = uberShaderProperty;
		}
		return array;
	}

	// Token: 0x0400304C RID: 12364
	private static Shader kReferenceShader;

	// Token: 0x0400304D RID: 12365
	private static Material kReferenceMaterial;

	// Token: 0x0400304E RID: 12366
	private static Shader kReferenceShaderNonSRP;

	// Token: 0x0400304F RID: 12367
	private static Material kReferenceMaterialNonSRP;

	// Token: 0x04003050 RID: 12368
	private static UberShaderProperty[] kProperties;

	// Token: 0x04003051 RID: 12369
	private static bool gInitialized = false;

	// Token: 0x04003052 RID: 12370
	public static UberShaderProperty TransparencyMode = UberShader.GetProperty(0);

	// Token: 0x04003053 RID: 12371
	public static UberShaderProperty Cutoff = UberShader.GetProperty(1);

	// Token: 0x04003054 RID: 12372
	public static UberShaderProperty ColorSource = UberShader.GetProperty(2);

	// Token: 0x04003055 RID: 12373
	public static UberShaderProperty BaseColor = UberShader.GetProperty(3);

	// Token: 0x04003056 RID: 12374
	public static UberShaderProperty GChannelColor = UberShader.GetProperty(4);

	// Token: 0x04003057 RID: 12375
	public static UberShaderProperty BChannelColor = UberShader.GetProperty(5);

	// Token: 0x04003058 RID: 12376
	public static UberShaderProperty AChannelColor = UberShader.GetProperty(6);

	// Token: 0x04003059 RID: 12377
	public static UberShaderProperty BaseMap = UberShader.GetProperty(7);

	// Token: 0x0400305A RID: 12378
	public static UberShaderProperty BaseMap_WH = UberShader.GetProperty(8);

	// Token: 0x0400305B RID: 12379
	public static UberShaderProperty TexelSnapToggle = UberShader.GetProperty(9);

	// Token: 0x0400305C RID: 12380
	public static UberShaderProperty TexelSnap_Factor = UberShader.GetProperty(10);

	// Token: 0x0400305D RID: 12381
	public static UberShaderProperty UVSource = UberShader.GetProperty(11);

	// Token: 0x0400305E RID: 12382
	public static UberShaderProperty AlphaDetailToggle = UberShader.GetProperty(12);

	// Token: 0x0400305F RID: 12383
	public static UberShaderProperty AlphaDetail_ST = UberShader.GetProperty(13);

	// Token: 0x04003060 RID: 12384
	public static UberShaderProperty AlphaDetail_Opacity = UberShader.GetProperty(14);

	// Token: 0x04003061 RID: 12385
	public static UberShaderProperty AlphaDetail_WorldSpace = UberShader.GetProperty(15);

	// Token: 0x04003062 RID: 12386
	public static UberShaderProperty MaskMapToggle = UberShader.GetProperty(16);

	// Token: 0x04003063 RID: 12387
	public static UberShaderProperty MaskMap = UberShader.GetProperty(17);

	// Token: 0x04003064 RID: 12388
	public static UberShaderProperty MaskMap_WH = UberShader.GetProperty(18);

	// Token: 0x04003065 RID: 12389
	public static UberShaderProperty LavaLampToggle = UberShader.GetProperty(19);

	// Token: 0x04003066 RID: 12390
	public static UberShaderProperty GradientMapToggle = UberShader.GetProperty(20);

	// Token: 0x04003067 RID: 12391
	public static UberShaderProperty GradientMap = UberShader.GetProperty(21);

	// Token: 0x04003068 RID: 12392
	public static UberShaderProperty DoTextureRotation = UberShader.GetProperty(22);

	// Token: 0x04003069 RID: 12393
	public static UberShaderProperty RotateAngle = UberShader.GetProperty(23);

	// Token: 0x0400306A RID: 12394
	public static UberShaderProperty RotateAnim = UberShader.GetProperty(24);

	// Token: 0x0400306B RID: 12395
	public static UberShaderProperty UseWaveWarp = UberShader.GetProperty(25);

	// Token: 0x0400306C RID: 12396
	public static UberShaderProperty WaveAmplitude = UberShader.GetProperty(26);

	// Token: 0x0400306D RID: 12397
	public static UberShaderProperty WaveFrequency = UberShader.GetProperty(27);

	// Token: 0x0400306E RID: 12398
	public static UberShaderProperty WaveScale = UberShader.GetProperty(28);

	// Token: 0x0400306F RID: 12399
	public static UberShaderProperty WaveTimeScale = UberShader.GetProperty(29);

	// Token: 0x04003070 RID: 12400
	public static UberShaderProperty UseWeatherMap = UberShader.GetProperty(30);

	// Token: 0x04003071 RID: 12401
	public static UberShaderProperty WeatherMap = UberShader.GetProperty(31);

	// Token: 0x04003072 RID: 12402
	public static UberShaderProperty WeatherMapDissolveEdgeSize = UberShader.GetProperty(32);

	// Token: 0x04003073 RID: 12403
	public static UberShaderProperty ReflectToggle = UberShader.GetProperty(33);

	// Token: 0x04003074 RID: 12404
	public static UberShaderProperty ReflectBoxProjectToggle = UberShader.GetProperty(34);

	// Token: 0x04003075 RID: 12405
	public static UberShaderProperty ReflectBoxCubePos = UberShader.GetProperty(35);

	// Token: 0x04003076 RID: 12406
	public static UberShaderProperty ReflectBoxSize = UberShader.GetProperty(36);

	// Token: 0x04003077 RID: 12407
	public static UberShaderProperty ReflectBoxRotation = UberShader.GetProperty(37);

	// Token: 0x04003078 RID: 12408
	public static UberShaderProperty ReflectMatcapToggle = UberShader.GetProperty(38);

	// Token: 0x04003079 RID: 12409
	public static UberShaderProperty ReflectMatcapPerspToggle = UberShader.GetProperty(39);

	// Token: 0x0400307A RID: 12410
	public static UberShaderProperty ReflectNormalToggle = UberShader.GetProperty(40);

	// Token: 0x0400307B RID: 12411
	public static UberShaderProperty ReflectTex = UberShader.GetProperty(41);

	// Token: 0x0400307C RID: 12412
	public static UberShaderProperty ReflectNormalTex = UberShader.GetProperty(42);

	// Token: 0x0400307D RID: 12413
	public static UberShaderProperty ReflectAlbedoTint = UberShader.GetProperty(43);

	// Token: 0x0400307E RID: 12414
	public static UberShaderProperty ReflectTint = UberShader.GetProperty(44);

	// Token: 0x0400307F RID: 12415
	public static UberShaderProperty ReflectOpacity = UberShader.GetProperty(45);

	// Token: 0x04003080 RID: 12416
	public static UberShaderProperty ReflectExposure = UberShader.GetProperty(46);

	// Token: 0x04003081 RID: 12417
	public static UberShaderProperty ReflectOffset = UberShader.GetProperty(47);

	// Token: 0x04003082 RID: 12418
	public static UberShaderProperty ReflectScale = UberShader.GetProperty(48);

	// Token: 0x04003083 RID: 12419
	public static UberShaderProperty ReflectRotate = UberShader.GetProperty(49);

	// Token: 0x04003084 RID: 12420
	public static UberShaderProperty HalfLambertToggle = UberShader.GetProperty(50);

	// Token: 0x04003085 RID: 12421
	public static UberShaderProperty ZFightOffset = UberShader.GetProperty(51);

	// Token: 0x04003086 RID: 12422
	public static UberShaderProperty ParallaxPlanarToggle = UberShader.GetProperty(52);

	// Token: 0x04003087 RID: 12423
	public static UberShaderProperty ParallaxToggle = UberShader.GetProperty(53);

	// Token: 0x04003088 RID: 12424
	public static UberShaderProperty ParallaxAAToggle = UberShader.GetProperty(54);

	// Token: 0x04003089 RID: 12425
	public static UberShaderProperty ParallaxAABias = UberShader.GetProperty(55);

	// Token: 0x0400308A RID: 12426
	public static UberShaderProperty DepthMap = UberShader.GetProperty(56);

	// Token: 0x0400308B RID: 12427
	public static UberShaderProperty ParallaxAmplitude = UberShader.GetProperty(57);

	// Token: 0x0400308C RID: 12428
	public static UberShaderProperty ParallaxSamplesMinMax = UberShader.GetProperty(58);

	// Token: 0x0400308D RID: 12429
	public static UberShaderProperty UvShiftToggle = UberShader.GetProperty(59);

	// Token: 0x0400308E RID: 12430
	public static UberShaderProperty UvShiftSteps = UberShader.GetProperty(60);

	// Token: 0x0400308F RID: 12431
	public static UberShaderProperty UvShiftRate = UberShader.GetProperty(61);

	// Token: 0x04003090 RID: 12432
	public static UberShaderProperty UvShiftOffset = UberShader.GetProperty(62);

	// Token: 0x04003091 RID: 12433
	public static UberShaderProperty UseGridEffect = UberShader.GetProperty(63);

	// Token: 0x04003092 RID: 12434
	public static UberShaderProperty UseCrystalEffect = UberShader.GetProperty(64);

	// Token: 0x04003093 RID: 12435
	public static UberShaderProperty CrystalPower = UberShader.GetProperty(65);

	// Token: 0x04003094 RID: 12436
	public static UberShaderProperty CrystalRimColor = UberShader.GetProperty(66);

	// Token: 0x04003095 RID: 12437
	public static UberShaderProperty LiquidVolume = UberShader.GetProperty(67);

	// Token: 0x04003096 RID: 12438
	public static UberShaderProperty LiquidFill = UberShader.GetProperty(68);

	// Token: 0x04003097 RID: 12439
	public static UberShaderProperty LiquidFillNormal = UberShader.GetProperty(69);

	// Token: 0x04003098 RID: 12440
	public static UberShaderProperty LiquidSurfaceColor = UberShader.GetProperty(70);

	// Token: 0x04003099 RID: 12441
	public static UberShaderProperty LiquidSwayX = UberShader.GetProperty(71);

	// Token: 0x0400309A RID: 12442
	public static UberShaderProperty LiquidSwayY = UberShader.GetProperty(72);

	// Token: 0x0400309B RID: 12443
	public static UberShaderProperty LiquidContainer = UberShader.GetProperty(73);

	// Token: 0x0400309C RID: 12444
	public static UberShaderProperty LiquidPlanePosition = UberShader.GetProperty(74);

	// Token: 0x0400309D RID: 12445
	public static UberShaderProperty LiquidPlaneNormal = UberShader.GetProperty(75);

	// Token: 0x0400309E RID: 12446
	public static UberShaderProperty VertexFlapToggle = UberShader.GetProperty(76);

	// Token: 0x0400309F RID: 12447
	public static UberShaderProperty VertexFlapAxis = UberShader.GetProperty(77);

	// Token: 0x040030A0 RID: 12448
	public static UberShaderProperty VertexFlapDegreesMinMax = UberShader.GetProperty(78);

	// Token: 0x040030A1 RID: 12449
	public static UberShaderProperty VertexFlapSpeed = UberShader.GetProperty(79);

	// Token: 0x040030A2 RID: 12450
	public static UberShaderProperty VertexFlapPhaseOffset = UberShader.GetProperty(80);

	// Token: 0x040030A3 RID: 12451
	public static UberShaderProperty VertexWaveToggle = UberShader.GetProperty(81);

	// Token: 0x040030A4 RID: 12452
	public static UberShaderProperty VertexWaveDebug = UberShader.GetProperty(82);

	// Token: 0x040030A5 RID: 12453
	public static UberShaderProperty VertexWaveEnd = UberShader.GetProperty(83);

	// Token: 0x040030A6 RID: 12454
	public static UberShaderProperty VertexWaveParams = UberShader.GetProperty(84);

	// Token: 0x040030A7 RID: 12455
	public static UberShaderProperty VertexWaveFalloff = UberShader.GetProperty(85);

	// Token: 0x040030A8 RID: 12456
	public static UberShaderProperty VertexWaveSphereMask = UberShader.GetProperty(86);

	// Token: 0x040030A9 RID: 12457
	public static UberShaderProperty VertexWavePhaseOffset = UberShader.GetProperty(87);

	// Token: 0x040030AA RID: 12458
	public static UberShaderProperty VertexWaveAxes = UberShader.GetProperty(88);

	// Token: 0x040030AB RID: 12459
	public static UberShaderProperty VertexRotateToggle = UberShader.GetProperty(89);

	// Token: 0x040030AC RID: 12460
	public static UberShaderProperty VertexRotateAngles = UberShader.GetProperty(90);

	// Token: 0x040030AD RID: 12461
	public static UberShaderProperty VertexRotateAnim = UberShader.GetProperty(91);

	// Token: 0x040030AE RID: 12462
	public static UberShaderProperty VertexLightToggle = UberShader.GetProperty(92);

	// Token: 0x040030AF RID: 12463
	public static UberShaderProperty InnerGlowOn = UberShader.GetProperty(93);

	// Token: 0x040030B0 RID: 12464
	public static UberShaderProperty InnerGlowColor = UberShader.GetProperty(94);

	// Token: 0x040030B1 RID: 12465
	public static UberShaderProperty InnerGlowParams = UberShader.GetProperty(95);

	// Token: 0x040030B2 RID: 12466
	public static UberShaderProperty InnerGlowTap = UberShader.GetProperty(96);

	// Token: 0x040030B3 RID: 12467
	public static UberShaderProperty InnerGlowSine = UberShader.GetProperty(97);

	// Token: 0x040030B4 RID: 12468
	public static UberShaderProperty InnerGlowSinePeriod = UberShader.GetProperty(98);

	// Token: 0x040030B5 RID: 12469
	public static UberShaderProperty InnerGlowSinePhaseShift = UberShader.GetProperty(99);

	// Token: 0x040030B6 RID: 12470
	public static UberShaderProperty StealthEffectOn = UberShader.GetProperty(100);

	// Token: 0x040030B7 RID: 12471
	public static UberShaderProperty UseEyeTracking = UberShader.GetProperty(101);

	// Token: 0x040030B8 RID: 12472
	public static UberShaderProperty EyeTileOffsetUV = UberShader.GetProperty(102);

	// Token: 0x040030B9 RID: 12473
	public static UberShaderProperty EyeOverrideUV = UberShader.GetProperty(103);

	// Token: 0x040030BA RID: 12474
	public static UberShaderProperty EyeOverrideUVTransform = UberShader.GetProperty(104);

	// Token: 0x040030BB RID: 12475
	public static UberShaderProperty UseMouthFlap = UberShader.GetProperty(105);

	// Token: 0x040030BC RID: 12476
	public static UberShaderProperty MouthMap = UberShader.GetProperty(106);

	// Token: 0x040030BD RID: 12477
	public static UberShaderProperty MouthMap_Atlas = UberShader.GetProperty(107);

	// Token: 0x040030BE RID: 12478
	public static UberShaderProperty MouthMap_AtlasSlice = UberShader.GetProperty(108);

	// Token: 0x040030BF RID: 12479
	public static UberShaderProperty UseVertexColor = UberShader.GetProperty(109);

	// Token: 0x040030C0 RID: 12480
	public static UberShaderProperty WaterEffect = UberShader.GetProperty(110);

	// Token: 0x040030C1 RID: 12481
	public static UberShaderProperty HeightBasedWaterEffect = UberShader.GetProperty(111);

	// Token: 0x040030C2 RID: 12482
	public static UberShaderProperty UseDayNightLightmap = UberShader.GetProperty(112);

	// Token: 0x040030C3 RID: 12483
	public static UberShaderProperty UseSpecular = UberShader.GetProperty(113);

	// Token: 0x040030C4 RID: 12484
	public static UberShaderProperty UseSpecularAlphaChannel = UberShader.GetProperty(114);

	// Token: 0x040030C5 RID: 12485
	public static UberShaderProperty Smoothness = UberShader.GetProperty(115);

	// Token: 0x040030C6 RID: 12486
	public static UberShaderProperty UseSpecHighlight = UberShader.GetProperty(116);

	// Token: 0x040030C7 RID: 12487
	public static UberShaderProperty SpecularDir = UberShader.GetProperty(117);

	// Token: 0x040030C8 RID: 12488
	public static UberShaderProperty SpecularPowerIntensity = UberShader.GetProperty(118);

	// Token: 0x040030C9 RID: 12489
	public static UberShaderProperty SpecularColor = UberShader.GetProperty(119);

	// Token: 0x040030CA RID: 12490
	public static UberShaderProperty SpecularUseDiffuseColor = UberShader.GetProperty(120);

	// Token: 0x040030CB RID: 12491
	public static UberShaderProperty EmissionToggle = UberShader.GetProperty(121);

	// Token: 0x040030CC RID: 12492
	public static UberShaderProperty EmissionColor = UberShader.GetProperty(122);

	// Token: 0x040030CD RID: 12493
	public static UberShaderProperty EmissionMap = UberShader.GetProperty(123);

	// Token: 0x040030CE RID: 12494
	public static UberShaderProperty EmissionMaskByBaseMapAlpha = UberShader.GetProperty(124);

	// Token: 0x040030CF RID: 12495
	public static UberShaderProperty EmissionUVScrollSpeed = UberShader.GetProperty(125);

	// Token: 0x040030D0 RID: 12496
	public static UberShaderProperty EmissionDissolveProgress = UberShader.GetProperty(126);

	// Token: 0x040030D1 RID: 12497
	public static UberShaderProperty EmissionDissolveAnimation = UberShader.GetProperty(127);

	// Token: 0x040030D2 RID: 12498
	public static UberShaderProperty EmissionDissolveEdgeSize = UberShader.GetProperty(128);

	// Token: 0x040030D3 RID: 12499
	public static UberShaderProperty EmissionUseUVWaveWarp = UberShader.GetProperty(129);

	// Token: 0x040030D4 RID: 12500
	public static UberShaderProperty GreyZoneException = UberShader.GetProperty(130);

	// Token: 0x040030D5 RID: 12501
	public static UberShaderProperty Cull = UberShader.GetProperty(131);

	// Token: 0x040030D6 RID: 12502
	public static UberShaderProperty StencilReference = UberShader.GetProperty(132);

	// Token: 0x040030D7 RID: 12503
	public static UberShaderProperty StencilComparison = UberShader.GetProperty(133);

	// Token: 0x040030D8 RID: 12504
	public static UberShaderProperty StencilPassFront = UberShader.GetProperty(134);

	// Token: 0x040030D9 RID: 12505
	public static UberShaderProperty USE_DEFORM_MAP = UberShader.GetProperty(135);

	// Token: 0x040030DA RID: 12506
	public static UberShaderProperty DeformMap = UberShader.GetProperty(136);

	// Token: 0x040030DB RID: 12507
	public static UberShaderProperty DeformMapIntensity = UberShader.GetProperty(137);

	// Token: 0x040030DC RID: 12508
	public static UberShaderProperty DeformMapMaskByVertColorRAmount = UberShader.GetProperty(138);

	// Token: 0x040030DD RID: 12509
	public static UberShaderProperty DeformMapScrollSpeed = UberShader.GetProperty(139);

	// Token: 0x040030DE RID: 12510
	public static UberShaderProperty DeformMapUV0Influence = UberShader.GetProperty(140);

	// Token: 0x040030DF RID: 12511
	public static UberShaderProperty DeformMapObjectSpaceOffsetsU = UberShader.GetProperty(141);

	// Token: 0x040030E0 RID: 12512
	public static UberShaderProperty DeformMapObjectSpaceOffsetsV = UberShader.GetProperty(142);

	// Token: 0x040030E1 RID: 12513
	public static UberShaderProperty DeformMapWorldSpaceOffsetsU = UberShader.GetProperty(143);

	// Token: 0x040030E2 RID: 12514
	public static UberShaderProperty DeformMapWorldSpaceOffsetsV = UberShader.GetProperty(144);

	// Token: 0x040030E3 RID: 12515
	public static UberShaderProperty RotateOnYAxisBySinTime = UberShader.GetProperty(145);

	// Token: 0x040030E4 RID: 12516
	public static UberShaderProperty USE_TEX_ARRAY_ATLAS = UberShader.GetProperty(146);

	// Token: 0x040030E5 RID: 12517
	public static UberShaderProperty BaseMap_Atlas = UberShader.GetProperty(147);

	// Token: 0x040030E6 RID: 12518
	public static UberShaderProperty BaseMap_AtlasSlice = UberShader.GetProperty(148);

	// Token: 0x040030E7 RID: 12519
	public static UberShaderProperty EmissionMap_Atlas = UberShader.GetProperty(149);

	// Token: 0x040030E8 RID: 12520
	public static UberShaderProperty EmissionMap_AtlasSlice = UberShader.GetProperty(150);

	// Token: 0x040030E9 RID: 12521
	public static UberShaderProperty DeformMap_Atlas = UberShader.GetProperty(151);

	// Token: 0x040030EA RID: 12522
	public static UberShaderProperty DeformMap_AtlasSlice = UberShader.GetProperty(152);

	// Token: 0x040030EB RID: 12523
	public static UberShaderProperty DEBUG_PAWN_DATA = UberShader.GetProperty(153);

	// Token: 0x040030EC RID: 12524
	public static UberShaderProperty SrcBlend = UberShader.GetProperty(154);

	// Token: 0x040030ED RID: 12525
	public static UberShaderProperty DstBlend = UberShader.GetProperty(155);

	// Token: 0x040030EE RID: 12526
	public static UberShaderProperty SrcBlendAlpha = UberShader.GetProperty(156);

	// Token: 0x040030EF RID: 12527
	public static UberShaderProperty DstBlendAlpha = UberShader.GetProperty(157);

	// Token: 0x040030F0 RID: 12528
	public static UberShaderProperty ZWrite = UberShader.GetProperty(158);

	// Token: 0x040030F1 RID: 12529
	public static UberShaderProperty AlphaToMask = UberShader.GetProperty(159);

	// Token: 0x040030F2 RID: 12530
	public static UberShaderProperty Color = UberShader.GetProperty(160);

	// Token: 0x040030F3 RID: 12531
	public static UberShaderProperty Surface = UberShader.GetProperty(161);

	// Token: 0x040030F4 RID: 12532
	public static UberShaderProperty Metallic = UberShader.GetProperty(162);

	// Token: 0x040030F5 RID: 12533
	public static UberShaderProperty SpecColor = UberShader.GetProperty(163);

	// Token: 0x040030F6 RID: 12534
	public static UberShaderProperty DayNightLightmapArray = UberShader.GetProperty(164);

	// Token: 0x040030F7 RID: 12535
	public static UberShaderProperty DayNightLightmapArray_AtlasSlice = UberShader.GetProperty(165);

	// Token: 0x040030F8 RID: 12536
	public static UberShaderProperty SingleLightmap = UberShader.GetProperty(166);
}
