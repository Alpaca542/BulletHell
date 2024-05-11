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
	private float[] _audioCategoryVolumes;

	public AudioSystem()
	{
		_audioCategoryVolumes = new float[Enum.GetValues(typeof(AudioCategory)).Length];
		for (int i = 0; i < _audioCategoryVolumes.Length; i++)
			_audioCategoryVolumes[i] = 1.0f;
	}

	public void PlayClip(AudioClip audioClip, AudioClipSettings audioSettings)
	{
		if (_activeAudioObjects >= _maxAudioObjects && !audioSettings.forcePlay)
			return;

        AudioObject audioObject = (AudioObject)GameManager.Instance.pool.Get<AudioObject>();
		AudioSource audioSource = audioObject.GetComponent<AudioSource>();
		audioSource.clip = audioClip;
		audioSource.loop = audioSettings.looping;
		audioSource.volume = _audioCategoryVolumes[(int)AudioCategory.master] * _audioCategoryVolumes[(int)audioSettings.category];
		audioSource.pitch = audioSettings.pitch;
		audioSource.time = 0f;
		audioSource.Play();
		_activeAudioObjects++;
	}

	public float GetVolume(AudioCategory category)
	{
		return _audioCategoryVolumes[(int)AudioCategory.master] * _audioCategoryVolumes[(int)category];
	}

	public void AudioObjectDeactivated()
	{
		_activeAudioObjects--;
	}

	public void SetVolume(AudioCategory category, float volume)
	{
		_audioCategoryVolumes[(int)category] = Mathf.Max(Mathf.Min(volume, 1f), 0f);
	}
}
