#r "Newtonsoft.Json"
using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings.Runtime;

public static async void Run(string myQueueItem, Binder binder, TraceWriter log)
{
    // Using a Binder allows us to dynamically change the output filename
    
    log.Info($"C# Queue trigger function processed: {myQueueItem}");
    
    VehicleImage vi = new VehicleImage();
    vi = JsonConvert.DeserializeObject<VehicleImage>(myQueueItem);

    log.Info($"Source URL: {vi.SourceURL}");

    var attributes = new Attribute[]
    {
        new BlobAttribute($"images-to-resize/" + Guid.NewGuid().ToString() + ".jpg", FileAccess.Write),
        new StorageAccountAttribute("clientdatastorage")
    };

    WebClient wc = new WebClient();
    using (MemoryStream stream = new MemoryStream(wc.DownloadData(vi.SourceURL)))
    {
        var byteArray = stream.ToArray();
        // await outBlob.WriteAsync(byteArray, 0, byteArray.Length);

        using (var writer = await binder.BindAsync<Stream>(attributes).ConfigureAwait(false))
        {
           await writer.WriteAsync(byteArray, 0, byteArray.Length);
        }

        log.Info($"Image Stored");        
    }

     log.Info($"Done ");
}

public class VehicleImage
{
    public int Id { get; set; }
    public string VIN { get; set; }
    public string SourceURL { get; set; }
    public string SizedURL { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateModified { get; set; }
}


// https://weblogs.asp.net/sfeldman/azure-functions-to-make-audit-queue-and-auditors-happy
