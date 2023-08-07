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

    public class PlasticauchoMySql
    {
        public MySqlTable tableForData { get; private set; } = null;
        public int ColumnSize { get; private set; } = 0;
        public PlasticauchoMySql(MySqlTable table)
        {
            tableForData = table;
            ColumnSize = table.Columns.Count;

        }

        int MODEL_NAME_COLUMN = 13;

        public MySqlResult Insert(string model_name, params int[] value)
        {
            string[] obj_name = new string[] { "m11", "m12", "m21", "m22", "m31", "m32", "m41", "m42", "m51", "m52", "m61", "m62" }; 
            string insert = "INSERT INTO models(model_name, m1_param1, m1_param2, m2_param1, m2_param2, m3_param1, m3_param2, m4_param1, m4_param2, m5_param1, m5_param2, m6_param1, m6_param2) VALUES(?m,?m11,?m12,?m21,?m22,?m31,?m32,?m41,?m42,?m51,?m52,?m61,?m62)";

            if (value.Length < MODEL_NAME_COLUMN - 1 || obj_name.Length != value.Length )
                return new MySqlResult()
                {
                    Error = RSACommon.Error.MYSQL_BAD_INSERT,
                    Message = $"Too few arguemnts: {value.Length}, expected {MODEL_NAME_COLUMN}"
                };

            MySqlParameter[] arryaOfParams = new MySqlParameter[MODEL_NAME_COLUMN];
            arryaOfParams[0] = new MySqlParameter("m", model_name);

            for(int i = 1; i < MODEL_NAME_COLUMN; i++)
            {
                arryaOfParams[i] = new MySqlParameter(obj_name[i-1], value[i-1]);
            }

            return tableForData.ExecuteNotQuery(insert, arryaOfParams);
        }


        public MySqlResult InsertAutomatic(object[] values)
        {
            string allcolumns = "";
            string allNameParam = "";
            string[] names = tableForData.Columns.ToArray();
            //tableForData.Columns.ForEach(x => allcolumns += x + ",");
            //tableForData.Column.ForEach(x => allNameParam += $"?{x},");

            for (int i = 0; i < ColumnSize; i++)
            {
                if(i != ColumnSize - 1)
                {
                    allcolumns += names[i] + ",";
                    allNameParam += $"?{names[i]},";
                }
                else
                {
                    allcolumns += names[i];
                    allNameParam += $"?{names[i]}";
                }
            }

            string insert = $"INSERT INTO models({allcolumns}) VALUES({allNameParam})";

            MySqlParameter[] arryaOfParams = new MySqlParameter[ColumnSize];

            for (int i = 0; i < ColumnSize; i++)
            {
                if (tableForData.Columns.Contains(names[i]))
                {
                    arryaOfParams[i] = new MySqlParameter(names[i], values[i]);
                }
            }

            return tableForData.ExecuteNotQuery(insert, arryaOfParams);
        }

        public MySqlResult UpdateAutomaticPrimaryKey(string key, string[] paramToUpdate, object[] values)
        {
            //string sql = $"SELECT * FROM {tableForData.Name} WHERE model_name = '{key}'";
            string allcolumns = "";

            for (int i = 0; i < paramToUpdate.Count(); i++)
            {
                allcolumns += $"{paramToUpdate[i]}=@{paramToUpdate[i]}";

                if (i != paramToUpdate.Count() - 1)
                {
                    allcolumns += ",";
                }
            }

            if (tableForData.PrimaryKey == string.Empty)
            {
                return new MySqlResult()
                {
                    Error = RSACommon.Error.MYSQL_PRIMARY_KEY_MISSING,
                    Message = "Primary key is missing",
                    Executed = ""
                };
            }

            string query = $"UPDATE {tableForData.Name} SET {allcolumns} WHERE {tableForData.PrimaryKey} = '{key}'";

            MySqlParameter[] arryaOfParams = new MySqlParameter[paramToUpdate.Count()];

            for (int i = 0; i < values.Count(); i++)
            {
                if (tableForData.Columns.Contains(paramToUpdate[i]))
                {
                    arryaOfParams[i] = new MySqlParameter(paramToUpdate[i], values[i]);
                }
            }

            return tableForData.ExecuteNotQuery(query, arryaOfParams);
        }

        public MySqlResult DeleteRowPrimaryKey(string key)
        {
            if (tableForData.PrimaryKey == string.Empty)
            {
                return new MySqlResult()
                {
                    Error = RSACommon.Error.MYSQL_PRIMARY_KEY_MISSING,
                    Message = "Primary key is missing",
                    Executed = ""
                };
            }

            string query = $"DELETE FROM {tableForData.Name} WHERE {tableForData.PrimaryKey} = '{key}'";

            return tableForData.ExecuteNotQuery(query, new MySqlParameter[] { });
        }

        /// <summary>
        /// Get data from primary key filter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public MySqlResult SelectByPrimaryKey(string key)
        {
            //string sql = $"SELECT * FROM {tableForData.Name} WHERE model_name = '{key}'";

            if (tableForData.PrimaryKey == string.Empty)
            {
                return new MySqlResult()
                {
                    Error = RSACommon.Error.MYSQL_PRIMARY_KEY_MISSING,
                    Message = "Primary key is missing",
                    Executed = ""
                };
            }

            string query = $"SELECT * FROM {tableForData.Name} WHERE {tableForData.PrimaryKey} = '{key}'";
            return new MySqlResult();
            //return tableForData.ExecuteQuery(query);
        }
    }
}
