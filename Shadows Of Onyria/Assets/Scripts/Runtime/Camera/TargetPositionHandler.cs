using System;
using UnityEngine;

namespace DoaT
{
    public class TargetPositionHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _target = default;

        [SerializeField] private Vector3 displacement = default;
        [Range(0f, 1f), SerializeField] private float cameraLerpRatio = 0.66f;

        private void Awake()
        {
            transform.SetParent(null, true);
        }

        private void Start()
        {
            if(_target == null) _target = World.GetPlayer().GameObject;

            if (displacement == default)
                displacement = transform.position - _target.transform.position;
        }

        private void LateUpdate()
        {
            MoveCamera();
        }

        private void MoveCamera()
        {
            transform.position = Vector3.Lerp(transform.position, _target.transform.position + displacement,
                cameraLerpRatio);
        }
    }
}
