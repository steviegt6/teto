using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using ShareX.UploadersLib.FileUploaders;
using Tomat.FNB.TMOD;
using Tomat.FNB.TMOD.Converters.Extractors;
using Tomat.FNB.TMOD.Utilities;

namespace Tomat.Teto.Plugin.Tml.Extract.Services;

public sealed class TmlExtractService(IServiceProvider services)
{
    private sealed class ExtractRequest(IAttachment attachment, IServiceProvider services)
    {
        public volatile bool Done;
        public volatile string Status = "Not started";

        public void Extract()
        {
            try
            {
                Status = "Downloading file...";

                using var s = Get(attachment.Url);

                try
                {
                    Status = "Reading file...";
                    var tmodFile = SerializableTmodFile.FromStream(s);

                    Status = "Converting files...";
                    var convertedFile = tmodFile.Convert([RawimgExtractor.GetRawimgExtractor(accelerate: false), new InfoExtractor()]);

                    Status = "Packing files...";
                    using var ms = new MemoryStream();
                    using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                    {
                        foreach (var entry in convertedFile.Entries)
                        {
                            var archiveEntry = archive.CreateEntry(entry.Key);
                            using var es = archiveEntry.Open();
                            using var sw = new StreamWriter(es);
                            sw.Write(entry.Value.ToArray());
                        }
                    }

                    Status = "Uploading ZIP archive...";
                    Status = UploadFileAsync(ms.ToArray(), attachment.Filename, services).GetAwaiter().GetResult();

                    Done = true;
                }
                catch (Exception e)
                {
                    Status = e.Message.Contains("Invalid TMOD header") ? "Wrong header, skipping" : "Failed to read";
                    Done = true;
                }
            }
            catch
            {
                Status = "Failed with exception";
                Done = true;
            }
        }

        public override string ToString()
        {
            return $"`{attachment.Filename}`: {Status}";
        }
    }

    /*private const string api = "https://uguu.se/upload.php?output=text";*/

    private static readonly HttpClient http = new();

    private static readonly Uguu uguu = new();

    public async Task ExtractAndUploadFilesAsync(IMessage message, Func<string, Task> msgUpdate)
    {
        var extractRequests = message.Attachments.Select(x => new ExtractRequest(x, services)).ToArray();

        _ = Task.Run(
            () =>
            {
                foreach (var r in extractRequests)
                {
                    r.Extract();
                }
            }
        );

        while (!extractRequests.All(x => x.Done))
        {
            await msgUpdate(string.Join('\n', extractRequests.Select(x => x.ToString())));
            await Task.Delay(1000);
        }

        await msgUpdate(string.Join('\n', extractRequests.Select(x => x.ToString())));
    }

    private static async Task<string> UploadFileAsync(byte[] bytes, string fileName, IServiceProvider services)
    {
        fileName = fileName.EndsWith(".tmod") ? Path.ChangeExtension(fileName, ".zip") : fileName + ".zip";

        var result = uguu.Upload(new MemoryStream(bytes), fileName);
        return result.IsSuccess ? result.Response : result.ResponseInfo.StatusCode.ToString();

        /*
        using var scope = services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ILitterboxClient>();
        var response = await client.UploadImage(
            new TemporaryStreamUploadRequest
            {
                Expiry = ExpireAfter.OneHour,
                FileName = fileName,
                Stream = new MemoryStream(bytes),
            }
        );

        if (response is null)
        {
            response = "<null litterbox response>";
        }
        else if (response.Length > 100 || response.StartsWith("<!DOCTYPE"))
        {
            response = "uh oh";
        }

        return response;
        */
    }

    private static Stream Get(string url)
    {
        var response = http.GetAsync(url).GetAwaiter().GetResult();
        return response.Content.ReadAsStream();
    }
}
