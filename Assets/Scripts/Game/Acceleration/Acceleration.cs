using System;
using UnityEngine;

public class Acceleration {
	private Vector3 LastAcceleration;

	private float AccelerometerUpdateInterval = 1.0f / 60.0f;
	private float LowPassKernelWidthInSeconds = 1.0f;

	private float LowPassFilterFactor;
	private Vector3 lowPassValue = Vector3.zero;

	public Acceleration() {
		LastAcceleration = Input.acceleration;
		LowPassFilterFactor = AccelerometerUpdateInterval / LowPassKernelWidthInSeconds;
	}

	private Vector3 LowPassFilterAccelerometer() {
	    return Vector3.Lerp(lowPassValue, Input.acceleration, LowPassFilterFactor);
	}

	public Vector3 GetlinearAcceleration() {
		Vector3 currentAcceleration = LowPassFilterAccelerometer();
		Vector3 res = currentAcceleration - LastAcceleration;
		LastAcceleration = currentAcceleration;
		res.z = 0;
		return res;
	}

	//private Vector3 GetAcceleration() {
	//	float period = 0f;
	//	Vector3 acc = Vector3.zero;
	//	foreach (AccelerationEvent evnt in Input.accelerationEvents)
	//	{
	//		acc += evnt.acceleration * evnt.deltaTime;
	//		period += evnt.deltaTime;
	//	}
	//	if (period > 0)
	//	{
	//		acc *= 1f / period;
	//	}

	//	acc.z = 0;

	//	return acc;
	//}
}
