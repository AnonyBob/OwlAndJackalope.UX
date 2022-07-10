using System;

namespace OJ.UX.Runtime.References.Serialized
{
    public class SerializedDetailDisplayAttribute : Attribute
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

        public SerializedDetailDisplayAttribute(string displayName, string folderName = null)
        {
            DisplayName = displayName;
            FolderName = folderName ?? string.Empty;
        }
    }
}