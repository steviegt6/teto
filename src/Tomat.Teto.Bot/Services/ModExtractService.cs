using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Discord;

using Tomat.FNB.Common.IO;
using Tomat.FNB.TMOD;
using Tomat.FNB.TMOD.Converters.Extractors;
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

                var s = Get(attachment.Url);
                var r = new ByteReader(s);

                try
                {
                    Status = "Reading file...";
                    var modFile = TmodFile.Read(ref r, Span<byte>.Empty, Span<byte>.Empty, ownsStream: true);

                    Status = "Extracting files...";

                    using var ms = new MemoryStream();
                    using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                    {
                        modFile.ProcessFiles(
                            [new InfoExtractor(), RawImgExtractor.GetInstance()],
                            (path, bytes) =>
                            {
                                var entry = archive.CreateEntry(path);
                                using var es = entry.Open();
                                using var sw = new StreamWriter(es);
                                sw.Write(bytes.ToArray());
                            }
                        );
                    }

                    Status = "Uploading ZIP archive...";
                    Status = UploadFileAsync(ms).GetAwaiter().GetResult();

                    Done = true;
                }
                catch (Exception e)
                {
                    Status = e.Message.Contains("magic header") ? "Wrong header, skipping" : "Failed to read";
                    Done = true;

                    r.Dispose();
                    s.Dispose();
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

    private static async Task<string> UploadFileAsync(Stream stream)
    {
        using var form = new MultipartFormDataContent();

        form.Add(new StringContent("fileupload"), "reqtype");
        form.Add(new StringContent(time), "time");
        form.Add(new StreamContent(stream));

        var response = await http.PostAsync(api, form);
        return await response.Content.ReadAsStringAsync();
    }

    private static Stream Get(string url)
    {
        var response = http.GetAsync(url).GetAwaiter().GetResult();
        return response.Content.ReadAsStream();
    }
}