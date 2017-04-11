using System;
using System.Configuration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

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

    //save to db
    using (var context = new InventoryContext()){
        context.VehicleImages.AddRange(NewImages);
        context.SaveChanges();
        log.Info($"----- Vehicles Saved: {NewImages.Count} -----");
    }

    //TODO: Delete when done
    //loop through to test
    log.Info("----- New Images -----");
    foreach (VehicleImage vi in NewImages)
    {        
        log.Info($"VIN: {vi.VIN}, URL: {vi.SourceURL}, Date: {vi.DateCreated}");
    }
}

[Table("VehicleImage")]
public class VehicleImage
{
    public VehicleImage(string vin, string sourceurl)
    {
        this.VIN = vin;
        this.SourceURL = sourceurl;
        this.DateCreated = DateTime.Now;
        this.DateModified = DateTime.Now;
    }
    public int Id { get; set; }
    [StringLength(100)]
    public string VIN { get; set; }
    [StringLength(1000)]
    public string SourceURL { get; set; }
    [StringLength(1000)]
    public string SizedURL { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateModified { get; set; }
}

public class InventoryContext : DbContext
{
    public InventoryContext()
        : base("name=InventoryContext")
    {
    }

    public virtual DbSet<VehicleImage> VehicleImages { get; set; }
}