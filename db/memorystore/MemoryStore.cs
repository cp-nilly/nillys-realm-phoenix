using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.memorystore
{
    public class MemoryStore
    {
        /*
        public string[] Struct_Tables = { "accounts", "arenalb", }
        public string[] Struct_Account = { "id", "uuid", "password", "name", "rank", "tag", "namechosen",
                                           "verfified", "guild", "guildFame", "guildRank", "vaultCount",
                                           "maxCharSlot", "regTime", "guest", "banned", "locked", "ignored",
                                           "bonuses" };
        
        private readonly MySqlConnection _con;

        public Dictionary<string, List<Dictionary<string, object>>> data;
        //Translation: Array<Table, Array<Row, Array<Index, Value>>>
        //Tables
        //  -> Rows
        //      -> Variable
        //          -> Value
        //eg Table: rotmg.accounts > Row 0 > email > admin@hotmail.fr

        public MemoryStore(string strConnectionInfo)
        {
            data = new Dictionary<string,List<Dictionary<string,object>>>();
            _con = new MySqlConnection(strConnectionInfo);
            _con.Open();
        }

        public bool LoadAccountLocal(int iAccountId)
        {
            MySqlCommand cmd = _con.CreateCommand();
            cmd.CommandText = "SELECT * FROM accounts WHERE id=@id;";
            cmd.Parameters.AddWithValue("@id", iAccountId);

            using (MySqlDataReader rdr = cmd.ExecuteReader()) //Account details
            {
                if (!rdr.HasRows)
                    return false;

                object[] values = new object[Struct_Account.Length];
                rdr.GetValues(values);
                Dictionary<string, object> row = _IntitializeRow("Accounts");

                for (int i = 0; i < Struct_Account.Length - 1; i++)
                    row[Struct_Account[i]] = values[i];

                
            }
        }



        private Dictionary<string, object> _IntitializeRow(string sTable)
        {
            Dictionary<string, object> row = new Dictionary<string, object>();
            data[sTable].Add(row);
            return row;
        }

        public void Synchronize()
        {

        }
         * */
    }
}
