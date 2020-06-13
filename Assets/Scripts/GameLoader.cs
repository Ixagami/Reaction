using System.Collections;
using System.Linq;
using Boo.Lang;
using UnityEngine;
using UnityEngine.UIElements;

public class GameLoader : MonoBehaviour {
    [SerializeField] private Vector3 _FigureSize;

    private GameObject[] Prefabs;

    private void LoadLevelResources() {
        Prefabs = Resources.LoadAll<GameObject>("");
    }

    private Vector3 _LTPosition;
    private Vector3 _RBPosition;

    private void FindScreenBorders() {
        _LTPosition = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelRect.height, 150));
        _RBPosition = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelRect.width, 0, 100));
    }

    private void SpawnFigure(GameObject prefab, Vector3 startPosition, Vector3 speed, float destroy_time) {
        var n_object = GameObject.Instantiate(prefab, startPosition, Quaternion.identity);
        n_object.SetActive(true);
        var fig = n_object.AddComponent<Figure>();
        fig.Speed = speed;
        fig.DestroyTime = destroy_time;
        n_object.AddComponent<SphereCollider>();
    }
    
    private void SpawnRandomFigure(float speed_value, float move_time) {
        var prefab = Prefabs[Random.Range(0, Prefabs.Length)];
        Vector3 start_position;
        Vector3 speed;
        switch (Random.Range(0, 4)) {
            case 0: // Top -> Bottom
                start_position = new Vector3(Random.Range(_LTPosition.x + _FigureSize.x, _RBPosition.x - _FigureSize.x),
                    _LTPosition.y + _FigureSize.y,
                    (_LTPosition.z + _RBPosition.z) / 2.0f);
                speed = new Vector3(0, - speed_value, 0);
                break;
            case 1: // Bottom -> Top
                start_position = new Vector3(Random.Range(_LTPosition.x + _FigureSize.x, _RBPosition.x - _FigureSize.x),
                    _RBPosition.y - _FigureSize.y,
                    (_LTPosition.z + _RBPosition.z) / 2.0f);
                speed = new Vector3(0, speed_value, 0);
                break;
            case 2: // Left -> Right
                start_position = new Vector3(_LTPosition.x - _FigureSize.x,
                    Random.Range(_RBPosition.y - _FigureSize.x, _LTPosition.y + _FigureSize.x),
                    (_LTPosition.z + _RBPosition.z) / 2.0f);
                speed = new Vector3(speed_value, 0, 0);
                break;
            case 3: // Left -> Right
                start_position = new Vector3(_RBPosition.x + _FigureSize.x,
                    Random.Range(_RBPosition.y - _FigureSize.x, _LTPosition.y + _FigureSize.x),
                    (_LTPosition.z + _RBPosition.z) / 2.0f);
                speed = new Vector3(-speed_value, 0, 0);
                break;
            default:
                return;
        }
        SpawnFigure(prefab, start_position, speed, move_time+1.0f);
    }

    IEnumerator Start() {
        LoadLevelResources();
        FindScreenBorders();
        yield return null;

        float move_time = 7.0f;
        float speed_value = Vector3.Distance(_LTPosition, _RBPosition) / move_time;

        while (true) {
            float t = 0.5f;
            while (t>0) {
                t -= Time.deltaTime;
                yield return null;
            }
            SpawnRandomFigure(speed_value, move_time);
        }
    }
}