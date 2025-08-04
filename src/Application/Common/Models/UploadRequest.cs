// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Models;

public class UploadRequest(
    string fileName,
    UploadType uploadType,
    byte[] data,
    bool overwrite = false,
    string? folder = null)
{
    public string FileName { get; set; } = fileName;
    public string? Extension { get; set; }
    public UploadType UploadType { get; set; } = uploadType;
    public bool Overwrite { get; set; } = overwrite;
    public byte[] Data { get; set; } = data;
    public string? Folder { get; set; } = folder;
}