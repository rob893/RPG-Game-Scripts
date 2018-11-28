using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AudioZone : MonoBehaviour {

	[SerializeField] private AudioClip zoneMusic;
	[SerializeField] private AudioClip zoneAmbience;

	private AudioManager audioManager;

	private void Start()
	{
		audioManager = AudioManager.Instance;
		GetComponent<BoxCollider>().isTrigger = true;
		gameObject.layer = (int)Layer.IgnoreRaycast;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			if(zoneMusic != null)
			{
				audioManager.ChangeEnvironmentMusic(zoneMusic);
			}

			if(zoneAmbience != null)
			{
				audioManager.ChangeEnvironmentAmbience(zoneAmbience);
			}
		}	
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			audioManager.ChangeZoneSoundsToDefault();
		}
	}
}
