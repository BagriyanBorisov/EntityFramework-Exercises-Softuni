using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using Footballers.Data.Models;
using Footballers.Data.Models.Enums;
using Footballers.DataProcessor.ImportDto;
using Newtonsoft.Json;

namespace Footballers.DataProcessor
{
    using Footballers.Data;
    using System.ComponentModel.DataAnnotations;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCoach
            = "Successfully imported coach - {0} with {1} footballers.";

        private const string SuccessfullyImportedTeam
            = "Successfully imported team - {0} with {1} footballers.";

        public static string ImportCoaches(FootballersContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            CoachImportDto[] coaches;

            XmlSerializer serializer =
                new XmlSerializer(typeof(CoachImportDto[]), new XmlRootAttribute("Coaches"));
            using (var reader = new StringReader(xmlString))
            {
                coaches = (CoachImportDto[])serializer.Deserialize(reader);
            }

            List<Coach> validCoaches = new List<Coach>();

            foreach (var coachDto in coaches)
            {
                Coach coach = new Coach();

                coach.Name = coachDto.Name ?? "";
                coach.Nationality = coachDto.Nationality;
                if (!IsValid(coach))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                validCoaches.Add(coach);
                foreach (var footballerDto in coachDto.Footballers)
                {
                    Footballer footballer = new Footballer();
                    try
                    {
                        footballer.Name = footballerDto.Name ?? "";
                        footballer.ContractStartDate =
                            DateTime.ParseExact(footballerDto.ContractStartDate, "dd/MM/yyyy",
                                CultureInfo.InvariantCulture);
                        footballer.ContractEndDate =
                            DateTime.ParseExact(footballerDto.ContractEndDate, "dd/MM/yyyy",
                                CultureInfo.InvariantCulture);
                        footballer.PositionType = (PositionType)footballerDto.PositionType;
                        footballer.BestSkillType = (BestSkillType)footballerDto.PositionType;

                        if (!IsValid(footballer) || footballer.ContractStartDate > footballer.ContractEndDate)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }
                    }
                    catch (Exception)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    
                    coach.Footballers.Add(footballer);
                }

                sb.AppendLine(string.Format(SuccessfullyImportedCoach, coach.Name, coach.Footballers.Count));

            }

            context.Coaches.AddRange(validCoaches);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportTeams(FootballersContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var teamsDtos = JsonConvert.DeserializeObject<TeamImportDto[]>(jsonString);

            var validTeams = new List<Team>();
            foreach (var teamDto in teamsDtos)
            {
                Team team = new Team();
                team.Name = teamDto.Name ?? "";
                team.Nationality = teamDto.Nationality ?? "";

                if (!int.TryParse(teamDto.Trophies, out var count) || count <= 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                team.Trophies = count;

                if (!IsValid(team))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                validTeams.Add(team);

                foreach (var footballerId in teamDto.Footballers.Distinct())
                {
                    Footballer? footballer = context.Footballers.Find(footballerId);
                    if (footballer == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    team.TeamsFootballers.Add(new TeamFootballer()
                    {
                        Footballer = footballer,
                        FootballerId = footballerId,
                        Team = team,
                        TeamId = team.Id
                    });
                }
                sb.AppendLine(string.Format(SuccessfullyImportedTeam, team.Name, team.TeamsFootballers.Count()));
            }

            context.Teams.AddRange(validTeams);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
