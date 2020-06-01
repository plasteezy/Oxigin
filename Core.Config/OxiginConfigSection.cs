using System.Configuration;

namespace Core.Config
{
    public class OxiginConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("modules", IsRequired = true)]
        public ProviderSettingsCollection Modules
        {
            get { return (ProviderSettingsCollection)base["modules"]; }
            set { base["modules"] = value; }
        }
    }
}