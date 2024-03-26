using Assets.Scripts;
using UnityEngine;

public class AntRadioScript : MonoBehaviour
{
    private AntGrpcClient _antGrpcClient;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Create AntGrpcClient");
        _antGrpcClient = new AntGrpcClient();
        _antGrpcClient.SearchForService();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
