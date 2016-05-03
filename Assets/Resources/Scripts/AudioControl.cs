using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AudioControl : MonoBehaviour {

	public enum clips {
		zombieRoar1,
		zombieRoar2,
		zombieRoar3,
		zombieCheer,
		zombieHoard, 
		alarm
	}

	AudioSource source;
	public AudioClip[] audioClips;
	GameManager gm;

	public void init (GameManager m) {
		gm = m;
		audioClips = new AudioClip[Enum.GetNames(typeof(clips)).Length];
		source = gameObject.AddComponent<AudioSource>();
		//print(source);

		audioClips[(int)clips.zombieRoar1] = Resources.Load("Audio/Diagetic Sounds/Zombie Roar 1", typeof(AudioClip)) as AudioClip;
		audioClips[(int)clips.zombieRoar2] = Resources.Load("Audio/Diagetic Sounds/Zombie Roar 2", typeof(AudioClip)) as AudioClip;
		audioClips[(int)clips.zombieRoar3] = Resources.Load("Audio/Diagetic Sounds/Zombie Roar 3", typeof(AudioClip)) as AudioClip;
		audioClips[(int)clips.zombieCheer] = Resources.Load("Audio/Diagetic Sounds/Zombie Cheer 1", typeof(AudioClip)) as AudioClip;
		audioClips[(int)clips.zombieHoard] = Resources.Load("Audio/Diagetic Sounds/Zombie Hoard Loop", typeof(AudioClip)) as AudioClip;
		audioClips[(int)clips.alarm] = Resources.Load("Audio/Diagetic Sounds/Arcade Alarm 2", typeof(AudioClip)) as AudioClip;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void playClip(int clipIndex){
		source.PlayOneShot(audioClips[clipIndex]);
	}

	public AudioSource getSource(){
		return source;
	}
}
