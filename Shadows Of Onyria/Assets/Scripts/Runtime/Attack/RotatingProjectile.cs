using System;
using DoaT;
using UnityEngine;

public enum Axis
{
    X,
    Y,
    Z
}

public class RotatingProjectile : GenericProjectile
{
    [SerializeField] private Axis _axis;
    [SerializeField] private float _anglePerSecond;

    private Vector3 _axisVector;
    
    protected override void Awake()
    {
        base.Awake();
        
        _axisVector = _axis switch
        {
            Axis.X => new Vector3(1, 0, 0),
            Axis.Y => new Vector3(0, 1, 0),
            Axis.Z => new Vector3(0, 0, 1),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (_isDisposed) return;
        //transform.Rotate(_axisVector, _anglePerSecond);
        transform.rotation *= Quaternion.Euler(_axisVector * (_anglePerSecond * Time.deltaTime));
    }
}
