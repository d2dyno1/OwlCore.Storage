﻿using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OwlCore.Storage;

/// <summary>
/// A file implementation which calls GET on a provided HTTP URL and returns it for <see cref="OpenStreamAsync"/>.
/// </summary>
public class HttpFile : IFile
{
    /// <summary>
    /// Creates a new instance of <see cref="HttpFile"/>.
    /// </summary>
    /// <param name="uri">The http address to GET for the file content.</param>
    public HttpFile(Uri uri)
        : this(uri, new HttpClient(new HttpClientHandler()))
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="HttpFile"/>.
    /// </summary>
    /// <param name="uri">The http address to GET for the file content.</param>
    public HttpFile(string uri)
        : this(uri, new HttpClient(new HttpClientHandler()))
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="HttpFile"/>.
    /// </summary>
    /// <param name="uri">The http address to GET for the file content.</param>
    /// <param name="httpClient">The client to use for requests.</param>
    public HttpFile(Uri uri, HttpClient httpClient)
    {
        Uri = uri;
        Name = Path.GetFileName(uri.AbsolutePath);
        Id = uri.OriginalString;

        Client = httpClient;
    }

    /// <summary>
    /// Creates a new instance of <see cref="HttpFile"/>.
    /// </summary>
    /// <param name="uri">The http address to GET for the file content.</param>
    /// <param name="httpClient">The client to use for requests.</param>
    public HttpFile(string uri, HttpClient httpClient)
        : this(new Uri(uri), httpClient)
    {
    }

    /// <summary>
    /// The message handler to use for making HTTP requests.
    /// </summary>
    public HttpClient Client { get; init; }

    /// <summary>
    /// The http address to GET for the file content.
    /// </summary>
    public Uri Uri { get; }

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public string Name { get; init; }

    /// <inheritdoc />
    public Task<Stream> OpenStreamAsync(FileAccess accessMode = FileAccess.Read, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (accessMode == 0)
            throw new ArgumentOutOfRangeException(nameof(accessMode), $"{nameof(FileAccess)}.{accessMode} is not valid here.");

        if (accessMode == FileAccess.Write)
            throw new NotSupportedException($"{nameof(FileAccess)}.{accessMode} is not supported over Http.");

        return Client.GetStreamAsync(Uri);
    }
}