using System;
using System.Configuration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Configuration;

public static void Run(ImageInfo myQueueItem, TraceWriter log)
{
    //log.Info($"C# Queue trigger function processed: {myQueueItem}");

    string blobURI = ConfigurationManager.AppSettings["storageuri"];
    string originalPath = blobURI + "images-to-resize/";
    string sizedPath = blobURI + "images-sized/";

    //save to db
    log.Info("----- Saving to SQL -----");
    using (var context = new InventoryContext()){
        VehicleImage vi = context.VehicleImages
            .Where(v => v.Id == myQueueItem.ImageId)
            .FirstOrDefault();

        vi.OriginalURL = originalPath + myQueueItem.BlobName;
        vi.SizedURL = sizedPath + myQueueItem.BlobName;
        vi.DateModified = DateTime.Now;
        
        context.SaveChanges();
    }

}

[Table("VehicleImage")]
public class VehicleImage
{
    public int Id { get; set; }
    [StringLength(100)]
    public string VIN { get; set; }
    [StringLength(1000)]
    public string SourceURL { get; set; }
    [StringLength(1000)]
    public string OriginalURL { get; set; }    
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

public class ImageInfo {

    public int ImageId { get; set; }
    public string BlobName { get; set; }

}