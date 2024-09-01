using SDKServer.Handlers;
using SDKServer.Middleware;

namespace SDKServer;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        Console.Title = "KunPS | Wuthering Waves | SDK Server";
        Console.WriteLine("                                                            \r\n                                                            \r\n                                                            \r\n                      .....:::.                             \r\n                  .--===--------:====-:.                    \r\n                :====----::::::::::---===-:                 \r\n             .-===-------::::::::::-------===:              \r\n           :====---==--::::::::::::----------==:.           \r\n         .-+=---=====-===--:-------------------==-.         \r\n       :===----====--*#%%%%#%%%%#=-::---------------:.      \r\n      +*=------=====#%%%%%%%%%%%@*-:::--------------===.    \r\n     .*------------=**#%%%%%#*+++=--:::---------------=*-   \r\n      -::::---==---++=--*%#-:+*###=-:::----------------=-   \r\n      =:.:::-----:*@@@@#.=:=%%@@@@#-::::-------::-:::::--   \r\n      =-....:-+*: +@@@@@* +@. =@@@@*-:::::-:-::::::::::=:   \r\n       =*::*=:@@@%@@@@@@# *@%%@@@@@@%+-::.:::::::.....-+    \r\n        -.+@%:=@@@@@@@@%:-:*@@@@@@@@@#:+==--::::::...::     \r\n         .=*++:-=*##*+-.:+-.:+*%%%#+--+*++**##**=.::::      \r\n         :----==**- .-=**+**+-:.===**+=----=*%#%=...        \r\n         .-====-*@*:===========.#@%@*-=====-=#%#..          \r\n          :-==-=#%%*-==========*%%%%*---==--=##::+++-.:.    \r\n           .:+%%%%%%%**+++++*#%%%%%%%*+====*%*: -=-:=+++-.  \r\n             :=%@%%%%%%%%@@@@%%%%%%##%%###%%=.:++:--::===-  \r\n            .  .=*%@%%%%%*++++***######**+- :**+:++:+=-+*+- \r\n             .    :+**+-.         .         :++:+*:=**+:=*+.\r\n             . ..                   .:-::+-   .=**=:*+*+:*+.\r\n            .  .++. .         ..::---: .**.    :*+*:=*+*:=: \r\n           .    =%= :-.   ..:::::.     *%=      +**--*++:.  \r\n          .     -*-  .-..:::.         -**.      :**=:+-.    \n SDK Server控制台\n");

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls("http://*:5500");
        builder.Logging.AddSimpleConsole();

        WebApplication app = builder.Build();
        app.UseMiddleware<NotFoundMiddleware>();

        app.MapGet("/api/login", LoginHandler.Login);
        app.MapGet("/index.json", ConfigHandler.GetBaseConfig);

        app.MapGet("/prod/client/:type/Android/KeyList_1.2.0.json", HotPatchHandler.OnKeyListRequest);
        app.MapGet("/prod/client/:type/Android/config.json", HotPatchHandler.OnConfigRequest);
        app.MapGet("/prod/client/:type/Android/client_key/1.2.0/dapYgT8nUyJBijsnRX/PakData", HotPatchHandler.OnPakDataRequest);

        await app.RunAsync();
    }
}
