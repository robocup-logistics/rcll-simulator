using MQTTnet;
using MQTTnet.Client;

namespace Simulator.MPS;

public class MQTThelper
{
    public async Task TestPublish()
    {
        var mqttclient = new MqttFactory();
        using (var mqttClient = mqttclient.CreateMqttClient())
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost")
                .Build();
            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
            var name = "C-BS";
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic("MPS/"+ name + "/enabled")
                .WithPayload("true")
                .Build();

            await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

            var mqttSubscribeOptions = mqttclient.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("MPS/" + name + "/enabled");
                    })
                .Build();
            var response = await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

            Console.WriteLine("MQTT client subscribed to topic.");

            Thread.Sleep(60000);

            await mqttClient.DisconnectAsync();

            Console.WriteLine("MQTT application message is published.");
        }
    }
}