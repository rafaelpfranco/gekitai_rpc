using Grpc.Core;
using GrpcGreeter;

namespace GrpcGreeter.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;
    public static int njogador = 0;
    public static string allMsgs = "";
    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<Empty> AddMsg(
        Msg request,
        ServerCallContext context)
    {
        string data = request.Text;
         if(request.Text =="Ready:?")
            {
                if(njogador == 2)
                {
                    allMsgs += "Ready:True\n";
                }else{
                    allMsgs = "Ready:False\n";
                }
            }else
            {
                allMsgs += data + "\n";
            }
                
        _logger.LogInformation("{allMsgs}", allMsgs);
        return Task.FromResult(new Empty());
    }

    public override Task<Msg> Listen(
        Empty request,
        ServerCallContext context)
    {
        return Task.FromResult(new Msg 
        {
            Text = allMsgs
        });
    }

     public override Task<Msg> GetColor(
        Empty request,
        ServerCallContext context)
    {
        njogador++;
        if(njogador == 1 )
        {
            return Task.FromResult(new Msg 
            {
                Text = "Cor:Vermelha"
            });
        }else
        {
            return Task.FromResult(new Msg 
            {
                Text = "Cor:Preta"
            });
        }
        
    }

}


