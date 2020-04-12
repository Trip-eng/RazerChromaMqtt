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
using System.Threading;

namespace RazerChromaMqtt
{
    public partial class Service1 : ServiceBase
    {
        ILog log;
        IChroma chroma;
        IMqtt mqttClient;

        string mqttHost, mqttUser, mqttPw, mqttPreTopic;


        public Service1()
        {
            InitializeComponent();

            //Logger
            XmlConfigurator.Configure(new FileInfo(ConfigurationManager.AppSettings["logConfig"]));
            log = LogManager.GetLogger("RazerChromaMqtt");
            


            // User Settings
            var userSetting = Properties.Settings.Default;
            mqttHost = userSetting.MqttIP + ":" + userSetting.MqttPort;
            mqttUser = userSetting.MqttUser;
            mqttPw = userSetting.MqttPw;
            if (mqttUser == "" || mqttPw == "") mqttUser = mqttPw = null;
            mqttPreTopic = userSetting.MqttPreTopic;

            mqttClient = MqttClientFactory.CreateClient("tcp://" + mqttHost, "chroma", mqttUser, mqttPw);
            mqttClient.Connected += MqttC_Connected;
            mqttClient.ConnectionLost += MqttC_ConnectionLost;
            mqttClient.PublishArrived += MqttC_PublishArrived;

            MqttAppender.mqttClient = mqttClient;
            MqttAppender.mqttPreTopic = mqttPreTopic;
            
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
                mqttClient.Connect(mqttPreTopic + "state", QoS.BestEfforts, new MqttPayload("offline"), false);

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
            mqttClient.Subscribe(mqttPreTopic + "set/#", QoS.BestEfforts);
            mqttClient.Publish(mqttPreTopic + "state", new MqttPayload("online"), QoS.BestEfforts, false);
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
                switch (topic[0].ToLower())
                {
                    case "set":
                        if (topic.Length >= 2)
                        {
                            switch (topic[1].ToLower())
                            {
                                case "all":
                                    chroma.SetAllAsync(c);
                                    break;
                                case "keyboard":
                                    if (topic.Length >= 3)
                                    {
                                        switch (topic[2].ToLower())
                                        {
                                            case "all":
                                                chroma.Keyboard.SetAllAsync(c);
                                                break;
                                            case "key":
                                                if (topic.Length < 4)
                                                {
                                                    log.Error("Key is no specified ");
                                                    return false;
                                                }
                                                Key key;
                                                if (Enum.TryParse(topic[3], out key))
                                                {
                                                    chroma.Keyboard[key] = c;
                                                }
                                                else
                                                {
                                                    log.Error($"\"{topic[3]}\" is no valid key");
                                                    return false;
                                                }
                                                break;
                                            case "grid":
                                                if (topic.Length < 5)
                                                {
                                                    log.Error("Position is no specified ");
                                                    return false;
                                                }
                                                int row, column;
                                                if(!int.TryParse(topic[3],out row))
                                                {
                                                    log.Error($"\"{topic[3]}\" nan");
                                                    return false;
                                                }
                                                if (!int.TryParse(topic[4], out column))
                                                {
                                                    log.Error($"\"{topic[4]}\" nan");
                                                    return false;
                                                }
                                                if (row >= KeyboardConstants.MaxRows)
                                                {
                                                    log.Error($"\"{row}\" out of range. Max:" + KeyboardConstants.MaxRows);
                                                    return false;
                                                }
                                                if (column >= KeyboardConstants.MaxColumns)
                                                {
                                                    log.Error($"\"{column}\" out of range. Max:" + KeyboardConstants.MaxColumns);
                                                    return false;
                                                }
                                                chroma.Keyboard[row, column] = c;
                                                break;
                                            case "zone":
                                                if (topic.Length < 4)
                                                {
                                                    log.Error("Zone is no specified ");
                                                    return false;
                                                }
                                                int zone;
                                                if (!int.TryParse(topic[3], out zone))
                                                {
                                                    log.Error($"\"{topic[3]}\" nan");
                                                    return false;
                                                }
                                                if (zone >= KeyboardConstants.MaxDeathstalkerZones)
                                                {
                                                    log.Error($"\"{zone}\" out of range. Max:" + KeyboardConstants.MaxDeathstalkerZones);
                                                    return false;
                                                }
                                                chroma.Keyboard[zone] = c;
                                                break;
                                            default:
                                                log.Error($"\"{topic[2]}\" unknown");
                                                return false;
                                        }
                                        
                                    }
                                    break;
                                case "keypad":
                                    chroma.Keypad.SetAllAsync(c);
                                    break;
                                case "mouse":
                                    if (topic.Length >= 3)
                                    {
                                        switch (topic[2].ToLower())
                                        {
                                            case "all":
                                                chroma.Mouse.SetAllAsync(c);
                                                break;
                                            case "gridled":
                                                if (topic.Length < 4)
                                                {
                                                    log.Error("Key is no specified ");
                                                    return false;
                                                }
                                                Colore.Effects.Mouse.GridLed gridLed;
                                                if (Enum.TryParse(topic[3], out gridLed))
                                                {
                                                    chroma.Mouse[gridLed] = c;
                                                }
                                                else
                                                {
                                                    log.Error($"\"{topic[3]}\" is no valid GridLed");
                                                    return false;
                                                }
                                                break;
                                            case "grid":
                                                if (topic.Length < 5)
                                                {
                                                    log.Error("Position is no specified ");
                                                    return false;
                                                }
                                                int row, column;
                                                if (!int.TryParse(topic[3], out row))
                                                {
                                                    log.Error($"\"{topic[3]}\" nan");
                                                    return false;
                                                }
                                                if (!int.TryParse(topic[4], out column))
                                                {
                                                    log.Error($"\"{topic[4]}\" nan");
                                                    return false;
                                                }
                                                if (row >= Colore.Effects.Mouse.MouseConstants.MaxRows)
                                                {
                                                    log.Error($"\"{row}\" out of range. Max:" + Colore.Effects.Mouse.MouseConstants.MaxRows);
                                                    return false;
                                                }
                                                if (column >= Colore.Effects.Mouse.MouseConstants.MaxColumns)
                                                {
                                                    log.Error($"\"{column}\" out of range. Max:" + Colore.Effects.Mouse.MouseConstants.MaxColumns);
                                                    return false;
                                                }
                                                chroma.Mouse[row, column] = c;
                                                break;
                                            default:
                                                log.Error($"\"{topic[2]}\" unknown");
                                                return false;
                                        }

                                    }
                                    break;
                                case "mousepad":
                                    chroma.Mousepad.SetAllAsync(c);
                                    break;
                                case "headset":
                                    chroma.Headset.SetAllAsync(c);
                                    break;
                                default:
                                    log.Error($"\"{topic[1]}\" unknown");
                                    return false;

                            }
                            string retTopic = "state/";
                            for (int i = 1; i < topic.Length; i++)
                            {
                                retTopic += topic[i] + "/";
                            }
                            mqttClient.Publish(mqttPreTopic + retTopic, new MqttPayload(ColoreToHex(c)), QoS.BestEfforts, false);
                        }
                        break;
                    default:
                        log.Error($"\"{topic[0]}\" unknown");
                        return false;
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
