using CCLLC.CDS.ProxyGenerator;
using System.IO;


namespace CCLLC.CDS.ProxyBuilderCmd.Extensions
{
    public static class SettingsExtensions
    {
        public static ISettings TemplateRelativeTo(this ISettings settings, string path)
        {
            if (Path.IsPathRooted(settings.TemplateFilePath))
                return settings;

            return new Settings
            {
                ActionsToExclude = settings.ActionsToExclude,
                ActionsToInclude = settings.ActionsToInclude,
                EntitiesToExclude = settings.EntitiesToExclude,
                EntitiesToInclude = settings.EntitiesToInclude,
                TargetEndPoint = settings.TargetEndPoint,
                TemplateFilePath = Path.Combine(path, settings.TemplateFilePath),
                TemplateLanguage = settings.TemplateLanguage,
                Namespace = settings.Namespace,
                OutputPath = settings.OutputPath
            };
        }

        public static ISettings OutputRelativeTo(this ISettings settings, string path)
        {
            if (Path.IsPathRooted(settings.OutputPath))
                return settings;

            return new Settings
            {
                ActionsToExclude = settings.ActionsToExclude,
                ActionsToInclude = settings.ActionsToInclude,
                EntitiesToExclude = settings.EntitiesToExclude,
                EntitiesToInclude = settings.EntitiesToInclude,
                TargetEndPoint = settings.TargetEndPoint,
                TemplateFilePath = settings.TemplateFilePath,
                TemplateLanguage = settings.TemplateLanguage,
                Namespace = settings.Namespace,
                OutputPath = Path.Combine(path, settings.OutputPath)
            };
        }
    }
}
