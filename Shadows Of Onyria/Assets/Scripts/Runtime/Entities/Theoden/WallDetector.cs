using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    public class WallDetector : MonoBehaviour, IUpdate, IUnloadable
    {
        private static readonly Vector3 EXTENTS = new Vector3(0.2f, 0.875f, 0.35f);
        private static readonly Vector3 CENTER_DISPLACEMENT = new Vector3(0,1, 0.2f);

        public bool IsColliding => _buckets.Count > 0 || _isColliding;

        public Vector3 debugDirection;
        
        private bool _isColliding;
        private readonly HashSet<Collider> _buckets = new HashSet<Collider>();

        public List<Collider> debugBuckets = new List<Collider>();

        private void Start()
        {
            ExecutionSystem.AddUpdate(this);
            EventManager.Subscribe(GameEvents.OnSceneUnload, Unload);
        }
        
        public void Unload(params object[] parameters)
        {
            ExecutionSystem.RemoveUpdate(this, true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!LayersUtility.IsInMask(LayersUtility.WALL_DETECTION_MASK, other.gameObject.layer)) return;

            if (!_buckets.Contains(other))
            {
                _buckets.Add(other);
                debugBuckets.Add(other);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!LayersUtility.IsInMask(LayersUtility.WALL_DETECTION_MASK, other.gameObject.layer)) return;

            if (!_buckets.Contains(other))
            {
                _buckets.Add(other);
                debugBuckets.Add(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!LayersUtility.IsInMask(LayersUtility.WALL_DETECTION_MASK, other.gameObject.layer)) return;

            if (_buckets.Contains(other))
            {
                _buckets.Remove(other);
                debugBuckets.Remove(other);
            }
        }
        
        public void OnUpdate()
        {
            if (!_isColliding || (_buckets.Count > 0 && _isColliding)) return;

            //ForceDetection();
        }

        // public void ForceDetection()
        // {
        //     var parent = transform.parent;
        //     var center = parent.position +
        //                  parent.right * CENTER_DISPLACEMENT.x +
        //                  parent.up * CENTER_DISPLACEMENT.y +
        //                  parent.forward * CENTER_DISPLACEMENT.z;
        //
        //     var usableExtents = EXTENTS;
        //     if (Framerate.Current <= 60)
        //     {
        //         var ratio = Framerate.Current / 60;
        //         usableExtents /= ratio;
        //     }
        //     
        //     var results = new Collider[1];
        //     var size = Physics.OverlapBoxNonAlloc(center, usableExtents, results, parent.rotation, LayersUtility.WALL_DETECTION_MASK, QueryTriggerInteraction.Collide);
        //     _isColliding = size > 0;
        // }
        //
        // public void ForceDetection(Vector3 forward)
        // {
        //     var up = Vector3.up;
        //     var right = Quaternion.AngleAxis(90f, Vector3.up) * forward;
        //     
        //     var parent = transform.parent;
        //     var center = parent.position +
        //                         right * CENTER_DISPLACEMENT.x +
        //                         up * CENTER_DISPLACEMENT.y +
        //                         forward * CENTER_DISPLACEMENT.z;
        //     
        //     var results = new Collider[1];
        //     var size = Physics.OverlapBoxNonAlloc(center, EXTENTS, results, parent.rotation, LayersUtility.WALL_DETECTION_MASK, QueryTriggerInteraction.Collide);
        //     
        //     _isColliding = size > 0;
        // }

        public bool CastDetection(Vector3 forward, float speed, out float possibleDistance)
        {
            var origin = transform.parent.position + Vector3.up * CENTER_DISPLACEMENT.y;
            var temp = Physics.Raycast(origin, forward, out var hit, speed * Time.deltaTime * 2, LayersUtility.WALL_DETECTION_MASK,  QueryTriggerInteraction.Collide);

            if (temp)
            {
                var hitPoint = hit.point;//.SetY(origin.y);
                possibleDistance = Vector3.Distance(hitPoint, origin);
            }
            else
            {
                possibleDistance = 0f;
            }
            
            return temp;
        }

        private void OnDestroy()
        {
            EventManager.Unsubscribe(GameEvents.OnSceneUnload, Unload);
        }

        // private void OnDrawGizmos()
        // {
        //     var forward = debugDirection;
        //     var up = Vector3.up;
        //     var right = Quaternion.AngleAxis(90f, Vector3.up) * debugDirection;
        //     
        //     var init = transform.position;
        //     Gizmos.color = Color.blue;
        //     Gizmos.DrawLine(init, init + forward);
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawLine(init, init + up);
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawLine(init, init + right);
        // }

        // private void OnDrawGizmos()
        // {
        //     var rad = 0.1f;
        //     
        //     var bounds = _collider.bounds;
        //     var parent = transform.parent;
        //     
        //     var up = parent.up;
        //     var forward = parent.forward;
        //     var right = parent.right;
        //     
        //     var center = parent.position + forward * 0.2f + up;
        //
        //     var extents = new Vector3(0.2f, 0.875f, 0.35f);
        //
        //     Gizmos.color = Color.cyan;
        //     Gizmos.DrawWireSphere(center + (right * 0.2f) + (up * 0.875f) + (forward * 0.35f), rad);
        //     Gizmos.DrawWireSphere(center + (right * -0.2f) + (up * 0.875f) + (forward * 0.35f), rad);
        //     Gizmos.color = Color.blue;
        //     Gizmos.DrawWireSphere(center + (right * 0.2f) + (up * 0.875f) + (forward * -0.35f), rad);
        //     Gizmos.DrawWireSphere(center + (right * -0.2f) + (up * 0.875f) + (forward * -0.35f), rad);
        //     
        //     Gizmos.color = Color.cyan;
        //     Gizmos.DrawWireSphere(center + (right * 0.2f) + (up * -0.875f) + (forward * 0.35f), rad);
        //     Gizmos.DrawWireSphere(center + (right * -0.2f) + (up * -0.875f) + (forward * 0.35f), rad);
        //     Gizmos.color = Color.blue;
        //     Gizmos.DrawWireSphere(center + (right * 0.2f) + (up * -0.875f) + (forward * -0.35f), rad);
        //     Gizmos.DrawWireSphere(center + (right * -0.2f) + (up * -0.875f) + (forward * -0.35f), rad);
        // }

    }
}
