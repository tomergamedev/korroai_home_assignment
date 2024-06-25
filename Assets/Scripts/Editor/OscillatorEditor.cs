using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Oscillator)), CanEditMultipleObjects]
public class OscillatorEditor : Editor
{
    protected virtual void OnSceneGUI()
    {
        Oscillator oscillator = (Oscillator)target;

        EditorGUI.BeginChangeCheck();
        Vector3 origin = oscillator.Space switch
        {
            Space.World => Vector3.zero,
            Space.Self => Application.isPlaying? oscillator.InitialPosition : oscillator.transform.position,
            _ => throw new System.NotImplementedException(),
        };
        Vector3 startPosition = Handles.PositionHandle(origin + oscillator.StartPosition , Quaternion.identity);
        Vector3 endPosition = Handles.PositionHandle(origin + oscillator.EndPosition, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(oscillator, "Change Oscillator Positions");
            oscillator.StartPosition = startPosition - origin;
            oscillator.EndPosition = endPosition - origin;
        }
    }
}
