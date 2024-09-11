/**
 * @QumartSeller_Client
 * https://github.com/Nailed34/QumartSeller_Client-demo.git
 *
 * Copyright (c) 2024 https://github.com/Nailed34
 * Released under the MIT license
 */

using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Media.Imaging;

namespace ClientWPF.Services
{
    file struct PhotoCache
    {
        public string productInfoId { get; set; }
        public string photoLink { get; set; }
    }

    internal class PhotoCacheService
    {
        const string PhotoCacheDirectory = "Cache/";
        const string PhotoCacheListPath = PhotoCacheDirectory + "PhotoCacheList.data";

        /// <summary>
        /// Event for get photos from service
        /// </summary>
        public event Action<BitmapImage?>? PhotoLoadedEvent;

        // Determine that PhotoCacheData was loaded
        private static bool PhotoCacheListLoaded { get; set; } = false;

        // Main cache     
        private static Dictionary<string, string> PhotoCacheData { get; set; } = new();

        // Determines saving status, used for access control
        private static bool SavingInProgress { get; set; } = false;

        // Task with loading cache data file
        private static Task? LoadingTask { get; set; }

        // Task with current save action
        private static Task? SavingTask { get; set; }

        // Task with autosave. Saved for close on shutdown
        private static Task? AutosaveDataTask { get; set; }

        // Cancellation source for cancel autosave task on shutdown
        private static CancellationTokenSource AutosaveDataCancellation { get; set; } = new();

        // Interval in ms for autosaving file
        private static int AutosaveInterval { get; set; } = 120000;

        // Web client for download files
        private static HttpClient HttpClient { get; set; } = new();

        /// <summary>
        /// Load photo data from file and run autosaving
        /// </summary>
        public static void RunPhotoCache()
        {
            // Load photo data from file
            LoadingTask = Task.Run(() =>
            {
                try
                {
                    // Check cache directory
                    if (!Directory.Exists(PhotoCacheDirectory))
                        Directory.CreateDirectory(PhotoCacheDirectory);

                    // Check cache photo data file
                    if (!File.Exists(PhotoCacheListPath))
                    {
                        FileStream NewFile = File.Create(PhotoCacheListPath);
                        NewFile.Close();
                        PhotoCacheListLoaded = true;
                        return;
                    }

                    using (StreamReader fileStream = new(PhotoCacheListPath))
                    {
                        string? line;
                        while ((line = fileStream.ReadLine()) != null)
                        {
                            if (line == "")
                                break;

                            var photo = JsonSerializer.Deserialize<PhotoCache>(line);
                            PhotoCacheData.Add(photo.productInfoId, photo.photoLink);
                        }
                    }

                    PhotoCacheListLoaded = true;
                }
                catch
                {
                    return;
                }
            });

            // Run autosaving
            AutosaveDataTask = Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        await Task.Delay(AutosaveInterval, AutosaveDataCancellation.Token);

                        if (!SavingInProgress)
                        {
                            SavingTask?.Dispose();
                            SavingTask = SavePhotoData();
                            SavingTask.Wait();
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }, AutosaveDataCancellation.Token);
        }

