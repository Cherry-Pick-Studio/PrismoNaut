using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using Cinemachine;
using System.Net;
[ExecuteInEditMode]
public class DimensionTransition : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject player3DModel;

    [Header("Curve")]
    [SerializeField] private float transitionTime = 1;
    [HideInInspector] public Vector3 startTangent = Vector3.zero;
    [HideInInspector] public Vector3 endTangent = Vector3.zero;
    [HideInInspector] public  Vector3 endPoint = new Vector3(5,5,0);
    private Vector3 startPoint => transform.position;
    private BoxCollider2D[] box2DColliders => GetComponents<BoxCollider2D>();
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
            Vector3 playerPosition = playerController.transform.position;
            Vector3 finalPosition = GetTravelPoint(playerPosition);
            Vector3 startTangentPos = finalPosition == endPoint ? startTangent : endTangent;
            Vector3 endTangentPos = finalPosition == endPoint ? endTangent : startTangent;
            StartCoroutine(TravelTransition(playerController, finalPosition, startTangentPos , endTangentPos, transitionTime));
        }
    }

    private Vector3 GetTravelPoint(Vector3 playerPosition)
    {
        float distanceToFirst = Vector3.Distance(playerPosition, startPoint);
        float distanceToSecond = Vector3.Distance(playerPosition, endPoint);
        return distanceToFirst < distanceToSecond ? endPoint : startPoint;
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

    public  void UpdateBoxColliders()
    {
        if (box2DColliders.Length < 2) return;

        box2DColliders[0].offset = Vector2.zero;
        box2DColliders[1].offset = transform.InverseTransformPoint(endPoint);

        foreach (var collider in box2DColliders)
        {
            collider.isTrigger = true;
        }
    }

    public IEnumerator TravelTransition(Player2DController player, Vector3 targetPosition, Vector3 startTangent, Vector3 endTangent, float totalTransitionTime)
    {
        Debug.Log(targetPosition);
        isTraveling = true;
        Vector3 playerPositon = player.transform.position;
        player.gameObject.SetActive(false);

        GameObject model3D = Instantiate(player3DModel, playerPositon, Quaternion.identity);
        model3D.transform.localScale = Vector3.one * 0.5f;
        virtualCamera.Follow = model3D.transform;

        float elapseTime = 0;
        while (elapseTime < totalTransitionTime)
        {
            float t = elapseTime / totalTransitionTime;
            float nextT = Mathf.Min((elapseTime + Time.deltaTime) / totalTransitionTime, 1);

            Vector3 currentCurvePoint = GetPointOnBezierCurve(playerPositon, startTangent, endTangent, targetPosition, t);
            Vector3 nextCurvePoint = GetPointOnBezierCurve(playerPositon, startTangent, endTangent, targetPosition, nextT);

            model3D.transform.position = currentCurvePoint;
            model3D.transform.LookAt(nextCurvePoint);

            elapseTime += Time.deltaTime;
            yield return null;
        }
        
        Destroy(model3D);
        player.transform.position = targetPosition;
        virtualCamera.Follow = player.transform;
        player.gameObject.SetActive(true);
        isTraveling = false;
    }

    Vector3 GetPointOnBezierCurve(Vector3 startPoint, Vector3 startTangent, Vector3 endTangent, Vector3 endPoint, float t){
        float u = 1 - t;
        float t2 = t * t;
        float u2 = u * u;
        float u3 = u2 * u;
        float t3 = t * t2;

        Vector3 result = 
            u3 * startPoint +
            3 * u2 * t * startTangent +
            3 * u * t2 * endTangent +
            t3 * endPoint;

        return result;
    }

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        if(collider.GetComponent<Player2DController>()){
            playerController = collider.GetComponent<Player2DController>();
        }
    }

    private void OnTriggerExit2D(Collider2D collider) 
    {
        if(collider.GetComponent<Player2DController>()){
            playerController = null;
        }
    }


    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.15f);


        Gizmos.color = Color.yellow;
        Vector3 previousPoint = startPoint;
        int segments = 16;

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 currentPoint = GetPointOnBezierCurve(startPoint, startTangent, endTangent, endPoint, t);

            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }

}
