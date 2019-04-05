using Gamelogic.Extensions;
using UnityEngine;

public class RunnerController_Ball : MonoBehaviour
{
    public RotatorModel[] ballPrefabs;

    public Transform modelsHolder;
    RotatorModel _current;

    void Update()
    {
        if (Runner.Instance.IsRunning)
        {
            var dir = Runner.Instance.Direction.ToVector3();
            var right = new Vector3(dir.z, dir.y, -dir.x);
            var rotationSpeed = Runner.Instance.Speed * 100;
            _current.Holder.Rotate(right * rotationSpeed, Space.World);
        }
    }

    void Start()
    {
        LoadModel(0);
    }

    public void LoadModel(int index)
    {
        
        modelsHolder.DestroyChildren();
        _current = Instantiate(ballPrefabs[index], modelsHolder);
    }
}