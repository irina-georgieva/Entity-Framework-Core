namespace Footballers.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Footballers.DataProcessor.ExportDto;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportCoachesWithTheirFootballers(FootballersContext context)
        {
            ExportCoachesDto[] coaches = context
                .Coaches
                .Where(c => c.Footballers.Count > 0)
                .ToArray()
                .Select(c => new ExportCoachesDto()
                {
                    CoachName = c.Name,
                    FootballersCount = c.Footballers.Count,
                    Footballers = c.Footballers
                    .Select(f => new ExportCoachFootballerDto
                    {
                        Name = f.Name,
                        Position = f.PositionType.ToString()
                    })
                    .OrderBy(f => f.Name)
                    .ToArray()
                })
                .OrderByDescending(c => c.FootballersCount)
                .ThenBy(c => c.CoachName)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new StringWriter(sb);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlRootAttribute root = new XmlRootAttribute("Coaches");
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ExportCoachesDto[]), root);

            xmlSerializer.Serialize(writer, coaches, namespaces);
            return sb.ToString().TrimEnd();
        }

        public static string ExportTeamsWithMostFootballers(FootballersContext context, DateTime date)
        {
            //var teams = context
            //    .Teams
            //    .Where(t => t.TeamsFootballers.Any(t => t.Footballer.ContractStartDate >= date))
            //    .Take(5)
            //    .ToArray()
            //    .Select(ft => new
            //    {
            //        Name = ft.Name,
            //        Footballers = ft.TeamsFootballers
            //        .Select(f => new
            //        {
            //            FootballerName = f.Footballer.Name,
            //            ContractStartDate = f.Footballer.ContractStartDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
            //            ContractEndDate = f.Footballer.ContractEndDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
            //            BestSkillType = f.Footballer.BestSkillType.ToString(),
            //            PositionType = f.Footballer.PositionType.ToString()
            //        })
            //        .OrderByDescending(f => f.ContractEndDate)
            //        .ThenBy(f => f.FootballerName)
            //    })
            //    .OrderByDescending(t => t.Footballers.Count())
            //    .ThenBy(t => t.Name)
            //    .ToArray();

            var teams = context
                .Teams
                .Where(t => t.TeamsFootballers.Any(tf => tf.Footballer.ContractStartDate >= date))
                .ToArray()
                .Select(t => new
                {
                    Name = t.Name,
                    Footballers = t.TeamsFootballers
                        .Where(tf => tf.Footballer.ContractStartDate >= date)
                        .OrderByDescending(tf => tf.Footballer.ContractEndDate)
                        .ThenBy(tf => tf.Footballer.Name)
                        .ToArray()
                        .Select(tf => new
                        {
                            FootballerName = tf.Footballer.Name,
                            ContractStartDate = tf.Footballer.ContractStartDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                            ContractEndDate = tf.Footballer.ContractEndDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                            BestSkillType = tf.Footballer.BestSkillType.ToString(),
                            PositionType = tf.Footballer.PositionType.ToString()
                        })
                        .ToArray()
                })
                .OrderByDescending(t => t.Footballers.Length)
                .ThenBy(t => t.Name)
                .Take(5)
                .ToArray();


            string json = JsonConvert.SerializeObject(teams, Formatting.Indented);
            return json;
        }
    }
}

