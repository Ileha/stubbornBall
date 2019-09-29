using System;
using UnityEngine;

public class Singleton {
	private static Singleton instanse;
	public static Singleton Instanse {
		get {
			if (instanse == null) {
				instanse = new Singleton();
			}
			return instanse;
		}
	}

	public Material LineMaterial { get; private set; }
	public ParticleSystem OnStar { get; private set; }
	public ParticleSystem OnEnd  { get; private set; }
	public ProgramData Data  { get; private set; }			//данные о игре (сохранения)
	public AdData AdSettings  { get; private set; }			//для работы с рекламой
	public LevelIcon LevelIcon  { get; private set; }
	public AudioClip[] clips  { get; private set; }

	public SceneIndependent SceneInformation;

	public Singleton() {
		LineMaterial = Resources.Load<Material>("materials/line");

		OnStar = Resources.Load<ParticleSystem>("OnStar");
		OnEnd = Resources.Load<ParticleSystem>("OnEnd");

		Data = new ProgramData();
		Data.Load();

		LevelIcon = Resources.Load<LevelIcon>("LevelIcon");

		AdSettings = Resources.Load<AdData>("AdSettings");
		AdSettings.Init();

		clips = Resources.LoadAll<AudioClip>("music");

		SceneInformation = new ShowMain();

		AudioPlayer.CurrentAudioPlayer.SetClips(clips);
		AudioPlayer.CurrentAudioPlayer.Play();
	}
}