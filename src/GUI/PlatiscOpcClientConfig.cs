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
            ClientDataConfig.Add(new OpcObjectData("pcM2JogSpeed", $"ns=2;s=Tags.Pad_print_interna/pc_velocita_a_jog", typeof(short)));
            ClientDataConfig.Add(new OpcObjectData("pcM2JogReset", $"ns=2;s=Tags.Pad_print_interna/pc_reset_jog", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2QuoteStart", $"ns=2;s=Tags.Pad_print_interna/pc_start_a_quota", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2ResetServoAlarm", $"ns=2;s=Tags.Pad_print_interna/pc_reset_allarme_servo", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2Homing", $"ns=2;s=Tags.Pad_print_interna/pc_homing", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2ResetHoming", $"ns=2;s=Tags.Pad_print_interna/pc_reset_homing", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2StartTest", $"ns=2;s=Tags.Pad_print_interna/pc_start_programma_in_test", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2TeachPointID", $"ns=2;s=Tags.Pad_print_interna/pc_numero_punto_in_teaching", typeof(short)));
            ClientDataConfig.Add(new OpcObjectData("pcM2TeachPointReg", $"ns=2;s=Tags.Pad_print_interna/pc_vai_a_punto_in_teaching", typeof(int[])));
            ClientDataConfig.Add(new OpcObjectData("pcM2SmallClampOpening", $"ns=2;s=Tags.Pad_print_interna/pc_apertura_pinza_bordo_stivale", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2SmallClampClosing", $"ns=2;s=Tags.Pad_print_interna/pc_chiusura_pinza_bordo_stivale", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2BigClampOpening", $"ns=2;s=Tags.Pad_print_interna/pc_apertura_pinza_grande", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2BigClampClosing", $"ns=2;s=Tags.Pad_print_interna/pc_chiusura_pinza_grande", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2CentrClampOpening", $"ns=2;s=Tags.Pad_print_interna/pc_apertura_pinze_centraggio", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2CentrClampClosing", $"ns=2;s=Tags.Pad_print_interna/pc_chiusura_pinze_centraggio", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2ContrOpening", $"ns=2;s=Tags.Pad_print_interna/pc_apertura_contrasto", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2ContrClosing", $"ns=2;s=Tags.Pad_print_interna/pc_chiusura_contrasto", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2Print", $"ns=2;s=Tags.Pad_print_interna/pc_stampa", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2StartStopWorkingBelt", $"ns=2;s=Tags.Pad_print_interna/pc_start_stop_nastro_lavoro", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2StartStopExitBelt", $"ns=2;s=Tags.Pad_print_interna/pc_start_stop_nastro_uscita", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2ManualSpeed", $"ns=2;s=Tags.Pad_print_interna/pc_percentuale_velocita_in_manuale", typeof(short)));
            ClientDataConfig.Add(new OpcObjectData("pcM2ManualQuote", $"ns=2;s=Tags.Pad_print_interna/pc_quota_finale_asse_in_manuale", typeof(short)));

            ClientDataConfig.Add(new OpcObjectData("pcM2Inclusion", $"ns=2;s=Tags.Pad_print_interna/pc_percentuale_velocita_in_manuale", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_inclusione_esclusione_da_ricetta", $"ns=2;s=Tags.Pad_print_interna/pc_quota_finale_asse_in_manuale", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pcM2Quote", $"ns=2;s=Tags.Pad_print_interna/pc_quota_longitudinale", typeof(short[])));
            ClientDataConfig.Add(new OpcObjectData("pcM2AutoSpeed", $"ns=2;s=Tags.Pad_print_interna/pc_velocita", typeof(int[])));
            ClientDataConfig.Add(new OpcObjectData("pcM2TestQuote", $"ns=2;s=Tags.Pad_print_interna/pc_quota_longitudinale_in_test", typeof(short[])));
            ClientDataConfig.Add(new OpcObjectData("pcM2TestSpeed", $"ns=2;s=Tags.Pad_print_interna/pc_velocita_in_test", typeof(short[])));
            ClientDataConfig.Add(new OpcObjectData("pcM2TeachQuote", $"ns=2;s=Tags.Pad_print_interna/pc_quota_longitudinale_in_teaching", typeof(short[])));
            ClientDataConfig.Add(new OpcObjectData("pcM2TeachSpeed", $"ns=2;s=Tags.Pad_print_interna/pc_velocita_in_teaching", typeof(short[])));

            ClientDataConfig.Add(new OpcObjectData("pc_start_stop", $"ns=2;s=Tags.Pad_print_interna/pc_start_stop", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pc_pausa", $"ns=2;s=Tags.Pad_print_interna/pc_pausa", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2PointsReached", $"ns=2;s=Tags.Pad_print_interna/pc_punto_1_raggiunto", typeof(bool[])));
            ClientDataConfig.Add(new OpcObjectData("pcM2HomingDone", $"ns=2;s=Tags.Pad_print_interna/pc_homing_done", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM2CurrentAxisQuote", $"ns=2;s=Tags.Pad_print_interna/pc_quota_attuale_asse", typeof(short)));
            ClientDataConfig.Add(new OpcObjectData("pc_numero_cicli", $"ns=2;s=Tags.Pad_print_interna/pc_numero_cicli", typeof(short)));
            

            ClientDataConfig.Add(new OpcObjectData("pcM2Status", $"ns=2;s=Tags.Pad_print_interna/pc_stato_macchina", typeof(short)));
            ClientDataConfig.Add(new OpcObjectData("pc_allarmi_generali ", $"ns=2;s=Tags.Pad_print_interna/pc_allarmi_generali", typeof(short[])));
            ClientDataConfig.Add(new OpcObjectData("pc_allarmi_timeout", $"ns=2;s=Tags.Pad_print_interna/pc_allarmi_timeout", typeof(short[])));
            ClientDataConfig.Add(new OpcObjectData("pc_input", $"ns=2;s=Tags.Pad_print_interna/pc_input", typeof(bool[])));
            ClientDataConfig.Add(new OpcObjectData("pc_output", $"ns=2;s=Tags.Pad_print_interna/pc_output", typeof(bool[])));

            ClientDataConfig.Add(new OpcObjectData("pcM1Status", $"ns=2;s=Tags.Pad_print_esterna/pc_stato_macchina", typeof(short)));
            ClientDataConfig.Add(new OpcObjectData("pcM3Status", $"ns=2;s=Tags.Pad_print_esterna/pc_stato_macchina", typeof(short)));
            ClientDataConfig.Add(new OpcObjectData("pcM4Status", $"ns=2;s=Tags.Pad_print_interna/pc_stato_macchina", typeof(short)));
            ClientDataConfig.Add(new OpcObjectData("pcM5Status", $"ns=2;s=Tags.Pad_print_interna/pc_stato_macchina", typeof(short)));
            ClientDataConfig.Add(new OpcObjectData("pcM6Status", $"ns=2;s=Tags.Pad_print_interna/pc_stato_macchina", typeof(short)));

            ClientDataConfig.Add(new OpcObjectData("pcM3HomingDone", $"ns=2;s=Tags.Pad_print_esterna/pc_homing_done", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM1HomingDone", $"ns=2;s=Tags.Pad_print_interna/pc_homing_done", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM4HomingDone", $"ns=2;s=Tags.Pad_print_interna/pc_homing_done", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM5HomingDone", $"ns=2;s=Tags.Pad_print_interna/pc_homing_done", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM6HomingDone", $"ns=2;s=Tags.Pad_print_interna/pc_homing_done", typeof(bool)));

            ClientDataConfig.Add(new OpcObjectData("pcM2AutoSpeed", $"ns=2;s=Tags.Pad_print_interna/pc_velocita", typeof(short)));
            //digital input
            ClientDataConfig.Add(new OpcObjectData("pcM2DI", $"ns=2;s=Tags.Pad_print_interna/pc_input", typeof(bool[])));
            ClientDataConfig.Add(new OpcObjectData("pcM2DO", $"ns=2;s=Tags.Pad_print_interna/pc_output", typeof(bool[])));

            ClientDataConfig.Add(new OpcObjectData("pcM5TranslatorFwd", $"ns=2;s=Tags.Manipolatore/pc_traslatore_avanti", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM5TranslatorBwd", $"ns=2;s=Tags.Manipolatore/pc_traslatore_indietro", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM5ClampFwd", $"ns=2;s=Tags.Manipolatore/pc_pinza_avanti", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM5ClampBwd", $"ns=2;s=Tags.Manipolatore/pc_pinza_indietro", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM5ClampOpening", $"ns=2;s=Tags.Manipolatore/pc_apertura_pinza", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM5ClampClosing", $"ns=2;s=Tags.Manipolatore/pc_chiusura_pinza", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM5CWRotation", $"ns=2;s=Tags.Manipolatore/pc_rotazione_oraria", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM5CCWRotation", $"ns=2;s=Tags.Manipolatore/pc_rotazione_antioraria", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM5NoRotation", $"ns=2;s=Tags.Manipolatore/pc_nessuna_rotazione", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM5V1ExtFwd", $"ns=2;s=Tags.Manipolatore/pc_estensione_verticale_1_su", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM5V1ExtBwd", $"ns=2;s=Tags.Manipolatore/pc_estensione_verticale_1_giu", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM5V2ExtFwd", $"ns=2;s=Tags.Manipolatore/pc_estensione_verticale_2_su", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM5V2ExtBwd", $"ns=2;s=Tags.Manipolatore/pc_estensione_verticale_2_giu", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM5StartStopTranslBelt", $"ns=2;s=Tags.Manipolatore/pc_start_stop_nastro_traslazione", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM5StartStopOutBelt1", $"ns=2;s=Tags.Manipolatore/pc_start_stop_nastro_uscita_1", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM5StartStopOutBelt2", $"ns=2;s=Tags.Manipolatore/pc_start_stop_nastro_uscita_2", typeof(bool)));
            ClientDataConfig.Add(new OpcObjectData("pcM5StartStopOutBelt3", $"ns=2;s=Tags.Manipolatore/pc_start_stop_nastro_uscita_3", typeof(bool)));
            
                
            #endregion

            return this;
        }

    }
}
