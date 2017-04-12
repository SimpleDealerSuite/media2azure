#r "Newtonsoft.Json"
using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings.Runtime;

public static async void Run(string myQueueItem, Binder binder, ICollector<ImageInfo> myQueueItemOut, TraceWriter log)
{
    // Using a Binder allows us to dynamically change the output filename

    log.Info($"C# Queue trigger function processed: {myQueueItem}");
    
    VehicleImage vi = new VehicleImage();
    vi = JsonConvert.DeserializeObject<VehicleImage>(myQueueItem);

    log.Info($"Source URL: {vi.SourceURL}");

    // Grab the filename & ext
    Uri uri = new Uri(vi.SourceURL);
    string filename = Path.GetFileName(uri.LocalPath);    
    string ext = Path.GetExtension(uri.LocalPath).ToLower();
    log.Info($"Filename: {filename}");
    log.Info($"Ext: {ext}");

    // Generate new filename
    string filenameNew = Guid.NewGuid().ToString() + ext;

    // Set attributes
    var attributes = new Attribute[]
    {
        new BlobAttribute($"images-to-resize/" + filenameNew, FileAccess.Write),
        new StorageAccountAttribute("clientdatastorage")
    };

    // Download the image
    WebClient wc = new WebClient();
    using (MemoryStream stream = new MemoryStream(wc.DownloadData(vi.SourceURL)))
    {
        var byteArray = stream.ToArray();

        // Write image to blob storage
        using (var writer = await binder.BindAsync<Stream>(attributes).ConfigureAwait(false))
        {
           await writer.WriteAsync(byteArray, 0, byteArray.Length);
        }

        log.Info($"Image {filenameNew} Stored");        
    }

    // Place message in queue
    ImageInfo img = new ImageInfo(vi.Id,filenameNew);
    myQueueItemOut.Add(img); 

    // We are done... 
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

public class ImageInfo {

    public ImageInfo(int id, string filename)
    {
        this.ImageId = id; 
        this.BlobName = filename;
    }
    public int ImageId { get; set; }
    public string BlobName { get; set; }
}

// https://weblogs.asp.net/sfeldman/azure-functions-to-make-audit-queue-and-auditors-happy
