using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestMapMarker : MonoBehaviour {

	private QuestMarker qMarker;
	private Vector3 startPos;
	private Quaternion startRotation;
	private Vector3 startScale;
	private Transform parent;


	private void Start()
	{
		qMarker = GetComponent<QuestMarker>();
		UIManager.Instance.GetWolrdMapController().OnMapClose += OnMapClosed;
		UIManager.Instance.GetWolrdMapController().OnMapOpen += OnMapOpened;
		startPos = transform.localPosition;
		startRotation = transform.localRotation;
		startScale = transform.localScale;
		parent = transform.parent;
	}

	private void OnMapOpened()
	{
		qMarker.ToggleLootAtCamera(false);
		transform.localPosition = new Vector3(0, 17, 0);
		transform.rotation = Quaternion.Euler(-90, 0, -180);
		transform.localScale = new Vector3(15, 15, 15);

		if (!transform.parent.gameObject.activeSelf)
		{
			transform.parent = null;
		}
	}

	private void OnMapClosed()
	{
		if(transform.parent != parent)
		{
			transform.parent = parent;
		}

		qMarker.ToggleLootAtCamera(true);
		transform.localPosition = startPos;
		transform.localRotation = startRotation;
		transform.localScale = startScale;
	}

	private void OnDestroy()
	{
		if(UIManager.Instance != null)
		{
			UIManager.Instance.GetWolrdMapController().OnMapClose -= OnMapClosed;
			UIManager.Instance.GetWolrdMapController().OnMapOpen -= OnMapOpened;
		}
	}
}
