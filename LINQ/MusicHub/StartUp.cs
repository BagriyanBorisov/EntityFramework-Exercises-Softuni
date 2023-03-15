using System.Text;

namespace MusicHub;

using System;

using Data;
using Initializer; 
public class StartUp
{
    public static void Main() 
    {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here
            string result = ExportSongsAboveDuration(context, 4);
            Console.WriteLine(result);
    }

    public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
    {
        StringBuilder sb = new StringBuilder();

        var albumsInfo = context.Albums
            .Where(a => a.ProducerId.HasValue && a.ProducerId.Value == producerId)
            .AsEnumerable()
            .OrderByDescending(a => a.Price)
            .Select(a => new
            {
                a.Name,
                ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy"),
                ProducerName = a.Producer!.Name,
                Songs = a.Songs
                    .Select(s => new
                    {
                        s.Name,
                        SongPrice = s.Price.ToString("f2"),
                        WriterName = s.Writer.Name
                    }).OrderByDescending(s => s.Name).ToArray(),
                AlbumPrice = a.Price.ToString("f2")
            }).ToArray();

        foreach (var a in albumsInfo)
        {
            sb
                .AppendLine($"-AlbumName: {a.Name}")
                .AppendLine($"-ReleaseDate: {a.ReleaseDate}")
                .AppendLine($"-ProducerName: {a.ProducerName}")
                .AppendLine($"-Songs:"); ;
            int songNum = 1;
            foreach (var s in a.Songs)
            {
                sb
                    .AppendLine($"---#{songNum}")
                    .AppendLine($"---SongName: {s.Name}")
                    .AppendLine($"---Price: {s.SongPrice}")
                    .AppendLine($"---Writer: {s.WriterName}");
                songNum++;
            }

            sb.AppendLine($"-AlbumPrice: {a.AlbumPrice}");
        }

        return sb.ToString().TrimEnd();
    }

    public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
    {
        StringBuilder sb = new StringBuilder();

        var songsInfo = context.Songs.AsEnumerable()
            .Where(s => s.Duration.TotalSeconds > duration)
            .Select(s => new
            {
                s.Name,
                PerformersName = s.SongPerformers
                    .Select(p =>  $"{p.Performer.FirstName} {p.Performer.LastName}")
                    .OrderBy(p => p)
                    .ToArray(),
                WriterName = s.Writer.Name,
                ProducerName = s.Album!.Producer!.Name,
                Duration = s.Duration.ToString("c")
            })
            .OrderBy(s => s.Name)
            .ThenBy(s => s.WriterName)
            .ToArray();

        int songNum = 1;
        foreach (var s in songsInfo)
        {
            sb
                .AppendLine($"-Song #{songNum}")
                .AppendLine($"---SongName: {s.Name}")
                .AppendLine($"---Writer: {s.WriterName}");

            foreach (var p in s.PerformersName)
            {
                sb.AppendLine($"---Performer: {p}");
            }

            sb
                .AppendLine($"---AlbumProducer: {s.ProducerName}")
                .AppendLine($"---Duration: {s.Duration}");
            songNum++;
        }


        return sb.ToString().TrimEnd();
    }
}

