using Confluent.Kafka;

var config = new ProducerConfig { BootstrapServers = "localhost:9092,localhost:9093,localhost:9094" };
var producer = new ProducerBuilder<string, double>(config).Build();

var stateString = "AL,AK,AZ,AR,CA,CO,CT,DE,FL,GA," +
                  "HI,ID,IL,IN,IA,KS,KY,LA,ME,MD,MA,MI,MN,MS,MO," +
                  "MT,NE,NV,NH,NJ,NM,NY,NC,ND,OH,OK,OR,PA,RI,SC," +
                  "SD,TN,TX,UT,VT,VA,WA,WV,WI,WY";

var stateArray = stateString.Split(',');
string topic = "myorders";

Random rand = new Random();

try
{
    for (int i = 0; i < 25000; i++)
    {
        int randomIndex = rand.Next(stateArray.Length);
        string randomState = stateArray[randomIndex];
        double value = Math.Floor(rand.NextDouble() * (10000 - 10 + 1) + 10);
        
        Message<string, double> message = new()
        {
            Key = randomState,
            Value = value
        };
        
        DeliveryResult<string, double> result = await producer.ProduceAsync(topic, message);
        
        Console.WriteLine($"State: {randomState}, Value: {value}, Topic:{result.Topic} Partition: {result.Partition} Offset: {result.Offset}");
        Thread.Sleep(5000);
    }
}
catch (ProduceException<string, double> e)
{
    // Log and handle appropriately
    Console.WriteLine($"Produce error: {e.Error.Reason}");
}
finally
{
    producer.Flush(TimeSpan.FromSeconds(10));
}