using UnityEngine;

/// <summary>
/// Third-person follow camera for "Act to Wish". It is NOT player-controlled — there is no mouse orbit. It
/// trails the player and swings around to stay behind his back as he turns, keeping him framed from behind.
///
/// Because <see cref="A_Movement"/> steers using <c>Camera.main.forward</c>, a behind-the-back camera means
/// "forward" always points where the player faces — pressing forward drives him straight ahead. (Holding a
/// pure strafe will gently curve him, which is normal for this style.) The swing-around is smoothed so it
/// eases behind him rather than snapping.
///
/// Put this on the Main Camera (the object tagged <b>MainCamera</b> with the <see cref="Camera"/> component).
/// Set the Target to the player, or leave it empty to auto-find by tag. Runs in LateUpdate so it moves after
/// the player and animation, avoiding jitter.
/// </summary>
public class T_ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Who to follow. Leave empty to auto-find by tag on start.")]
    [SerializeField] private Transform _target;
    [Tooltip("Tag used to auto-find the target if none is assigned.")]
    [SerializeField] private string _targetTag = "Player";

    [Header("Framing")]
    [Tooltip("Offset from the target, relative to the way he faces: behind (-Z), to the side (X), and above (Y).")]
    [SerializeField] private Vector3 _offset = new Vector3(0f, 4f, -6f);
    [Tooltip("Height above the target's pivot that the camera aims at (roughly chest/head height).")]
    [SerializeField] private float _lookAtHeight = 1.5f;

    [Header("Smoothing")]
    [Tooltip("How long the camera takes to follow the player's position (walking along). Higher = lazier. 0 = snap.")]
    [SerializeField] private float _followSmoothTime = 0.2f;
    [Tooltip("How gently the camera swings around behind the player when he turns. Higher = lazier and comfier. (0 uses a sensible default.)")]
    [SerializeField] private float _turnSmoothTime = 0.5f;
    [Tooltip("How quickly the camera rotates to face the target. Higher = snappier aim.")]
    [SerializeField] private float _lookSmoothSpeed = 12f;

    [Header("Obstruction (optional)")]
    [Tooltip("Pull the camera in when something blocks the view of the target.")]
    [SerializeField] private bool _avoidObstructions = true;
    [Tooltip("Layers that should block the camera (walls, scenery). Leave empty to disable pull-in.")]
    [SerializeField] private LayerMask _obstructionMask;
    [Tooltip("Thickness of the probe so the camera doesn't clip through thin walls.")]
    [SerializeField] private float _obstructionRadius = 0.3f;
    [Tooltip("Closest the camera is allowed to pull toward the target.")]
    [SerializeField] private float _minDistance = 1f;

    private Vector3 _followVelocity;
    // The camera's own yaw, eased toward the player's yaw so it arcs around behind him instead of snapping.
    private float _yaw;
    private float _yawVelocity;

    private void Start()
    {
        if (_target == null && !string.IsNullOrEmpty(_targetTag))
        {
            GameObject found = GameObject.FindGameObjectWithTag(_targetTag);
            if (found != null) _target = found.transform;
        }

        if (_target == null)
        {
            Debug.LogWarning($"{nameof(T_ThirdPersonCamera)}: no target set and none tagged '{_targetTag}' found. Assign the Target in the Inspector.", this);
            return;
        }

        // Start already framed behind him so there's no big swoop on the first frame.
        SnapToTarget();
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        // Ease our yaw toward the player's facing so the camera drifts around behind him along a gentle arc.
        // A larger turn-smooth time keeps sharp player turns from whipping the camera around.
        float turnSmooth = _turnSmoothTime > 0f ? _turnSmoothTime : 0.5f;
        _yaw = Mathf.SmoothDampAngle(_yaw, _target.eulerAngles.y, ref _yawVelocity, turnSmooth);

        Vector3 lookPoint = _target.position + Vector3.up * _lookAtHeight;
        Vector3 desiredPosition = ResolveObstruction(DesiredPosition(), lookPoint);

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _followVelocity, _followSmoothTime);

        Vector3 toTarget = lookPoint - transform.position;
        if (toTarget.sqrMagnitude > 0.0001f)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(toTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, _lookSmoothSpeed * Time.deltaTime);
        }
    }

    /// <summary>Jump the camera straight to its framed position/rotation with no smoothing (e.g. on spawn or teleport).</summary>
    public void SnapToTarget()
    {
        if (_target == null) return;

        _yaw = _target.eulerAngles.y;

        Vector3 lookPoint = _target.position + Vector3.up * _lookAtHeight;
        transform.position = ResolveObstruction(DesiredPosition(), lookPoint);

        Vector3 toTarget = lookPoint - transform.position;
        if (toTarget.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(toTarget);
        }
        _followVelocity = Vector3.zero;
        _yawVelocity = 0f;
    }

    /// <summary>Assign the follow target at runtime (e.g. when the act's player spawns) and re-frame instantly.</summary>
    public void SetTarget(Transform target)
    {
        _target = target;
        SnapToTarget();
    }

    // Where the camera wants to sit: the offset rotated by the camera's (eased) yaw, so -Z stays behind the player.
    private Vector3 DesiredPosition()
    {
        Quaternion yawRotation = Quaternion.Euler(0f, _yaw, 0f);
        return _target.position + yawRotation * _offset;
    }

    // If a wall sits between the target and the wanted camera spot, slide the camera in to the near side of it.
    private Vector3 ResolveObstruction(Vector3 wantedPosition, Vector3 lookPoint)
    {
        if (!_avoidObstructions || _obstructionMask == 0) return wantedPosition;

        Vector3 direction = wantedPosition - lookPoint;
        float distance = direction.magnitude;
        if (distance < 0.0001f) return wantedPosition;
        direction /= distance;

        if (Physics.SphereCast(lookPoint, _obstructionRadius, direction, out RaycastHit hit, distance, _obstructionMask, QueryTriggerInteraction.Ignore))
        {
            float pulled = Mathf.Max(_minDistance, hit.distance - _obstructionRadius);
            return lookPoint + direction * pulled;
        }
        return wantedPosition;
    }
}
