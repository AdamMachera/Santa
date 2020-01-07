using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Santa
{
	class Program
	{
		static void Main(string[] args)
		{
			List<Person> persons = new List<Person>
			{
				new Person { Name = "Adam", Email = "adam.machera@gmail.com", Spouse = "Monika" },
				new Person { Name = "Monika", Email = "monikamejer@op.pl", Spouse = "Adam"},
				new Person { Name = "AndrzejSenior", Email = "agencjamam@wp.pl", Spouse = "Małgorzata" },
				new Person { Name = "Małgorzata", Email = "agencjamam@wp.pl", Spouse = "AndrzejSenior" },
				new Person { Name = "Ania", Email = "amachera@kochamoliwe.com", Spouse = null },
				new Person { Name = "Aleksandra", Email = "ola_mach@wp.pl", Spouse = "Paweł" },
				new Person { Name = "Paweł", Email = "pawelgrzywaczewski@gmail.com", Spouse = "Aleksandra" },
				new Person { Name = "AndrzejJunior", Email = "andrzejmachera@gmail.com", Spouse = "Ula" },
				new Person { Name = "Ula", Email = "ula.wrzosek@gmail.com", Spouse = "AndrzejJunior" },
			};

			Random randomizer = new Random();

			Dictionary<string, Person> givers = new Dictionary<string, Person>();
			int index = 0;
			int countWithoutConclusion = 0;
			while (givers.Count < persons.Count)
			{
				var next = randomizer.Next(0, persons.Count);
				if (countWithoutConclusion > 1000)
				{
					countWithoutConclusion = 0;
					index = 0;
					givers.Clear();
				}

				if (next == index || givers.Values.Contains(persons[next]) || persons[index].Name == persons[next].Spouse)
				{
					countWithoutConclusion++;
					continue;
				}

				givers.Add(persons[index].Name, persons[next]);
				index++;
			}

			var apiKey = "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";
			var client = new SendGridClient(apiKey);

			//foreach (var item in givers)
			//{
			//	SendEmail(client, item.Value, item.Key);
			//}

			SendEmail(client, new Person { Name = "Ula", Email = "ula.wrzosek@gmail.com", Spouse = "AndrzejJunior" }, "Ania");
		}

		private static void SendEmail(SendGridClient client, Person buyer, string receiver)
		{
			// Send a Single Email using the Mail Helper
			var from = new EmailAddress("santa@santa.com", "Santa Claus");
			var subject = "Mikołaj potrzebuje pomocy!";
			var to = new EmailAddress(buyer.Email);
			var plainTextContent = $"Witaj {buyer.Name}, z racji tego, że spotkały mnie duże problemy w Laponii, " +
				"żona jest piąty raz na odwyku, a renifery uciekły albo przebiegłe elfy je zjadły, potrzebuje Twojej pomocy. " +
				$"Wypadało by kupić prezent takiej osobie: {receiver}. Ponoć była grzeczna, ale któż tam to wie. " +
				"Z góry dzięki za pomoc, Santa. PS. Limit na prezent to w przeliczeniu na PLN 100. Niedługo Ci je oddam, pewnie po Nowym Roku.";
			var htmlContent = $"<strong>Witaj {buyer.Name},</strong><p>z racji tego, że spotkały mnie duże problemy w Laponii, <br />" +
				"żona jest piąty raz na odwyku, a renifery uciekły albo przebiegłe elfy je zjadły, potrzebuje Twojej pomocy. <br />" +
				$"Wypadało by kupić prezent takiej osobie: {receiver}. Ponoć była grzeczna, ale któż tam to wie. <br />" + 
				"Z góry dzięki za pomoc, <br /> Santa. <br />" +
				"PS. Limit na prezent to w przeliczeniu na PLN 100. Niedługo Ci je oddam, pewnie po Nowym Roku.</p>";
			var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

			try
			{
				var response = client.SendEmailAsync(msg).Result;
				LogData($"Success: {buyer.Name} [{buyer.Email}] is buying for: {receiver} [emailStatusCode: {response.StatusCode}]");
			}
			catch (Exception ex)
			{
				LogData($"Failure: {buyer.Name} [{buyer.Email}] is buying for: {receiver} [{ex}]");
			}
		}

		private static void LogData(string message)
		{
			File.AppendAllText(@"c:\temp\santa.txt", "\r\n" + message);
		}
	}
}
