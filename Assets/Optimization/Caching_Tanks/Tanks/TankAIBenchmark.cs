using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAIBenchmark : MonoBehaviour
{
    Transform[] tanks;
    public int numberOfTanks;
    public GameObject tankPrefab;
    private Vector3 speed;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {

        if (player != null)
        {
            this.enabled = false;
        }
        player = GameObject.FindGameObjectWithTag("Player");
        GameObject tanksG = new GameObject();
        tanksG.name = "tanks";
        tanks = new Transform[numberOfTanks];
        for (int i = 0; i < numberOfTanks; i++)
        {
            var rr = Random.Range(0, 1);
            speed = new Vector3(0, 0, 0.02f + rr);

            var currTank = tanks[i] = Instantiate(tankPrefab).transform;
            tanks[i].position = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
            tanks[i].parent = tanksG.transform;
            tanks[i] = currTank;
        }
    }

    // Update is called once per frame
    void Update()
    {
        var currPos = player.transform.position;
        foreach (Transform t in tanks)
        {
            t.transform.LookAt(currPos);
            t.transform.Translate(speed);
        }
    }
}
