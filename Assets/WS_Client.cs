using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using Grpc.Core;
using GrpcGreeterClient;
public class WS_Client : MonoBehaviour
{

    Channel channel;
    Greeter.GreeterClient client;
    public GameObject controller;
    public int readLine;

    // Start is called before the first frame update
    void Start()
    {
        // Cria um novo objeto Channel com o endereço IP e a porta do servidor de destino. Nesse caso, o endereço IP é "127.0.0.1" e a porta é "5048".
        channel = new Channel("127.0.0.1:5048", ChannelCredentials.Insecure);

        // Cria um novo objeto GreeterClient usando o objeto Channel criado acima. O GreeterClient é usado para se comunicar com o servidor usando o protocolo gRPC.
        client = new Greeter.GreeterClient(channel);

        // Controle das linhas já lidas
        readLine = 0;

        // Chama o método GetColor do cliente para solicitar uma cor para jogar e armazena a resposta na variável msg.
        var msg = client.GetColor(new Empty{}).Text.Split(new[] {':'}, 2);;

        // Separa a mensagem recebida em dois campos: tipo e payload. O campo tipo é a primeira parte da mensagem antes do caractere ':', e o campo payload é a segunda parte da mensagem após o caractere ':'.
        string type = msg[0];
        string payload = msg[1];

        // Chama o método MsgProcess para processar a mensagem recebida. O método MsgProcess é responsável por executar a lógica necessária com base no tipo de mensagem recebida e no payload associado a ela.
        MsgProcess(type, payload);

    }
    
    // Envia string do cliente para o servidor
    public void SendServer(string msg)
    {
        // Verifica se o canal de comunicação com o servidor está ativo.
        if(channel != null)
        {
            // Se sim, chama o método AddMsg do cliente para enviar uma mensagem para o servidor. A mensagem enviada é um objeto Msg com o campo Text definido como o valor da variável msg.
            client.AddMsg(new Msg{Text = msg});
        }

    }

    // Processa a mensagem e dependendo do type envia para um método diferente no controller
    public void MsgProcess(string type, string payload)
    {
        Debug.Log(payload);
        Debug.Log(type);
        if(type == "Cor")
        {
            controller.transform.GetComponent<controller>().SetColorPlayer(payload);
        }
        if(type == "Chat")
        {
            controller.transform.GetComponent<controller>().chat.text += payload + "\n";
        }
        if(type == "Move")
        {
            controller.transform.GetComponent<controller>().NewPiece(payload);
        }
        if(type == "Surrender")
        {
            controller.transform.GetComponent<controller>().Surrender(payload);
        }
        if(type == "PlayAgain")
        {
            controller.transform.GetComponent<controller>().PlayAgain();
        }
        if(type == "Ready")
        {
            controller.transform.GetComponent<controller>().StartGame(payload);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // Chama o método Listen do cliente para receber as mensagens do servidor e armazena a resposta na variável linha.
        string[] linha = client.Listen(new Empty{}).Text.ToString().Split("\n");

        // Itera sobre as linhas recebidas, começando pela linha readLine. O objetivo é processar apenas as linhas que ainda não foram lidas.
        for(int i = readLine; i < linha.Length; i++)
        {
            // Verifica se a linha atual contém alguma informação.
            if (linha[i].Length > 0) 
            {
                // Se sim, separa a mensagem recebida em dois campos: tipo e payload. O campo tipo é a primeira parte da mensagem antes do caractere ':', e o campo payload é a segunda parte da mensagem após o caractere ':'.
                var msg = linha[i].Split(new[] {':'}, 2);
                string type = msg[0];
                string payload = msg[1];

                // Chama o método MsgProcess para processar a mensagem recebida. O método MsgProcess é responsável por executar a lógica necessária com base no tipo de mensagem recebida e no payload associado a ela.
                MsgProcess(type, payload);
            }          
        }

        // Atualiza a variável readLine para a próxima linha a ser lida.
        readLine = linha.Length - 1;
    
    }
}
