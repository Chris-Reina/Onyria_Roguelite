using DoaT.AI;
using UnityEngine;

namespace DoaT
{
    public class EnemyShieldMonitor : MonoBehaviour
    {
        [SerializeField] private GameObject _enemy;
        [SerializeField] private GameObject _icon;
        [SerializeField] private GameObject _background;

        private IEnemyModel _model;
        
        private void Start()
        {
            _model = _enemy.GetComponent<IEnemyModel>();

            _icon.SetActive(_model.Shielded);
            _background.SetActive(_model.Shielded);
        }
    }
}
