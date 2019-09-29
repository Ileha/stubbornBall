using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class HelpForLevel : MonoBehaviour, IPointerClickHandler
{
	public GameObject[] Part;

	public void OnPointerClick(PointerEventData eventData) {
		for (int i = 0; i < Part.Length; i++) {
			Destroy(Part[i]);	
		}
	}
}
