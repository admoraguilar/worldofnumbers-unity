using UnityEngine;


public class ChildObjectSpawner : CustomMonoBehaviour {
    [SerializeField] private ObjectSetData objectsSet;
    [SerializeField] private SpawnWhen spawnWhen = SpawnWhen.Start;
    [SerializeField] private bool isWorldPositionStays = false;


    private void Spawn() {
        foreach(var obj in objectsSet.Objects) {
            var go = Instantiate(obj) as GameObject;
            if(go) {
                go.transform.SetParent(transform, isWorldPositionStays);
                go.name = obj.name;
            }
        }
    }

    private void Awake() {
        if(spawnWhen == SpawnWhen.Awake) Spawn(); 
    }

    private void Start() {
        if(spawnWhen == SpawnWhen.Start) Spawn();
    }

    protected override void OnEnable() {
        base.OnEnable();
        if(spawnWhen == SpawnWhen.OnEnable) Spawn();
    }


    public enum SpawnWhen {
        Awake,
        Start,
        OnEnable
    }
}
