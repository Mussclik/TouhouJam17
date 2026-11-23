using System;
using UnityEngine;

public class PositionJitter : MonoBehaviour
{
    private Vector3 basePosition;
    [SerializeField] private Vector3 jitterFactor;

    private void Start()
    {
        basePosition = transform.position;
    }

    private void Update()
    {
        RandomiseJitter();
    }

    private void RandomiseJitter()
    {
        float x = UnityEngine.Random.Range(0f, 0.1f) * jitterFactor.x;
        float y = UnityEngine.Random.Range(0f, 0.1f) * jitterFactor.y;
        float z = UnityEngine.Random.Range(0f, 0.1f) * jitterFactor.z;

        transform.position = basePosition + new Vector3(x, y, z);
    }
}
