using TMPro;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class SoftwareVersionDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMesh;
    
#if !UNITY_EDITOR
    private void Awake()
    {
        _textMesh.text = Application.version;
    }
#else
    private void Update()
    {
        if (_textMesh == null) return;
        
        _textMesh.text = Application.version;
    }
#endif
}
