using System.Collections;
using UnityEngine;

namespace DoaT
{
    public class RigidbodyAnimationRecorder : MonoBehaviour
    {
        public Rigidbody rb;

        public PhysicsAnimationClusterData dataHolder;
        public float recordTime;
        public float kinematicTime;
        public float coroutineTimerWaitTime;

        private bool _canRecord = true;
        private bool _shouldStop;
        private readonly TimerHandler _handler = new TimerHandler();
        private readonly TimerHandler _handlerShort = new TimerHandler();

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            _canRecord = dataHolder.AddObjectToRecord(gameObject);
        }

        private void Start()
        {
            if(!_canRecord) Destroy(this);
            rb.isKinematic = true;

            TimerManager.SetTimer(new TimerHandler(), StartRecording, kinematicTime);
        }

        private void StartRecording()
        {
            rb.isKinematic = false;   
            TimerManager.SetTimer(_handler, () => _shouldStop = true, recordTime);
            StartCoroutine(RecordTimed());
        }

        private IEnumerator RecordTimed()
        {
            while (!_shouldStop)
            {
                var tfm = transform;
                dataHolder.AddData(gameObject, tfm.position, tfm.rotation);
                yield return new WaitForSeconds(coroutineTimerWaitTime);
            }
            
            dataHolder.AddFinishedObject(gameObject);
        }
    }
}
