﻿using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace StaticSharp.Gears;


public abstract record Genome: KeyProvider {

    public abstract object Create();

    private static readonly JsonSerializerOptions JsonSerializerOptions = new() {
        IncludeFields = true,

    };
    private static readonly string CachedDataJsonFileName = "data.json";
    protected string CachedDataJsonFilePath => Path.Combine(CacheSubDirectory, CachedDataJsonFileName);
    protected string CacheSubDirectory => Path.Combine(Cache.Directory, KeyHash);
    protected string ContentFilePath => Path.Combine(CacheSubDirectory, "content");
    protected string KeyHash => Hash.CreateFromString(Key).ToString();

    public virtual Genome[]? Sources => null;

    protected bool LoadData<T>(out T data) where T : new() {
        if (!File.Exists(CachedDataJsonFilePath)) {
            data = new();
            return false;
        }

        var json = FileUtils.ReadAllText(CachedDataJsonFilePath);
        var deserializationResult = JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
        if (deserializationResult == null) {
            data = new();
            return false;
        }
        data = deserializationResult;
        return true;
    }

    protected void StoreData<T>(T data) {
        string json = JsonSerializer.Serialize(data, JsonSerializerOptions);
        File.WriteAllText(CachedDataJsonFilePath, json);
    }

    protected void CreateCacheSubDirectory() {
        Directory.CreateDirectory(CacheSubDirectory);
    }

    public void DeleteCacheSubDirectory() {
        DeleteDirectory(CacheSubDirectory);
    }

    private void DeleteDirectory(string directoryPath) {
        DirectoryInfo dir = new DirectoryInfo(directoryPath);

        foreach (FileInfo file in dir.GetFiles()) {
            file.Delete();
        }

        foreach (DirectoryInfo directory in dir.GetDirectories()) {
            DeleteDirectory(directory.FullName);
            directory.Delete();
            while (directory.Exists) {
                Thread.Sleep(100);
            }
        }
    }


    


}




public abstract record Genome<TCacheable> : Genome where TCacheable : class {
    public TCacheable CreateOrGetCached() {
        return Cache.CreateOrGet(this);    
    }

    public override abstract TCacheable Create();
}