        // Save current photo cache data in file
        private static Task SavePhotoData()
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!PhotoCacheListLoaded)
                        return;

                    SavingInProgress = true;

                    using (StreamWriter fileStream = new(PhotoCacheListPath, false))
                    {
                        foreach (var photo in PhotoCacheData)
                            if (photo.Key != null && photo.Value != null)
                                fileStream.WriteLine(JsonSerializer.Serialize(new PhotoCache() { productInfoId = photo.Key, photoLink = photo.Value }));
                    }

                    SavingInProgress = false;
                }
                catch
                {
                    SavingInProgress = false;
                }
            });
        }

        /// <summary>
        /// Save current photo cache data and close autosave task. Call this method on shutdown program
        /// </summary>
        public static void SavePhotoDataAndClose()
        {
            if (!PhotoCacheListLoaded)
            {
                AutosaveDataCancellation.Cancel();
                LoadingTask?.Wait();
            }
            else if (SavingInProgress)
            {
                SavingTask?.Wait();
                AutosaveDataCancellation.Cancel();
            }
            else
            {
                AutosaveDataCancellation.Cancel();
                var lastSave = SavePhotoData();
                lastSave.Wait();
            }
        }

        /// <summary>
        /// Add new request for load photo from cache or download from link. Result of loading returns by PhotoLoadedEvent (null if failed)
        /// </summary>
        public void LoadPhoto(string photoId, string photoLink)
        {
            Task.Run(async () =>
            {
                // Check data loading and wait it
                if (!PhotoCacheListLoaded)
                {
                    if (LoadingTask != null)
                    {
                        await LoadingTask;
                        if (!PhotoCacheListLoaded)
                        {
                            PhotoLoadedEvent?.Invoke(null);
                            return;
                        }
                    }
                    else
                    {
                        PhotoLoadedEvent?.Invoke(null);
                        return;
                    }
                }

                // Check saving and wait it
                if (SavingInProgress)
                {
                    if (SavingTask != null)
                        await SavingTask;
                    else
                    {
                        PhotoLoadedEvent?.Invoke(null);
                        return;
                    }
                }

                // Find photo
                string? savedPhotoLink = "";
                var findResult = PhotoCacheData.TryGetValue(photoId, out savedPhotoLink);

                // Photo doesn't exist
                if (!findResult)
                {
                    var downloadResult = await DownloadPhoto(photoId, photoLink);
                    if (downloadResult != null)
                    {
                        // Add new photo
                        PhotoCacheData.Add(photoId, photoLink);
                        PhotoLoadedEvent?.Invoke(downloadResult);
                        return;
                    }
                    else
                    {
                        PhotoLoadedEvent?.Invoke(null);
                        return;
                    }
                }
                // Different photo links, redownload image
                else if (savedPhotoLink != photoLink)
                {
                    File.Delete(PhotoCacheDirectory + photoId + ".jpg");
                    var downloadResult = await DownloadPhoto(photoId, photoLink);
                    if (downloadResult != null)
                    {
                        // Change photo link
                        PhotoCacheData[photoId] = photoLink;
                        PhotoLoadedEvent?.Invoke(downloadResult);
                        return;
                    }
                    else
                    {
                        PhotoLoadedEvent?.Invoke(null);
                        return;
                    }
                }
                // Success find photo
                else
                {
                    try
                    {
                        byte[]? imageBytes = null;

                        using (FileStream stream = new FileStream(PhotoCacheDirectory + photoId + ".jpg", FileMode.Open, FileAccess.Read))
                        {
                            imageBytes = new byte[stream.Length];

                            int totalRead = 0;
                            while (totalRead < imageBytes.Length)
                            {
                                int bytesRead = stream.Read(imageBytes.AsSpan(totalRead));
                                if (bytesRead == 0) break;
                                totalRead += bytesRead;
                            }
                        }

                        PhotoLoadedEvent?.Invoke(GetBitmapFromByteArray(imageBytes));
                        return;
                    }
                    catch
                    {
                        PhotoLoadedEvent?.Invoke(null);
                        return;
                    }
                }
            });          
        }

        // Download photo from link and save it in cache directory. Return null if failed
        private async Task<BitmapImage?> DownloadPhoto(string photoId, string photoLink)
        {
            try
            {
                // Download bytes
                var imageBytes = await HttpClient.GetByteArrayAsync(photoLink);

                // Create bitmap with decode pixel height (resized for UI)
                var resizedImage = GetBitmapFromByteArray(imageBytes);

                // Crate encoder for save bitmap to cache directory
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(resizedImage));

                // Save
                using (var fileStream = new FileStream(PhotoCacheDirectory + photoId + ".jpg", FileMode.Create))
                {
                    encoder.Save(fileStream);
                }

                return resizedImage;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Create bitmap from image byte array. Use this from UI thread for render image
        /// </summary>
        private BitmapImage GetBitmapFromByteArray(byte[] bytes)
        {         
            BitmapImage resultImage = new BitmapImage();
            using (var memStream = new MemoryStream(bytes))
            {
                resultImage.BeginInit();
                resultImage.CacheOption = BitmapCacheOption.OnLoad;
                resultImage.DecodePixelHeight = 70;
                resultImage.StreamSource = memStream;
                resultImage.EndInit();
                resultImage.Freeze();          
            }
            return resultImage;
        }
    }
}
