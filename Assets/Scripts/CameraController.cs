using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 _target;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _smoothing;
    [SerializeField] private Transform currentRoom;

    void Start()
    {
        UpdateCurrentRoom(currentRoom);
    }

    void FixedUpdate()
    {
        Vector3 newPosition = Vector3.Lerp(transform.position, _target + _offset, _smoothing * Time.deltaTime);
        transform.position = newPosition;
    }

    public Transform GetCurrentRoom()
    {
        return currentRoom;
    }

    public void UpdateCurrentRoom(Transform newRoom)
    {
        currentRoom = newRoom;
        _target = currentRoom.position;
    }
}
