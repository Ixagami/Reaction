using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Figure : MonoBehaviour {
    public Vector3 Speed;

    public event Action OnPress;
    public event Action OnOutOfScreen;

    public void Press() {
        OnPress?.Invoke();
        Destroy(this.gameObject);
    }

    public Vector3 Position {
        get => _Transform.position;
        set => _Transform.position = value;
    }

    public int Score;

    private Transform _Transform;

    public Vector3 Rotation;
    private void Awake() {
        _Transform = this.transform;
        Rotation = new Vector3(Random.Range(0, 10), Random.Range(0, 100), Random.Range(0, 100));
        Rotation.Normalize();
    }

    public float DestroyTime;

    private void Update() {
        DestroyTime -= Time.deltaTime;
        if (DestroyTime > 0) {
            _Transform.position += Speed * Time.deltaTime;
            _Transform.Rotate(Rotation.x, Rotation.y, Rotation.z);
        } else {
            OnOutOfScreen?.Invoke();
            Destroy(this.gameObject);
        }
    }
}