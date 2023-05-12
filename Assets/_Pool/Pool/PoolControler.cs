using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PoolControler : MonoBehaviour
{
    [Header("Pool")]
    public PoolAmount[] Pool;

    [Header("Particle")]
    public ParticleAmount[] Particle;


    public void Awake()
    {
        for (int i = 0; i < Particle.Length; i++)
        {
            ParticlePool.Preload(Particle[i].prefab, Particle[i].amount, Particle[i].root);
            ParticlePool.Shortcut(Particle[i].particleType, Particle[i].prefab);
        }

        for (int i = 0; i < Pool.Length; i++)
        {
            SimplePool.Preload(Pool[i].prefab, Pool[i].amount, Pool[i].root, Pool[i].collect, Pool[i].clamp);
        }

    }
}


public enum ParticleType 
{
    Hit
}

public enum IngameType
{
    None,
    PLAYER,
    ENEMY,
    HpBar,
}


public enum PoolType
{
    None,
    Bullet_1,
    Bullet_2,
    Bullet_3,
    Bullet_4,
}


