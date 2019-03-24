using UnityEngine;

public class PlayerBallController : MonoBehaviour
{
    public Transform model;
    
    void Update()
    {
        if (Player.Instance.IsRunning)
        {
            var dir = Player.Instance.Direction.ToVector3();
            var right = new Vector3(dir.z, dir.y, -dir.x);
            var rotationSpeed = Player.Instance.Speed * 100;
            model.Rotate( right * rotationSpeed, Space.World);
        }
    }
}
