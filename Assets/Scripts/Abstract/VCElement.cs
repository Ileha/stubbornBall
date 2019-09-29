using System;
using UnityEngine;

namespace MVS {
	public abstract class View<T> : MonoBehaviour where T : MonoBehaviour {
		private T _app;
		public T app {
			get {
				if (_app == null) {
					_app = FindObjectOfType<T>();
				}
				return _app;
			}
		}
	}
}
