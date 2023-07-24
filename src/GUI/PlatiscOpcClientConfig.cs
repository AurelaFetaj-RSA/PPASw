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

            ClientDataConfig.Add(new OpcObjectData("pc_jog_basso", $"ns=2;s=Tags.FP0H/pc_jog_basso", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_jog_alto", $"ns=2;s=Tags.FP0H/pc_jog_basso", typeof(int)));
            ClientDataConfig.Add(new OpcObjectData("pc_velocità_jog", $"ns=2;s=Tags.FP0H/pc_velocità_jog", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pc_start_a_quota", $"ns=2;s=Tags.FP0H/pc_start_a_quota", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_reset_allarme_servo", $"ns=2;s=Tags.FP0H/pc_reset_allarme_servo", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_homing", $"ns=2;s=Tags.FP0H/pc_homing", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_reset_homing", $"ns=2;s=Tags.FP0H/pc_reset_homing", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_start_programma_in_test", $"ns=2;s=Tags.FP0H/pc_start_programma_in_test", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pc_numero_punto_in_teaching", $"ns=2;s=Tags.FP0H/pc_numero_punto_in_teaching", typeof(int)));
            ClientDataConfig.Add(new OpcObjectData("pc_vai_a_punto_in_teaching", $"ns=2;s=Tags.FP0H/pc_vai_a_punto_in_teaching", typeof(int[])));
            ClientDataConfig.Add(new OpcObjectData("pc_apertura_pinza_bordo_stivale", $"ns=2;s=Tags.FP0H/pc_apertura_pinza_bordo_stivale", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_chiusura_pinza_bordo_stivale", $"ns=2;s=Tags.FP0H/pc_chiusura_pinza_bordo_stivale", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pc_apertura_pinze_centraggio", $"ns=2;s=Tags.FP0H/pc_apertura_pinze_centraggio", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_chiusura_pinze_centraggio", $"ns=2;s=Tags.FP0H/pc_chiusura_pinze_centraggio", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_apertura_contrasto", $"ns=2;s=Tags.FP0H/pc_apertura_contrasto", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_chiusura_contrasto", $"ns=2;s=Tags.FP0H/pc_chiusura_contrasto", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_stampa", $"ns=2;s=Tags.FP0H/pc_stampa", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_start_stop_nastro_lavoro", $"ns=2;s=Tags.FP0H/pc_start_stop_nastro_lavoro", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_start_stop_nastro_uscita", $"ns=2;s=Tags.FP0H/pc_start_stop_nastro_uscita", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pc_percentuale_velocità_in_manuale", $"ns=2;s=Tags.FP0H/pc_percentuale_velocità_in_manuale", typeof(int)));
            ClientDataConfig.Add(new OpcObjectData("pc_quota_finale_asse_in_manuale", $"ns=2;s=Tags.FP0H/pc_quota_finale_asse_in_manuale", typeof(int)));

            ClientDataConfig.Add(new OpcObjectData("pc_inclusione_esclusione_generale", $"ns=2;s=Tags.FP0H/pc_percentuale_velocità_in_manuale", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_inclusione_esclusione_da_ricetta", $"ns=2;s=Tags.FP0H/pc_quota_finale_asse_in_manuale", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pc_quota_longitudinale", $"ns=2;s=Tags.FP0H/pc_quota_longitudinale", typeof(int[])));
            ClientDataConfig.Add(new OpcObjectData("pc_velocità", $"ns=2;s=Tags.FP0H/pc_velocità", typeof(int[])));
            ClientDataConfig.Add(new OpcObjectData("pc_quota_longitudinale_in_test", $"ns=2;s=Tags.FP0H/pc_quota_longitudinale_in_test", typeof(int[])));
            ClientDataConfig.Add(new OpcObjectData("pc_velocità_in_test", $"ns=2;s=Tags.FP0H/pc_velocità_in_test", typeof(int[])));
            ClientDataConfig.Add(new OpcObjectData("pc_quota_longitudinale_in_teaching", $"ns=2;s=Tags.FP0H/pc_quota_longitudinale_in_teaching", typeof(int[])));
            ClientDataConfig.Add(new OpcObjectData("pc_velocità_in_teaching", $"ns=2;s=Tags.FP0H/pc_velocità_in_teaching", typeof(int[])));

            ClientDataConfig.Add(new OpcObjectData("pc_start_stop", $"ns=2;s=Tags.FP0H/pc_start_stop", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_pausa", $"ns=2;s=Tags.FP0H/pc_pausa", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pc_punto_1_raggiunto", $"ns=2;s=Tags.FP0H/pc_punto_1_raggiunto", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_punto_2_raggiunto", $"ns=2;s=Tags.FP0H/pc_punto_2_raggiunto", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_punto_3_raggiunto", $"ns=2;s=Tags.FP0H/pc_punto_3_raggiunto", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_punto_4_raggiunto", $"ns=2;s=Tags.FP0H/pc_punto_4_raggiunto", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_homing_done", $"ns=2;s=Tags.FP0H/pc_homing_done", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_quota_attuale_asse", $"ns=2;s=Tags.FP0H/pc_quota_attuale_asse", typeof(int)));
            ClientDataConfig.Add(new OpcObjectData("pc_numero_cicli", $"ns=2;s=Tags.FP0H/pc_numero_cicli", typeof(int)));
            ClientDataConfig.Add(new OpcObjectData("pc_ready", $"ns=2;s=Tags.FP0H/pc_ready", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pc_stato_macchina", $"ns=2;s=Tags.FP0H/pc_stato_macchina", typeof(int)));
            ClientDataConfig.Add(new OpcObjectData("pc_allarmi_generali ", $"ns=2;s=Tags.FP0H/pc_allarmi_generali", typeof(short[])));
            ClientDataConfig.Add(new OpcObjectData("pc_allarmi_timeout", $"ns=2;s=Tags.FP0H/pc_allarmi_timeout", typeof(short[])));
            ClientDataConfig.Add(new OpcObjectData("pc_input", $"ns=2;s=Tags.FP0H/pc_input", typeof(bool[])));
            ClientDataConfig.Add(new OpcObjectData("pc_output", $"ns=2;s=Tags.FP0H/pc_output", typeof(bool[])));
            return this;
        }

    }
}
