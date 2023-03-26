const WebSocket = require('ws')
// Starta o server na porta 8080
const wss = new WebSocket.Server({port: 8080}, ()=>{
    console.log('Server started')
}), webSockets = {}

var nPlayers = 0

wss.on('listening', ()=>{
    console.log('server ir listening on port 8080')
}) 

// Recebe o cliente e envia novos dados
wss.on('connection', function connection(ws) {
    if(nPlayers < 2){
        ws.id = nPlayers++
        console.log(ws.id)
        // Decide quem vai ser o vermelho e o preto
        // O primeiro a se conectar sem será o vermelho
        if(ws.id === 0)
        {
            ws.send("Cor:Vermelha")
        }else
        {
            ws.send("Cor:Preta")
        }
        // Recebe dados do cliente
        ws.on('message', function incoming(data) {
            console.log("Recived %s", data)
            // Aguarda os 2 clientes
            if(String(data) === "Ready:?")
            {
                if(nPlayers === 2)
                {
                    data = "Ready:True"
                }else{
                    data = "Ready:False"
                }
            }
            // Envia o dado para o cliente
            wss.clients.forEach(function each(client) {
                client.send(String(data));
          })

        // Resesta servidor e quantidade de jogadores
        if((String(data) === 'PlayAgain:Preta') || (String(data) === 'PlayAgain:Vermelha'))
        {
            nPlayers = 0
            wss.clients.forEach(function each(client) {
                client.terminate();
          })
        }
        })
    }
  })