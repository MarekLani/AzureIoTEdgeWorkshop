using System;
using System.IO;
using MQTTnet.Client;
using MQTTnet;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace SimulatedCameraDevice
{
    class Program
    {
        static async Task Main(string[] args)
        {

            byte[] content;
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
             var options = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1", 1888) // Port is optional
                .Build();
            

                await mqttClient.ConnectAsync(options);

            while(true){
                Console.WriteLine("If you want to send penguin picture, please type 1 and press enter otherwise type 2");
                var option = Console.ReadLine();
                if(option == "1")
                {
                    Console.WriteLine("Please write number of image you want to send from 1-3?");
                    var number = Console.ReadLine();
                   //Send penguin picture     
                   content = File.ReadAllBytes($"Pictures\\penguin{number}.jpg");
                }
                else
                {
                    //Send not penguin picture
                    content = File.ReadAllBytes("Pictures\\nopenguin.jpg");
                }

               

                Random r = new Random();
                try{
                

                    //Send message with temperature info every two seconds
                    
                    //      HttpResponseMessage response = await SendRequestToML(content);
                    // JObject jobject = JObject.Parse(await response.Content.ReadAsStringAsync());
                    // Console.WriteLine(await response.Content.ReadAsStringAsync());
                    // if (jobject.GetValue("predictions")[0]["tagName"].ToString() == "penguin" && Convert.ToDouble(jobject.GetValue("predictions")[0]["probability"]) > 0.5)
                    //     Console.WriteLine(jobject.GetValue("predictions")[0]["tagName"]);
                        var message = new MqttApplicationMessageBuilder()
                        .WithTopic("MyTopic")
                        .WithPayload(Convert.ToBase64String(content))
                        .WithExactlyOnceQoS()
                        .WithRetainFlag()
                        .Build();
                        await mqttClient.PublishAsync(message);
                        
                
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                };

            }
        }

         static  async Task<HttpResponseMessage> SendRequestToML(byte[] content)
        {
            using (var formDataContent = new MultipartFormDataContent())
            {
                formDataContent.Add(new ByteArrayContent(content), "imageData", "photo");
               
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.PostAsync("http://localhost:8085/image", formDataContent);
                    return response;
                }
            }
        }

    }
}
