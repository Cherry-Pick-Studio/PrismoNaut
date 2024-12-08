using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.InputSystem;
using System;
using Cinemachine;

[ExecuteInEditMode]
public class DimensionTransition : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 secondTravelPoint = Vector2.one * 5;
    [SerializeField] private GameObject player3DModel;

    private Vector3 secondTravelWorldPoint => transform.position + (Vector3)secondTravelPoint;
    private BoxCollider2D[] box2DColliders => GetComponents<BoxCollider2D>();

    private Vector3 firstTravelPoint => transform.position;
    private CinemachineVirtualCamera virtualCamera => FindObjectOfType<CinemachineVirtualCamera>();
    private bool isTraveling;
    private Player2DController playerController = null;
    private InputAction trianglePowerAction;
    private InputAction squarePowerAction;
    private InputAction circlePowerAction;

    private void Awake()
    {
        EnsureRequiredBoxColliders();
    }

    private void Start()
    {
        UpdateBoxColliders();
        InitializeInputActions();
    }

    private void Update() 
    {
        HandleTravelInput();
    }

    private void InitializeInputActions()
    {
        var inputActions = InputSystem.actions;
        trianglePowerAction = inputActions.FindAction("TrianglePower");
        squarePowerAction = inputActions.FindAction("SquarePower");
        circlePowerAction = inputActions.FindAction("CirclePower");
    }

    private void HandleTravelInput()
    {
        if (playerController == null || isTraveling) return;

        if (trianglePowerAction.WasPressedThisFrame() || squarePowerAction.WasPressedThisFrame() || circlePowerAction.WasPressedThisFrame())
        {
            var playerPosition = playerController.transform.position;
            var closestPoint = GetTravelPoint(playerPosition);
            StartCoroutine(TravelTransition(playerController, closestPoint));
        }
    }

    private Vector3 GetTravelPoint(Vector3 playerPosition)
    {
        float distanceToFirst = Vector3.Distance(playerPosition, firstTravelPoint);
        float distanceToSecond = Vector3.Distance(playerPosition, secondTravelWorldPoint);
        return distanceToFirst < distanceToSecond ? secondTravelWorldPoint : firstTravelPoint;
    }


    private void EnsureRequiredBoxColliders()
    {
        while (box2DColliders.Length < 2) 
        {
            gameObject.AddComponent<BoxCollider2D>();
        } 

        while(box2DColliders.Length > 2) 
        {
            Destroy(box2DColliders[box2DColliders.Length - 1]);
        }
    }

    private void UpdateBoxColliders()
    {
        if (box2DColliders.Length < 2) return;

        box2DColliders[0].offset = Vector2.zero;
        box2DColliders[1].offset = secondTravelPoint;

        foreach (var collider in box2DColliders)
        {
            collider.isTrigger = true;
        }
    }
    public IEnumerator TravelTransition(Player2DController player, Vector3 targetPosition)
    {
        isTraveling = true;
        player.gameObject.SetActive(false);

        GameObject model3D = Instantiate(player3DModel, player.transform.position, Quaternion.identity);
        model3D.transform.localScale = Vector3.one * 0.5f;
        model3D.transform.LookAt(targetPosition);
        virtualCamera.Follow = model3D.transform;

        Tween travel = model3D.transform.DOMove(targetPosition, 1.5f).SetEase(Ease.Linear);
        yield return travel.WaitForCompletion();
        
        Destroy(model3D);
        player.transform.position = targetPosition;
        virtualCamera.Follow = player.transform;
        player.gameObject.SetActive(true);
        isTraveling = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.15f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(secondTravelWorldPoint, 0.15f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position,secondTravelWorldPoint);
    }


    void OnTriggerEnter2D(Collider2D collider) 
    {
        if(collider.GetComponent<Player2DController>()){
            playerController = collider.GetComponent<Player2DController>();
        }
    }

    void OnTriggerExit2D(Collider2D collider) 
    {
        if(collider.GetComponent<Player2DController>()){
            playerController = null;
        }
    }

    public void OnValidate()
    {
        UpdateBoxColliders();
    }

}
