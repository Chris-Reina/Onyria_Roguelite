using System.Collections;
using UnityEngine;

public class RigidbodyAnimationPlayer : MonoBehaviour
{
    private float _playSpeed;
    private bool _isPlaying = false;
    private PhysicsAnimationData _data;

    private void Start()
    {
        if(_data == default)
        {
            Destroy(this);
            return;
        }
        transform.position = _data.spatialInformation[0].position.ToVector3();
        transform.rotation = _data.spatialInformation[0].rotation.ToQuaternion();
    }

    public void Play()
    {
        if (_isPlaying) return;
        
        StartCoroutine(Animation());
        _isPlaying = true;
    }

    public void Reset()
    {
        if (_isPlaying)
        {
            StopAllCoroutines();
            _isPlaying = false;
        }
        
        var positionalInfo = _data.spatialInformation[0];
        transform.position = positionalInfo.position.ToVector3();
        transform.rotation = positionalInfo.rotation.ToQuaternion();
    }

    private IEnumerator Animation()
    {
        for (int i = 1; i < _data.spatialInformation.Count; i++)
        {
            var positionalInfo = _data.spatialInformation[i];
            transform.position = positionalInfo.position.ToVector3();
            transform.rotation = positionalInfo.rotation.ToQuaternion();
            yield return new WaitForSeconds(_playSpeed);
        }

        _isPlaying = false;
        //Destroy(this);
    }

    public void SetData(PhysicsAnimationData getAnimationData, float playSpeed)
    {
        _data = getAnimationData;
        _playSpeed = playSpeed;
    }
}