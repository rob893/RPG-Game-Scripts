using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNPC : QuestObject {

	public AudioClip[] greetings;

	private AudioSource audioSource;

	protected override void Start()
	{
		base.Start();
		audioSource = GetComponent<AudioSource>();
	}

	public override void Interact(Transform interacter)
	{
		base.Interact(interacter);
		if(greetings.Length > 0)
		{
			audioSource.PlayOneShot(greetings[Random.Range(0, greetings.Length)]);
		}
		StartCoroutine(FaceTargetCoroutine(interacter));
	}

	protected override void OnMouseEnter()
	{
		return;
	}

	protected override void OnMouseExit()
	{
		return;
	}
}
