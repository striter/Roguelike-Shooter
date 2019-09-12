Shader "Game/Effect/ScanEffect"
{
	Properties
	{
		_MaskColor("Scan Color",Color)=(1,1,1,1)
		_MaskTex("Scan Tex",2D)="white"
		_MaskParam1("Scan Speed",float)=1
		_MaskParam2("Scan Tex Scale",float) = 1
	}
	SubShader
	{
		USEPASS "Hidden/ExtraPass/OnWallMasked/TEXFLOW"
	}

}
