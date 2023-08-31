using log4net;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using Opc.Ua;
using Org.BouncyCastle.Utilities.Collections;
using RSACommon.DatabasesUtils;
using RSACommon.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace PlasticauchoUtils
{

    //public class PlasticauchoMySql
    //{
    //    public MySqlTable tableForData { get; private set; } = null;
    //    public int ColumnSize { get; private set; } = 0;
    //    public PlasticauchoMySql(MySqlTable table)
    //    {
    //        tableForData = table;
    //        ColumnSize = table.Columns.Count;

    //    }

    //    int MODEL_NAME_COLUMN = 13;

    //    public MySqlResult Insert(string model_name, params int[] value)
    //    {
    //        string[] obj_name = new string[] { "m11", "m12", "m21", "m22", "m31", "m32", "m41", "m42", "m51", "m52", "m61", "m62" }; 
    //        string insert = "INSERT INTO models(model_name, m1_param1, m1_param2, m2_param1, m2_param2, m3_param1, m3_param2, m4_param1, m4_param2, m5_param1, m5_param2, m6_param1, m6_param2) VALUES(?m,?m11,?m12,?m21,?m22,?m31,?m32,?m41,?m42,?m51,?m52,?m61,?m62)";

    //        if (value.Length < MODEL_NAME_COLUMN - 1 || obj_name.Length != value.Length )
    //            return new MySqlResult()
    //            {
    //                Error = RSACommon.Error.MYSQL_BAD_INSERT,
    //                Message = $"Too few arguemnts: {value.Length}, expected {MODEL_NAME_COLUMN}"
    //            };

    //        MySqlParameter[] arryaOfParams = new MySqlParameter[MODEL_NAME_COLUMN];
    //        arryaOfParams[0] = new MySqlParameter("m", model_name);

    //        for(int i = 1; i < MODEL_NAME_COLUMN; i++)
    //        {
    //            arryaOfParams[i] = new MySqlParameter(obj_name[i-1], value[i-1]);
    //        }

    //        return tableForData.ExecuteNotQuery(insert, arryaOfParams);
    //    }
    //}
}
