using System;
using System.Collections;
using UnityEngine;

public class CoroutineExecutor : MonoBehaviour {
	private static CoroutineExecutor instanse;

	public static CoroutineExecutor GetCoroutineExecutor() {
		if (instanse == null) {
			GameObject result = new GameObject("DontDestroy", typeof(CoroutineExecutor));
			DontDestroyOnLoad(result.gameObject);
			instanse = result.GetComponent<CoroutineExecutor>();	
		}

		return instanse;
	}
}

