using System.Collections.ObjectModel;
using System.Diagnostics;
using Hitomi.NET;
using Microsoft.UI.Xaml.Media.Imaging;


namespace Hitomi.Uno.ViewModels;

public partial class DetailViewModel : ObservableObject
{
    //public int thread { get; set; } = 1;
    
    private readonly IDispatcher _dispatcher;
    //private int listIndex;

    [ObservableProperty]
    private string? _imageUrl;

    public ObservableCollection<ImageItem>? ImageItem { get; set; }

    public DetailViewModel(IDispatcher dispatcher, Entity entity)
    {
        _dispatcher = dispatcher;

        GetImageItem(int.Parse(entity.Value));
    }

    private async void GetImageItem(int number) 
    {
        ImageItem = new ObservableCollection<ImageItem>();
        HitomiWebp hitomiWebp = new HitomiWebp();

        var imageList = await hitomiWebp.HitomiImageSingleList(number);

        await _dispatcher.ExecuteAsync(async () =>
        {
            for (int i = 0; i < imageList.Count; i++) ImageItem!.Add(new ImageItem() { ImageBitmap = null});
            //for (int i = 0; i < imageList.Count; i++) ImageItem!.Add(null);

            //List<Task> tasks = new List<Task>();
            //SemaphoreSlim semaphore = new SemaphoreSlim(thread);
            
            foreach (var item in imageList)
            {
                await Task.Delay(500);
                var bitmapImage = await GetBitmapImageAsync(item);
                ImageItem[imageList.IndexOf(item)].ImageBitmap = bitmapImage;
            }
            
        });

    }

    private async Task<BitmapImage> GetBitmapImageAsync(string url)
    {
        try
        {   
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Referer", "https://hitomi.la/"); 
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36");
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(stream.AsRandomAccessStream());
            return bitmapImage;
        }
        catch (Exception ex)
        {
            // Handle exceptions
            Debug.WriteLine($"Failed to get bitmap image: {ex.Message}");
            return null;
        }
    }
}
