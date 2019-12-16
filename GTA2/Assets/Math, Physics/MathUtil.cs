using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtil
{
    public static bool isArrived(Vector3 pos, Vector3 Destination)
	{
		if(Vector3.SqrMagnitude(pos - Destination) < 0.1f)
			return true;
		else
			return false;
	}
	
	
}
