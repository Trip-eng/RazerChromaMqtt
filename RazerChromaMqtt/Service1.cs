using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using ColoreColor = Colore.Data.Color;
using MqttLib;
using Colore;
using Colore.Effects.Keyboard;

namespace RazerChromaMqtt
{
    public partial class Service1 : ServiceBase
    {
        ILog log;
        IChroma chroma;
        IMqtt mqttC;

        string mqttHost, mqttUser, mqttPw, mqttPreTopic;


        public Service1()
        {
            InitializeComponent();

            //Logger
            XmlConfigurator.Configure(new FileInfo(ConfigurationManager.AppSettings["logConfig"]));


            // User Settings
            var userSetting = Properties.Settings.Default;
            log = LogManager.GetLogger("RazerChromaMqtt");
            mqttHost = userSetting.MqttIP + ":" + userSetting.MqttPort;
            mqttUser = userSetting.MqttUser;
            mqttPw = userSetting.MqttPw;
            if (mqttUser == "" || mqttPw == "") mqttUser = mqttPw = null;
            mqttPreTopic = userSetting.MqttPreTopic;

            mqttC = MqttClientFactory.CreateClient("tcp://" + mqttHost, "chroma", mqttUser, mqttPw);
            mqttC.Connected += MqttC_Connected;
            mqttC.ConnectionLost += MqttC_ConnectionLost;
            mqttC.PublishArrived += MqttC_PublishArrived;
        }


        public void OnDebugStart()
        {
            OnStart(null);
        }

        public void OnDebugStop()
        {
            OnStop();
        }

        protected override void OnStart(string[] args)
        {
            log.Debug("start");

            Task<IChroma> task = ColoreProvider.CreateNativeAsync();
            task.Wait();

            chroma = task.Result;
            try
            {
                mqttC.Connect(mqttPreTopic + "state", QoS.BestEfforts, new MqttPayload("offline"), false);

            }
            catch (Exception e)
            {
                log.Error("Mqtt connect eror : " + e.Message);
                base.Stop();
            }
        }


        private void MqttC_Connected(object sender, EventArgs e)
        {
            log.Info("Mqtt Connect");
            mqttC.Subscribe(mqttPreTopic + "set/#", QoS.BestEfforts);
            mqttC.Publish(mqttPreTopic + "state", new MqttPayload("online"), QoS.BestEfforts, false);
        }

        private void MqttC_ConnectionLost(object sender, EventArgs e)
        {
            log.Info("MqttC ConnectionLost");
        }

        private bool MqttC_PublishArrived(object sender, PublishArrivedArgs e)
        {
            string[] topic = e.Topic.Substring(mqttPreTopic.Length).Split('/');
            string payload = e.Payload;
            ColoreColor c;
            try
            {
                c = HexToColore(payload);
            }
            catch
            {
                log.Error(payload + " is no valid color");
                return false;
            }



            if (topic.Length >= 1)
                switch (topic[0])
                {
                    case "set":
                        if (topic.Length >= 2)
                        {
                            switch (topic[1])
                            {
                                case "all":
                                    chroma.SetAllAsync(c);
                                    break;
                                case "mouse":
                                    chroma.Mouse.SetAllAsync(c);
                                    break;
                                case "keyboard":
                                    switch (topic.Length)
                                    {
                                        case 2:
                                            chroma.Keyboard.SetAllAsync(c);
                                            break;
                                        case 3:
                                            Key key;
                                            if (Enum.TryParse(topic[2], out key))
                                            {
                                                chroma.Keyboard[key] = c;
                                            }
                                            else
                                            {
                                                log.Error(topic[2] + " is no valid key");
                                                return false;
                                            }
                                            break;

                                        default:
                                            return false;
                                    }
                                    break;
                                default:
                                    return false;

                            }
                            string retTopic = "state/";
                            for (int i = 1; i < topic.Length; i++)
                            {
                                retTopic += topic[i] + "/";
                            }
                            mqttC.Publish(mqttPreTopic + retTopic, new MqttPayload(ColoreToHex(c)), QoS.BestEfforts, false);
                        }
                        break;
                    default:

                        break;
                }

            return true;
        }

        protected override void OnStop()
        {
#if DEBUG
            Environment.Exit(0); 
#endif  
        }

        #region Helper 

        ColoreColor HexToColore(string hex)
        {

            int ui = int.Parse(hex.Substring(1), System.Globalization.NumberStyles.HexNumber);

            byte rb = (byte)((ui >> 16) & 0xFF);

            byte gb = (byte)((ui >> 8) & 0xFF);

            byte bb = (byte)((ui >> 0) & 0xFF);

            ColoreColor c = new ColoreColor(rb, gb, bb);

            return c;

        }
        string ColoreToHex(ColoreColor colore)
        {
            return $"#{colore.R:X2}{colore.G:X2}{colore.B:X2}";

        }

        #endregion
    }
}
