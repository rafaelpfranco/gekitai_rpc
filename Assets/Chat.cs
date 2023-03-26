using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chat : MonoBehaviour
{
    public GameObject controller, wss;
    public TMP_InputField input;
    // Start is called before the first frame update
    void Start()
    {
       
    }
    
    // Envia mensagem
    public void sendMsg()
    {
        if(input.text != "")
        {
            wss.transform.GetComponent<WS_Client>().SendServer(
            "Chat:"+
            controller.transform.GetComponent<controller>().colorPlayers.text+
            ":"+
            input.text+
            "\n"
            );
            input.text = "";
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
