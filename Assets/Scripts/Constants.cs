﻿/*  Constants - Everyone
 * 
 *  Desc:   Holds all of the numbers used in the codebase.
 *          DebugParametersController can adjust these numbers.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants {
	public static System.Random R_Random = new System.Random();

    // Global Constants
    public static class Global {
        public enum Color { RED, BLUE, NULL };
        public enum Side { LEFT = -1, RIGHT = 1 };
		public enum DamageType { WIND, ICE, ELECTRICITY, MAGICMISSILE, ENEMY, RIFT, DEATHBOLT, RUNE, PUCK };
        public static string C_BuildNumber = "3.70";
        public static bool C_CanPause = true;
        public static Global.Color C_WinningTeam = Global.Color.BLUE;
		
    }

    // Team Stats
    public static class TeamStats {
        public static int C_RedTeamScore = 0;
        public static int C_BlueTeamScore = 0;
    }

    // Player Stats
    public static class PlayerStats {
        public static float C_MovementSpeed = 6.0f;
        public static float C_WispMovementSpeed = 1.5f;
        public static float C_RespawnTimer = 5.0f;
        public static float C_RespawnHealthSubDivision = 100.0f;
        public static float C_MaxHealth = 300.0f;
        public static float C_StepSoundDelay = 0.4f;
        public static float C_InvulnTime = 2.0f;
        public static float C_PlayerWindPushMultiplier = 2f;

        public static Global.Color C_p1Color = Global.Color.RED;
        public static Global.Color C_p2Color = Global.Color.RED;
        public static Global.Color C_p3Color = Global.Color.BLUE;
        public static Global.Color C_p4Color = Global.Color.BLUE;

        public static Vector3 C_r1Start = new Vector3(-7.62f, 0.0f, -3.87f);
        public static Vector3 C_r2Start = new Vector3(-7.62f, 0.0f, 2.45f);
        public static Vector3 C_b1Start = new Vector3(7.62f, 0.0f, 2.45f);
        public static Vector3 C_b2Start = new Vector3(7.62f, 0.0f, -3.87f);

        public static int C_p1Hat = 0;
        public static int C_p2Hat = 1;
        public static int C_p3Hat = 2;
        public static int C_p4Hat = 3;
    }

    // Spell Stats
    // TODO: Damage players?
    public static class SpellStats {
        // Common Spell Stats
        public enum SpellType { MAGICMISSILE, WIND, ICE, ELECTRICITY, ELECTRICITYAOE };
        public static float C_RiftDamageMultiplier = 2.0f;
        public static float C_SpellLiveTime = 2.0f;
        public static float C_SpellChargeTime = 3.0f;
        public static float C_NextSpellDelay = 0.5f;
        public static float C_PlayerProjectileSize = 0.5f;
        public static float C_SpellScaleMultiplier = 1.15f;
        public static float C_SpellParticlesLiveTime = 2.0f;

        // Magic Missile Stats
        public static float C_MagicMissileLiveTime = 0.35f; //2 sec in GDD
		public static float C_MagicMissileSpeed = 10.0f;
        public static float C_MagicMissileDamage = 25.0f; 
        public static float C_MagicMissileCooldown = 0.75f;
        public static float C_MagicMissileChargeTime = C_SpellChargeTime;
        public static float C_MagicMissileChargeCooldown = 0.5f;
        public static float C_MagicMissileHeal = 25.0f;

        // Wind Stats
        public static float C_WindLiveTime = 0.2f;
        public static float C_WindSpeed = 15.0f;
        public static float C_WindDamage = 50.0f;
        public static float C_WindCooldown = 1.0f;
        public static float C_WindChargeTime = C_SpellChargeTime;
        public static float C_WindRiftDamageMultiplier = C_RiftDamageMultiplier;
        public static float C_WindForce = 1500.0f; // 5m worth of distance - we need to do our own
        public static float C_WindPushTime = 0.3f;

        // Ice Stats
        // TODO: UHHHHH....? (live time vs. charge time... FIGHT)
        public static float C_IceLiveTime = 3.00f;
        public static float C_IceSpeed = 8.0f;
        public static float C_IceDamage = 75.0f;
        public static float C_IceCooldown = 5.0f;
        public static float C_IceChargeTime = C_SpellChargeTime;
        public static float C_IceRiftDamageMultiplier = C_RiftDamageMultiplier;
        public static float C_IceFreezeTime = 3.0f;
        public static float C_IceWallLiveTime = 5.0f;

        // it's ELECTRIC! (boogie woogie woogie) Stats
        // TODO: make charge time and Live time tied
        public static float C_ElectricLiveTime = 0.1f;
        public static float C_ElectricSpeed = 15.0f;
        //public static float C_ElectricDamage = 40.0f;
        public static float C_ElectricDamage = 1.3f;
        public static float C_ElectricCooldown = 8.0f;
        public static float C_ElectricChargeTime = C_SpellChargeTime;
        public static float C_ElectricRiftDamageMultiplier = C_RiftDamageMultiplier;
        public static float C_ElectricAOESlowDownMultiplier = 0.5f;
        public static float C_ElectricAOELiveTime = 5.0f;
        public static float C_ElectricAOEDamageRate = 0.5f;
    }

    // Objective Stats
    public static class ObjectiveStats {
        // Generic Objective Spawning Stats
        //public static Vector3 C_GenericRedObjectiveTargetSpawn = new Vector3(20.0f, 0.5f, 0f);
        //public static Vector3 C_GenericBlueObjectiveTargetSpawn = new Vector3(-20.0f, 0.5f, 0f);
        //public static Vector3 C_GenericRedObjectiveGoalSpawn = new Vector3(-4.5f, .01f, 0f);
        //public static Vector3 C_GenericBlueObjectiveGoalSpawn = new Vector3(4.5f, .01f, 0f);
        public static float C_NotificationTimer = 10.0f;
        public static float C_ScoringParticleLiveTime = 1.0f;

        // Potato Stats
        //public static Vector3 C_RedPotatoSpawn = new Vector3(-7.5f, 0.5f, 0f);
        //public static Vector3 C_BluePotatoSpawn = new Vector3(7.5f, 0.5f, 0f);
        public static int C_PotatoCompletionTimer = 30;
        public static int C_PotatoSelfDestructTimer = 15;
        public static int C_EnemySpawnAmount = 2;

        // CTF Stats
        public static Vector3 C_RedFlagSpawn = new Vector3(-16.0f, 0.5f, 0f);
        public static Vector3 C_BlueFlagSpawn = new Vector3(16.0f, 0.5f, 0f);
        //public static Vector3 C_RedCTFGoalSpawn = C_GenericRedObjectiveGoalSpawn;
        //public static Vector3 C_BlueCTFGoalSpawn = C_GenericBlueObjectiveGoalSpawn;
        public static int C_CTFMaxScore = 3;
        public static float C_FlagResetTimer = 10.0f;

        // Crystal Stats
        //public static Vector3 C_RedCrystalSpawn = C_GenericBlueObjectiveTargetSpawn;
        //public static Vector3 C_BlueCrystalSpawn = C_GenericRedObjectiveTargetSpawn;
        public static float C_CrystalMaxHealth = 500.0f;
        public static float C_CrystalRegenHeal = 5.0f;
        public static float C_CrystalHealRate = 1.0f;
        public static float C_CrystalHealDelay = 3.0f;
        //public static float C_CrystalSpellDamage = -25.0f;
        //public static float C_CrystalMagicMissileDamage = -5.0f;

        // Ice Hockey Stats
        public static Vector3 C_RedPuckSpawn = new Vector3(-5.0f, 0.5f, 0f);
        public static Vector3 C_BluePuckSpawn = new Vector3(5.0f, 0.5f, 0f);
        //public static Vector3 C_RedHockeyGoalSpawn = new Vector3(-20.0f, 0.01f, 0.0f);
        //public static Vector3 C_BlueHockeyGoalSpawn = new Vector3(20.0f, 0.01f, 0.0f);
        public static int C_HockeyMaxScore = 3;
        public static float C_PuckDamage = 10.0f;
        public static float C_PuckSpeedDecayDelay = 3.0f;       // time after hit to wait before decreasing speed
        public static float C_PuckSpeedDecayRate = 1.0f;        // time between successive speed decreases (after first)
        public static float C_PuckSpeedDecreaseAmount = 2.0f;   // speed decrease amount at each interval
        public static float C_PuckSpeedHitIncrease = 5.0f;      // speed increase every time puck is hit
        public static float C_PuckBaseSpeed = 10.0f;
        public static float C_PuckMaxSpeed = 15.0f;
		public static float C_PuckWindPushMultiplier = 1f;

        // Defeat Necromancers Stats
        public static int C_NecromancersMaxScore = 3;
        public static Vector3 C_RedNecromancerSpawn = new Vector3(-5.0f, 0.5f, 0f);
        public static Vector3 C_BlueNecromancerSpawn = new Vector3(5.0f, 0.5f, 0f);
        public static float C_NecromancerSpawnTime = 6.0f;
        public static float C_NecromancerTeleportHealthThreshold = 0.5f;

        // Rift Boss Stats
        //public static Vector3 C_RedRiftBossSpawn = new Vector3(-5.0f, 0.5f, 0f);
        //public static Vector3 C_BlueRiftBossSpawn = new Vector3(5.0f, 0.5f, 0f);
        public static float C_RiftBossMaxHealth = 4000f;
        public static int C_RiftBossHealthReductionMultiplier = 200;
        public static float C_RuneSpawnInterval = 8.0f;
        public static float C_DeathBoltCooldown = 6.0f;
        public static float C_ForceFieldCooldown = 7.0f;
        public static float C_RuneDamage = 75.0f;
		public static int C_RiftBossEnemies = 3;
    }
       
    // Enemy Stats
    public static class EnemyStats {

        public static float C_NecromancerBaseSpeed = 1.5f;
        public static float C_NecromancerHealth = 400.0f;
		public static float C_NecromancerAvoidDistance = 5.0f;
		public static int C_NecromancerSpawnCapPerSide = 1;
		public static float C_WanderingRadius = 10.0f;
		public static float C_RuneExplosionCountDownTime = 0.5f;
        public static float C_RuneExplosionLiveTime = 4f;
        public static float C_RuneTimer = 4.0f;
		public static float C_SummonTimer = 8.0f;
        public static float C_RuneDamage = 75.0f;

        public static int C_EnemySpawnCapPerSide = 6;
        public static float C_EnemyBaseSpeed = 1.2f;
		public static float C_EnemyAttackRange = 1.5f;
        public static float C_EnemyHealth = 125.0f;
        public static float C_EnemyDamage = 25.0f;
        public static float C_SpawnRadius = 2.5f;
        //public static float C_EnemySpawnDelayDuration = 2.0f;
        public static float C_MapBoundryXAxis = 14.5f;
        public static float C_MapBoundryZAxis = 9.5f;
		
		public static float C_SkeletonWindPushMultiplier = 0.5f;
		public static float C_NecromancerWindPushMultiplier = 25.0f;
    }

    // Rift Stats
    public static class RiftStats {
		public static int C_RuneSpawnCapPerSide = 4;
		public static float C_VolatilityNecromancerSpawnTimer = 30.0f;
        public static Vector3 C_RiftTeleportOffset = new Vector3(-2, 0, 0);
        public static float C_PortalTeleportOffset = 0.75f;

        public static float C_VolatilityResetTime = 0.0f;
        //public static float C_Volatility_CameraFlipTime = 5.0f;
        public static float C_VolatilityEnemySpawnTimer = 7.0f;
        public static float C_VolatilityDeathboltLiveTimer = 12.0f;
        public static float C_VolatilityDeathboltSpeed = 10.0f;
        public static float C_VolatilityDeathboltDamage = 300.0f;
        public static float C_RiftTeleportDelay = 0.5f;

        public static float C_VolatilityIncrease_RoomAdvance = 5.0f;
        public static float C_VolatilityIncrease_SpellCross = 0.5f;
        public static float C_VolatilityIncrease_PlayerDeath = 2.5f;

        public static float C_VolatilityMultiplier_L1 = 0.0f;
        public static float C_VolatilityMultiplier_L2 = 0.2f;
        public static float C_VolatilityMultiplier_L3 = 0.5f;
        public static float C_VolatilityMultiplier_L4 = 1.0f;

        public enum Volatility { ZERO, FIVE, TWENTYFIVE, THIRTYFIVE, FIFTY, SIXTYFIVE, SEVENTYFIVE, ONEHUNDRED };
    }

    // Text descriptions for Objectives
    public static class ObjectiveText {
        public static string C_CTFTitle = "Capture The Gem";
        public static string C_CTFDescription = "Pick up the opponent's gem with [Interact] and drag it back to your goal!";
        public static string C_CrystalDestructTitle = "Crystal Destruction";
        public static string C_CrystalDestructDescription = "Cast spells at the enemy team's crystal to destroy it! Heal your own crystal with your own spells!";
        public static string C_HockeyTitle = "Hockey";
        public static string C_HockeyDescription = "Shoot and parry your puck into the enemy's goal! Careful, you can't score from behind!";
        public static string C_PotatoTitle = "Keep Away";
        public static string C_PotatoDescription = "Shove your object onto the opponent's side and keep it there. Be careful! If you leave yours on your side for too long, bad things will happen!";
        public static string C_DefeatNecromancersTitle = "Defeat Necromancers";
        public static string C_DefeatNecromancersDescription = "Defeat Necromancers of your team's color!  They teleport across The Rift when they get low on health!";
        public static string C_BossTitle = "The Final Trial";
        public static string C_BossDescription = "Use all your prowess and spells to defeat the Rift!";
    }

    // Carry over for volume options from the main menu
    public static class VolOptions {
        public static float C_MasterVolume = 1f;
        public static float C_BGMVolume = 1f;
        public static float C_SFXVolume = 1f;
        public static float C_VOIVolume = 1f;
    }


    public static class UnitTests {
        public static bool C_RunningCTFTests = false;

        public static float C_WaitTime = 1.0f;
    }
    
}

static class RandomExtensions
{
    public static void Shuffle<T>(this System.Random rng, T[] array)
    {
        int n = array.Length;
        while (n > 1) {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}
