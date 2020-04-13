using System;
using System.ServiceProcess;
using System.Threading.Tasks;
using log4net;
using ColoreColor = Colore.Data.Color;
using MqttLib;
using Colore;
using Colore.Effects.Keyboard;
using System.Xml;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace RazerChromaMqtt
{
    public partial class Service1 : ServiceBase
    {
        ILog log;
        IChroma chroma;
        IMqtt mqttClient;

        string mqttHost, mqttPort, mqttUser, mqttPw, mqttPreTopic;


        public Service1()
        {
            InitializeComponent();

            //Logger
            log = LogManager.GetLogger("RazerChromaMqtt");

        }


        public void OnDebugStart()
        {
            init();
        }

        public void OnDebugStop()
        {
            OnStop();
        }


        void init()
        {
            // User Settings
            
            XmlDocument xmlConfig = new XmlDocument();
            xmlConfig.Load(AppDomain.CurrentDomain.BaseDirectory + "config.xml");
            var mqttConfig = xmlConfig["RazerChromaMqtt"]["MQTT"];
            var v = mqttConfig["MqttUser"].InnerText.Trim();
            mqttHost = mqttConfig["MqttHost"].InnerText.Trim();
            mqttPort = mqttConfig["MqttPort"].InnerText.Trim();
            mqttUser = mqttConfig["MqttUser"].InnerText.Trim();
            mqttPw = mqttConfig["MqttPw"].InnerText.Trim();
            mqttPreTopic = mqttConfig["MqttPreTopic"].InnerText.Trim();
            
            if (mqttUser == "" || mqttPw == "") mqttUser = mqttPw = null;


            // MQTT 
            mqttClient = MqttClientFactory.CreateClient($"tcp://{mqttHost}:{mqttPort}", "chroma", mqttUser, mqttPw);
            mqttClient.Connected += MqttC_Connected;
            mqttClient.ConnectionLost += MqttC_ConnectionLost;
            mqttClient.PublishArrived += MqttC_PublishArrived;

            MqttAppender.mqttClient = mqttClient;
            MqttAppender.mqttPreTopic = mqttPreTopic;
            
            try
                {
                    mqttClient.Connect(mqttPreTopic + "state", QoS.BestEfforts, new MqttPayload("offline"), false);

                }
                catch (Exception e)
                {
                    log.Error("Mqtt connect eror : " + e.Message);
                    base.Stop();
                }
            

            Task<IChroma> connectChroma = ColoreProvider.CreateNativeAsync();
            connectChroma.Wait();
            chroma = connectChroma.Result;
        }
        protected override void OnStart(string[] args)
        {
            log.Info("Service Start");
            init();
        }


        private void MqttC_Connected(object sender, EventArgs e)
        {
            log.Info("Mqtt Connect");
            mqttClient.Subscribe(mqttPreTopic + "set/#", QoS.BestEfforts);
            mqttClient.Publish(mqttPreTopic + "state", new MqttPayload("online"), QoS.BestEfforts, false);
        }

        private void MqttC_ConnectionLost(object sender, EventArgs e)
        {
            log.Warn("MqttC ConnectionLost");
        }

        private bool MqttC_PublishArrived(object sender, PublishArrivedArgs e)
        {
            log.Debug("Mqtt Rx : " + e.Topic + " : " + e.Payload);
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
                                                    log.Error("Key is not specified ");
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
                                                    log.Error("Position is not specified ");
                                                    return false;
                                                }
                                                int row, column;
                                                if(!int.TryParse(topic[3],out row))
                                                {
                                                    log.Error($"\"{topic[3]}\" NaN");
                                                    return false;
                                                }
                                                if (!int.TryParse(topic[4], out column))
                                                {
                                                    log.Error($"\"{topic[4]}\" NaN");
                                                    return false;
                                                }
                                                if (row < 0 || row >= KeyboardConstants.MaxRows)
                                                {
                                                    log.Error($"\"{row}\" out of range. Max:" + (KeyboardConstants.MaxRows - 1));
                                                    return false;
                                                }
                                                if (column < 0 || column >= KeyboardConstants.MaxColumns)
                                                {
                                                    log.Error($"\"{column}\" out of range. Max:" + (KeyboardConstants.MaxColumns - 1));
                                                    return false;
                                                }
                                                chroma.Keyboard[row, column] = c;
                                                break;
                                            case "zone":
                                                if (topic.Length < 4)
                                                {
                                                    log.Error("Zone is not specified ");
                                                    return false;
                                                }
                                                int zone;
                                                if (!int.TryParse(topic[3], out zone))
                                                {
                                                    log.Error($"\"{topic[3]}\" NaN");
                                                    return false;
                                                }
                                                if (zone < 0 || zone >= KeyboardConstants.MaxDeathstalkerZones)
                                                {
                                                    log.Error($"\"{zone}\" out of range. Max:" + (KeyboardConstants.MaxDeathstalkerZones - 1));
                                                    return false;
                                                }
                                                chroma.Keyboard[zone] = c;
                                                break;
                                            default:
                                                log.Error($"\"{topic[2]}\" unknown");
                                                return false;
                                        }
                                    }
                                    else
                                    {
                                        log.Error("no specification (\"all\", \"key\", \"grid\", \"zone\")");
                                        return false;
                                    }
                                    break;
                                case "keypad":
                                    if (topic.Length >= 3)
                                    {
                                        switch (topic[2].ToLower())
                                        {
                                            case "all":
                                                chroma.Keypad.SetAllAsync(c);
                                                break;
                                            case "grid":
                                                if (topic.Length < 5)
                                                {
                                                    log.Error("Position is not specified ");
                                                    return false;
                                                }
                                                int row, column;
                                                if (!int.TryParse(topic[3], out row))
                                                {
                                                    log.Error($"\"{topic[3]}\" NaN");
                                                    return false;
                                                }
                                                if (!int.TryParse(topic[4], out column))
                                                {
                                                    log.Error($"\"{topic[4]}\" NaN");
                                                    return false;
                                                }
                                                if (row < 0 || row >= Colore.Effects.Keypad.KeypadConstants .MaxRows)
                                                {
                                                    log.Error($"\"{row}\" out of range. Max:" + (Colore.Effects.Keypad.KeypadConstants.MaxRows - 1));
                                                    return false;
                                                }
                                                if (column < 0 || column >= Colore.Effects.Keypad.KeypadConstants.MaxColumns)
                                                {
                                                    log.Error($"\"{column}\" out of range. Max:" + (Colore.Effects.Keypad.KeypadConstants.MaxColumns - 1));
                                                    return false;
                                                }
                                                chroma.Keypad[row, column] = c;
                                                break;
                                            default:
                                                log.Error($"\"{topic[2]}\" unknown");
                                                return false;
                                        }

                                    }
                                    else
                                    {
                                        log.Error("no specification (\"all\", \"grid\")");
                                        return false;
                                    }
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
                                                    log.Error("Key is not specified ");
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
                                                    log.Error("Position is not specified ");
                                                    return false;
                                                }
                                                int row, column;
                                                if (!int.TryParse(topic[3], out row))
                                                {
                                                    log.Error($"\"{topic[3]}\" NaN");
                                                    return false;
                                                }
                                                if (!int.TryParse(topic[4], out column))
                                                {
                                                    log.Error($"\"{topic[4]}\" NaN");
                                                    return false;
                                                }
                                                if (row < 0 || row >= Colore.Effects.Mouse.MouseConstants.MaxRows)
                                                {
                                                    log.Error($"\"{row}\" out of range. Max:" + (Colore.Effects.Mouse.MouseConstants.MaxRows - 1));
                                                    return false;
                                                }
                                                if (column < 0 || column >= Colore.Effects.Mouse.MouseConstants.MaxColumns)
                                                {
                                                    log.Error($"\"{column}\" out of range. Max:" + (Colore.Effects.Mouse.MouseConstants.MaxColumns - 1));
                                                    return false;
                                                }
                                                chroma.Mouse[row, column] = c;
                                                break;
                                            default:
                                                log.Error($"\"{topic[2]}\" unknown");
                                                return false;
                                        }

                                    }
                                    else
                                    {
                                        log.Error("no specification (\"all\", \"gridled\", \"grid\")");
                                        return false;
                                    }
                                    break;
                                case "mousepad":
                                    if (topic.Length >= 3)
                                    {
                                        switch (topic[2].ToLower())
                                        {
                                            case "all":
                                                chroma.Mousepad.SetAllAsync(c);
                                                break;
                                            case "index":
                                                if (topic.Length < 4)
                                                {
                                                    log.Error("Index is not specified ");
                                                    return false;
                                                }
                                                int index;
                                                if (!int.TryParse(topic[3], out index))
                                                {
                                                    log.Error($"\"{topic[3]}\" NaN");
                                                    return false;
                                                }
                                                if (index < 0 || index >= Colore.Effects.Mousepad.MousepadConstants.MaxLeds)
                                                {
                                                    log.Error($"\"{index}\" out of range. Max:" + (Colore.Effects.Mousepad.MousepadConstants.MaxLeds-1));
                                                    return false;
                                                }
                                                chroma.Mousepad[index] = c;
                                                break;
                                            default:
                                                log.Error($"\"{topic[2]}\" unknown");
                                                return false;
                                        }

                                    }
                                    else
                                    {
                                        log.Error("no specification (\"all\", \"index\")");
                                        return false;
                                    }
                                    break;
                                case "headset":
                                    if (topic.Length >= 3)
                                    {
                                        switch (topic[2].ToLower())
                                        {
                                            case "all":
                                                chroma.Headset.SetAllAsync(c);
                                                break;
                                            case "index":
                                                if (topic.Length < 4)
                                                {
                                                    log.Error("Index is not specified ");
                                                    return false;
                                                }
                                                int index;
                                                if (!int.TryParse(topic[3], out index))
                                                {
                                                    log.Error($"\"{topic[3]}\" NaN");
                                                    return false;
                                                }
                                                if (index < 0 || index >= Colore.Effects.Headset.HeadsetConstants.MaxLeds)
                                                {
                                                    log.Error($"\"{index}\" out of range. Max:" + (Colore.Effects.Headset.HeadsetConstants.MaxLeds - 1));
                                                    return false;
                                                }
                                                chroma.Headset[index] = c;
                                                break;
                                            default:
                                                log.Error($"\"{topic[2]}\" unknown");
                                                return false;
                                        }
                                    }
                                    else
                                    {
                                        log.Error("no specification (\"all\", \"index\")");
                                        return false;
                                    }
                                    break;
                                case "link":
                                    if(topic.Length >= 3)
                                    {
                                        switch (topic[2])
                                        {
                                            case "all":
                                                chroma.ChromaLink.SetAllAsync(c);
                                                break;
                                            case "index":
                                                if (topic.Length < 4)
                                                {
                                                    log.Error("Index is not specified ");
                                                    return false;
                                                }
                                                int index;
                                                if (!int.TryParse(topic[3], out index))
                                                {
                                                    log.Error($"\"{topic[3]}\" NaN");
                                                    return false;
                                                }
                                                if (index < 0 || index >= Colore.Effects.ChromaLink.ChromaLinkConstants.MaxLeds)
                                                {
                                                    log.Error($"\"{index}\" out of range. Max:" + (Colore.Effects.ChromaLink.ChromaLinkConstants.MaxLeds - 1));
                                                    return false;
                                                }
                                                chroma.ChromaLink[index] = c;
                                                break;
                                            default:
                                                log.Error($"\"{topic[2]}\" unknown");
                                                return false;
                                        }
                                    }
                                    else
                                    {
                                        log.Error("no specification (\"all\", \"index\")");
                                        return false;
                                    }
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
            if(mqttClient.IsConnected) mqttClient.Disconnect();

#if DEBUG
            Environment.Exit(0);
#else
            log.Info("Service Stop");
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
