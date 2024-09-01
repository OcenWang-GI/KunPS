using Core.Config;
using Core.Extensions;
using GameServer.Controllers.ChatCommands;
using GameServer.Controllers.Combat;
using GameServer.Controllers.Factory;
using GameServer.Controllers.Manager;
using GameServer.Extensions;
using GameServer.Models;
using GameServer.Network;
using GameServer.Network.Kcp;
using GameServer.Network.Messages;
using GameServer.Network.Rpc;
using GameServer.Settings;
using GameServer.Systems.Entity;
using GameServer.Systems.Event;
using GameServer.Systems.Notify;
using Microsoft.Extensions.Configuration;   // for IConfiguration
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GameServer;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        Console.Title = "KunPS | Wuthering Waves | Game Server";
        Console.WriteLine("                                                            \r\n                                                            \r\n                                                            \r\n                      .....:::.                             \r\n                  .--===--------:====-:.                    \r\n                :====----::::::::::---===-:                 \r\n             .-===-------::::::::::-------===:              \r\n           :====---==--::::::::::::----------==:.           \r\n         .-+=---=====-===--:-------------------==-.         \r\n       :===----====--*#%%%%#%%%%#=-::---------------:.      \r\n      +*=------=====#%%%%%%%%%%%@*-:::--------------===.    \r\n     .*------------=**#%%%%%#*+++=--:::---------------=*-   \r\n      -::::---==---++=--*%#-:+*###=-:::----------------=-   \r\n      =:.:::-----:*@@@@#.=:=%%@@@@#-::::-------::-:::::--   \r\n      =-....:-+*: +@@@@@* +@. =@@@@*-:::::-:-::::::::::=:   \r\n       =*::*=:@@@%@@@@@@# *@%%@@@@@@%+-::.:::::::.....-+    \r\n        -.+@%:=@@@@@@@@%:-:*@@@@@@@@@#:+==--::::::...::     \r\n         .=*++:-=*##*+-.:+-.:+*%%%#+--+*++**##**=.::::      \r\n         :----==**- .-=**+**+-:.===**+=----=*%#%=...        \r\n         .-====-*@*:===========.#@%@*-=====-=#%#..          \r\n          :-==-=#%%*-==========*%%%%*---==--=##::+++-.:.    \r\n           .:+%%%%%%%**+++++*#%%%%%%%*+====*%*: -=-:=+++-.  \r\n             :=%@%%%%%%%%@@@@%%%%%%##%%###%%=.:++:--::===-  \r\n            .  .=*%@%%%%%*++++***######**+- :**+:++:+=-+*+- \r\n             .    :+**+-.         .         :++:+*:=**+:=*+.\r\n             . ..                   .:-::+-   .=**=:*+*+:*+.\r\n            .  .++. .         ..::---: .**.    :*+*:=*+*:=: \r\n           .    =%= :-.   ..:::::.     *%=      +**--*++:.  \r\n          .     -*-  .-..:::.         -**.      :**=:+-.    \n Game Server\n");

        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        builder.Logging.AddConsole();

        builder.SetupConfiguration();
        builder.Services.UseLocalResources()
                        .AddControllers()
                        .AddCommands()
                        .AddSingleton<ConfigManager>()
                        .AddSingleton<KcpGateway>().AddScoped<PlayerSession>()
                        .AddScoped<MessageManager>().AddSingleton<EventHandlerFactory>()
                        .AddScoped<RpcManager>().AddScoped<IRpcEndPoint, RpcSessionEndPoint>()
                        .AddSingleton<SessionManager>()
                        .AddScoped<EventSystem>().AddScoped<EntitySystem>().AddScoped<IGameActionListener, NotifySystem>()
                        .AddScoped<EntityFactory>()
                        .AddScoped<ModelManager>().AddScoped<ControllerManager>()
                        .AddScoped<CombatManager>().AddScoped<ChatCommandManager>()
                        .AddHostedService<WWGameServer>();

        IHost host = builder.Build();

        ILogger logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("WutheringWaves");
        logger.LogInformation("Support: https://https://github.com/subai66/KunPS"); //discord.gg/reversedrooms or discord.xeondev.com
        logger.LogInformation("Preparing server...");

        host.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStarted.Register(() =>
        {
            logger.LogInformation("Server started! Let's play Wuthering Waves!");
        });

        await host.RunAsync();
    }



    private static void SetupConfiguration(this HostApplicationBuilder builder)
    {

         // 添加 JSON 文件配置
        builder.Configuration.AddJsonFile("gameplay.json");     //optional: true, reloadOnChange: true

        // 配置服务
        builder.Services.Configure<GatewaySettings>(builder.Configuration.GetRequiredSection("Gateway"));
        builder.Services.Configure<PlayerStartingValues>(builder.Configuration.GetRequiredSection("StartingValues"));
        builder.Services.Configure<GameplayFeatureSettings>(builder.Configuration.GetRequiredSection("Features"));
    }
}