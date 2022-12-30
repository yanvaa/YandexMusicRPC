using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Media.Control;
using Windows.Storage.Streams;
using YandexMusicApi.Client;

namespace YandexMusicRPC
{
    class MusicData
    {
        public string Title;
        public string Artist;
        public BitmapImage Thumbnail;
        public string url;
        private GlobalSystemMediaTransportControlsSessionMediaProperties mediaProperties;

        public MusicData() { }
        public MusicData(string title, string artist, BitmapImage image, string url_) { Title = title; Artist = artist; Thumbnail = image; url = url_; }

        public async Task<MusicData> GetData()
        {
            var client = new YandexMusicClient();
            var gsmtcsm = await GetSystemMediaTransportControlsSessionManager();
            var mediaProperties = await GetMediaProperties(gsmtcsm.GetCurrentSession());
            Title = mediaProperties.Title;
            Artist = mediaProperties.Artist;
            var thumbnail = await mediaProperties.Thumbnail.OpenReadAsync();
            BitmapImage thumb;
            using (DataReader dataReader = new DataReader(thumbnail))
            {
                var bytes = new byte[thumbnail.Size];
                await dataReader.LoadAsync((uint)thumbnail.Size);
                dataReader.ReadBytes(bytes);
                var image = new BitmapImage();
                using (var ms = new System.IO.MemoryStream(bytes))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad; // here
                    image.StreamSource = ms;
                    image.EndInit();
                }
                thumb = image;
            }
            Thumbnail = thumb;
            var track = await client.Tracks.SearchAsync(Title + " " + Artist);
            if (track != null)
            {
                var result = track.Results.ElementAt(0);
                return new MusicData(Title, Artist, Thumbnail, $"https://music.yandex.ru/track/{result.RealId}");
            }

            return new MusicData(Title, Artist, Thumbnail, $"https://github.com/yanvaa/YandexMusicRPC");
        }

        private static async Task<GlobalSystemMediaTransportControlsSessionManager> GetSystemMediaTransportControlsSessionManager() =>
        await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();

        private static async Task<GlobalSystemMediaTransportControlsSessionMediaProperties> GetMediaProperties(GlobalSystemMediaTransportControlsSession session) =>
            await session.TryGetMediaPropertiesAsync();
    }
}
