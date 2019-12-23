float Dither(float2 pixelPos, fixed ditherRef)
{
	fixed4x4 thresholdMatrix =
	{
		1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
		13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
		4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
		16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
	};

	return step(thresholdMatrix[((uint)pixelPos.x) % 4][((uint)pixelPos.y) % 4], ditherRef);
}

float DitherScreenPos(float2 screenPos, fixed ditherRef)
{
	return Dither(screenPos *float2(_ScreenParams.x / 4, _ScreenParams.y / 4), ditherRef);
}
