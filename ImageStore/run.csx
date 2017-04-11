#r "Newtonsoft.Json"
using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

public static void Run(string myQueueItem, TraceWriter log, out Stream newImage)
{
    log.Info($"C# Queue trigger function processed: {myQueueItem}");

    VehicleImage vi = new VehicleImage();
    vi = JsonConvert.DeserializeObject<VehicleImage>(myQueueItem);

    log.Info($"Source URL: {vi.SourceURL}");

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