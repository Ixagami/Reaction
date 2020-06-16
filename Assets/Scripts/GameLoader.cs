using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GameLoader : MonoBehaviour {
    [SerializeField] private Vector3 _FigureSize;
    [SerializeField] private UICore _UI;

    private GameObject[] Prefabs;

    private void LoadLevelResources() {
        Prefabs = Resources.LoadAll<GameObject>("Figures");
    }

    private Vector3 _LTPosition;
    private Vector3 _RBPosition;

    private void FindScreenBorders() {
        _LTPosition = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelRect.height, 150));
        _RBPosition = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelRect.width, 0, 100));
    }

    private IEnumerable<Vector3> GetShowcasePositions(int count) {
        int max_in_line = Mathf.FloorToInt((_RBPosition.x - _LTPosition.x) / _FigureSize.x) - 1;
        int line_count = count / max_in_line;
        if (count % max_in_line != 0)
            line_count++;

        Debug.Log($"{max_in_line} {line_count}");

        float center_y = (_LTPosition.y + _RBPosition.y) / 2.0f;

        float start_y = center_y + line_count * _FigureSize.y / 2.0f - _FigureSize.y;


        float center_x = (_RBPosition.x + _LTPosition.x) / 2.0f;

        float z = (_RBPosition.z + _LTPosition.z) / 2.0f;

        while (count>0) {
            if (count < max_in_line) {
                max_in_line = count;
                count = 0;
            } else 
                count -= max_in_line;
            
            float start_x = center_x - (max_in_line) * _FigureSize.x / 2.0f + _FigureSize.x*1.2f;

            for (int i = 0; i < max_in_line; i++) {
                yield return new Vector3(start_x, start_y, z);
                start_x += _FigureSize.x;
            }

            start_y -= _FigureSize.y;
        }

    }

    private Figure SpawnFigure(GameObject prefab, float speed_value, float move_time) {
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
                return null;
        }

        var n_object = GameObject.Instantiate(prefab, start_position, Quaternion.identity);
        n_object.SetActive(true);
        n_object.AddComponent<SphereCollider>();

        var fig = n_object.AddComponent<Figure>();
        fig.Speed = speed;
        fig.DestroyTime = move_time + 1.0f;

        return fig;
    }

    public static EventIntValue Level = new EventIntValue();
    public static EventIntValue Collected = new EventIntValue();
    public static EventIntValue Missed = new EventIntValue();
    public static EventIntValue Lives = new EventIntValue();

    private void OnStatusChanged(int val1, int val2) {
        _UI.StatusText.text = $"Уровень: {Level.Value}\nЖизни: {Lives.Value} | Собрано: {Collected.Value} | Пропущено: {Missed.Value}";
    }

    private void OnTimeChanged(float time) {
        _UI.TimerText.text = $"Время: {time:000.00} с.";
    }

    IEnumerator Start() {
        // Init game session
        LoadLevelResources();
        FindScreenBorders();
        
        Level.OnValueChanged += OnStatusChanged;
        Collected.OnValueChanged += OnStatusChanged;
        Lives.OnValueChanged += OnStatusChanged;
        Missed.OnValueChanged += OnStatusChanged;

        // Инициализация значений
        Level.Value = 1;
        Collected.Value = 0;
        Lives.Value = GameConfig.BaseLifeAmount;
        Missed.Value = 0;


        _UI.ShowcaseGroup.gameObject.SetActive(false);
        _UI.UserUIGroup.gameObject.SetActive(false);

        // Main Loop
        while (true) {
            var cfg = GameConfig.GetForLevel(Level.Value);

            float speed = Vector3.Distance(_LTPosition, _RBPosition);

            List<int> prefab_stack = new List<int>(Prefabs.Length);
            for (int i = 0; i < Prefabs.Length; i++) 
                prefab_stack.Add(i);

            // Список префабов объектов, 
            // <Префаб объекта, true - для "хороших" объектов, false - для "плохих">
            List<Tuple<GameObject, bool>> game_objects = new List<Tuple<GameObject, bool>>();

            // Выбираем "хорошие" фигуры
            var good_kinds = (cfg.UserObjectKinds < prefab_stack.Count / 2)
                ? cfg.UserObjectKinds
                : prefab_stack.Count / 2;

            for (int i = 0; i < good_kinds; i++) {
                var pos = Random.Range(0, prefab_stack.Count);
                game_objects.Add( new Tuple<GameObject, bool>(Prefabs[prefab_stack[pos]], true));
                prefab_stack.RemoveAt(pos);
            }

            // Выбираем "плохие" фигуры
            var bad_kinds = cfg.TotalObjectKinds - good_kinds;
            bad_kinds = (bad_kinds < prefab_stack.Count)
                ? bad_kinds
                : prefab_stack.Count;

            for (int i = 0; i < bad_kinds; i++) {
                var pos = Random.Range(0, prefab_stack.Count);
                game_objects.Add(new Tuple<GameObject, bool>(Prefabs[prefab_stack[pos]], false));
                prefab_stack.RemoveAt(pos);
            }

            
            // Показываем фигуры
            var showcase_points = GetShowcasePositions(good_kinds).GetEnumerator();
            
            List<GameObject> showcase_objects = new List<GameObject>();

            foreach (var obj in game_objects.Where(q => q.Item2).Select(q=>q.Item1)) {
                if (!showcase_points.MoveNext())
                    throw new Exception("Количество точек недостаточно!");
                var go = GameObject.Instantiate(obj, showcase_points.Current, Quaternion.identity);
                go.SetActive(true);
                showcase_objects.Add(go);
            }
            
            _UI.ShowcaseGroup.gameObject.SetActive(true);

            float timer = GameConfig.ShowcaseTime;

            while (timer>0) {
                timer -= Time.deltaTime;
                _UI.ShowcaseTimerText.text = timer.ToString("####");
                yield return null;
            }

            foreach (var showcase_object in showcase_objects) 
                Destroy(showcase_object);

            timer = GameConfig.LevelTime;
            float time_show = 0.0f;

            _UI.ShowcaseGroup.gameObject.SetActive(false);
            _UI.UserUIGroup.gameObject.SetActive(true);

            while (timer > 0) {
                timer -= Time.deltaTime;
                time_show += Time.deltaTime;

                OnTimeChanged(timer);

                int figure_amount = Mathf.FloorToInt(time_show / cfg.ObjectTimeDelta);
                time_show -= time_show * figure_amount;

                for (int i = 0; i < figure_amount; i++) {
                    var fig = game_objects[Random.Range(0, game_objects.Count)];
                    var figure = SpawnFigure(fig.Item1, speed/cfg.MoveTime, cfg.MoveTime + 1);
                    if (fig.Item2) {
                        figure.OnPress += () => Collected.Value++;
                        figure.OnOutOfScreen += () => Missed.Value++;
                    } else
                        figure.OnPress += () => Lives.Value--;
                }

                if (Lives.Value <= 0)
                    yield break; // Game over

                yield return null;
            }

            _UI.UserUIGroup.gameObject.SetActive(false);
        }
    }
}