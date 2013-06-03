using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Net.Mail;
using System.Net;

namespace EvalStatusChanger
{
    public partial class DBAdapter : Component
    {
        bool email = false;
        public static String SRHR_EMAILADDRESS = "bcortez@fintrac.com";
        private SqlConnection con;
        private SqlDataAdapter dataAdapter;
        private string conString = "Data Source=. \\Fintrac ;Initial Catalog=Timesheet;User ID=xyz;Password=mangookra";
        public DBAdapter()
        {
            InitializeComponent();
            con = new SqlConnection(conString);
        }

        public DBAdapter(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }



        public  void open()
        {
            if (con.State == ConnectionState.Closed) con.Open();
        }

        public void close()
        {
            if (con.State == ConnectionState.Open) con.Close();
        }

        public  SqlConnection getCon()
        {
            return con;
        }


        public SqlCommand getSqlCommand(String query)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = getCon();
            open();
            cmd.CommandTimeout = 60;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query;
            return cmd;
        }


        public DataTable popualateDataTable(String query)
        {
            dataAdapter = new SqlDataAdapter(query, conString);
            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            dataAdapter.Fill(table);
            return table;
        }

        public String DataReader(String query)
        {
            if (getCon().State == ConnectionState.Closed) con.Open();
            String value = null;
            SqlCommand cmd = getSqlCommand(query);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                value = (String)dr.GetValue(0).ToString();
                break;
            }
            dr.Close();
            return (value == "") ? null : value;
        }

        private SqlCommand addParameters(String query, String[] parameters, String[] values)
        {
            SqlCommand cmd = getSqlCommand(query);
            int j = 0;
            foreach (String parameter in parameters)
            {
                if (values[j] == "" || values[j] == null)
                {
                    cmd.Parameters.AddWithValue("@" + parameter, DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@" + parameter, values[j]);
                }
                j += 1;
            }
            return cmd;
        }

        public Boolean ShowLocalCurrency( String employeeid)
        {
            if (isExpat(employeeid) == true)
            {
                return false;
            }
            string localcurrency = DataReader("SELECT   [showlocalcurrency]  FROM [Timesheet].[dbo].[Location3] where ID = '" + getlocatonid(employeeid) + "'");
            if (localcurrency == "y")
                return true;
            else
                return false;
        }

        public String getlocatonid(String employeeid)
        {
            return DataReader("select location3id from employees where id = '" + employeeid + "'");
        }

        public String GetEffectinveDate(String employeeid, String Year)
        {
            String seffectivedate = DataReader("Select AdjustedAnniversaryDate From employees where ID = '" + employeeid + "' ");
            String DTeffectivedate = Convert.ToDateTime(seffectivedate).Month.ToString() + "/" + Convert.ToDateTime(seffectivedate).Day.ToString() + "/" + Year;//Convert.ToDateTime(seffectivedate).AddYears(DateTime.Now.Year - Convert.ToDateTime(seffectivedate).Year).Year.ToString();
            return DTeffectivedate;
        }


        public String getJobTitleId(String position)
        {
            return DataReader("select id from positiontitle where name = '" + position + "'");
        }

        public Boolean ShowLocalCurrency(string locatonID, String employeeid)
        {
            if (isExpat(employeeid) == true)
            {
                return false;
            }
            string localcurrency = DataReader("SELECT   [showlocalcurrency]  FROM [Timesheet].[dbo].[Location3] where ID = '" + locatonID + "'");
            if (localcurrency == "y")
                return true;
            else
                return false;
        }

        public Boolean isExpat(String employeeid)
        {
            String expat = DataReader("SELECT  ExpatType.Name FROM   Employees INNER JOIN  " +
                   "   ExpatType ON Employees.ExpatTypeID = ExpatType.ID  where Employees.ID = '" + employeeid + "'");

            return (expat.ToLower() != "non expat") ? true : false;
        }


        public String getSalaryIncreaseReason()
        {
            return DataReader("select id from SalaryIncreaseReason where name = 'Evaluation'");
        }

        public String getEAFChangeReasonId(string changeReason)
        {
            return DataReader("select id from eafchangereason where name = '" + changeReason + "'");
        }

        public String getSupervisorEmail(String empId)
        {

           return DataReader( "SELECT   Employees_1.Email  FROM  Employees INNER JOIN  Employees AS Employees_1 ON Employees.PrimarySupervisorID = Employees_1.ID WHERE Employees.ID = '" + empId + "'");
                      
          
        }
        public void insertEmployeeHistory(String employeeId, String name, String firstname, String lastname, String evaluationId,
         String supervisorId, String location, String email, String date, String salaryIncreaseReasonId, String payRateUS, String eafchangereasonid, String approvedbyid, String Changeto, String SalaryIncreaseEffectiveDate, String changedbyid, String PayRateCurrency, String approveddate)
        {



            String query = "insert into employeesHistory (employeeId, name, firstname, lastname, evaluationId, primarySupervisorID,  location3ID, email, [Date], salaryIncreaseReasonId, payRateUS, eafchangereasonid, approvedby, Changeto, SalaryIncreaseEffectiveDate,changedbyid,EafChangeCOPReviewed, COPReviewedDate,PayRateCurrency,approveddate ) values" +
                " (@employeeId, @name, @firstname, @lastname, @evaluationId, @primarySupervisorID,  @location3ID, @email, @Date, @salaryIncreaseReasonId, @payRateUS, @eafchangereasonid, @approvedby, @Changeto, @SalaryIncreaseEffectiveDate,@changedbyid,@EafChangeCOPReviewed, @COPReviewedDate,@PayRateCurrency,@approveddate )";

            String[] parameters = new String[] { "employeeId", "name", "firstname", "lastname", "evaluationId", "primarySupervisorID", "location3ID", "email", "Date", "salaryIncreaseReasonId", "payRateUS", "eafchangereasonid", "approvedby", "Changeto", "SalaryIncreaseEffectiveDate", "changedbyid", "EafChangeCOPReviewed", "COPReviewedDate", "PayRateCurrency", "approveddate" };
            String[] values = new String[] { employeeId, name, firstname, lastname, evaluationId, supervisorId,  location, email, date, salaryIncreaseReasonId, payRateUS, eafchangereasonid, approvedbyid, Changeto, SalaryIncreaseEffectiveDate, changedbyid, getCOPReviwer(evaluationId), getCOPReviwerDate(evaluationId), PayRateCurrency, approveddate };
            SqlCommand cmd = addParameters(query, parameters, values);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            close();

        }

        public String getCOPReviwer(String evaluationid)
        {
            return DataReader("select COPReviewer from  EmployeeEvaluation where ID = '" + evaluationid + "'");
        }

        public String getCOPReviwerDate(String evaluationid)
        {
            return DataReader("select COPReviewerDate from EmployeeEvaluation  where ID = '" + evaluationid + "'");
        }

        public String geteidfromemail(String email)
        {
            return DataReader("select id from Employees where email = '" + email + "'");
        }


        public void updadteSalary(String employeeId, String salary, string PayRateCurrency)
        {
            String query = string.Empty;
            if (PayRateCurrency == "")
                query = "update Employees set payrateus = " + salary + " where id = '" + employeeId + "'";
            else
                query = "update Employees set payrateus = " + salary + ", PayRateCurrency=" + PayRateCurrency + " where id = '" + employeeId + "'";
            SqlCommand cmd = getSqlCommand(query);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            close();
        }
        public bool CompleteEvaluation(String employeeid, string fullname, String firstname, String lastname, String evaluationid, String primarysupid, String email, string locaitonid, string localsalary, string ussalary,
         string evaluationsalaryincrease, string evaluationPeriod, string evaldate)
        {

            try
            {

                if (ShowLocalCurrency(employeeid) == true)
                {
                    Double Localsalary = (Convert.ToDouble(localsalary) * (Convert.ToDouble(evaluationsalaryincrease) / 100)) + Convert.ToDouble(localsalary);
                    Double USDsalary = (Convert.ToDouble(ussalary) * (Convert.ToDouble(evaluationsalaryincrease) / 100)) + Convert.ToDouble(ussalary);

                    insertEmployeeHistory(employeeid, fullname, firstname, lastname, evaluationid,
                  primarysupid, locaitonid, email, evaldate, getSalaryIncreaseReason(), USDsalary.ToString(), getEAFChangeReasonId("Local Salary")
                  , geteidfromemail(SRHR_EMAILADDRESS), Math.Round(Localsalary, 2).ToString(), GetEffectinveDate(employeeid, evaluationPeriod.Substring(evaluationPeriod.Length - 2)), primarysupid, localsalary, DateTime.Now.Date.ToString());

                    updadteSalary(employeeid, Math.Round(USDsalary, 2).ToString(), Math.Round(Localsalary, 2).ToString());

                }
                else
                {
                    Double USDsalary = (Convert.ToDouble(ussalary) * (Convert.ToDouble(evaluationsalaryincrease) / 100)) + Convert.ToDouble(ussalary);

                    insertEmployeeHistory(employeeid, fullname, firstname, lastname, evaluationid,
                        primarysupid, locaitonid, email, evaldate, getSalaryIncreaseReason(), ussalary, getEAFChangeReasonId("Salary")
                      , geteidfromemail(SRHR_EMAILADDRESS), Math.Round(USDsalary, 2).ToString(), GetEffectinveDate(employeeid, evaluationPeriod.Substring(evaluationPeriod.Length - 2)), primarysupid, "", DateTime.Now.Date.ToString());

                    updadteSalary(employeeid, ussalary, "");
                }
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }




        public string getmsg(string submitdate, string employeeid, string location)
        {
            string msg = "Dear  " + getFirstName(employeeid) + ", <br /> <br />";

            if (location == "overseas")
                msg += "Your evaluation has been approved by your supervisor since " + Convert.ToDateTime(submitdate).ToString("MMMM dd, yyyy") + ", but it is still pending in the HRIS system awaiting input from you.  To finalize the " +
            "processing of your performance review and the accurate reporting of your information, the system has set the status of your " +
            "evaluation to “Need COP Review” as of " + DateTime.Now.Date.ToString("MMMM dd, yyyy") + ". Feel free to view your evaluation, please log on to hr.fintrac.com, clicking on the “Employee Evaluation,” then selecting  the evaluation you would like to view. <br /><br />";
            else
                msg += "Your evaluation has been approved by your supervisor and HR since " + Convert.ToDateTime(submitdate).ToString("MMMM dd, yyyy") + ", but it is still pending in the HRIS system awaiting input from you.  To finalize  the " +
             "processing of your performance review and the accurate reporting of your information, the system has set the status of your "+
             "evaluation to “complete” as of " + DateTime.Now.Date.ToString("MMMM dd, yyyy") + ". Feel free to view your evaluation, please log on to hr.fintrac.com, clicking on the “Employee Evaluation,” then selecting the evaluation would like to view.<br /> <br />";

             msg += "Please contact HR@fintrac.com if you have any questions ";
            return msg;
        }

        public string getFirstName(string employeeid)
        {
            return popualateDataTable("select firstname  from employees where id = '" + employeeid + "'").Rows[0][0].ToString();
        }
        public DataTable getOSEvaluation()
        {
            return popualateDataTable(" SELECT        evaluationid, EmployeeID, employeeemial, SubmitDate, NameLF, FirstName, LastName, primarysupid, Location3ID, payRateLocal, payRateUS, " +
                     "    SalaryIncreasePercent, EvaluationPeriod, EvaluationDate" +
                  "  FROM            vu_ForceEmpEvaltoNextStatus WHERE        (location = 'overseas')");
        }
        public DataTable getHOEvaluation()
        {
            return popualateDataTable("SELECT        evaluationid, EmployeeID, employeeemial, SubmitDate, NameLF, FirstName, LastName, primarysupid, Location3ID, payRateLocal, payRateUS, " +
                       "  SalaryIncreasePercent, EvaluationPeriod, EvaluationDate" +
                        "  FROM            vu_ForceEmpEvaltoNextStatus where location = 'usa'");
        }

        public void updateEvalutionStatus(String EvaluaitonId, String employeeid, int status)
        {

            String query = "UPDATE   EmployeeEvaluation SET   Status = '" + status + "', SystemReviewDate = '" + DateTime.Now.Date.ToString() + "'  WHERE     (ID = '" + EvaluaitonId + "')";
            SqlCommand cmd = getSqlCommand(query);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            close();
        }



        public void SendMailMessage(string from, string[] to, string[] bcc, string[] cc, string subject, string body)
        {
            MailMessage mMailMessage = new MailMessage();
            mMailMessage.From = new MailAddress(from);

            if (to != null)
            {
                for (int x = 0; x < to.Length; x++)
                {
                    mMailMessage.To.Add(new MailAddress(to[x]));
                }
            }

            if (bcc != null)
            {
                for (int y = 0; y < bcc.Length; y++)
                {
                    mMailMessage.Bcc.Add(new MailAddress(bcc[y]));
                }
            }

            if (cc != null)
            {
                for (int z = 0; z < cc.Length; z++)
                {
                    mMailMessage.CC.Add(new MailAddress(cc[z]));
                }
            }

            mMailMessage.Subject = subject;
            mMailMessage.Body = string.Format(body, "\r\n");
            mMailMessage.IsBodyHtml = true;
            mMailMessage.Priority = MailPriority.Normal;

            NetworkCredential basicAuthenticationInfo = new System.Net.NetworkCredential("hrisnotification", "mango");

            SmtpClient mSmtpClient = new SmtpClient();

            mSmtpClient.Host = "webmail.fintrac.com";

            mSmtpClient.Port = 25;
            mSmtpClient.Credentials = basicAuthenticationInfo;
            mSmtpClient.Send(mMailMessage);
        }

    }
}
