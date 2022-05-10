using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TestProducer1.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace TestProducer1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {

        [HttpPost]
        public void Post([FromBody] Models.Task gettask)
        {
            string responseInString = "hi";
            using (var wb = new WebClient())
            {
                var data = new NameValueCollection();
                data["email"] = "eve.holt@reqres.in";
                data["password"] = "pistol";
                data["task"] = "any Task";
               
                var response = wb.UploadValues("https://reqres.in/api/register", "POST", data);
                responseInString = Encoding.UTF8.GetString(response);
                Console.WriteLine(responseInString);
            }

            var factory = new ConnectionFactory()
            {
                //HostName = "localhost" , 
                //Port = 30724
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
                Port = Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT"))
            };

            Console.WriteLine(factory.HostName + ":" + factory.Port);
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "TaskQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                channel.QueueDeclare(queue: "TokenQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = gettask.task;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "TaskQueue",
                                     basicProperties: null,
                                     body: body);


                channel.QueueDeclare(queue: "TokenQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var res_body = Encoding.UTF8.GetBytes(responseInString);
                channel.BasicPublish(exchange: "",
                                     routingKey: "TokenQueue",
                                     basicProperties: null,
                                     body: res_body);


            }
        }
    }
}
