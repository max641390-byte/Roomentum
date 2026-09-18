using UnityEngine;

public class ChangeRoom : MonoBehaviour
{
    [SerializeField] private Transform roomOne;
    [SerializeField] private Transform roomTwo;
    [SerializeField] private GameObject camera;

    // This need improvements
    [SerializeField] private Vector3 changRoomOffset;

    private Transform currentRoom;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Transform playerTransform = other.gameObject.transform;
            ChangeTheRoom(playerTransform);
        }
    }

    private void ChangeTheRoom(Transform playerTransform)
    {
        // Maybe turn camera.GetComponent<CameraController>() into a variable
        currentRoom = camera.GetComponent<CameraController>().GetCurrentRoom();
        if (currentRoom == roomOne)
        {
            camera.GetComponent<CameraController>().UpdateCurrentRoom(roomTwo);
            // Maybe there is a better way then just a teleport 
            playerTransform.position = transform.position + changRoomOffset;
        }
        else if (currentRoom == roomTwo)
        {
            camera.GetComponent<CameraController>().UpdateCurrentRoom(roomOne);
            // Maybe there is a better way then just a teleport 
            playerTransform.position = transform.position + changRoomOffset;
        }
        else
        {
            print("Something is wrong with what the current room is");
            print("Expected either " + roomTwo.name + " " + roomOne.name);
            print("Got " + currentRoom.name);
        }
    }
}