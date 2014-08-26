using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class Settings
    {
        /// <summary>
        /// The default module of the generated interface, if not specified by the TypeScriptInterfaceAttribute
        /// </summary>
        public string DefaultModule { get; set; }

        /// <summary>
        /// The default value for Optional, if not specified by the TypeScriptMemberAttribute
        /// </summary>
        public bool DefaultOptional { get; set; }

        /// <summary>
        /// The default value for the CamelCase flag for an interface member name, if not specified by the TypeScriptMemberAttribute
        /// </summary>
        public bool DefaultCamelCaseMemberNames { get; set; }

        /// <summary>
        /// The default string to prefix interface names with. For instance, you might want to prefix the names with an "I" to get conventional interface names.
        /// </summary>
        public string DefaultInterfaceNamePrefix { get; set; }

        /// <summary>
        /// The version of Typescript that is targeted
        /// </summary>
        public Version CompatibilityVersion { get; set; }

        /// <summary>
        /// If true translates System.DateTime to native date
        /// </summary>
        public bool UseNativeDates { get; set; }

        /// <summary>
        /// List of the project names to process. If null - all the projects will be processed.
        /// </summary>
        public string[] ProjectNamesToProcess { get; set; }

        public static Settings Parse(Dictionary<string,object> settingsValues)
        {
            // Read settings from T4TS.tt.settings.tt
            return new Settings
            {
                DefaultModule = ParseSettingReferenceType(settingsValues, "DefaultModule", s => s as string, "T4TS"),
                DefaultOptional = ParseSettingNullableType(settingsValues, "DefaultOptional", false),
                DefaultCamelCaseMemberNames = ParseSettingNullableType(settingsValues, "DefaultCamelCaseMemberNames", false),
                DefaultInterfaceNamePrefix = ParseSettingReferenceType(settingsValues, "DefaultInterfaceNamePrefix", s => s as string, string.Empty),
                CompatibilityVersion = ParseSettingReferenceType(settingsValues, "CompatibilityVersion", v => v as Version, new Version(0, 9, 1, 1)),
                UseNativeDates = ParseSettingNullableType(settingsValues, "UseNativeDates", false),
                ProjectNamesToProcess = ParseSettingReferenceType(settingsValues, "ProjectNamesToProcess", s => s == null ? null : s.ToString().Replace(" ", "").Split(','), null)
            };
        }

        private static T ParseSettingReferenceType<T>(Dictionary<string, object> settingsValues, string key, Func<object, T> convert, T defaultValue) where T : class
        {
            object val;
            if (settingsValues.TryGetValue(key, out val))
                return convert(val) ?? defaultValue;

            return defaultValue;
        }

        private static T ParseSettingNullableType<T>(Dictionary<string, object> settingsValues, string key, T defaultValue) where T : struct
        {
            object val;
            if (settingsValues.TryGetValue(key, out val))
            {
                var nullable = val as Nullable<T>;
                if (nullable == null || !nullable.HasValue)
                    return defaultValue;

                return nullable.Value;
            }

            return defaultValue;
        }

        private static T ParseConfigValueType<T>(Dictionary<string, object> settingsValues, string key, Func<object, T> convert, T defaultValue)
        {
            object val;
            if (settingsValues.TryGetValue(key, out val))
                return convert(val);

            return defaultValue;
        }
    }
}
