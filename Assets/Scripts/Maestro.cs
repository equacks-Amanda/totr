﻿/*  Maestro - Noah Nam
 * 
 *  Desc:   Facilitates all in-game music and sound effects.
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public sealed class Maestro : MonoBehaviour {
    [SerializeField] private AudioMixer am_masterMix;

	[Header("Audio Sources")]
	[SerializeField] private AudioSource as_bgmA;			// background music audio source
	[SerializeField] private AudioSource as_bgmB;			// background music audio source
	[SerializeField] private AudioSource as_volatility;	// volatility ambience audio source
	[SerializeField] private AudioSource as_sfxHi;		// high priority sound effect audio source
	[SerializeField] private AudioSource as_sfxMe;		// medium priority sound effect audio source
	[SerializeField] private AudioSource as_sfxLo;		// low priority sound effect audio source
	[SerializeField] private AudioSource as_voi;			// voice audio source
	public AudioSource As_voi{get{return as_voi;}}
	
	[Header("Audio Clips")]
	[SerializeField] private AudioClip ac_windShoot;
	[SerializeField] private AudioClip ac_windHit;
	[SerializeField] private AudioClip ac_iceShoot;
	[SerializeField] private AudioClip ac_electricShoot;
	[SerializeField] private AudioClip[] ac_magicMissileShoot;
	[SerializeField][Range(0, 2)] private float f_spellVolume;
	[SerializeField] private AudioClip ac_contact_generic;
	[SerializeField] private AudioClip ac_spell_charge;
	
	[SerializeField] private AudioClip ac_enemyHit;
	[SerializeField] private AudioClip[] ac_skeleton_die;
	[SerializeField] private AudioClip[] ac_heavy_skeleton_die;
	[SerializeField] private AudioClip[] ac_necromancer_die;
	[SerializeField] private AudioClip[] ac_skeleton_spawn;
	[SerializeField] private AudioClip ac_necromancer_spawn;
	[SerializeField][Range(0, 6)] private float f_necromancer;
	[SerializeField] private AudioClip[] ac_heavy_skeleton_footstep;
	
	[SerializeField] private AudioClip ac_playerSpawn;
	[SerializeField] private AudioClip ac_playerDie;
	[SerializeField][Range(0, 6)] private float f_playerDie;
	
	[SerializeField] private AudioClip ac_portal;
	[SerializeField] private AudioClip ac_score;
	[SerializeField] private AudioClip ac_objective_start;
	
	[SerializeField] private AudioClip[] ac_puck_bounce;
	[SerializeField][Range(0, 2)] private float f_puck_bounce;
	
	[SerializeField] private AudioClip[] ac_rift_roar;
	
	[SerializeField] private AudioClip[] ac_bgm;
	[SerializeField] private AudioClip ac_bgm_menu;
	

    [SerializeField] private AudioClip[] ac_volatility_ambience;
	[SerializeField] private AudioClip[] ac_volatility_noise_0;
	[SerializeField] private AudioClip[] ac_volatility_noise_1;
	[SerializeField] private AudioClip[] ac_volatility_noise_2;
	[SerializeField] private AudioClip[] ac_volatility_noise_3;
	[SerializeField] private AudioClip[] ac_volatility_noise_4;
	[SerializeField] private AudioClip[] ac_player_footstep;
	[SerializeField] private AudioClip[] ac_player_hit;
	[SerializeField] private AudioClip[] ac_player_clothing;
	
	[Header("Audio Clips (Announcer)")]
	[SerializeField] private AudioClip[] ac_generic;
	[SerializeField] private AudioClip[] ac_team_encouragement;
	[SerializeField] private AudioClip[] ac_volatility_up;
	[SerializeField] private AudioClip[] ac_trial_transition;
	[SerializeField] private AudioClip[] ac_wisp_generic;
	[SerializeField] private AudioClip[] ac_intro;
	[SerializeField] private AudioClip[] ac_enemy_hit_player;
	[SerializeField] private AudioClip[] ac_rift_hit_player;
	[SerializeField] private AudioClip[] ac_wind_hit_player;
	[SerializeField] private AudioClip[] ac_ice_hit_player;
	[SerializeField] private AudioClip[] ac_spell_hit_player;
	[SerializeField] private AudioClip[] ac_generic_hit_player;
	[SerializeField] private AudioClip[] ac_board_clear;
	[SerializeField] private AudioClip[] ac_first_score;
	[SerializeField] private AudioClip[] ac_score_comeback;
	[SerializeField] private AudioClip[] ac_score_loser;
	[SerializeField] private AudioClip[] ac_friendly_fire;
	[SerializeField] private AudioClip[] ac_player_spawn;
	[SerializeField] private AudioClip[] ac_portal_announcement_one;
	[SerializeField] private AudioClip[] ac_portal_announcement_two;
	[SerializeField] private AudioClip[] ac_portal_announcement_three;
	[SerializeField] private AudioClip[] ac_portal_announcement_four;
	[SerializeField] private AudioClip[] ac_score_announcement;
	[SerializeField] private AudioClip[] ac_idle;
	[SerializeField] private AudioClip[] ac_begin_ctf;
	[SerializeField] private AudioClip[] ac_begin_hockey;
	[SerializeField] private AudioClip[] ac_begin_crystal_destruction;
	[SerializeField] private AudioClip[] ac_begin_necromancer;
	[SerializeField] private AudioClip[] ac_begin_rift_boss;
	[SerializeField] private AudioClip[] ac_begin_potato;
	[SerializeField] private AudioClip[] ac_tutorial;
	[SerializeField] private AudioClip ac_tutorial_more;
	
	[Header("UI")]
	[SerializeField] private AudioClip[] ac_page_turn;
	[SerializeField] private AudioClip[] ac_tap;
	[SerializeField] private AudioClip[] ac_click;
	[SerializeField] private AudioClip[] ac_buzz;
	
	[Header("Settings")]
	[SerializeField] private float f_announcementDelay;		// An announcement can only play after this many seconds have elapsed.
	[SerializeField] private float f_genericAnnouncementDelay;
	[Range(0,1)] public float f_announcementChance;	// Announcements have a probability of playing.
	private bool b_announcementOk = false;
	private bool b_ctfExplained = false;
	private bool b_hockeyExplained = false;
	private bool b_potatoExplained = false;
	private bool b_destroyExplained = false;
	private bool b_necromancerExplained = false;
	private bool b_bossExplained = false;
	private bool b_bgmAIsOn = true;	// BGM switches between Sources A and B for crossfade. Which one is the current source we're hearing from?
	private bool b_alreadyFading = false;
	
    // Singleton
    private static Maestro instance;
	
	void Awake(){
		instance = this;
	}
	
    public static Maestro Instance {
        get { return instance; }
    }

    public void PlayVolatilityAmbience(int i) {
        as_volatility.clip = ac_volatility_ambience[i];
        as_volatility.Play();
    }
	
	public void PlayVolatilityNoise(int i) {
		
		switch(i){
			case(0):
				as_sfxMe.PlayOneShot(ac_volatility_noise_0[Constants.R_Random.Next(0, ac_volatility_noise_0.Length)],0.5f);
				break;
			case(1):
				as_sfxMe.PlayOneShot(ac_volatility_noise_1[Constants.R_Random.Next(0, ac_volatility_noise_1.Length)],0.5f);
				break;
			case(2):
				as_sfxMe.PlayOneShot(ac_volatility_noise_2[Constants.R_Random.Next(0, ac_volatility_noise_2.Length)],0.5f);
				break;
			case(3):
				as_sfxMe.PlayOneShot(ac_volatility_noise_3[Constants.R_Random.Next(0, ac_volatility_noise_3.Length)],0.5f);
				break;
			case(4):
				as_sfxMe.PlayOneShot(ac_volatility_noise_4[Constants.R_Random.Next(0, ac_volatility_noise_4.Length)],0.5f);
				break;
		}
    }
	
	private void PlaySingle(AudioSource s, AudioClip c, float volume = 1f){
		s.PlayOneShot(c,volume);
	}
	
	private void PlayRandom(AudioSource s, AudioClip[] c, float volume = 1f){
		
		s.PlayOneShot(c[Constants.R_Random.Next(0, c.Length)],volume);
	}
	
	private void PlaySingleAnnouncement(AudioClip c){
		
		if(b_announcementOk && Constants.R_Random.NextDouble() <= f_announcementChance && !as_voi.isPlaying){
				b_announcementOk = false;
				as_voi.clip = c;
				as_voi.Play();
				Invoke("AnnouncementOk",f_announcementDelay);
		}
	}
	private void PlayRandomAnnouncement(AudioClip[] c){
		
		if(b_announcementOk && Constants.R_Random.NextDouble() <= f_announcementChance && !as_voi.isPlaying){
				b_announcementOk = false;
				as_voi.clip = c[Constants.R_Random.Next(0, c.Length)];
				as_voi.Play();
				Invoke("AnnouncementOk",f_announcementDelay);
		}
	}
	
	public void PlayWindShoot(){
		PlaySingle(as_sfxHi,ac_windShoot,f_spellVolume);
	}
	public void PlayWindHit(){
		PlaySingle(as_sfxHi,ac_windHit,f_spellVolume);
	}
	public void PlayIceShoot(){
		PlaySingle(as_sfxHi,ac_iceShoot,f_spellVolume);
	}
	public void PlayElectricShoot(){
		PlaySingle(as_sfxHi,ac_electricShoot,f_spellVolume);
	}
	public void PlayMagicMissileShoot(){
		PlayRandom(as_sfxHi,ac_magicMissileShoot,f_spellVolume);
	}
	public void PlayContactGeneric(){
		PlaySingle(as_sfxHi,ac_contact_generic);
	}
	public void PlaySpellCharge(){
		//PlaySingle(as_sfxHi,ac_spell_charge);
	}
	public void PlayEnemyHit(){
		//PlaySingle(as_sfxLo,ac_enemyHit);
		as_sfxLo.PlayOneShot(ac_enemyHit,0.5f);
	}
	public void PlaySkeletonDie(){
		PlayRandom(as_sfxLo,ac_skeleton_die);
	}
	public void PlayHeavySkeletonDie(){
		PlayRandom(as_sfxLo,ac_heavy_skeleton_die);
	}
	public void PlayNecromancerDie(){
		PlayRandom(as_sfxHi,ac_necromancer_die,f_necromancer);
	}
	public void PlayNecromancerSpawn(){
		//PlaySingle(as_sfxHi,ac_necromancer_spawn);
		as_sfxHi.PlayOneShot(ac_necromancer_spawn,f_necromancer);
	}
	public void PlaySkeletonSpawn(){
		PlayRandom(as_sfxMe,ac_skeleton_spawn);
	}
	public void PlayHeavySkeletonFootstep(){
		PlayRandom(as_sfxLo,ac_heavy_skeleton_footstep);
	}
	public void PlayPlayerSpawn(){
		//PlaySingle(as_sfxHi,ac_playerSpawn);
		as_sfxHi.PlayOneShot(ac_playerSpawn,1.5f);
	}
	public void PlayPlayerDie(){
		PlaySingle(as_sfxHi,ac_playerDie,f_playerDie);
	}
	public void PlayPortal(){
		PlaySingle(as_sfxHi,ac_portal);
	}
	public void PlayScore(){
		PlaySingle(as_sfxHi,ac_score);
	}
	public void PlayObjectiveStart(){
		PlaySingle(as_sfxHi,ac_objective_start);
	}
	public void PlayPlayerFootstep(){
		//PlayRandom(as_sfxMe,ac_player_footstep);
	}
	public void PlayPlayerClothing(){
		//PlayRandom(as_sfxLo,ac_player_clothing);
	}
	public void PlayPlayerHit(){
		
		if(Constants.R_Random.NextDouble() <= .2f){
				//PlayRandom(as_sfxMe,ac_player_hit);
				as_sfxMe.PlayOneShot(ac_player_hit[Constants.R_Random.Next(0, ac_player_hit.Length)],0.4f);
			}
		//PlayRandom(as_sfxMe,ac_player_hit);
	}
	public void PlayPuckBounce(){
		PlayRandom(as_sfxLo,ac_puck_bounce,f_puck_bounce);
	}
	public void PlayRiftRoar(){
		PlayRandom(as_sfxHi,ac_rift_roar);
	}
	
	public void PlayAnnouncmentPlayerHit(int playerNum, Constants.Global.DamageType d){
		
		if(b_announcementOk && Constants.R_Random.NextDouble() <= f_announcementChance && !as_voi.isPlaying){
			b_announcementOk = false;
			switch(d){
				case(Constants.Global.DamageType.ENEMY):
					if(Constants.R_Random.NextDouble() <= .2f){
						as_voi.clip = ac_enemy_hit_player[Constants.R_Random.Next(0, ac_enemy_hit_player.Length)];
						as_voi.Play();
					}
					break;
				case(Constants.Global.DamageType.RIFT):
					as_voi.clip = ac_rift_hit_player[Constants.R_Random.Next(0, ac_rift_hit_player.Length)];
					as_voi.Play();
					break;
				case(Constants.Global.DamageType.WIND):
					as_voi.clip = ac_wind_hit_player[Constants.R_Random.Next(0, ac_wind_hit_player.Length)];
					as_voi.Play();
					break;
				case(Constants.Global.DamageType.ICE):
					as_voi.clip = ac_ice_hit_player[Constants.R_Random.Next(0, ac_ice_hit_player.Length)];
					as_voi.Play();
					break;
				case(Constants.Global.DamageType.MAGICMISSILE):
				case(Constants.Global.DamageType.ELECTRICITY):
					as_voi.clip = ac_spell_hit_player[Constants.R_Random.Next(0, ac_spell_hit_player.Length)];
					as_voi.Play();
					break;
				default:
					as_voi.clip = ac_generic_hit_player[Constants.R_Random.Next(0, ac_generic_hit_player.Length)];
					as_voi.Play();
					break;
			}
			Invoke("AnnouncementOk",f_announcementDelay);
		}
	}
	public void PlayAnnouncementFirstScore(){
		PlayRandomAnnouncement(ac_first_score);
	}
	public void PlayAnnouncementScoreComeback(){
		PlayRandomAnnouncement(ac_score_comeback);
	}
	public void PlayAnnouncementScoreLoser(){
		PlayRandomAnnouncement(ac_score_loser);
	}
	public void PlayAnnouncementScore(){
		PlayRandomAnnouncement(ac_score_announcement);
	}
	public void PlayAnnouncementFriendlyFire(){
		PlayRandomAnnouncement(ac_friendly_fire);
	}
	public void PlayAnnouncementPlayerSpawn(){
		PlayRandomAnnouncement(ac_player_spawn);
	}
	public void PlayAnnouncementIdle(){
		PlayRandomAnnouncement(ac_idle);
	}
	public void PlayAnnouncementPortal(int playerNum){
		if(b_announcementOk && Constants.R_Random.NextDouble() <= f_announcementChance && !as_voi.isPlaying){
			b_announcementOk = false;
			switch(playerNum){
				case(0):
					as_voi.clip = ac_portal_announcement_one[Constants.R_Random.Next(0, ac_portal_announcement_one.Length)];
					as_voi.Play();
					break;
				case(1):
					as_voi.clip = ac_portal_announcement_two[Constants.R_Random.Next(0, ac_portal_announcement_two.Length)];
					as_voi.Play();
					break;
				case(2):
					as_voi.clip = ac_portal_announcement_three[Constants.R_Random.Next(0, ac_portal_announcement_three.Length)];
					as_voi.Play();
					break;
				case(3):
					as_voi.clip = ac_portal_announcement_four[Constants.R_Random.Next(0, ac_portal_announcement_four.Length)];
					as_voi.Play();
					break;
			}
			Invoke("AnnouncementOk",f_announcementDelay);
		}
	}
	public void PlayAnnouncementGeneric(){
		PlayRandomAnnouncement(ac_generic);
	}
	public void PlayTeamEncouragement(){
		PlayRandomAnnouncement(ac_team_encouragement);
	}
	public void PlayAnnouncementVolatilityUp(){
		PlayRandomAnnouncement(ac_volatility_up);
	}
	public void PlayAnnouncementTrialTransition(){
		PlayRandomAnnouncement(ac_trial_transition);
	}
	public void PlayAnnouncementWispGeneric(){
		PlayRandomAnnouncement(ac_wisp_generic);
	}
	public void PlayAnnouncementTutorial(){
		if(!as_voi.isPlaying){
			StartCoroutine("PlayAnnouncementTutorialFull");
		}
		else
			Invoke("PlayAnnouncementTutorial",1);
	}
	IEnumerator PlayAnnouncementTutorialFull(){
		b_announcementOk = false;
		as_voi.clip = ac_tutorial[Constants.R_Random.Next(0, ac_intro.Length)];
		as_voi.Play();
		yield return new WaitForSecondsRealtime(as_voi.clip.length);
		as_voi.clip = ac_tutorial_more;
		as_voi.Play();
	}
	public void PlayAnnouncementIntro(){
		
		if(!as_voi.isPlaying){
			b_announcementOk = false;
			as_voi.clip = ac_intro[Constants.R_Random.Next(0, ac_intro.Length)];
			as_voi.Play();
			Invoke("AnnouncementOk",f_announcementDelay);
		}
		else
			Invoke("PlayAnnouncementIntro",1);
	}
	public void PlayAnnouncementBoardClear(){
		PlayRandomAnnouncement(ac_board_clear);
	}
	public void PlayBeginCTF(){
		if(b_ctfExplained)return;
		if(!as_voi.isPlaying){
			b_ctfExplained = true;
			as_voi.clip = ac_begin_ctf[Constants.R_Random.Next(0, ac_begin_ctf.Length)];
			as_voi.Play();
		}
		else
			Invoke("PlayBeginCTF",1);
	}
	public void PlayBeginNecromancer(){
		if(b_necromancerExplained)return;
		if(!as_voi.isPlaying){
			b_necromancerExplained = true;
			as_voi.clip = ac_begin_necromancer[Constants.R_Random.Next(0, ac_begin_necromancer.Length)];
			as_voi.Play();
		}
		else
			Invoke("PlayBeginNecromancer",1);
	}
	public void PlayBeginCrystalDestruction(){
		if(b_destroyExplained)return;
		if(!as_voi.isPlaying){
			b_destroyExplained = true;
			as_voi.clip = ac_begin_crystal_destruction[Constants.R_Random.Next(0, ac_begin_crystal_destruction.Length)];
			as_voi.Play();
		}
		else
			Invoke("PlayBeginCrystalDestruction",1);
	}
	public void PlayBeginHockey(){
		if(b_hockeyExplained)return;
		if(!as_voi.isPlaying){
			b_hockeyExplained = true;
			as_voi.clip = ac_begin_hockey[Constants.R_Random.Next(0, ac_begin_hockey.Length)];
			as_voi.Play();
		}
		else
			Invoke("PlayBeginHockey",1);
	}
	public void PlayBeginRiftBoss(){
		if(b_bossExplained)return;
		if(!as_voi.isPlaying){
			b_bossExplained = true;
			as_voi.clip = ac_begin_rift_boss[Constants.R_Random.Next(0, ac_begin_rift_boss.Length)];
			as_voi.Play();
		}
		else
			Invoke("PlayBeginRiftBoss",1);
	}
	public void PlayBeginPotato(){
		if(b_potatoExplained)return;
		if(!as_voi.isPlaying){
			b_potatoExplained = true;
			as_voi.clip = ac_begin_potato[Constants.R_Random.Next(0, ac_begin_potato.Length)];
			as_voi.Play();
		}
		else
			Invoke("PlayBeginPotato",1);
	}
	
	
	public void PlayUIMove(){
		PlayRandom(as_sfxHi,ac_click);
	}
	public void PlayUISubmit(){
		PlayRandom(as_sfxHi,ac_tap);
	}

	// Use this for initialization
	void Start () {
		b_announcementOk = true;
		InvokeRepeating("GenericOk",f_genericAnnouncementDelay,f_genericAnnouncementDelay);
		if(SceneManager.GetActiveScene().name == "WarmUp")
			PlayAnnouncementTutorial();
		else if(SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "RegisterPlayers" || SceneManager.GetActiveScene().name == "EndGame")
			as_bgmA.clip = ac_bgm_menu;
			as_bgmA.Play();
	}
	
	// Takes volatility level as a parameter.
	public void ChangeBGM(int i){
		AudioSource from, to;
		if(b_bgmAIsOn){
			from = as_bgmA;
			to = as_bgmB;;
		}
		else{
			from = as_bgmB;
			to = as_bgmA;
		}
		
		
		if(!b_alreadyFading){
			to.clip = ac_bgm[i+1];
			to.timeSamples = from.timeSamples;
			to.volume = 0;
			from.volume = 1;
			to.Play();
			StartCoroutine(Crossfade(from,to));
			b_bgmAIsOn = !b_bgmAIsOn;
		}
		else{
			StartCoroutine("Wait",i);
		}
	}
	
	IEnumerator Crossfade(AudioSource from, AudioSource to){
		b_alreadyFading = true;
		while(to.volume < 1f){
			from.volume -= 0.1f;
			to.volume += 0.1f;
			yield return new WaitForSecondsRealtime(.02f);
		}
		from.Stop();
		b_alreadyFading = false;
	}
	
	IEnumerator Wait(int i){
		yield return new WaitForSecondsRealtime(1f);
		ChangeBGM(i);
	}
	
	private void AnnouncementOk(){
		b_announcementOk = true;
	}
	
	private void GenericOk(){
		PlayAnnouncementGeneric();
	}

    public void AdjustMasterVolume(float f_volIn) {
		Constants.VolOptions.C_MasterVolume = f_volIn;
        am_masterMix.SetFloat("VolumeMaster",20*Mathf.Log(Constants.VolOptions.C_MasterVolume,10));
    }

    public void AdjustBGMVolume(float f_volIn) {
        Constants.VolOptions.C_BGMVolume = f_volIn;
        am_masterMix.SetFloat("VolumeBGM",20*Mathf.Log(Constants.VolOptions.C_BGMVolume,10));
    }

    public void AdjustSFXVolume(float f_volIn) {
        Constants.VolOptions.C_SFXVolume = f_volIn;
        am_masterMix.SetFloat("VolumeSFX",20*Mathf.Log(Constants.VolOptions.C_SFXVolume,10));
    }

    public void AdjustVOIVolume(float f_volIn) {
        Constants.VolOptions.C_VOIVolume = f_volIn;
        am_masterMix.SetFloat("VolumeVOI",20*Mathf.Log(Constants.VolOptions.C_VOIVolume,10));
    }
}
