using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Footballers.DataProcessor.ExportDto;
using Newtonsoft.Json;

namespace Footballers.DataProcessor
{
    using Data;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportCoachesWithTheirFootballers(FootballersContext context)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FootballersProfile>();
            }));
            XmlHelper xmlHelper = new XmlHelper();

            ExportCoachDto[] coaches = context.Coaches
                .OrderByDescending(c => c.Footballers.Count)
                .ToArray().Select(c => new ExportCoachDto()
                {
                   
                    Name = c.Name,
                    Footballers = c.Footballers.Select(f => new ExportFootballerDto()
                    {
                        Name = f.Name,
                        PositionType = f.PositionType
                    })
                        .OrderBy(f => f.Name)
                        .ToArray(),
                    FootballersCount = c.Footballers.Count()
                }).ToArray();

            return xmlHelper.Serialize(coaches, "Coaches");
        }

        public static string ExportTeamsWithMostFootballers(FootballersContext context, DateTime date)
        {
            var teams = context.Teams
                .Where(t => t.TeamsFootballers.Any(tf => tf.Footballer.ContractStartDate >= date))
                .ToArray()
                .Select(t => new
                    {
                        t.Name,
                        Footballers = t.TeamsFootballers
                            .Where(tf => tf.Footballer.ContractStartDate >= date)
                            .Select(tf => new
                            {
                                FootballerName = tf.Footballer.Name,
                                ContractStartDate = tf.Footballer.ContractStartDate
                                    .ToString("d",CultureInfo.InvariantCulture),
                                ContractEndDate = tf.Footballer.ContractEndDate
                                    .ToString("d", CultureInfo.InvariantCulture),
                                BestSkillType = tf.Footballer.BestSkillType.ToString(),
                                PositionType = tf.Footballer.PositionType.ToString()
                            })
                            .OrderByDescending(f => DateTime.Parse(f.ContractEndDate))
                            .ThenBy(f=> f.FootballerName)
                            .ToArray()
                    }
                ).OrderByDescending(t => t.Footballers.Length)
                .ThenBy(t => t.Name)
                .Take(5)
                .ToArray();

            return JsonConvert.SerializeObject(teams,Formatting.Indented);
        }
    }
}
