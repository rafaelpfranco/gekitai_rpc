using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;


public class controller : MonoBehaviour
{
    public Text turnText, redCounter, blackCounter;
    public Text colorPlayers, chat;
    public GameObject[,] field;
    public GameObject playAgain, wss;
    public int redPiece, blackPiece, turn;
    public bool gameOver, gameStart;
    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;
        gameStart = false;
        turn = 1;
        redPiece = 8;
        blackPiece = 8;
        redCounter.text = "Peças vermelhas restantes: " + redPiece.ToString();
        blackCounter.text = "Peças vermelhas restantes: " + blackPiece.ToString();
        field = new GameObject[6,6];
        for(int i = 0; i < 6; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                field[i, j] = GameObject.Find("Tabuleiro/" + i.ToString() + j.ToString());
            }
        }
    }
    
    // Imprime na tela a cor do jogador
    public void SetColorPlayer(string color)
    {
        colorPlayers.text = color;
    }

    // Pergunta ao servidor se já pode iniciar a partida
    public void sendReady()
    {
        wss.transform.GetComponent<WS_Client>().SendServer("Ready:?");
    }

    // Inicia o jogo se ambos jogadores estiverem conectado
    public void StartGame(string payload)
    {
        if(payload == "True")
        {
            Debug.Log("Startou o game");
            gameStart = true;
        }
    }

    // Controla o tabuleiro 
    public void NewPiece(string name)
    {
        int x = name[0] - '0';
        int y = name[1] - '0';

        // Inserir o sprite da peça
        if(turn%2 == 1)
        {   
            field[x, y].transform.GetComponent<House>().spriteRenderer.sprite = Resources.Load<Sprite>("Image/PecaVermelha");
            field[x, y].transform.GetComponent<House>().colorPiece = "Vermelha";
            redPiece--;
            
        }else
        {
            field[x, y].transform.GetComponent<House>().spriteRenderer.sprite = Resources.Load<Sprite>("Image/PecaPreta");
            field[x, y].transform.GetComponent<House>().colorPiece = "Preta";
            blackPiece--;
        }
        field[x, y].transform.GetComponent<House>().empty = false;

        // Verifica peças ao redor da peça escolhida
        for(int i = (x > 0 ? x - 1 : 0); i <= (x < 5 ? x + 1 : 5); i++)
        {
            for(int j = (y > 0 ? y - 1 : 0); j <= (y < 5 ? y + 1 : 5); j++)
            {
                // Desconsidera a própria peça
                if(x == i  && y == j)
                {
                    continue;
                }
                if(!field[i, j].transform.GetComponent<House>().empty)
                {
                    // Controle para saber a peça ao lado da peça empurrada
                    // A peça empurrada é inserida no novo lugar e apagada no lugar anterior
                    int k = 2*i - x;
                    int w = 2*j - y;
                    bool getOut = false;
                    if(k > 5 || k < 0)
                    {
                        getOut = true;
                    }
                    if(w > 5 || w < 0)
                    {
                        getOut = true;
                    }
                    if(!getOut)
                    {
                        if(field[k, w].transform.GetComponent<House>().empty)
                        {
                            field[k, w].transform.GetComponent<House>().OnePiece(field[i, j].transform.GetComponent<House>().colorPiece);
                            field[i, j].transform.GetComponent<House>().ErasePiece();
                        }
                    }else{
                        string colorTemp = field[i, j].transform.GetComponent<House>().colorPiece;
                        field[i, j].transform.GetComponent<House>().ErasePiece();
                        if(colorTemp == "Vermelha"){
                            redPiece++;
                        }else
                        {
                            blackPiece++;
                        }
                    }
                }
            }
        }
        // Verifica quem venceu
        bool draw = VerifyAlign("Preta") && VerifyAlign("Vermelha");
        if(blackPiece == 0 || (VerifyAlign("Preta") && !draw))
        {
            //Debug.Log(GameObject.Find("PretoVenceu"));
            GameObject.Find("PretoVenceu").transform.GetComponent<SpriteRenderer>().enabled = true;
            playAgain.SetActive(true);
            gameOver = true;
        }
        if(redPiece == 0 || (VerifyAlign("Vermelha") && !draw))
        {
            //Debug.Log(GameObject.Find("VermelhoVenceu"));
            GameObject.Find("VermelhoVenceu").transform.GetComponent<SpriteRenderer>().enabled = true;
            playAgain.SetActive(true);
            gameOver = true;
        }
        // Caso ambos jogadores vençam a vitória é de quem fez a jogada
        if(draw)
        {
            if(turn%2 == 0){
                GameObject.Find("PretoVenceu").transform.GetComponent<SpriteRenderer>().enabled = true;               
            }else
            {
                GameObject.Find("VermelhoVenceu").transform.GetComponent<SpriteRenderer>().enabled = true;
            }
            playAgain.SetActive(true);
            gameOver = true;
        }
        turn++;
        
        // Controle na tela para definir de quem é a vez
        if(turn%2 == 0)
        {
            turnText.text = "Turno dos pretos";
        }else{
            turnText.text = "Turno dos vermelhos";
        }
        // Controle na tela para definir quantas peças restam
        redCounter.text = "Peças vermelhas restantes: " + redPiece.ToString();
        blackCounter.text = "Peças pretas restantes: " + blackPiece.ToString();
    }

    // Verifica se as peças estão alinhadas
    public bool VerifyAlign(string color)
    {

        for(int i = 0; i < 6; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                // ignora as pontas
                if((i == 0 || i == 5) && ((j == 0 || j == 5)))
                {
                    continue;
                }
                // verifica apenas a horizontal do tabuleiro
                if(i == 0 || i == 5)
                {
                    // verfica as 3 casas
                    if(
                        (field[i, j -1].transform.GetComponent<House>().colorPiece == color) && 
                        (field[i, j].transform.GetComponent<House>().colorPiece == color) && 
                        (field[i, j +1].transform.GetComponent<House>().colorPiece == color)
                    )
                    {
                        return true;
                    }
                    continue;
                }
                // verifica apenas a vertical do tabuleiro
                if(j == 0 || j == 5)
                {
                    // verifica as 3 casas
                    if(
                        (field[i - 1, j].transform.GetComponent<House>().colorPiece == color) && 
                        (field[i, j].transform.GetComponent<House>().colorPiece == color) && 
                        (field[i + 1, j].transform.GetComponent<House>().colorPiece == color)
                    )
                    {
                        return true;
                    }
                    continue;
                }
                // verifica interior do tabuleiro
                if((
                    (field[i - 1, j].transform.GetComponent<House>().colorPiece == color) && 
                    (field[i, j].transform.GetComponent<House>().colorPiece == color) && 
                    (field[i + 1, j].transform.GetComponent<House>().colorPiece == color)
                    ) ||
                   (
                    (field[i, j -1].transform.GetComponent<House>().colorPiece == color) && 
                    (field[i, j].transform.GetComponent<House>().colorPiece == color) && 
                    (field[i, j +1].transform.GetComponent<House>().colorPiece == color)
                    ) ||
                    (
                    (field[i - 1, j -1].transform.GetComponent<House>().colorPiece == color) && 
                    (field[i, j].transform.GetComponent<House>().colorPiece == color) && 
                    (field[i + 1, j + 1].transform.GetComponent<House>().colorPiece == color)
                    ) ||
                    (
                    (field[i + 1, j -1].transform.GetComponent<House>().colorPiece == color) && 
                    (field[i, j].transform.GetComponent<House>().colorPiece == color) && 
                    (field[i - 1, j + 1].transform.GetComponent<House>().colorPiece == color)
                    )
                )
                {
                    return true;
                }

            }
        }
        return false;
    }
    // Envia jogar novamente para o cliente
    public void SendPlayAgain()
    {
        wss.transform.GetComponent<WS_Client>().SendServer(
            "PlayAgain:"+
            colorPlayers.text
            );
    }
    
    // Cliente chama o método e reinicia o tabuleiro
    public void PlayAgain()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Envia desistencia para o cliente
    public void SendSurrender()
    {
       wss.transform.GetComponent<WS_Client>().SendServer(
            "Surrender:"+
            colorPlayers.text
            );
    }

    // Cliente chama o método e desiste
    public void Surrender(string color)
    {
        if(color == "Vermelha")
        {
            GameObject.Find("PretoVenceu").transform.GetComponent<SpriteRenderer>().enabled = true;
            playAgain.SetActive(true);
        }
        if(color == "Preta")
        {
            GameObject.Find("VermelhoVenceu").transform.GetComponent<SpriteRenderer>().enabled = true;
            playAgain.SetActive(true);
        }
        gameOver = true;
    }

    void Update()
    {
        if(!gameStart)
        {
            sendReady();
        }
    }
}
