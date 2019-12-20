using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtil
{
    public static bool isArrived(Vector3 pos, Vector3 Destination, float allowedRange = 0.2f)
	{
		if(Vector3.SqrMagnitude(pos - Destination) < allowedRange)
			return true;
		else
			return false;
	}
	public static bool isArrivedIn2D(Vector3 pos, Vector3 Destination, float allowedRange = 0.2f)
    {
        if (Vector2.SqrMagnitude(new Vector2(pos.x, pos.z) - new Vector2(Destination.x, Destination.z)) < allowedRange)
            return true;
        else
            return false;
    }
}
