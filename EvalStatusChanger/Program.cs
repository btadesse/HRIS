using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;

namespace EvalStatusChanger
{ 
    class Program
    {  
       private SqlConnection con = new SqlConnection( "Data Source=. \\Fintrac ;Initial Catalog=Timesheet;User ID=xyz;Password=mangookra");
      
        bool email = false;
        public static String SRHR_EMAILADDRESS = "bcortez@fintrac.com";



        static void Main(string[] args)
        {  DBAdapter dbhelper = new DBAdapter();
            bool email = false;
            DataTable dt = dbhelper.getOSEvaluation();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (email == true)
                {
                   dbhelper.updateEvalutionStatus(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), 7);  //to NEED COP REVIEW              
                   dbhelper.SendMailMessage("HRISNotification@fintrac.com", new String[] { dt.Rows[i][2].ToString() }, new String[] { "hr@fintrac.com", dbhelper.getSupervisorEmail(dt.Rows[i][1].ToString()) }, new String[] { "betty@fintrac.com" }, "Evaluation Reviewed", dbhelper.getmsg(dt.Rows[i][3].ToString(), dt.Rows[i][1].ToString(), "overseas"));
                }
                else
                {
                    string msg = dbhelper.getmsg(dt.Rows[i][3].ToString(), dt.Rows[i][1].ToString(), "overseas") ;
                    msg += dt.Rows[i][2].ToString()+ "  " + dbhelper.getSupervisorEmail(dt.Rows[i][1].ToString());
                   
                   dbhelper.updateEvalutionStatus(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), 7);  //to NEED COP REVIEW
                   dbhelper.SendMailMessage("HRISNotification@fintrac.com", new String[] { }, new String[] { "betty@fintrac.com" }, new String[] { }, "Evaluation Reviewed", msg);
                }
            }

            dt = dbhelper.getHOEvaluation();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (email == true)
                {
                    dbhelper.CompleteEvaluation(dt.Rows[i][1].ToString(), dt.Rows[i][4].ToString(), dt.Rows[i][5].ToString(), dt.Rows[i][6].ToString(), dt.Rows[i][0].ToString(),
                     dt.Rows[i][7].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][8].ToString(), dt.Rows[i][9].ToString(), dt.Rows[i][10].ToString(), dt.Rows[i][11].ToString(), dt.Rows[i][12].ToString(), dt.Rows[i][13].ToString());
                   
                    dbhelper.updateEvalutionStatus(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), 6);  //to Complete

                    dbhelper.SendMailMessage("HRISNotification@fintrac.com", new String[] { dt.Rows[i][2].ToString() }, new String[] { "hr@fintrac.com", dbhelper.getSupervisorEmail(dt.Rows[i][1].ToString()) }, new String[] { "betty@fintrac.com" }, "Evaluation Complete", dbhelper.getmsg(dt.Rows[i][3].ToString(), dt.Rows[i][1].ToString(), "usa"));
                }
                else
                {
                    string msg = dbhelper.getmsg(dt.Rows[i][3].ToString(), dt.Rows[i][1].ToString(), "usa");
                    msg += dt.Rows[i][2].ToString() + "  " + dbhelper.getSupervisorEmail(dt.Rows[i][1].ToString());
                    
                    dbhelper.CompleteEvaluation(dt.Rows[i][1].ToString(), dt.Rows[i][4].ToString(), dt.Rows[i][5].ToString(), dt.Rows[i][6].ToString(), dt.Rows[i][0].ToString(),
                       dt.Rows[i][7].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][8].ToString(), dt.Rows[i][9].ToString(), dt.Rows[i][10].ToString(), dt.Rows[i][11].ToString(), dt.Rows[i][12].ToString(), dt.Rows[i][13].ToString());

                    dbhelper.updateEvalutionStatus(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), 6);

                    dbhelper.SendMailMessage("HRISNotification@fintrac.com", new String[] { }, new String[] { "betty@fintrac.com" }, new String[] { }, "Evaluation Complete", msg);
                }
            }

        }

     

    }
}
