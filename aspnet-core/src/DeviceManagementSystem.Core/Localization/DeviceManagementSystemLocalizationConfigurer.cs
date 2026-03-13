using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace DeviceManagementSystem.Localization
{
    public static class DeviceManagementSystemLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(DeviceManagementSystemConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(DeviceManagementSystemLocalizationConfigurer).GetAssembly(),
                        "DeviceManagementSystem.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
