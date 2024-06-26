using UnityEngine;

public class Oscillator : MonoBehaviour
{
    //This is public strictly for the editor
    public Vector3 StartPosition;
    public Vector3 EndPosition;
    public Space Space;
    [HideInInspector] public Vector3 InitialPosition;

    [SerializeField] private float _speed;
    [SerializeField, Range(0f, 2f)] private float _initialTime;

    private Vector3 _positionDelta;

    private void Start()
    {
        InitialPosition = transform.position;
    }

    private void Update()
    {
        Vector3 offset = Space switch
        {
            Space.World => Vector3.zero,
            Space.Self => InitialPosition,
            _ => throw new System.NotImplementedException(),
        };

        _positionDelta = Vector3.Lerp(StartPosition + offset, EndPosition + offset, Mathf.Cos((Time.timeSinceLevelLoad * _speed + 1 + _initialTime) * Mathf.PI) / 2f + 0.5f) - transform.position;
        transform.Translate(_positionDelta);
    }

    public Vector3 GetVelocity() => _positionDelta;
}
