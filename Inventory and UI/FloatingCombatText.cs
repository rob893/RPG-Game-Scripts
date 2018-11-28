using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingCombatText : MonoBehaviour {

	private Transform cam;
	private float timer = 0;
	private TextMeshProUGUI text;
	private int moveDirIndex;
	private Vector3 startingScale = new Vector3(0.01f, 0.01f, 0.01f);
	private Animator anim;
	private float activeTime = 1;


	private void Awake()
	{
		cam = Camera.main.transform;
		text = GetComponentInChildren<TextMeshProUGUI>();
		anim = GetComponent<Animator>();
	}

	private void OnEnable()
	{
		moveDirIndex = Random.Range(0, 3);
		timer = 0;
	}

	private void OnDisable()
	{
		text.rectTransform.localScale = startingScale;
		text.rectTransform.localPosition = Vector3.zero;
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if(timer >= activeTime)
		{
			gameObject.SetActive(false);
		}
		transform.forward = cam.forward;
	}

	public void SetText(string combatText, bool crit = false)
	{
		text.text = combatText;

		if (!crit)
		{
			activeTime = 1;

			if(moveDirIndex == 0)
			{
				anim.SetTrigger("Hit");
			}
			else if(moveDirIndex == 1)
			{
				anim.SetTrigger("Right");
			}
			else
			{
				anim.SetTrigger("Left");
			}
		}
		else
		{
			activeTime = 1.25f;
			anim.SetTrigger("Crit");
		}

		
	}
}
