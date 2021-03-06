using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : Semaphore
{
    private TankManager tankManager;
    public EntityHealth entityHealth;

    [Tooltip("Enable this to automatically set the spawn point as the scene initialization position")]
    public bool spawnAtStart;
    public Vector3 spawnPosition;

    public float spawnDelay = 2f;
    protected float _spawnDelay = 2f;
    
    public Rigidbody rb;
    public BoxCollider boxCollider;
    public GameObject gfxGameobject;
    
    public Behaviour[] toggleBehaviours;

    protected bool isDead;
    public event Action onPlayerRespawned;

    protected override void SephamoreStart(Manager manager)
    {
        base.SephamoreStart(manager);
        tankManager = manager as TankManager;

        if (spawnAtStart)
        {
            spawnPosition = transform.position;
        }

        entityHealth.onKilled += EntityHealth_onKilled;
    }

    private void EntityHealth_onKilled()
    {
        isDead = true;
        _spawnDelay = spawnDelay;
        TogglePlayer(false);
    }

    private void Update()
    {
        if (isDead)
        {
            if (_spawnDelay <= 0)
            {
                tankManager.InitializeEntity();

                ResetPosition();

                isDead = false;
                TogglePlayer(true);
            }
            else
            {
                _spawnDelay -= Time.deltaTime;
            }
        }
    }

    private void TogglePlayer(bool value)
    {
        if (!gfxGameobject)
        {
            Debug.Log("GFX not referenced in " + name, gameObject);

        }
        else
        {
            gfxGameobject.SetActive(value);
        }

        boxCollider.enabled = value;
        rb.isKinematic = !value;

        for (int i = 0; i < toggleBehaviours.Length; i++)
        {
            toggleBehaviours[i].enabled = value;
        }

        if (value)
        {
            Invincible invincible = gameObject.AddComponent<Invincible>();
            invincible.Activate(3f);

            if (transform.GetComponent<PowerUpUI>())
            {
                transform.GetComponent<PowerUpUI>().RespawnShield(3f);

            }

            onPlayerRespawned?.Invoke();
        }
    }

    public void ResetPosition()
    {
        transform.position = spawnPosition;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
