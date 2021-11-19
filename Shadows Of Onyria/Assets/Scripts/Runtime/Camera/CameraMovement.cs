using DoaT.Inputs;
using UnityEngine;

[ExecuteAlways]
public class CameraMovement : MonoBehaviour
{
    [Range(0f, 1f)] public float cameraDistanceLerp;
    [Range(0.01f, 3f)] public float sensitivity = 1f;

    #pragma warning disable 649
    [SerializeField] private GameObject _target;
    [SerializeField] private bool editorUpdate;
    #pragma warning restore 649

    private float CameraDistanceLerp
    {
        get => cameraDistanceLerp;
        set => cameraDistanceLerp = Mathf.Clamp01(value);
    }
    private Vector3 Direction
    {
        get
        {
            var temp  = (_target.transform.position - transform.position).normalized;

            if (temp == Vector3.zero)
                temp += transform.forward;

            return temp;
        }
    }

    [Header("Near")]
    [Range(0f, 360f),SerializeField] private float yRotationNear = 45;
    [Range(0f, 90f),SerializeField] private float xRotationNear = 45f;
    [Range(1f, 50f),SerializeField] private float distanceNear = 10;
    
    [Header("Far")]
    [Range(0f, 360f),SerializeField] private float yRotationFar = 45;
    [Range(0f, 90f),SerializeField] private float xRotationFar = 45f;
    [Range(1f, 50f),SerializeField] private float distanceFar = 10;
    private bool _isTargetNull;

    private float YRotation => Mathf.Lerp(yRotationNear, yRotationFar, CameraDistanceLerp);
    private float XRotation => Mathf.Lerp(xRotationNear, xRotationFar, CameraDistanceLerp);
    private float Distance => Mathf.Lerp(distanceNear, distanceFar, CameraDistanceLerp);

    
    private void Awake()
    {
        if (!Application.isPlaying) return;
        
        if(_target == null)
            _target = transform.parent.gameObject;
        
        _isTargetNull = _target == null;

        cameraDistanceLerp = 1f;
    }
    
#if UNITY_EDITOR
    private void Start()
    {
        if (!Application.isPlaying) return;
        InputSystem.BindAxis(InputProfile.Gameplay, "Camera Zoom", AxisEvent.Fixed, Scroll);
    }
#endif

    private void OnEnable()
    {
        if(_target == null)
            _target = transform.parent.gameObject;
        
        _isTargetNull = _target == null;
    }

    private void Scroll(float value)
    {
        if (!Application.isPlaying) return;
        if(value != 0) CameraDistanceLerp += -value * sensitivity;
    }

    private void LateUpdate()
    {
                                                    // true  != true  => false -> continue
                                                    // true  != false => true  -> return
                                                    // false != true  => true  -> return
                                                    // false != false => false -> continue
        if (!Application.isPlaying || !Application.isPlaying != editorUpdate) return;
        if (_isTargetNull)
        {
            DebugManager.LogError("Target is Null.");
            return;
        }
        
        _target.transform.rotation = Quaternion.Euler(XRotation, YRotation, _target.transform.rotation.eulerAngles.z);
        transform.position = _target.transform.position - (Direction * Distance);
    }

#if UNITY_EDITOR
    private void OnDestroy()
    {
        if (!Application.isPlaying) return;
        InputSystem.UnbindAxis(InputProfile.Gameplay, "Camera Zoom", AxisEvent.Fixed, Scroll);
    }
#endif
}

