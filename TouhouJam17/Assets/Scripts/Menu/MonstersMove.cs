using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonstersMove : MonoBehaviour
{
    [SerializeField] private Transform start;
    [SerializeField] private Transform end;
    [SerializeField] private float time = 8f;
    [SerializeField] private float elapsed = 0f;

    private void Update()
    {
        elapsed += Time.deltaTime;
        transform.position = Vector3.Lerp(start.position, end.position, elapsed / time);


        if (elapsed > time)
        {
            SceneManager.LoadScene(0);
        }
    }
}
