using OpcCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    public class PlasticOpcClientConfig: IOpcClientConfigurator
    {
        string stringToFill = $"ns=2;s=Tags.{0}/{1}";
        public OpcClientObject ClientDataConfig { get; private set; } = new OpcClientObject();
        public IOpcClientConfigurator Config()
        {

            //ClientDataConfig.Add(new OpcObjectData("pc_jog_alto", string.Format(stringToFill, "FPH0", "pc_jog_alto")));

            ClientDataConfig.Add(new OpcObjectData("pc_jog_alto", $"ns=2;s=Tags.FPH0/pc_jog_alto"));
            ClientDataConfig.Add(new OpcObjectData("pc_jog_basso", string.Format(stringToFill, "FPH0", "pc_jog_bassodd")));
            //ClientDataConfig.Add("pc_jog_alto", string.Format(stringToFill,  "FPH0","pc_jog_alto"));
            //ClientDataConfig.Add("pc_jog_basso", string.Format(stringToFill, "FPH0", "pc_jog_basso"));
            //ClientDataConfig.Add("pc_velocità_jog", string.Format(stringToFill, "FPH0", "pc_velocità_jog"));
            //ClientDataConfig.Add("pc_start_a_quota", string.Format(stringToFill, "FPH0", "pc_start_a_quota"));
            //ClientDataConfig.Add("pc_reset_allarme_servo", string.Format(stringToFill, "FPH0", "pc_reset_allarme_servo"));

            //ClientDataConfig.Add("pc_homing", string.Format(stringToFill, "FPH0", "pc_homing"));
            //ClientDataConfig.Add("pc_reset_homing", string.Format(stringToFill, "FPH0", "pc_reset_homing"));
            //ClientDataConfig.Add("pc_velocità_jog", string.Format(stringToFill, "FPH0", "pc_velocità_jog"));
            //ClientDataConfig.Add("pc_start_a_quota", string.Format(stringToFill, "FPH0", "pc_start_a_quota"));
            //ClientDataConfig.Add("pc_reset_allarme_servo", string.Format(stringToFill, "FPH0", "pc_reset_allarme_servo"));

            return this;
        }

    }
}
