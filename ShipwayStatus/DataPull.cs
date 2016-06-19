using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace ShipwayStatus
{
    public class DataPull
    {
        SqlConnection scn;

        public DataPull(string dbServer, string dbName, string dbUser, string dbPassword)
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            csb.InitialCatalog = dbName;
            csb.DataSource = dbServer;
            csb.UserID = dbUser;
            csb.Password = dbPassword;
            scn = new SqlConnection(csb.ConnectionString);
        }

        public List<Pad> ExecuteQuery(Dictionary<string, string> phases, string projectName, string performerKeyWork)
        {
            string strSql = @"select performer.performer, pvt.* from 
                                (
	                                select 
		                                ac1.short_name as pad_num,
		                                ac2.short_name as pad_name,
		                                (
			                                case ";
            foreach (KeyValuePair<string,string> pair in phases)
            {
                strSql += String.Format("when t.task_name like '{0}%' then '{1}' ", pair.Value, pair.Key);
            }
            strSql += @"else 'unknown'
			                                end
		                                ) as task_name,
		                                (
			                                case 
			                                when t.status_code='TK_Complete' then 1
			                                when t.status_code='TK_NotStart' then 0
                                            when t.status_code='TK_Active' and t.task_type = 'TT_LOE' then 
                                                convert(decimal,(t.restart_date - t.act_start_date))/convert(decimal,(t.early_end_date - t.act_start_date))
			                                else (1-t.remain_drtn_hr_cnt/t.target_drtn_hr_cnt)
		                                end) as task_percent
	                                from task t
		                                inner join PROJECT p on p.proj_id = t.proj_id

		                                inner join TASKACTV ta1 on ta1.task_id = t.task_id
		                                inner join ACTVCODE ac1 on ac1.actv_code_id = ta1.actv_code_id
		                                inner join ACTVTYPE at1 on at1.actv_code_type_id = ac1.actv_code_type_id

		                                inner join TASKACTV ta2 on ta2.task_id = t.task_id
		                                inner join ACTVCODE ac2 on ac2.actv_code_id = ta2.actv_code_id
		                                inner join ACTVTYPE at2 on at2.actv_code_type_id = ac2.actv_code_type_id
	                                where 
		                                p.proj_short_name = '" + projectName + @"' 
		                                and at1.actv_code_type = 'Ось' 
		                                and at2.actv_code_type = 'Плита'
		                                and ( ";
            foreach (KeyValuePair<string, string> pair in phases)
            {
                strSql += String.Format("t.task_name like '{0}%' or ", pair.Value);
            }
            strSql += @"t.task_name = 'not_using_this_value' )
                                ) t
                                pivot (
	                                max(task_percent) for task_name in (";
            foreach (KeyValuePair<string, string> pair in phases)
            {
                strSql += String.Format("[{0}], ", pair.Key);
            };
            strSql += @"[unknown]
	                                )
                                ) pvt
                                left join (
                                select 
                                    ac1.short_name as pad_num,
                                    ac2.short_name as pad_name,
                                    ac3.short_name as performer
                                from TASK t
		                                inner join PROJECT p on p.proj_id = t.proj_id

		                                inner join TASKACTV ta1 on ta1.task_id = t.task_id
		                                inner join ACTVCODE ac1 on ac1.actv_code_id = ta1.actv_code_id
		                                inner join ACTVTYPE at1 on at1.actv_code_type_id = ac1.actv_code_type_id

		                                inner join TASKACTV ta2 on ta2.task_id = t.task_id
		                                inner join ACTVCODE ac2 on ac2.actv_code_id = ta2.actv_code_id
		                                inner join ACTVTYPE at2 on at2.actv_code_type_id = ac2.actv_code_type_id
		
		                                left join TASKACTV ta3 on ta3.task_id = t.task_id 
		                                left join ACTVCODE ac3 on ac3.actv_code_id = ta3.actv_code_id
		                                left join ACTVTYPE at3 on at3.actv_code_type_id = ac3.actv_code_type_id
                                where at3.actv_code_type='Участок'
                                and at1.actv_code_type = 'Ось' 
                                and at2.actv_code_type = 'Плита'
                                and p.proj_short_name = '"+projectName+
                                "' and t.task_name like '"+performerKeyWork+
                               @"%') performer on performer.pad_name = pvt.pad_name and performer.pad_num = pvt.pad_num
                                order by pad_num, pad_name";
            scn.Open();
            SqlCommand sc = new SqlCommand(strSql, scn);
            SqlDataReader myDataReader = sc.ExecuteReader();
            List<Pad> resultPadList = new List<Pad>();
            while (myDataReader.Read())
            {
                Pad pad = new Pad();
                pad.Performer = myDataReader.GetValue(0).ToString();
                pad.Axis = Convert.ToInt32(myDataReader.GetValue(1));
                pad.Name = myDataReader.GetValue(2).ToString();
                for (int i = 3; i < myDataReader.FieldCount; i++)
                {
                    PadPhase pp = new PadPhase()
                    {
                        Name = myDataReader.GetName(i).ToString(),
                        Percent = myDataReader.GetValue(i) is Decimal ?
                                                   Convert.ToDouble(myDataReader.GetValue(i)) : -1.0
                    };
                    pad.AddPhase(pp);
                }
                resultPadList.Add(pad);
            }
            scn.Close();
            return resultPadList;
        }

        public string ExecuteRecalcDate(string projectName)
        {
            string strSql = "select p.last_recalc_date from project p where p.proj_short_name = '" + projectName + "'";
            scn.Open();
            SqlCommand sc = new SqlCommand(strSql, scn);
            string str = sc.ExecuteScalar().ToString();
            scn.Close();
            return str;
        }
    }
}