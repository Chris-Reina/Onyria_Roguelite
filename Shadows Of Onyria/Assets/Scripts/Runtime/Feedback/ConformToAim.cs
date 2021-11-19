using UnityEngine;

namespace DoaT
{
    public class ConformToAim : MonoBehaviour
    {
        [SerializeField] private TheodenController _controller;
        [SerializeField] private bool _showArrow;
        [SerializeField] private GameObject _mesh;

        private void Start()
        {
            if (_controller == null) _controller = GetComponentInParent<TheodenController>();

            _controller.OnRangeAttackBegin += x => UpdateState(true);
            _controller.OnRangeAttackRelease += () => UpdateState(false);
            _controller.OnRangeAttackCharging += ChargeUpdate;
            _controller.OnRangeCancel += () => UpdateState(false);
        }

        private void UpdateState(bool show)
        {
            if (_showArrow == show) return;

            _mesh.SetActive(show);
            _showArrow = show;
        }

        private void ChargeUpdate(float size)
        {
            if (!_showArrow) return;

            var t = transform;
            var aimingDirection = _controller.AimingDirection;

            t.localScale = new Vector3(1, 1, size);
            t.forward = aimingDirection;
        }
    }
}
