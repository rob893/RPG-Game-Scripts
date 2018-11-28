using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemScript : MonoBehaviour {

	public float Lifetime = 2;

	private ParticleSystem[] system;

	
	private void Awake()
	{
		system = GetComponentsInChildren<ParticleSystem>();
	}

	private void OnEnable ()
    {
		StartCoroutine(ParticleCoroutine(Lifetime));
    }

    public IEnumerator ParticleCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);

		transform.parent = null;
		gameObject.SetActive(false);
    }

	private void OnDisable()
	{
		foreach (ParticleSystem p in system)
		{
			p.Stop(true);
			p.Clear(true);
		}
	}
}
