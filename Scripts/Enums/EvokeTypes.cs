using System;

[Flags]
public enum EvokeTypes {
	None = 0,
	OnDraw = 1 << 0,
	OnWounded = 1 << 1,
	OnHurt = 1 << 2,
	
}