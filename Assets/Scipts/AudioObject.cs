using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioObject : MonoBehaviour, IPoolable
{
	public AudioCategory audioCategory;

	public void OnGetFromPool()
	{
		gameObject.SetActive(true);
	}

	public void OnReturnToPool()
	{
		gameObject.SetActive(false);
		GameManager.Instance.audioSystem.AudioObjectDeactivated();
	}
    private void OnEnable()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.volume = GameManager.Instance.audioSystem.GetVolume(audioCategory);
    }
    private void Update()
	{
		AudioSource audioSource = GetComponent<AudioSource>();
		if (!audioSource.isPlaying && !audioSource.loop)
			GameManager.Instance.pool.Return(this);
	}
}
