using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AudioControl : MonoBehaviour {

	public enum clips {
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
		captureSound3
	}

	public enum survivor {
		christmas,
		gimmeSome,
		letsGo,
		myLife,
		ohYeah1,
		ohYeah2,
		oneShot,
		outtaWay,
		huh
	}

	public enum death{
		death1,
		death2,
		death3,
		death4,
		death5
	}
		

	AudioSource onClickSource;
	AudioSource onPointSource;
	public AudioClip[] audioClips;
	public AudioClip[] survivorClips;
	public AudioClip[] survivorDeath;
	GameManager gm;

	public void init (GameManager m) {
		gm = m;
		audioClips = new AudioClip[Enum.GetNames(typeof(clips)).Length];
		survivorClips = new AudioClip[Enum.GetNames(typeof(survivor)).Length];
		survivorDeath = new AudioClip[Enum.GetNames(typeof(death)).Length];
		onClickSource = gameObject.AddComponent<AudioSource>();
		onPointSource = gameObject.AddComponent<AudioSource>();
		//print(source);

		audioClips[(int)clips.zombieRoar1] = Resources.Load("Audio/Diagetic Sounds/Zombie Roar 1", typeof(AudioClip)) as AudioClip;
		audioClips[(int)clips.zombieRoar2] = Resources.Load("Audio/Diagetic Sounds/Zombie Roar 2", typeof(AudioClip)) as AudioClip;
		audioClips[(int)clips.zombieRoar3] = Resources.Load("Audio/Diagetic Sounds/Zombie Roar 3", typeof(AudioClip)) as AudioClip;
		audioClips[(int)clips.zombieCheer1] = Resources.Load("Audio/Diagetic Sounds/Zombie Cheer 1", typeof(AudioClip)) as AudioClip;
		audioClips[(int)clips.zombieCheer2] = Resources.Load("Audio/Diagetic Sounds/Zombie Cheer 2", typeof(AudioClip)) as AudioClip;
		audioClips[(int)clips.zombieHoard] = Resources.Load("Audio/Diagetic Sounds/Zombie Hoard Loop", typeof(AudioClip)) as AudioClip;
		audioClips[(int)clips.zombieHoard] = Resources.Load("Audio/Diagetic Sounds/Zombie Hoard Loop", typeof(AudioClip)) as AudioClip;
		audioClips[(int)clips.gunFire1] = Resources.Load("Audio/Diagetic Sounds/Arcade Blip 1", typeof(AudioClip)) as AudioClip;
		audioClips[(int)clips.gunFire2] = Resources.Load("Audio/Diagetic Sounds/Arcade Blip 2", typeof(AudioClip)) as AudioClip;
		audioClips[(int)clips.captureSound1] = Resources.Load("Audio/Voice/Asides/Wrapped Up", typeof(AudioClip)) as AudioClip;
		audioClips[(int)clips.captureSound2] = Resources.Load("Audio/Voice/Asides/Christmas", typeof(AudioClip)) as AudioClip;
		audioClips[(int)clips.captureSound3] = Resources.Load("Audio/Voice/Asides/That's Over", typeof(AudioClip)) as AudioClip;

		survivorClips[(int)survivor.gimmeSome] = Resources.Load("Audio/Voice/Asides/Gimmie Some", typeof(AudioClip)) as AudioClip;
		survivorClips[(int)survivor.letsGo] = Resources.Load("Audio/Voice/Asides/Let's Go", typeof(AudioClip)) as AudioClip;
		survivorClips[(int)survivor.myLife] = Resources.Load("Audio/Voice/Asides/My Life", typeof(AudioClip)) as AudioClip;
		survivorClips[(int)survivor.ohYeah1] = Resources.Load("Audio/Voice/Asides/Oh Yeah 1", typeof(AudioClip)) as AudioClip;
		survivorClips[(int)survivor.ohYeah2] = Resources.Load("Audio/Voice/Asides/Oh Yeah 2", typeof(AudioClip)) as AudioClip;
		survivorClips[(int)survivor.oneShot] = Resources.Load("Audio/Voice/Asides/One Shot One Kill", typeof(AudioClip)) as AudioClip;
		survivorClips[(int)survivor.outtaWay] = Resources.Load("Audio/Voice/Asides/Outta My Way", typeof(AudioClip)) as AudioClip;
		survivorClips[(int)survivor.huh] = Resources.Load("Audio/Voice/Asides/Uh Huh", typeof(AudioClip)) as AudioClip;

		survivorDeath[(int)death.death1] = Resources.Load("Audio/Voice/Body Noises/Cough 1", typeof(AudioClip)) as AudioClip;
		survivorDeath[(int)death.death2] = Resources.Load("Audio/Voice/Body Noises/Cough 2", typeof(AudioClip)) as AudioClip;
		survivorDeath[(int)death.death3] = Resources.Load("Audio/Voice/Body Noises/Cough 3", typeof(AudioClip)) as AudioClip;
		survivorDeath[(int)death.death4] = Resources.Load("Audio/Voice/Body Noises/Ugh", typeof(AudioClip)) as AudioClip;
		survivorDeath[(int)death.death5] = Resources.Load("Audio/Voice/Body Noises/Vomit", typeof(AudioClip)) as AudioClip;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void playClip(int clipIndex, AudioSource source){
		source.PlayOneShot(audioClips[clipIndex]);
	}

	public void playSurvivorClip(int clipIndex, AudioSource source){
		source.PlayOneShot(survivorClips[clipIndex]);
	}

	public void playDeathClip(int clipIndex, AudioSource source){
		source.PlayOneShot(survivorDeath[clipIndex]);
	}



	public AudioClip[] getSurvivorSounds(){
		return survivorClips;
	}

	public AudioClip[] getDeathSounds(){
		return survivorDeath;
	}

	public AudioSource getClickSource(){
		return onClickSource;
	}

	public AudioSource getPointSource(){
		return onPointSource;
	}
}
