using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ABD.ADOrders.Dto;
using ABD.Customers.Dto;
using ABD.Hangfire.Dto;
using Microsoft.AspNetCore.Hosting;

namespace ABD.EmailBuilder
{
    public static class EmailBuilder
    {

        public static SendEmailJobArgs GenerateAdOrderMail(AdOrderMailInput input, string filePath, string baseUrl)
        {
            var pathToFile = filePath
                             + Path.DirectorySeparatorChar
                             + "EmailTemplates"
                             + Path.DirectorySeparatorChar
                             + "AgencyDirectoryOrder.html";

            string body = string.Empty;
            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                body = SourceReader.ReadToEnd();
            }

            body = body.Replace("images/", baseUrl + "EmailTemplates/images/");
            body = body.Replace("{orderId}", input.OrderId.ToString());
            body = body.Replace("{orderName}", input.Description);
            body = body.Replace("{orderDate}", input.OrderDate.ToShortDateString());
            body = body.Replace("{salesPerson}", input.SalesUser);
            body = body.Replace("{listCount}", input.RecordCount.ToString());
            body = body.Replace("{additionalCount}", input.CtCount.ToString());
            body = body.Replace("{customerName}", input.CustomerName);
            body = body.Replace("{paymentType}", input.PaymentType);
            body = body.Replace("{checkNumber}", input.CheckNo);
            body = body.Replace("{amountCharged}", input.OrderValue.ToString());

            var emailJobArgs = new SendEmailJobArgs
            {
                Subject = "Agency Directory Order ("+ input.CustomerName +")",
                Body = body,
                TargetUser = input.TargetUser,
                AttachedFile = "ADReceipt_" + input.OrderId + ".pdf"
            };

            return emailJobArgs;
        }

        public static SendEmailJobArgs GenerateCustomerSignUpMail(CustomerDto input, string filePath)
        {
            var pathToFile = filePath
                             + Path.DirectorySeparatorChar
                             + "EmailTemplates"
                             + Path.DirectorySeparatorChar
                             + "AgencyDirectorySignUp.html";

            string body = string.Empty;
            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                body = SourceReader.ReadToEnd();
            }

            //body = body.Replace("{orderId}", input.OrderId.ToString());
            //body = body.Replace("{orderName}", input.Description);
            //body = body.Replace("{orderDate}", input.OrderDate.ToShortDateString());
            //body = body.Replace("{salesPerson}", input.SalesUser);
            //body = body.Replace("{listCount}", input.RecordCount.ToString());
            //body = body.Replace("{additionalCount}", input.CtCount.ToString());
            //body = body.Replace("{customerName}", input.CustomerName);
            //body = body.Replace("{paymentType}", input.PaymentType);
            //body = body.Replace("{checkNumber}", input.CheckNo);
            //body = body.Replace("{amountCharged}", input.OrderValue.ToString());

            var emailJobArgs = new SendEmailJobArgs
            {
                Subject = "Agency Directory Sign Up - " + input.CompanyName,
                Body = body,
                TargetUser = input.Email,
                AttachedFile = ""
            };

            return emailJobArgs;
        }
    }
}
