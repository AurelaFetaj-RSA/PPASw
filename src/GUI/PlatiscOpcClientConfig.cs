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
            #region(* M2 OPCUA variables *)
            ClientDataConfig.Add(new OpcObjectData("pcM2JogDown", $"ns=2;s=Tags.Pad_print_interna/pc_jog_basso", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2JogUp", $"ns=2;s=Tags.Pad_print_interna/pc_jog_alto", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2JogSpeed", $"ns=2;s=Tags.Pad_print_interna/pc_velocità_jog", typeof(short[])));
            ClientDataConfig.Add(new OpcObjectData("pcM2JogReset", $"ns=2;s=Tags.Pad_print_interna/pc_reset_jog", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2QuoteStart", $"ns=2;s=Tags.Pad_print_interna/pc_start_a_quota", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2ResetServoAlarm", $"ns=2;s=Tags.Pad_print_interna/pc_reset_allarme_servo", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pcM2Homing", $"ns=2;s=Tags.Pad_print_interna/pc_homing", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2ResetHoming", $"ns=2;s=Tags.Pad_print_interna/pc_reset_homing", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_start_programma_in_test", $"ns=2;s=Tags.Pad_print_interna/pc_start_programma_in_test", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pc_numero_punto_in_teaching", $"ns=2;s=Tags.Pad_print_interna/pc_numero_punto_in_teaching", typeof(short)));
            ClientDataConfig.Add(new OpcObjectData("pc_vai_a_punto_in_teaching", $"ns=2;s=Tags.Pad_print_interna/pc_vai_a_punto_in_teaching", typeof(int[])));
            ClientDataConfig.Add(new OpcObjectData("pc_apertura_pinza_bordo_stivale", $"ns=2;s=Tags.Pad_print_interna/pc_apertura_pinza_bordo_stivale", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_chiusura_pinza_bordo_stivale", $"ns=2;s=Tags.Pad_print_interna/pc_chiusura_pinza_bordo_stivale", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pc_apertura_pinze_centraggio", $"ns=2;s=Tags.Pad_print_interna/pc_apertura_pinze_centraggio", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_chiusura_pinze_centraggio", $"ns=2;s=Tags.Pad_print_interna/pc_chiusura_pinze_centraggio", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_apertura_contrasto", $"ns=2;s=Tags.Pad_print_interna/pc_apertura_contrasto", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_chiusura_contrasto", $"ns=2;s=Tags.Pad_print_interna/pc_chiusura_contrasto", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_stampa", $"ns=2;s=Tags.Pad_print_interna/pc_stampa", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_start_stop_nastro_lavoro", $"ns=2;s=Tags.Pad_print_interna/pc_start_stop_nastro_lavoro", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_start_stop_nastro_uscita", $"ns=2;s=Tags.Pad_print_interna/pc_start_stop_nastro_uscita", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pcM2ManualSpeed", $"ns=2;s=Tags.Pad_print_interna/pc_percentuale_velocit__in_manuale", typeof(short)));
            ClientDataConfig.Add(new OpcObjectData("pcM2ManualQuote", $"ns=2;s=Tags.Pad_print_interna/pc_quota_finale_asse_in_manuale", typeof(short)));

            ClientDataConfig.Add(new OpcObjectData("pcM2Inclusion", $"ns=2;s=Tags.Pad_print_interna/pc_percentuale_velocità_in_manuale", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_inclusione_esclusione_da_ricetta", $"ns=2;s=Tags.Pad_print_interna/pc_quota_finale_asse_in_manuale", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pc_quota_longitudinale", $"ns=2;s=Tags.Pad_print_interna/pc_quota_longitudinale", typeof(short[])));
            ClientDataConfig.Add(new OpcObjectData("pc_velocità", $"ns=2;s=Tags.Pad_print_interna/pc_velocità", typeof(int[])));
            ClientDataConfig.Add(new OpcObjectData("pc_quota_longitudinale_in_test", $"ns=2;s=Tags.Pad_print_interna/pc_quota_longitudinale_in_test", typeof(short[])));
            ClientDataConfig.Add(new OpcObjectData("pc_velocità_in_test", $"ns=2;s=Tags.Pad_print_interna/pc_velocità_in_test", typeof(short[])));
            ClientDataConfig.Add(new OpcObjectData("pc_quota_longitudinale_in_teaching", $"ns=2;s=Tags.Pad_print_interna/pc_quota_longitudinale_in_teaching", typeof(short[])));
            ClientDataConfig.Add(new OpcObjectData("pc_velocità_in_teaching", $"ns=2;s=Tags.Pad_print_interna/pc_velocità_in_teaching", typeof(short[])));

            ClientDataConfig.Add(new OpcObjectData("pc_start_stop", $"ns=2;s=Tags.Pad_print_interna/pc_start_stop", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_pausa", $"ns=2;s=Tags.Pad_print_interna/pc_pausa", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pc_punto_1_raggiunto", $"ns=2;s=Tags.Pad_print_interna/pc_punto_1_raggiunto", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_punto_2_raggiunto", $"ns=2;s=Tags.Pad_print_interna/pc_punto_2_raggiunto", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_punto_3_raggiunto", $"ns=2;s=Tags.Pad_print_interna/pc_punto_3_raggiunto", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_punto_4_raggiunto", $"ns=2;s=Tags.Pad_print_interna/pc_punto_4_raggiunto", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2HomingDone", $"ns=2;s=Tags.Pad_print_interna/pc_homing_done", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_quota_attuale_asse", $"ns=2;s=Tags.Pad_print_interna/pc_quota_attuale_asse", typeof(short)));
            ClientDataConfig.Add(new OpcObjectData("pc_numero_cicli", $"ns=2;s=Tags.Pad_print_interna/pc_numero_cicli", typeof(short)));
            ClientDataConfig.Add(new OpcObjectData("pcM2Ready", $"ns=2;s=Tags.Pad_print_interna/pc_ready", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pcM2Status", $"ns=2;s=Tags.Pad_print_interna/pc_stato_macchina", typeof(short)));
            ClientDataConfig.Add(new OpcObjectData("pc_allarmi_generali ", $"ns=2;s=Tags.Pad_print_interna/pc_allarmi_generali", typeof(short[])));
            ClientDataConfig.Add(new OpcObjectData("pc_allarmi_timeout", $"ns=2;s=Tags.Pad_print_interna/pc_allarmi_timeout", typeof(short[])));
            ClientDataConfig.Add(new OpcObjectData("pc_input", $"ns=2;s=Tags.Pad_print_interna/pc_input", typeof(bool[])));
            ClientDataConfig.Add(new OpcObjectData("pc_output", $"ns=2;s=Tags.Pad_print_interna/pc_output", typeof(bool[])));

            #endregion

            return this;
        }

    }
}
