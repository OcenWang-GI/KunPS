using Microsoft.AspNetCore.Http.HttpResults;
using SDKServer.Models.BaseConfig;

namespace SDKServer.Handlers;

internal static class ConfigHandler
{
    public static JsonHttpResult<BaseConfigModel> GetBaseConfig()
    {
        return TypedResults.Json(new BaseConfigModel
        {
            CdnUrl = [
                new CdnUrlEntry
                {
                    Url = "http://127.0.0.1:5500/dev/client/",
                    Weight = "100"
                },
                new CdnUrlEntry
                {
			         Url = "https://cdn-aws-hw-mc.aki-game.net/prod/client/",
                     Weight = "384"
	        	},
                new CdnUrlEntry
	        	{
                     Url = "https://cdn-qcloud-hw-mc.aki-game.net/prod/client/",
                     Weight = "565"
	         	},
                new CdnUrlEntry
	          	{
                    Url = "https://cdn-akamai-hw-mc.aki-game.net/prod/client/",
                    Weight = "384"
		        },
                new CdnUrlEntry
		        {
                    Url = "https://cdn-huoshan-hw-mc.aki-game.net/prod/client/",
                    Weight = "284"
		        }
            ],
            
            SecondaryUrl = [],

            GmOpen = false,
            PayUrl = "http://114.132.150.182:12281/ReceiptNotify/PayNotify",
            TDCfg = new TDConfig
            {
                Url = "https://ali-sh-datareceiver.kurogame.xyz",
                AppID = "bf3f44edc6cf43c582e347ba660876c0"
            },
            LogReport = new LogReportConfig
            {
                Ak = "AKIDseIrMkz66ymcSBrjpocFt9IO0lT1SiIk",
                Sk = "MXeeVBfs0ywnleS83xiGczCPVROCnFds",
                Name = "ap-singapore",
                Region = "hw-aki-upload-log-1319073642"
            },

           // GARUrl = "https://gar-service.aki-game.net"
            NoticUrl = "https://prod-alicdn-gmserver-static.kurogame.com",
            LoginServers = [
                new LoginServerEntry
                {
                    Id = "1074",
                    Name = "Censerver",
                    Ip = "127.0.0.1"
                }
            ],
            PrivateServers = new PrivateServersConfig
            {
                Enable = false,
                ServerUrl = ""
            }
        });
    }
}
