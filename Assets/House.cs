using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    public bool empty;
    public GameObject controller, wss;
    public SpriteRenderer spriteRenderer;
    public string caminhoDaImagem;
    public GameObject peca;
    public string colorPiece;
    // Start is called before the first frame update
    void Start()
    {
        empty = true;
        colorPiece = "Sem peça";
        peca = new GameObject("Peca");
        peca.transform.SetParent(this.transform);
        peca.transform.localPosition = new Vector3(0, 0, 0);
        peca.transform.localScale = new Vector2(0.25f, 0.25f);
        peca.AddComponent<SpriteRenderer>();
        spriteRenderer = peca.GetComponent<SpriteRenderer>();
        wss = GameObject.Find("wss");
        controller = GameObject.Find("Controller");
    }

    // Verifica de quem é o turno
    public bool TurnVerify(int turn, string color)
    {
        return (turn%2 == 1 && color == "Vermelha") || (turn%2 == 0 && color == "Preta");
    }

    // Envia a jogada para o servidor 
    public void sendPlay()
    {
       wss.transform.GetComponent<WS_Client>().SendServer(
            "Move:"+
            this.name
            );        
    }

    // Evento de clique
    private void OnMouseDown()
    {
        Debug.Log(TurnVerify(controller.transform.GetComponent<controller>().turn, controller.transform.GetComponent<controller>().colorPlayers.text));

        // Se o jogo não acabou, se a partida tiver iniciado e se a cor do jogador for a mesma da cor do turno
        if(empty &&
         !controller.transform.GetComponent<controller>().gameOver &&
         controller.transform.GetComponent<controller>().gameStart &&
         TurnVerify(controller.transform.GetComponent<controller>().turn, controller.transform.GetComponent<controller>().colorPlayers.text))
        {
            sendPlay();
        }

    }

    // Apaga peça, utilizado para quando a peça é empurrada ou saí do tabuleiros
    public void ErasePiece()
    {
        spriteRenderer.sprite = null;
        empty = true;
        colorPiece = "Sem peça";
    }

    // Insere uma peça 
    public void OnePiece(string color)
    {
        spriteRenderer.sprite = Resources.Load<Sprite>("Image/Peca"+color);
        empty = false;
        colorPiece = color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
