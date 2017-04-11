using System.IO;

public static void Run(Stream myBlob, string name, TraceWriter log)
{
    log.Info($"C# Blob trigger function Processed blob\n Name:{name} ");

    List<VehicleImage> NewImages = new List<VehicleImage>();

    //read csv
    log.Info("----- Reading from CSV -----");
    using(var reader = new StreamReader(myBlob)){
        while (!reader.EndOfStream){
            var line = reader.ReadLine();
            var values = line.Split(',');                        
            log.Info($"Vin: {values[0]} URL: {values[1]}");
            NewImages.Add(new VehicleImage(values[0],values[1]));
        }
    }

    //loop through to test
    log.Info("----- New Images -----");
    foreach (VehicleImage vi in NewImages)
    {        
        log.Info($"VIN: {vi.VIN}, URL: {vi.SourceURL}");
    }
}

public class VehicleImage
{
    public VehicleImage(string vin, string sourceurl)
    {
        this.VIN = vin;
        this.SourceURL = sourceurl;
    }
    public string ImageId { get; set; }
    public string VIN { get; set; }
    public string SourceURL { get; set; }
    public string SizedURL { get; set; }
    public DateTime DateCreated { get; set; }
}