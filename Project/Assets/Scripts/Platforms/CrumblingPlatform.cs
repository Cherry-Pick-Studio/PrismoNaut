using System;
using System.Collections;
using System.Collections.Generic;
using PlayerSystem;
using UnityEngine;

public class CrumblingPlatform : MonoBehaviour, IPlatform
{
    [SerializeField] private float crumbleWait = 1f;
    [SerializeField] private float respawnWait = 2f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool isCrumbling = false;

    private BoxCollider2D boxCollider;
    private PlatformType _platformType = PlatformType.CrumblingPlatform;

    public PlatformType PlatformType { get => _platformType; set => _platformType = value; }

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private IEnumerator Crumble()
    {
        isCrumbling = true;
        yield return new WaitForSeconds(crumbleWait);
        boxCollider.enabled = false;
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(respawnWait);
        boxCollider.enabled = true;
        spriteRenderer.enabled = true;
        isCrumbling = false;
    }

    private IEnumerator DestroyPlatform()
    {
        boxCollider.enabled = false;
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(respawnWait);
        boxCollider.enabled = true;
        spriteRenderer.enabled = true;
    }

    public void PlatformEnterAction(PlayerSystem.PlayerState playerState, Rigidbody2D playerRigidBody)
    {
        if (playerState.activePower == Power.Square)
        {
            StartCoroutine(DestroyPlatform());
        }
        if (isCrumbling) return;
        else
        {
            StartCoroutine(Crumble());
        }
    }

    public void PlatformExitAction(Rigidbody2D playerRigidBody)
    {
    }
}
