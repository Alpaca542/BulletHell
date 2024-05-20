using System;
using System.Collections.Generic;
using UnityEngine;

public enum AudioCategory
{
	master,
	music,
	sfx
}

public class AudioClipSettings
{
	public AudioCategory category = AudioCategory.sfx;
	public bool looping = false;
	public bool forcePlay = false;
	public float pitch = 1f;
}

public class AudioSystem
{
	private const int _maxAudioObjects = 20;
	private int _activeAudioObjects;

	public AudioSystem()
	{
	}

	public AudioSource PlayClip(AudioClip audioClip, AudioClipSettings audioSettings)
	{
		if (_activeAudioObjects >= _maxAudioObjects && !audioSettings.forcePlay)
			return null;

        AudioObject audioObject = (AudioObject)GameManager.Instance.pool.Get<AudioObject>();
		AudioSource audioSource = audioObject.GetComponent<AudioSource>();
		audioSource.clip = audioClip;
		audioSource.loop = audioSettings.looping;
		audioSource.volume = GetVolume(audioSettings.category);
		audioSource.pitch = audioSettings.pitch;
		audioSource.time = 0f;
		audioSource.Play();
		_activeAudioObjects++;
		return audioSource;
	}

	public float GetVolume(AudioCategory category)
	{
		return PlayerPrefs.GetFloat(AudioCategory.master.ToString()) * PlayerPrefs.GetFloat(category.ToString());
	}

	public float GetVolumeRaw(AudioCategory category)
	{
		return PlayerPrefs.GetFloat(category.ToString());
	}

	public void AudioObjectDeactivated()
	{
		_activeAudioObjects--;
	}

	public void SetVolume(AudioCategory category, float volume)
	{
		PlayerPrefs.SetFloat(category.ToString(), Mathf.Max(Mathf.Min(volume, 1f), 0f));
		foreach (AudioObject gmb in GameObject.FindObjectsOfType(typeof(AudioObject)))
		{
			gmb.GetComponent<AudioSource>().volume = GetVolume(gmb.audioCategory);

        }
	}
}
