using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class FramerateDisplay : MonoBehaviour
{
    private enum DisplayMode
    {
        None,
        TagAndNumber,
        Number,
    }

    [SerializeField] private KeyCode _key = KeyCode.F9;
    [SerializeField] private TextMeshProUGUI _display;
    [SerializeField, Range(0.1f,1f)] private float _displayPrintRate = 1f;

    private DisplayMode _mode = DisplayMode.None;
    private int _iterations = 1;
    private int _accumulator = 1;
    
    private int ChunkFPS => _accumulator / _iterations;

    private void Update()
    {
        _iterations += 1;
        _accumulator += Framerate.Current;
        
        if (!Input.GetKeyUp(_key)) return;

        switch (_mode)
        {
            case DisplayMode.None:
                _mode = DisplayMode.TagAndNumber;
                StartCoroutine(Display());
                break;
            case DisplayMode.TagAndNumber:
                _mode = DisplayMode.Number;
                break;
            case DisplayMode.Number:
                _mode = DisplayMode.None;
                _display.text = "";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IEnumerator Display()
    {
        while (_mode != DisplayMode.None)
        {
            _display.text = GetFPS();
            yield return new WaitForSeconds(_displayPrintRate);
        }
    }

    private string GetFPS()
    {
        var chunk = ChunkFPS;
        
        var s = _mode == DisplayMode.TagAndNumber ? $"FPS: {chunk}" : $"{chunk}";

        _accumulator = chunk;
        _iterations = 1;
        
        return s;
    }
    
    public static class Framerate
    {
        public static int Current => Mathf.CeilToInt(1f / Time.unscaledTime);
    }
}
