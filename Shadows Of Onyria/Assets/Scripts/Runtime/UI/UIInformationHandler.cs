using UnityEngine;

public abstract class UIInformationHandler : MonoBehaviour
{
    public abstract void Setup(string infoName, int value);
    public abstract int GetValue();
    public abstract void SetValue(int value);
}