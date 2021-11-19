using UnityEngine;

namespace DoaT.Attributes
{
    public interface IAttributeManager
    {
        Attribute ManagedAttribute { get; }
    }
    
    //TODO: Extract Implementation from Health & Darkness components
    public abstract class AttributeManager : MonoBehaviour, IAttributeManager
    {
        public abstract Attribute ManagedAttribute { get; }
    }
}