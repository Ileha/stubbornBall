using System;
using System.Collections;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {
	private enum AudioPlayerState {
		play,
		stop,
		pause
	}

	private static AudioPlayer instanse;
	public static AudioPlayer CurrentAudioPlayer {
		get {
			if (instanse == null) {
				GameObject result = new GameObject("AudioPlayer", typeof(AudioSource), typeof(AudioPlayer));
				DontDestroyOnLoad(result.gameObject);
				instanse = result.GetComponent<AudioPlayer>();
			}
			return instanse;
		}
	}

	public float Volume {
		get { return source.volume; }
		set { source.volume = value; }
	}

	private AudioSource source;
	private AudioClip[] clips;
	private Coroutine playClip;
	private AudioPlayerState state;

	void Awake() {
		source = gameObject.GetComponent<AudioSource>();
		state = AudioPlayerState.stop;
	}

	public void SetClips(AudioClip[] clips) {
		this.clips = clips;
	}

	public void Play() {
		if (clips == null) { return; }
		if (state == AudioPlayerState.pause) {
			playClip = StartCoroutine(playAudio());
		}
		else if (state == AudioPlayerState.stop || state == AudioPlayerState.play) {
			source.clip = GetNextClip();
			playClip = StartCoroutine(playAudio());
		}
		state = AudioPlayerState.play;
	}

	public void Pause() {
		source.Pause();
		StopCoroutine(playClip);
		state = AudioPlayerState.pause;
	}

	public void Stop() {
		source.Stop();
        StopCoroutine(playClip);
		state = AudioPlayerState.stop;
	}

	private AudioClip GetNextClip() {
		return clips[UnityEngine.Random.Range(0, clips.Length)];
	}

	IEnumerator playAudio() {
		source.Play();
		yield return new WaitForSeconds(source.clip.length - source.time);
		finishPlaySound();
	}

	void finishPlaySound() {
		source.Stop();
		Play();
	}
}