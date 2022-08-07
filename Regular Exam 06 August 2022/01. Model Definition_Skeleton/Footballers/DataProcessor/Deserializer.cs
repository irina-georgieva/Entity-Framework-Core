namespace Footballers.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;
    using Data;
    using Footballers.Data.Models;
    using Footballers.Data.Models.Enums;
    using Footballers.DataProcessor.ImportDto;
    using Newtonsoft.Json;

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

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Coaches");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCoachesDto[]), xmlRoot);

            using StringReader reader = new StringReader(xmlString);

            ImportCoachesDto[] coachesDtos = (ImportCoachesDto[])
                xmlSerializer.Deserialize(reader);

            ICollection<Coach> validCoaches = new List<Coach>();

            foreach (var coachDto in coachesDtos)
            {
                if (!IsValid(coachDto))
                {
                    sb.AppendLine($"Invalid data!");
                    continue;
                }

                if (!IsValid(coachDto.Name))
                {
                    sb.AppendLine($"Invalid data!");
                    continue;
                }

                if (!IsValid(coachDto.Nationality))
                {
                    sb.AppendLine($"Invalid data!");
                    continue;
                }

                Coach coach = new Coach()
                {
                    Name = coachDto.Name,
                    Nationality = coachDto.Nationality
                };

                foreach (var footDto in coachDto.Footballers)
                {
                    if (!IsValid(footDto))
                    {
                        sb.AppendLine($"Invalid data!");
                        continue;
                    }

                    bool isBestSkillEnumValid =
                        Enum.TryParse(typeof(BestSkillType), footDto.BestSkillType, out object bestSkillObj);
                    if (!isBestSkillEnumValid)
                    {
                        sb.AppendLine($"Invalid data!");
                        continue;
                    }

                    bool isPositionTypeEnumValid =
                        Enum.TryParse(typeof(PositionType), footDto.PositionType, out object positionTypeObj);
                    if (!isPositionTypeEnumValid)
                    {
                        sb.AppendLine($"Invalid data!");
                        continue;
                    }

                    bool isStartDateValid =
                        DateTime.TryParseExact(footDto.ContractStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime startDate);

                    if (!isStartDateValid)
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }

                    bool isEndDateValid =
                        DateTime.TryParseExact(footDto.ContractEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime endDate);

                    if (!isEndDateValid)
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }

                    if(startDate > endDate)
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }


                    Footballer footballer = new Footballer()
                    {
                        Name = footDto.Name,
                        ContractStartDate = startDate,
                        ContractEndDate = endDate,
                        BestSkillType = (BestSkillType)bestSkillObj,
                        PositionType = (PositionType)positionTypeObj
                    };
                    
                    coach.Footballers.Add(footballer);
                };

                validCoaches.Add(coach);
                sb.AppendLine($"Successfully imported coach - {coach.Name} with {coach.Footballers.Count} footballers.");
            }

            context.Coaches.AddRange(validCoaches);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportTeams(FootballersContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportTeamsDto[] teamsDtos = JsonConvert
                .DeserializeObject<ImportTeamsDto[]>(jsonString);

            ICollection<Team> validTeams = new List<Team>();

            foreach (var tDto in teamsDtos)
            {
                if (!IsValid(tDto))
                {
                    sb.AppendLine($"Invalid data!");
                    continue;
                }

                if(tDto.Trophies <= 0)
                {
                    sb.AppendLine($"Invalid data!");
                    continue;
                }

                List<int> footballerIds = context
                    .Footballers
                    .Select(f => f.Id)
                    .ToList();


                ICollection<TeamFootballer> validFootballerIds = new List<TeamFootballer>();

                foreach (int footballerId in tDto.Footballers)
                {
                    if (!footballerIds.Contains(footballerId))
                    {
                        sb.AppendLine($"Invalid data!");
                        continue;
                    }

                    TeamFootballer teamFootballer = new TeamFootballer()
                    {
                        FootballerId = footballerId,
                    };

                    validFootballerIds.Add(teamFootballer);
                }

                Team team = new Team()
                {
                    Name = tDto.Name,
                    Nationality = tDto.Nationality,
                    Trophies = tDto.Trophies,
                    TeamsFootballers = validFootballerIds
                };

                validTeams.Add(team);
                sb.AppendLine($"Successfully imported team - {team.Name} with {team.TeamsFootballers.Count} footballers.");

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
