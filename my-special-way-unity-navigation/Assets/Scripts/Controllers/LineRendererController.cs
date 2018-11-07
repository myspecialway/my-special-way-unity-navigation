using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineRendererController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Color _color;
    [SerializeField] private float _lineWidth;
    [SerializeField] private float _updateInterval;
    
    private float        _elapsedTime;
    private int          _currentIndex = 0;

    protected void Awake()
    {
        _lineRenderer.startColor      = _color;
        _lineRenderer.endColor        = _color;
        _lineRenderer.widthMultiplier = _lineWidth;
    }
    
    protected void Update()
    {

        var currentPosition = _camera.transform.position;
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime > _updateInterval)
        {
            _elapsedTime = 0f;
            AddNode(currentPosition);
        }
    }
    
    private void AddNode(Vector3 position)
    {
        _currentIndex++;
        _lineRenderer.positionCount = _currentIndex;
        _lineRenderer.SetPosition(_currentIndex - 1, position);
    }
}