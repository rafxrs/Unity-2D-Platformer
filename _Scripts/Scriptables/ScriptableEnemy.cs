using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[CreateAssetMenu(fileName = "New Enemy", menuName = "ScriptableObjects/Enemy")]
public class ScriptableEnemy : ScriptableUnitBase
{
    public EnemyType enemyType;
    public LayerMask playerLayer;
    public RuntimeAnimatorController controller;
    public GameObject[] impactPrefabs;



    [SerializeField] private Stats _advancedStats;
    public Stats avancedStats => _advancedStats;

    [Serializable]
    public enum EnemyType {
        Bat,
        Bee,
        BigBoar,
        Boar,
        GoblinAxe,
        GoblinHalberd,
        GoblinSpearThrower,
        GoblinSpear,
        GoblinBoar,
        SkeletonSword,
        SkeletonSpear,
        SkeletonBow,
        SkeletonMage,
        SkeletonShield,
        SkeletonBoss,
        Slime,
        Spider
    }

    /// <summary>
    /// Keeping base stats as a struct on the scriptable keeps it flexible and easily editable.
    /// We can pass this struct to the spawned prefab unit and alter them depending on conditions.
    /// </summary>
    [Serializable]
    public struct Stats
    {
        public int chaseSpeed;
        public int patrolSpeed;
        public float nextWayPointDist;
        public bool hasAttacks;
        public int attackDamage;
        public float weaponAttackRange;
        public float attackDistance;
        public float attackRate;
        public bool isBouncable;
        public int bounceDamage;
        public bool isStatic;
        public bool canFly;
        public float jumpForce;
    }


}
