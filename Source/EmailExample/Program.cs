using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;

namespace EmailExample
{
    public class EmailExample
    {
        public static void Main(string[] args)
        {
            TestExampleData testExampleData = GetTestExampleData();
            MailAddress fromAddress = new MailAddress(testExampleData.FromAddress, "From Name");
            MailAddress toAddress = new MailAddress(testExampleData.ToAddress, "To Name");
            string fromPassword = testExampleData.FromPassword;
            const string subject = "Subject";
            string body = testExampleData.TestInterestingData;

            SmtpClient smtp = new SmtpClient
            {
                Host = testExampleData.SmtpServer,
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (MailMessage message = new MailMessage(fromAddress, toAddress) { Subject = subject, Body = body })
            {
                smtp.Send(message);
            }
        }

        static TestExampleData GetTestExampleData()
        {
            int emailId = 1;
            string fromAddress = "";
            string fromPassword = "";
            string smtpServer = "";
            string testInterestingData = "";
            string toAddress = "";

            using (SqlConnection myConnection = new SqlConnection(@"Server=(localdb)\Projects;Integrated Security=true;"))
            {
                var commandText = "USE exampleDatabase " +
                                  "SELECT EmailId, FromAddress, FromPassword, SmtpServer, TestInterestingData, ToAddress " +
                                  "FROM TestExampleEmail";
                SqlCommand cmd = new SqlCommand(commandText, myConnection);                          
                myConnection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {    
                        emailId = (int)reader["EmailId"];
                        fromAddress = reader["FromAddress"].ToString();   
                        fromPassword = reader["FromPassword"].ToString();
                        smtpServer = reader["SmtpServer"].ToString();
                        testInterestingData = reader["TestInterestingData"].ToString();
                        toAddress = reader["ToAddress"].ToString();
                    }

                    myConnection.Close();
                }               
            }

            return new TestExampleData
            {
                EmailId = emailId,
                FromAddress = fromAddress,
                FromPassword = fromPassword,
                SmtpServer = smtpServer,
                TestInterestingData = testInterestingData,
                ToAddress = toAddress
            };
        }
    }

    public class TestExampleData
    {
        public int EmailId { get; set; }
        public string FromAddress { get; set; }
        public string FromPassword { get; set; }
        public string SmtpServer { get; set; }
        public string TestInterestingData { get; set; }
        public string ToAddress { get; set; }        
    }
}