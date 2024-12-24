using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour, IPlatform
{
    public float fallWait = 2f;
    public float destroyWait = 1f;
    public float respawnWait = 1f;
    public SpriteRenderer spriteRenderer;

    private Rigidbody2D rigidBody;
    private bool isFalling = false;
    private Vector3 startingPosition;

    private PlatformType _platformType = PlatformType.FallingPlatform;

    public PlatformType PlatformType { get => _platformType; set => _platformType = value; }

    void Awake()
    {
        startingPosition = transform.position;
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private IEnumerator Fall()
    {
        isFalling = true;
        yield return new WaitForSeconds(fallWait);
        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        yield return new WaitForSeconds(destroyWait);
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(respawnWait);
        rigidBody.bodyType = RigidbodyType2D.Static;
        transform.position = startingPosition;
        spriteRenderer.enabled = true;
        isFalling = false;
    }

    public void PlatformEnterAction(PlayerSystem.PlayerState playerState, Rigidbody2D playerRigidBody)
    {
        if (isFalling) return;
        playerRigidBody.transform.parent = transform;
        playerRigidBody.interpolation = RigidbodyInterpolation2D.None;
        StartCoroutine(Fall());
    }

    public void PlatformExitAction(Rigidbody2D playerRigidBody)
    {
        playerRigidBody.interpolation = RigidbodyInterpolation2D.Interpolate;
        playerRigidBody.transform.parent = null;
    }
}
