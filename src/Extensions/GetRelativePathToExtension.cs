﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OwlCore.Storage;

public static partial class FolderExtensions
{
    /// <summary>
    /// Crawls the ancestors of <paramref name="to" /> until <paramref name="from"/> is found, then returns the constructed relative path.
    /// </summary>
    /// <remarks>Example code. Not tested.</remarks>
    public static async Task<string> GetRelativePathToAsync(this IFolder from, IStorableChild to)
    {
        var pathComponents = new List<string>
        {
            to.Name,
        };

        await RecursiveAddParentToPathAsync(to);

        // Relative path to a folder should end with a directory separator '/'
        // Relative path to a file should end with the file name.
        return to switch
        {
            IFolder => $"/{string.Join(@"/", pathComponents)}/",
            IFile => $"/{string.Join(@"/", pathComponents)}",
            _ => throw new NotSupportedException($"{to.GetType()} is not an {nameof(IFile)} or an {nameof(IFolder)}. Unable to generate a path."),
        };

        async Task RecursiveAddParentToPathAsync(IStorableChild item)
        {
            var parent = await item.GetParentAsync();
            if (parent is IStorableChild child && parent.Id != from.Id)
            {
                pathComponents.Insert(0, item.Name);
                await RecursiveAddParentToPathAsync(child);
            }
        }
    }
}