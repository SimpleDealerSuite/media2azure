#r "System.Drawing"
using System;
using ImageResizer;
using System.Drawing;

public static void Run(ImageInfo myQueueItem, Stream myInputBlob, string BlobName,  Stream myOutputBlob, ICollector<ImageInfo> myQueueItemOut, TraceWriter log)
{
    // log.Info($"C# Queue trigger function processed: {myQueueItem}");

    var imageBuilder = ImageResizer.ImageBuilder.Current;
    var size = imageDimensionsTable[ImageSize.Small];
    
    var sizedImg = imageBuilder.Build(
        myInputBlob,
        new ResizeSettings(size.Item1, size.Item2, FitMode.Max, null), false);

    // save new image to blob    
    sizedImg.Save(myOutputBlob, System.Drawing.Imaging.ImageFormat.Jpeg);

    // place message in queue to udpate SQL db
    // we can add the same one back to a queue because ID and Name are the same
    myQueueItemOut.Add(myQueueItem); 
    
}

public class ImageInfo {

    public int ImageId { get; set; }
    public string BlobName { get; set; }

}

public enum ImageSize
{
    ExtraSmall, Small, Medium
}

private static Dictionary<ImageSize, Tuple<int, int>> imageDimensionsTable = new Dictionary<ImageSize, Tuple<int, int>>()
{
    { ImageSize.ExtraSmall, Tuple.Create(320, 200) },
    { ImageSize.Small,      Tuple.Create(640, 400) },
    { ImageSize.Medium,     Tuple.Create(800, 600) }
};