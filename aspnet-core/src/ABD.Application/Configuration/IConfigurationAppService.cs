using System.Threading.Tasks;
using ABD.Configuration.Dto;

namespace ABD.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
