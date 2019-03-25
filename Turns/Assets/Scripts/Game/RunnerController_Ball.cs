using Gamelogic.Extensions;
using UnityEngine;

public class RunnerController_Ball : MonoBehaviour
{
    public GameObject[] ballPrefabs;
    
    public Transform model;

    void Update()
    {
        if (Runner.Instance.IsRunning)
        {
            var dir = Runner.Instance.Direction.ToVector3();
            var right = new Vector3(dir.z, dir.y, -dir.x);
            var rotationSpeed = Runner.Instance.Speed * 100;
            model.Rotate(right * rotationSpeed, Space.World);
        }
    }

    void Start()
    {
        LoadModel(0);
    }

    public void LoadModel(int index)
    {
        model.DestroyChildren();
        Instantiate(ballPrefabs[index], model);
    }
}