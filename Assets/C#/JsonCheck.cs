using UnityEngine;
using Newtonsoft.Json;

public class JsonCheck : MonoBehaviour {
    void Start() {
        var json = "{\"name\":\"Jiarui\", \"score\":95}";
        var data = JsonConvert.DeserializeObject<Player>(json);
        Debug.Log($"{data.name} 得分 {data.score}");
    }

    class Player {
        public string name;
        public int score;
    }
}
