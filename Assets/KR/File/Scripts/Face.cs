using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
        //var f = GameConfig.instance.data.faces;
        //smr.material = f[Random.Range(0, f.Count)];

    }

    ///xx/z/x/
}
