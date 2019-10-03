using Abp.Configuration;
using Abp.MailKit;
using Abp.Net.Mail;
using Abp.Net.Mail.Smtp;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace ABD.Configuration
{
    public class MySmtpEmailSenderConfiguration : DefaultMailKitSmtpBuilder
    {
        ISettingManager settingManager;

        public virtual string Host => settingManager.GetSettingValue(EmailSettingNames.Smtp.Host);
        public virtual int Port => settingManager.GetSettingValue<int>(EmailSettingNames.Smtp.Port);
        public virtual string UserName => settingManager.GetSettingValue(EmailSettingNames.Smtp.UserName);
        public virtual string Password => settingManager.GetSettingValue(EmailSettingNames.Smtp.Password);
        public virtual string Domain => settingManager.GetSettingValue(EmailSettingNames.Smtp.Domain);
        public virtual bool EnableSsl => settingManager.GetSettingValue<bool>(EmailSettingNames.Smtp.EnableSsl);
        public virtual bool UseDefaultCredentials => settingManager.GetSettingValue<bool>(EmailSettingNames.Smtp.UseDefaultCredentials);


        public MySmtpEmailSenderConfiguration(ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration,IAbpMailKitConfiguration iAbpMailKitConfiguration)
        : base(smtpEmailSenderConfiguration, iAbpMailKitConfiguration)
        {
           
        }
        //protected override void ConfigureClient(SmtpClient client)
        //{
            
        //    client.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
        //    //client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
        //    base.ConfigureClient(client);
        //}
    }
}
