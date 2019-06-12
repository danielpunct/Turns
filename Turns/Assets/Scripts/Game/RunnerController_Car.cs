using Gamelogic.Extensions;
using UnityEngine;

public class RunnerController_Car : MonoBehaviour
{
    public RunnerModel[] ballPrefabs;

    public Transform modelsHolder;
    RunnerModel _current;

    void Update()
    {
        if (Runner.Instance.State == Runner.RunnerState.Running)
        {
            var dir = Runner.Instance.Direction.ToVector3();
            var right = new Vector3(dir.z, dir.y, -dir.x);
            var rotationSpeed = Runner.Instance.Speed * 150;
            //_current.Holder.Rotate(right * rotationSpeed, Space.World);
        }
    }

    void Awake()
    {
        LoadModel(0);
    }

    public void LoadModel(int index)
    {
        
        modelsHolder.DestroyChildren();
        _current = Instantiate(ballPrefabs[index], modelsHolder);
        Runner.Instance.SetModel(_current);
    }
}