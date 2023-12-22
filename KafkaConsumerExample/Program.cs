using Confluent.Kafka;

string groupId = args.Length > 0 ? args[0] : "default-group";

// this can be loaded externally
/*
services.Configure<ConsumerConfig> (
    hostContext.Configuration.GetSection("Consumer")
);
*/

var config = new ConsumerConfig
{ 
    BootstrapServers = "localhost:9092",
    ClientId = "ConsumerExample",
    GroupId = groupId,
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = true
    // Authentication methods...
};

// Note: in this example, I am just sending over a double. However, if it was a complex
// type, you would want to deserialize it with json

/*var consumer = new ConsumerBuilder<string, Biometrics>(config).SetValueDeserializer(
        new JsonDeserializer<Biometrics>().AsSyncOverAsync()
    )
    .Build();*/

using var c = new ConsumerBuilder<string, double>(config).Build();

// subscribe to the topic
c.Subscribe("myorders");

CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

var cancellationToken = cancellationTokenSource.Token;

Console.CancelKeyPress += (_, e) => {
    e.Cancel = true;
    cancellationTokenSource.Cancel();
};

try
{
    while (!cancellationToken.IsCancellationRequested)
    {
        try
        {
            var cr = c.Consume(cancellationToken);
            Console.WriteLine($"Received message with key: {cr.Message.Key} and value: {cr.Message.Value}");
            Console.WriteLine($"It comes from partition: {cr.Partition}");
            
            Thread.Sleep(1000);
        }
        catch (ConsumeException e)
        {
            Console.WriteLine($"Error occurred: {e.Error.Reason}");
        }
    }
}
catch (OperationCanceledException)
{
    c.Close();
}