using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace hangfireDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        /*
             .---------------- นาที (0 - 59) 
             |  .------------- ชั่วโมง (0 - 23)
             |  |  .---------- วันที่ของเดือน (1 - 31)
             |  |  |  .------- เลขเดือน (1 - 12) หรือตัวย่อชื่อเดือน 3 ตัวแรก jan,feb,mar,apr ... 
             |  |  |  |  .---- วันในสัปดาห์ (0 - 6) (อาทิตย์=0 หรือ 7) หรือตัวย่อชื่อวัน 3 ตัวแรก sun,mon ...
             *  *  *  *  *  <command to be executed คำสั่งหรือ path ไฟล์ที่จะเรียกใหทำงาน>

         */
        private readonly IBackgroundJobClient _backgroundJob;
        public JobsController(IBackgroundJobClient backgroundJob)
        {
            _backgroundJob = backgroundJob;
        }
        [HttpPost]
        public string Test()
        {
            string Result = "Ok";
            try
            {
                string StartTime = "0 5,7,9,11,13,15,17,19 * * * MON,TUE,WED,THU,FRI";
                RecurringJob.AddOrUpdate(
                  () => SendMail(),
                    StartTime);

            }
            catch (Exception ex)
            {
                Result = ex.Message;
            }
            return Result;
        }

        public void SendMail()
        {
            string _body = string.Format("Test Run at : {0}",DateTime.Now);
            using (var message = new MailMessage())
            {
                message.To.Add(new MailAddress("jerateep.123@gmail.com", "jerateep.s"));
                message.From = new MailAddress("jerateep.s@gmail.com", "Admin Hangfire");
                message.Subject = "Hangfire";
                message.Body =_body;
                message.IsBodyHtml = true;

                using (var client = new SmtpClient("smtp.gmail.com"))
                {
                    client.UseDefaultCredentials = true;
                    client.Port = 587;
                    client.Credentials = new NetworkCredential("jerateep.s@gmail.com", "xxxxxxx");
                    client.EnableSsl = true;
                    client.Send(message);
                }
            }
        }
    }
}
