using System.Collections.Generic;
using Abp.Configuration;
using Abp.Net.Mail;
using ABD.Domain.Dtos;
using Microsoft.Extensions.Options;

namespace ABD.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        private readonly SmtpSettings _smtpSettings;

        public AppSettingProvider(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(AppSettingNames.UiTheme, "red", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, isVisibleToClients: true),
                /////Email Settings
                new SettingDefinition(EmailSettingNames.Smtp.Host, _smtpSettings.Host),
                new SettingDefinition(EmailSettingNames.Smtp.Port, _smtpSettings.Port),
                new SettingDefinition(EmailSettingNames.Smtp.UserName, _smtpSettings.User),
                new SettingDefinition(EmailSettingNames.Smtp.Password, _smtpSettings.Pass),
               // new SettingDefinition(EmailSettingNames.Smtp.Domain, "gmail.com"),
                new SettingDefinition(EmailSettingNames.Smtp.EnableSsl,_smtpSettings.EnableSsl),
                new SettingDefinition(EmailSettingNames.Smtp.UseDefaultCredentials, _smtpSettings.UseDefaultCredentials),
                new SettingDefinition(EmailSettingNames.DefaultFromAddress, _smtpSettings.FromAddress),
                new SettingDefinition(EmailSettingNames.DefaultFromDisplayName, _smtpSettings.DisplayName)
            };
        }
    }
}
