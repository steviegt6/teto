using System;
using System.Diagnostics;
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

namespace Tml.Plugin.Extract.Services;

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

    private const string api = "https://uguu.se/upload?output=text";

    private static readonly HttpClient http = new();

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
        if (bytes == null || bytes.Length == 0)
            return "Error: File is empty.";

        // Normalize extension for Uguu
        if (fileName.EndsWith(".tmod", StringComparison.OrdinalIgnoreCase))
            fileName = Path.ChangeExtension(fileName, ".zip");

        var psi = new ProcessStartInfo
        {
            FileName = "curl",
            Arguments = $"-s -F \"files[]=@-;filename={fileName}\" \"https://uguu.se/upload?output=text\"",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using var process = Process.Start(psi);

            // Write the file bytes directly into curl’s stdin
            await process.StandardInput.BaseStream.WriteAsync(bytes, 0, bytes.Length);
            process.StandardInput.Close();

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
                return $"Error: curl failed ({process.ExitCode}): {error.Trim()}";

            return output.Trim();
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    private static Stream Get(string url)
    {
        var response = http.GetAsync(url).GetAwaiter().GetResult();
        return response.Content.ReadAsStream();
    }
}
