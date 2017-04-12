#r "Newtonsoft.Json"
using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings.Runtime;

public static async void Run(string myQueueItem, Stream outBlob, TraceWriter log)
{
    log.Info($"C# Queue trigger function processed: {myQueueItem}");
    
    VehicleImage vi = new VehicleImage();
    vi = JsonConvert.DeserializeObject<VehicleImage>(myQueueItem);

    log.Info($"Source URL: {vi.SourceURL}");

    WebClient wc = new WebClient();
    using (MemoryStream stream = new MemoryStream(wc.DownloadData(vi.SourceURL)))
    {
        var byteArray = stream.ToArray();
        await outBlob.WriteAsync(byteArray, 0, byteArray.Length);
    }

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


    // TODO: Replace with code below so that we can 
    // rename the file to exactly what we want
    // 
    // https://weblogs.asp.net/sfeldman/azure-functions-to-make-audit-queue-and-auditors-happy
    // var attributes = new Attribute[]
    // {
    //     new BlobAttribute($"images-to-resize/" + Guid.NewGuid().ToString() + ".jpg"),
    //     new StorageAccountAttribute("clientdatastorage")
    // };

    // using (var writer = await binder.BindAsync<TextWriter>(attributes).ConfigureAwait(false))
    // {
    //     writer.Write(myQueueItem);
    // }

    // log.Info($"Done ");