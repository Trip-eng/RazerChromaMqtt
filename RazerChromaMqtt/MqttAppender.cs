using log4net.Appender;
using log4net.Core;
using MqttLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazerChromaMqtt
{

    public class MqttAppender : AppenderSkeleton
    {
        public static IMqtt mqttClient;
        public static string mqttPreTopic;
        protected override void Append(LoggingEvent loggingEvent)
        {
            if(mqttClient != null && mqttClient.IsConnected && mqttPreTopic != null)
            {
                mqttClient.Publish(mqttPreTopic + loggingEvent.Level.Name, new MqttPayload(loggingEvent.RenderedMessage), QoS.BestEfforts, false);
            }
        }
    }
}
