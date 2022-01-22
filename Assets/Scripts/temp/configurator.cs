// using System.Collections;
// using System.Collections.Generic;
// #if UNITY_EDITOR
// using UnityEditor.Events;
// using UnityEditor.SceneManagement;
// using UnityEditor;
// #endif
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.UI;
//
// public class configurator : GameElement
// {
// #if UNITY_EDITOR
//
// 	[ContextMenu("Initialization")]
// 	public void Configurate()
// 	{
// 		//ConfigButton(GameObject.Find("cancel").GetComponent<Button>(), app.CancelLastLine);
// 		ConfigButton(GameObject.Find("Start").GetComponent<Button>(), app.Play);
// 		ConfigButton(GameObject.Find("home").GetComponent<Button>(), app.ToMeny);
// 		ConfigButton(GameObject.Find("reset").GetComponent<Button>(), app.Restart);
//
// 		EditorSceneManager.MarkAllScenesDirty();
//
// 		Debug.Log("configurate");
// 	}
//
// 	private void ConfigButton(Button btn, UnityAction call)
// 	{
// 		int count = btn.onClick.GetPersistentEventCount();
// 		for (int i = 0; i < count; i++)
// 		{
// 			UnityEventTools.RemovePersistentListener(btn.onClick, 0);
// 		}
// 		UnityEventTools.AddPersistentListener(btn.onClick, call);
// 		EditorUtility.SetDirty(btn);
// 	}
//
// 	public void Configurate2() {
// 		DestroyImmediate(Camera.main.GetComponent<DrawLine>());
// 		DrawLine draw = Resources.Load<DrawLine>("GameComponents/UI/interact");
// 		SceneSingleton single = Camera.main.GetComponent<SceneSingleton>();
// 		single.lineDrawer = Instantiate<DrawLine>(draw, GameObject.Find("Canvas").transform);
//
// 		single.lineDrawer = PrefabUtility.ConnectGameObjectToPrefab(single.lineDrawer.gameObject, 
// 		                                                            draw.gameObject).GetComponent<DrawLine>();
//
// 		DestroyImmediate(GameObject.Find("cancel"));
//
// 		RectTransform skip = GameObject.Find("skip").GetComponent<RectTransform>();
// 		skip.anchorMax = new Vector2(1, 0);
// 		skip.anchorMin = new Vector2(1, 0);
//
// 		skip.anchoredPosition = new Vector2(-(skip.sizeDelta.x/2), skip.anchoredPosition.y);
//
// 		EditorSceneManager.MarkAllScenesDirty();
// 	}
// #endif
// }
//
// #if UNITY_EDITOR
// namespace Editors
// {
// 	[CustomEditor(typeof(configurator))]
// 	public class configuratorEditor : Editor
// 	{
// 		configurator data { get { return target as configurator; } }
//
// 		public override void OnInspectorGUI()
// 		{
// 			if (GUILayout.Button("Configurate"))
// 			{
// 				data.Configurate2();
// 			}
// 			//if (GUI.changed)
// 			//{
// 			//	Undo.RecordObject(target, "Test Scriptable Editor Modify");
// 			//	EditorUtility.SetDirty(target);
// 			//}
// 		}
// 	}
// }
//
// #endif
