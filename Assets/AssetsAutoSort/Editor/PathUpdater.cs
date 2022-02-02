using System;
using System.IO;
using UnityEngine;

public class PathUpdater
{
    public static string GetAbsolutePath(string projectRelativePath) {
        return NormalizePathSlashes(Path.Combine(Path.GetDirectoryName(NormalizePathSlashes(Application.dataPath)), projectRelativePath));
    }

    public static string GetProjectRelativePath(string absolutePath) {
        return NormalizePathSlashes(Path.Combine("Assets", GetRelativePath(absolutePath, Application.dataPath)));
    }

    public static string GetRelativePath(string path, string relativeTo) {
        var normalizedPath = NormalizePathSlashes(path);
        var normalizedRelativeTo = NormalizePathSlashes(relativeTo);
        if (!normalizedPath.StartsWith(normalizedRelativeTo, StringComparison.OrdinalIgnoreCase))
            return null;

        return normalizedPath.Remove(0, normalizedRelativeTo.Length + 1);
    }

    /// <summary>
    /// Unsure that path uses backslashes on windows, otherwise left as is
    /// </summary>
    /// <param name="path">Path to normalize</param>
    /// <returns>Normalized path with correct slashes</returns>
    public static string NormalizePathSlashes(string path) {
        if (Application.platform == RuntimePlatform.WindowsEditor) {
            return path.Replace('/', '\\');
        }
        return path;
    }
}