using Google.Protobuf;
using Protocol;

namespace TrafficAnalyzer
{
    internal static class Program
    {
        private const int StdInBufferSize = 65535;
        private const string ProtoAssemblyName = "Protocol";
        private const string ProtoNamespace = "Protocol";
        private const string MessageParserPropertyName = "Parser";

        private static readonly DumpOptions s_objectDumperOpts = new()
        {
            DumpStyle = DumpStyle.CSharp,
            IndentSize = 4,
            IndentChar = ' ',
            IgnoreDefaultValues = true
        };

        private static void Main(string[] args)
        {
            // Set up standard input for larger buffers
            Console.SetIn(new StreamReader(Console.OpenStandardInput(StdInBufferSize), Console.InputEncoding, false, StdInBufferSize));

            var inputMessages = new List<Tuple<int, byte[]>>();

            string? messageIdInput;
            string? payloadInput;

            // Read messages from standard input
            while (!string.IsNullOrEmpty(messageIdInput = Console.ReadLine()) && (payloadInput = Console.ReadLine()) != null)
            {
                int messageId = int.Parse(messageIdInput);
                byte[] payload = Convert.FromHexString(payloadInput);

                inputMessages.Add(Tuple.Create(messageId, payload));
            }

            // Process each message
            foreach (var (messageId, payload) in inputMessages)
            {
                string messageName = ((MessageId)messageId).ToString();

                Type? messageType = Type.GetType($"{ProtoNamespace}.{messageName},{ProtoAssemblyName}");
                if (messageType is null)
                {
                    Console.WriteLine($"Message with ID {messageName} was not found in the protobuf definition.");
                    continue;
                }

                try
                {
                    // Retrieve the message parser and parse the message
                    var messageParser = (MessageParser)messageType.GetProperty(MessageParserPropertyName)!.GetValue(null)!;
                    IMessage message = messageParser.ParseFrom(payload);

                    // Dump the message content
                    string outputInitializer = ObjectDumper.Dump(message, s_objectDumperOpts);

                    Console.WriteLine($"Message: {messageName}");
                    Console.WriteLine(outputInitializer);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to process message with ID {messageName}: {ex.Message}");
                }
            }
        }
    }
}