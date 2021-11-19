using UnityEngine;

namespace DoaT
{
    public abstract class MonoBehaviourInit : MonoBehaviour, IInitializationProcess
    {
        public abstract float OnInitialization();
    }

    public interface IInitializationProcess
    {
        float OnInitialization();
    }
}