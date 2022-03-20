using UnityEngine;

public interface Iinput {
	void OnStart(Vector3 ScreenPosition);
	void OnMove(Vector3 ScreenPosition);
	void OnEnd(Vector3 ScreenPosition);
}
