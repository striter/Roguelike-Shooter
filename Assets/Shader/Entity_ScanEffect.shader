Shader "Game/Extra/ScanEffect"
{
	Properties
	{
		_MaskColor("Scan Color",Color)=(1,1,1,1)
		_MaskParam1("Scan Speed",float)=1
		_MaskParam2("Scan Tex Scale",float) = 100
	}
	SubShader
	{
		USEPASS "Hidden/ExtraPass/OnWallMasked/TEXFLOW"
	}
}
