using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Discord;

using Tomat.FNB.TMOD;
using Tomat.FNB.TMOD.Converters.Extractors;
using Tomat.FNB.TMOD.Utilities;
using Tomat.Teto.Bot.DependencyInjection;

namespace Tomat.Teto.Bot.Services;

public sealed class ModExtractService : IService
{
    private sealed class ExtractRequest(IAttachment attachment)
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
                    Status = UploadFileAsync(ms.ToArray(), attachment.Filename).GetAwaiter().GetResult();

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

    private const string api = "https://litterbox.catbox.moe/resources/internals/api.php";
    private const string time = "12h";

    private static readonly HttpClient http = new();

    public async Task ExtractAndUploadFilesAsync(IMessage message, Func<string, Task> msgUpdate)
    {
        var extractRequests = message.Attachments.Select(x => new ExtractRequest(x)).ToArray();

        _ = Task.Run(() =>
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

    private static async Task<string> UploadFileAsync(byte[] bytes, string fileName)
    {
        using var form = new MultipartFormDataContent();

        form.Add(new StringContent("fileupload"), "reqtype");
        form.Add(new StringContent(time), "time");

        var sc = new ByteArrayContent(bytes);
        sc.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        form.Add(sc, "fileToUpload", fileName + ".zip");

        var response = await http.PostAsync(api, form);
        return await response.Content.ReadAsStringAsync();
    }

    private static Stream Get(string url)
    {
        var response = http.GetAsync(url).GetAwaiter().GetResult();
        return response.Content.ReadAsStream();
    }
}