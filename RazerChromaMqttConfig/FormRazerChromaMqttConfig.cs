using System;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Xml;

namespace RazerChromaMqttConfig
{
    public partial class FormRazerChromaMqttConfig : Form
    {
        ServiceController _serviceController;
        ServiceController serviceController
        {
            get
            {
                _serviceController.Refresh();
                return _serviceController;
            }
            set
            {
                _serviceController = value;
            }
        }
        XmlDocument xmlConfig;
        XmlElement mqttConfig;

        string ConfigFile = "config.xml";
        public FormRazerChromaMqttConfig()
        {

            InitializeComponent();


            serviceController = new ServiceController("RazerChromaMqtt");

            xmlConfig = new XmlDocument();

            if (System.IO.File.Exists(ConfigFile))
            {
                xmlConfig.Load(ConfigFile);
            }
            structure(xmlConfig);

            mqttConfig = xmlConfig["RazerChromaMqtt"]["MQTT"];
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            updateTbStatus();
            updateTimer.Start();
            if (mqttConfig["MqttHost"].InnerText.Trim() != "") tbMqttHost.Text = mqttConfig["MqttHost"].InnerText.Trim();
            if (mqttConfig["MqttPort"].InnerText.Trim() != "") tbMqttPort.Text = mqttConfig["MqttPort"].InnerText.Trim();
            if (mqttConfig["MqttUser"].InnerText.Trim() != "") tbMqttUser.Text = mqttConfig["MqttUser"].InnerText.Trim();
            if (mqttConfig["MqttPw"].InnerText.Trim() != "") tbMqttPW.Text = mqttConfig["MqttPw"].InnerText.Trim();
            if (mqttConfig["MqttPreTopic"].InnerText.Trim() != "") tbMqttPreTopic.Text = mqttConfig["MqttPreTopic"].InnerText.Trim();
        }


        void structure(XmlDocument xml)
        {
            if (xml["RazerChromaMqtt"] == null) xml.AppendChild(xml.CreateNode(XmlNodeType.Element, "RazerChromaMqtt", null));
            XmlElement xmlElement = xml["RazerChromaMqtt"];

            foreach (string item in new string[] { "MQTT" })
            {
                    if (xmlElement[item] == null) xmlElement.AppendChild(xml.CreateNode(XmlNodeType.Element, item, null));
                    xmlElement = xmlElement[item];
            }
            foreach (string item in new string[] { "MqttHost","MqttPort","MqttUser","MqttPw", "MqttPreTopic" })
            {
                if (xmlElement[item] == null) xmlElement.AppendChild(xml.CreateNode(XmlNodeType.Element, item, null));
            }
        }

        private void butServiceStrat_Click(object sender, EventArgs e)
        {
            if(serviceController.Status == ServiceControllerStatus.Stopped)
                serviceController.Start();
        }

        private void butServiceStop_Click(object sender, EventArgs e)
        {
            if (serviceController.Status == ServiceControllerStatus.Running)
            {
                serviceController.Stop();
                updateTbStatus();
            }
        }

        void updateTbStatus()
        {
            labServieState.Text = serviceController.Status.ToString();
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            updateTbStatus();
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            mqttConfig["MqttHost"].InnerText = tbMqttHost.Text;
            mqttConfig["MqttPort"].InnerText = tbMqttPort.Text;
            mqttConfig["MqttUser"].InnerText = tbMqttUser.Text;
            mqttConfig["MqttPw"].InnerText = tbMqttPW.Text;
            mqttConfig["MqttPreTopic"].InnerText = tbMqttPreTopic.Text;
            xmlConfig.Save(ConfigFile);
        }

        private void butSaveRestart_Click(object sender, EventArgs e)
        {
            butSave_Click(sender, e);
            if (serviceController.Status == ServiceControllerStatus.Running)
            { 
                serviceController.Stop();
                updateTbStatus();
            }
            serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
            updateTbStatus();
            serviceController.Start();
            
        }
    }
}
