using UnityEngine;
#nullable enable

public class FilePathSelectorAttribute: PropertyAttribute {
    public string Title { get; }
    public string Extension { get; }

    public FilePathSelectorAttribute(string title, string extension = "*") {
        Title = title;
        Extension = extension;
    }
}
