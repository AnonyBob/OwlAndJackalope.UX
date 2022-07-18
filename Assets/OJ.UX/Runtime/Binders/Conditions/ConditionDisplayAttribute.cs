using System;

namespace OJ.UX.Runtime.Binders.Conditions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConditionDisplayAttribute : Attribute
    {
        public readonly string DisplayName;
        public readonly string FolderName;

        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(FolderName))
                    return DisplayName;

                return $"{FolderName}/{DisplayName}";
            }
        }

        public ConditionDisplayAttribute(string displayName, string folderName = null)
        {
            DisplayName = displayName;
            FolderName = folderName ?? string.Empty;
        }
    }
}