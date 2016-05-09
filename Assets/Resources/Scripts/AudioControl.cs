using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AudioControl : MonoBehaviour {
	private int numZombieClips = 12;
	private int numSurvivorClips = 8;
	private int numDeathClips = 5;
	private int zombieStart = 0;
	private int survivorStart, deathStart;
	private Dictionary<int, float> volDict;

	public enum Clip {
		zombieRoar1,
		zombieRoar2,
		zombieRoar3,
		zombieCheer1,
		zombieCheer2,
		zombieHoard, 
		gunFire1,
		gunFire2,
		captureSound1,
		captureSound2,
		captureSound3,
		beepSound,

		christmas,
		gimmeSome,
		letsGo,
		myLife,
		ohYeah1,
		ohYeah2,
		outtaWay,
		huh,

		death1,
		death2,
		death3,
		death4,
		death5
	}
		

	AudioSource source1;
	AudioSource source2;
	AudioSource source3;

	public AudioClip[] audioClips;
	public AudioClip[] survivorClips;
	public AudioClip[] survivorDeath;
	GameManager gm;

	public void init (GameManager m) {
		gm = m;
		audioClips = new AudioClip[Enum.GetNames(typeof(Clip)).Length];
		survivorClips = new AudioClip[numSurvivorClips];
		survivorDeath = new AudioClip[numDeathClips];
		source1 = gameObject.AddComponent<AudioSource>();
		source2 = gameObject.AddComponent<AudioSource>();
		source3 = gameObject.AddComponent<AudioSource>();
		//print(source);

		audioClips[(int)Clip.zombieRoar1] = Resources.Load("Audio/Diagetic Sounds/Zombie Roar 1", typeof(AudioClip)) as AudioClip;
		audioClips[(int)Clip.zombieRoar2] = Resources.Load("Audio/Diagetic Sounds/Zombie Roar 2", typeof(AudioClip)) as AudioClip;
		audioClips[(int)Clip.zombieRoar3] = Resources.Load("Audio/Diagetic Sounds/Zombie Roar 3", typeof(AudioClip)) as AudioClip;
		audioClips[(int)Clip.zombieCheer1] = Resources.Load("Audio/Diagetic Sounds/Zombie Cheer 1", typeof(AudioClip)) as AudioClip;
		audioClips[(int)Clip.zombieCheer2] = Resources.Load("Audio/Diagetic Sounds/Zombie Cheer 2", typeof(AudioClip)) as AudioClip;
		audioClips[(int)Clip.zombieHoard] = Resources.Load("Audio/Diagetic Sounds/Zombie Hoard Loop", typeof(AudioClip)) as AudioClip;
		audioClips[(int)Clip.gunFire1] = Resources.Load("Audio/Diagetic Sounds/Arcade Blip 1", typeof(AudioClip)) as AudioClip;
		audioClips[(int)Clip.gunFire2] = Resources.Load("Audio/Diagetic Sounds/Arcade Blip 2", typeof(AudioClip)) as AudioClip;
		audioClips[(int)Clip.captureSound1] = Resources.Load("Audio/Voice/Asides/Wrapped Up", typeof(AudioClip)) as AudioClip;
		audioClips[(int)Clip.captureSound2] = Resources.Load("Audio/Voice/Asides/Christmas", typeof(AudioClip)) as AudioClip;
		audioClips[(int)Clip.captureSound3] = Resources.Load("Audio/Voice/Asides/That's Over", typeof(AudioClip)) as AudioClip;
		audioClips[(int)Clip.beepSound] = Resources.Load("Audio/Diagetic Sounds/Arcade Beep 02", typeof(AudioClip)) as AudioClip;

		survivorStart = (int)Clip.gimmeSome;
		survivorClips[(int)Clip.gimmeSome - survivorStart] = Resources.Load("Audio/Voice/Asides/Gimmie Some", typeof(AudioClip)) as AudioClip;
		survivorClips[(int)Clip.letsGo - survivorStart] = Resources.Load("Audio/Voice/Asides/Let's Go", typeof(AudioClip)) as AudioClip;
		survivorClips[(int)Clip.myLife - survivorStart] = Resources.Load("Audio/Voice/Asides/My Life", typeof(AudioClip)) as AudioClip;
		survivorClips[(int)Clip.ohYeah1 - survivorStart] = Resources.Load("Audio/Voice/Asides/Oh Yeah 1", typeof(AudioClip)) as AudioClip;
		survivorClips[(int)Clip.ohYeah2 - survivorStart] = Resources.Load("Audio/Voice/Asides/Oh Yeah 2", typeof(AudioClip)) as AudioClip;
		survivorClips[(int)Clip.outtaWay - survivorStart] = Resources.Load("Audio/Voice/Asides/Outta My Way", typeof(AudioClip)) as AudioClip;
		survivorClips[(int)Clip.huh - survivorStart] = Resources.Load("Audio/Voice/Asides/Uh Huh", typeof(AudioClip)) as AudioClip;

		deathStart = (int)Clip.death1;
		survivorDeath[(int)Clip.death1 - deathStart] = Resources.Load("Audio/Voice/Body Noises/Cough 1", typeof(AudioClip)) as AudioClip;
		survivorDeath[(int)Clip.death2 - deathStart] = Resources.Load("Audio/Voice/Body Noises/Cough 2", typeof(AudioClip)) as AudioClip;
		survivorDeath[(int)Clip.death3 - deathStart] = Resources.Load("Audio/Voice/Body Noises/Cough 3", typeof(AudioClip)) as AudioClip;
		survivorDeath[(int)Clip.death4 - deathStart] = Resources.Load("Audio/Voice/Body Noises/Ugh", typeof(AudioClip)) as AudioClip;
		survivorDeath[(int)Clip.death5 - deathStart] = Resources.Load("Audio/Voice/Body Noises/Vomit", typeof(AudioClip)) as AudioClip;

		volDict = new Dictionary<int, float>();
		volDict.Add((int)Clip.beepSound, .3f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void playClip(int clipIndex, AudioSource source){
		if (clipIndex >= deathStart) {
			clipIndex -= deathStart;
		}
		else if (clipIndex >= survivorStart) {
			clipIndex -= survivorStart;
		}
		float vol = 1;
		if (volDict.ContainsKey(clipIndex)) {
			vol = volDict[clipIndex];
		}
		else if (clipIndex <= (int)Clip.zombieHoard) {
			vol = .3f;
		}
		else if (clipIndex <= (int)Clip.gunFire2) {
			vol = .15f;
		}
		source.PlayOneShot(audioClips[clipIndex], vol);
	}

	public void playClip(Clip clip, AudioSource source) {
		playClip((int)clip, source);
	}

	public AudioClip[] getSurvivorSounds(){
		return survivorClips;
	}

	public AudioClip[] getDeathSounds(){
		return survivorDeath;
	}

	public AudioSource getSource1(){
		return source1;
	}

	public AudioSource getSource2(){
		return source2;
	}

	public AudioSource getSource3(){
		return source3;
	}
}
