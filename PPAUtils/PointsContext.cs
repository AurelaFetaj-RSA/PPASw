using MySql.Data.EntityFramework;
using MySql.Data.MySqlClient;
using RSACommon;
using RSACommon.DatabasesUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace PPAUtils
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class PadLaserProgramsContext: DbContext
    {
        public PadLaserProgramsContext(DbConnection conn): base(conn,true)
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<padlaserprogram>();
        }

        public virtual DbSet<padlaserprogram> PadLaserPrograms { get; set; }

    }

    public class sizes : RSACommon.DatabasesUtils.IDBSet
    {
        public sizes()
        {

        }

        public sizes(sizes toCopy)
        {
            size = toCopy.size;
            s1_param1 = toCopy.s1_param1;
        }

        public bool CopyFromDB(MySqlDataReader toCopy)
        {
            try
            {
                size = short.Parse(toCopy["size"].ToString());
                s1_param1 = short.Parse(toCopy["s1_param1"].ToString());
            }
            catch
            {
                return false;
            }

            return true;
        }

        [Key]
        public short size { get; set; } = 24;
        public short s1_param1 { get; set; } = 0;    
    }

    public class recipies: RSACommon.DatabasesUtils.IDBSet
    {
        public recipies()
        {

        }

        public recipies(recipies toCopy)
        {
            model_name = toCopy.model_name;
            m1_param1 = toCopy.m1_param1;
            m1_param2 = toCopy.m1_param2;
            m2_param1 = toCopy.m2_param1;
            m2_param2 = toCopy.m2_param2;
            m3_param1 = toCopy.m3_param1;
            m3_param2 = toCopy.m3_param2;
            m4_param1 = toCopy.m4_param1;
            m4_param2 = toCopy.m4_param2;
            m4_param3 = toCopy.m4_param3;
            m4_param4 = toCopy.m4_param4;
            
            m5_param1 = toCopy.m5_param1;
            m5_param2 = toCopy.m5_param2;
            m6_param1 = toCopy.m6_param1;
            m6_param2 = toCopy.m6_param2;
            m_description = toCopy.m_description;
            m4_param5 = toCopy.m4_param5;
        }

        public bool CopyFromDB(MySqlDataReader toCopy)
        {
            try
            {
                model_name = toCopy["model_name"].ToString();
                m1_param1 = Convert.ToInt32(toCopy["m1_param1"]);
                m1_param2 = Convert.ToInt32(toCopy["m1_param2"]);
                m2_param1 = Convert.ToInt32(toCopy["m2_param1"]);
                m2_param2 = Convert.ToInt32(toCopy["m2_param2"]);
                m3_param1 = Convert.ToInt32(toCopy["m3_param1"]);
                m3_param2 = Convert.ToInt32(toCopy["m3_param2"]);
                m4_param1 = Convert.ToInt32(toCopy["m4_param1"]);
                m4_param2 = toCopy["m4_param2"].ToString();
                m4_param3 = Convert.ToString(toCopy["m4_param3"]);
                m4_param4 = Convert.ToString(toCopy["m4_param4"]);
                
                m5_param1 = Convert.ToInt32(toCopy["m5_param1"]);
                m5_param2 = Convert.ToInt32(toCopy["m5_param2"]);
                m6_param1 = Convert.ToInt32(toCopy["m6_param1"]);
                m6_param2 = Convert.ToInt32(toCopy["m6_param2"]);
                m_description = toCopy["m_description"].ToString();
                m4_param5 = toCopy["m4_param5"].ToString();
            }
            catch
            {
                return false;
            }

            return true;
        }

        [Key]
        public string model_name { get; set; } = "TEST";
        public int m1_param1 { get; set; } = 0;
        public int m1_param2 { get; set; } = 0;
        public int m2_param1 { get; set; } = 0;
        public int m2_param2 { get; set; } = 0;
        public int m3_param1 { get; set; } = 0;
        public int m3_param2 { get; set; } = 0;
        public int m4_param1 { get; set; } = 0;
        public string m4_param2 { get; set; } = "";
        public string m4_param3 { get; set; } = "";
        public string m4_param4 { get; set; } = "";
        public string m4_param5 { get; set; } = "";
        public int m5_param1 { get; set; } = 0;
        public int m5_param2 { get; set; } = 0;
        public int m6_param1 { get; set; } = 0;
        public int m6_param2 { get; set; } = 0;
        public string m_description { get; set; } = "";
    }
    public class padlaserprogram: IDBSet
    {
        public padlaserprogram()
        {
        }
        public padlaserprogram(padlaserprogram toCopy)
        {
            program_name = toCopy.program_name;
            Q1 = toCopy.Q1;
            Q2 = toCopy.Q2;
            Q3 = toCopy.Q3;
            Q4 = toCopy.Q4;
            S1 = toCopy.S1;
            S2 = toCopy.S2;
            S3 = toCopy.S3;
            S4 = toCopy.S4;
            T1 = toCopy.T1;
        }
        [Key]
        public string program_name { get; set; } = "TTTT";        
        public float Q1 { get; set; } = 0;
        public float Q2 { get; set; } = 0;
        public float Q3 { get; set; } = 0;
        public float Q4 { get; set; } = 0;
        public int S1 { get; set; } = 0;
        public int S2 { get; set; } = 0;
        public int S3 { get; set; } = 0;
        public int S4 { get; set; } = 0;
        public float T1 { get; set; } = 0;
        public bool CopyFromDB(MySqlDataReader toCopy)
        {
            try
            {
                program_name = toCopy["program_name"].ToString();
                Q1 = float.Parse(toCopy["Q1"].ToString());
                Q2 = float.Parse(toCopy["Q2"].ToString());
                Q3 = float.Parse(toCopy["Q3"].ToString());
                Q4 = float.Parse(toCopy["Q4"].ToString());
                S1 = Convert.ToInt32(toCopy["S1"]);
                S2 = Convert.ToInt32(toCopy["S2"]);
                S3 = Convert.ToInt32(toCopy["S3"]);
                S4 = Convert.ToInt32(toCopy["S4"]);
                T1 = float.Parse(toCopy["T1"].ToString());
            }
            catch
            {
                return false;
            }

            return true;
        }
    }

}
