using System;
using System.Collections.Generic;

namespace T4TS
{
    public class Settings
    {
        /// <summary>
        ///     The default module of the generated interface, if not specified by the TypeScriptInterfaceAttribute
        /// </summary>
        public string DefaultModule { get; set; }

        /// <summary>
        ///     The default value for Optional, if not specified by the TypeScriptMemberAttribute
        /// </summary>
        public bool DefaultOptional { get; set; }

        /// <summary>
        ///     The default value for the CamelCase flag for an interface member name, if not specified by the
        ///     TypeScriptMemberAttribute
        /// </summary>
        public bool DefaultCamelCaseMemberNames { get; set; }

        /// <summary>
        ///     The default string to prefix interface names with. For instance, you might want to prefix the names with an "I" to
        ///     get conventional interface names.
        /// </summary>
        public string DefaultInterfaceNamePrefix { get; set; }

        /// <summary>
        ///     The default string to prefix enum names with. For instance, you might want to prefix the names with an "E" to get
        ///     conventional enum names.
        /// </summary>
        public string DefaultEnumNamePrefix { get; set; }

        /// <summary>
        ///     The version of Typescript that is targeted
        /// </summary>
        public Version CompatibilityVersion { get; set; }

        /// <summary>
        ///     If true translates System.DateTime to native date
        /// </summary>
        public bool UseNativeDates { get; set; }

        /// <summary>
        ///     List of the project names to process. If null - all the projects will be processed.
        /// </summary>
        public string[] ProjectNamesToProcess { get; set; }

        /// <summary>
        ///     If equals <c>true</c> - classes marked with <see cref="DataContractAttribute"/> will be processed.
        /// </summary>
        public bool ProcessDataContracts { get; set; }

        /// <summary>
        ///     If equals <c>true</c> - parent classes will be processed even if they weren't marked with <see cref="TypeScriptInterfaceAttribute"/> or <see cref="DataContractAttribute"/>.
        /// </summary>
        public bool ProcessParentClasses { get; set; }

        /// <summary>
        ///     Additional list of the additional attribute short names which prevents processing of the members. 
        ///     If null - the properties marked with <see cref="JsonIgnoreAttribute"/> will be ignored.
        /// </summary>
        public string[] MemberIgnoreAttributes { get; set; }


        public Settings()
        {
            DefaultModule = "T4TS";
            MemberIgnoreAttributes = new[] {"JsonIgnoreAttribute"};
        }

        public static Settings Parse(Dictionary<string, object> settingsValues)
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
                ProjectNamesToProcess = ParseSettingReferenceType(settingsValues, "ProjectNamesToProcess", s => s == null ? null : s.ToString().Replace(" ", "").Split(','), null),
                ProcessDataContracts = ParseSettingNullableType(settingsValues, "ProcessDataContracts", false),
                ProcessParentClasses = ParseSettingNullableType(settingsValues, "ProcessParentClasses", false),
                MemberIgnoreAttributes = ParseSettingReferenceType(settingsValues, "MemberIgnoreAttributes", s => s as string, "JsonIgnoreAttribute").Split(','),
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
                return (val as T?) ?? defaultValue;
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