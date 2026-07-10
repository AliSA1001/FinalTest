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
/// Wall handling: the follow position eases toward the ideal spot behind the player and is never itself
/// clamped; obstruction only pulls the camera *closer along the view line* (snap in, ease out). Smoothing the
/// distance instead of clamping the world position is what keeps it from vibrating against walls.
///
/// Put this on the Main Camera (the object tagged <b>MainCamera</b> with the <see cref="Camera"/> component).
/// Set the Target to the player, or leave it empty to auto-find by tag. Runs in LateUpdate so it moves after
/// the player and animation, avoiding jitter. For pull-in to work, set the Obstruction Mask to your wall layers.
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

    // The camera's own yaw, eased toward the player's yaw so it arcs around behind him instead of snapping.
    private float _yaw;
    private float _yawVelocity;
    // Continuously-smoothed follow position toward the UNOBSTRUCTED ideal. Never clamped, so smoothing stays stable.
    private Vector3 _followPosition;
    private Vector3 _followVelocity;
    // Current view distance along the look line, adjusted for walls (snap in, ease out).
    private float _distance;
    private float _distanceVelocity;

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
        float turnSmooth = _turnSmoothTime > 0f ? _turnSmoothTime : 0.5f;
        _yaw = Mathf.SmoothDampAngle(_yaw, _target.eulerAngles.y, ref _yawVelocity, turnSmooth);

        Vector3 lookPoint = _target.position + Vector3.up * _lookAtHeight;

        // Follow the unobstructed ideal spot. This value is never clamped against walls, so there's no
        // smoothing/clamp feedback loop — that loop is what made the camera shake against geometry.
        _followPosition = Vector3.SmoothDamp(_followPosition, IdealPosition(), ref _followVelocity, _followSmoothTime);

        transform.position = ApplyObstruction(lookPoint, _followPosition);

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
        _followPosition = IdealPosition();
        _followVelocity = Vector3.zero;
        _yawVelocity = 0f;
        _distanceVelocity = 0f;

        Vector3 lookPoint = _target.position + Vector3.up * _lookAtHeight;
        // Seed the distance to the framed (obstruction-aware) value so we don't ease out on the first frame.
        Vector3 offset = _followPosition - lookPoint;
        _distance = AllowedDistance(lookPoint, offset);
        transform.position = ApplyObstruction(lookPoint, _followPosition, snap: true);

        Vector3 toTarget = lookPoint - transform.position;
        if (toTarget.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(toTarget);
        }
    }

    /// <summary>Assign the follow target at runtime (e.g. when the act's player spawns) and re-frame instantly.</summary>
    public void SetTarget(Transform target)
    {
        _target = target;
        SnapToTarget();
    }

    // Where the camera wants to sit: the offset rotated by the camera's (eased) yaw, so -Z stays behind the player.
    private Vector3 IdealPosition()
    {
        Quaternion yawRotation = Quaternion.Euler(0f, _yaw, 0f);
        return _target.position + yawRotation * _offset;
    }

    // Move the camera closer along the look line if a wall blocks the view, adjusting only the DISTANCE
    // (snap in, ease out) so the world position is never hard-clamped mid-smooth.
    private Vector3 ApplyObstruction(Vector3 lookPoint, Vector3 followPosition, bool snap = false)
    {
        Vector3 offset = followPosition - lookPoint;
        float wantedDistance = offset.magnitude;
        if (wantedDistance < 0.0001f) return followPosition;

        Vector3 direction = offset / wantedDistance;
        float allowed = AllowedDistance(lookPoint, offset);

        if (snap || allowed < _distance)
        {
            // Pull in immediately so we never show through the wall.
            _distance = allowed;
            _distanceVelocity = 0f;
        }
        else
        {
            // Ease back out once the view is clear again.
            _distance = Mathf.SmoothDamp(_distance, allowed, ref _distanceVelocity, _followSmoothTime);
        }

        return lookPoint + direction * _distance;
    }

    // Furthest the camera may sit from the look point along the given offset before a wall gets in the way.
    private float AllowedDistance(Vector3 lookPoint, Vector3 offset)
    {
        float wantedDistance = offset.magnitude;
        if (!_avoidObstructions || _obstructionMask == 0 || wantedDistance < 0.0001f) return wantedDistance;

        Vector3 direction = offset / wantedDistance;
        if (Physics.SphereCast(lookPoint, _obstructionRadius, direction, out RaycastHit hit, wantedDistance, _obstructionMask, QueryTriggerInteraction.Ignore))
        {
            return Mathf.Max(_minDistance, hit.distance - _obstructionRadius);
        }
        return wantedDistance;
    }
}
