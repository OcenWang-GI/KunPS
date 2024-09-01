using Microsoft.AspNetCore.Http.HttpResults;

namespace SDKServer.Handlers;

internal static class HotPatchHandler
{

    //由于部分涉及wwps1.1key 所以不能公开
    public static string OnConfigRequest() => "{\n    \"PackageVersion\": \"1.3.0\",\n    \"LauncherVersion\": \"1.3.6\",\n    \"ResourceVersion\": \"1.3.6\",\n    \"LauncherIndexSha1\": {\n        \"1.3.1\": \"90FDF17EA0B4015D43C344CB7229E76AB32549DD\",\n        \"1.3.2\": \"C9A587AB1FA6CA57CD23E0FB3F0103BFDCAA8E37\",\n        \"1.3.3\": \"1C7AF02F13DBE69637DB43039E2FFB8C9AD9A04B\",\n        \"1.3.4\": \"DA50F315041E216568A7713074C6475F6AB4530E\",\n        \"1.3.5\": \"EA9C6F6D5E920F47F96D8F8BC366A4CED62A0346\",\n        \"1.3.6\": \"8CA7E6573A52B16CFAA29E996D389918B6829E7A\"\n    },\n    \"ResourceIndexSha1\": {\n        \"1.3.1\": \"2D635E549EB6F99659571D72741B62249473A77A\",\n        \"1.3.2\": \"C5814A80EA3E7D80D4CFBCD884D1FD158BF0AD9D\",\n        \"1.3.3\": \"1E0F05333B09A9215B4AA5C437BFC7DC4014E348\",\n        \"1.3.4\": \"6155D492540A99ECF0DA06D2B7EEBFE36231FBC2\",\n        \"1.3.5\": \"1E60C8F60CA1AAA9955441B4F4265C8288B95F33\",\n        \"1.3.6\": \"AA10A8DD1025D5033E291060C686B816513ADCAD\"\n    },\n    \"ChangeList\": \"2262454\",\n    \"CompatibleChangeLists\": [],\n    \"Versions\": [\n        {\n            \"Name\": \"en\",\n            \"Version\": \"1.3.0\",\n            \"IndexSha1\": {\n                \"1.3.0\": \"6FB5B66EF8B3EECBBBEBE74A82BC23E3FC35450B\"\n            }\n        },\n        {\n            \"Name\": \"ja\",\n            \"Version\": \"1.3.0\",\n            \"IndexSha1\": {\n                \"1.3.0\": \"E4DA1960DB36CE8166C042AD8B9AF98C1A9119F3\"\n            }\n        },\n        {\n            \"Name\": \"ko\",\n            \"Version\": \"1.3.0\",\n            \"IndexSha1\": {\n                \"1.3.0\": \"498B379E95FC617385CCD832B8C359FA5AC220CE\"\n            }\n        },\n        {\n            \"Name\": \"zh\",\n            \"Version\": \"1.3.0\",\n            \"IndexSha1\": {\n                \"1.3.0\": \"CC58C357A80E7B3846264918197FC3ECAA1FE190\"\n            }\n        }\n    ],\n    \"UpdateTime\": 1725002861\n}";



//If it exceeds the time limit, please modify it
    public static string OnKeyListRequest() => "{\r\n    \"Hash\": \"    Hash    \",\r\n    \"Random\": \" Random       \",\r\n    \"Key\": \"key        \"\r\n}";



    public static FileContentHttpResult OnPakDataRequest() 
        => TypedResults.File([]);
        
}
