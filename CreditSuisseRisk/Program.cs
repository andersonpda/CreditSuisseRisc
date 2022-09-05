using CreditSuisseRisk;
using System.Globalization;

var categories = new[]
{
	new Category { Name = "EXPIRED", Eval = (ITrade trade, DateTime dateRef) => dateRef.Subtract(trade.NextPaymentDate).Days >= 30 },
	new Category { Name = "HIGHRISK", Eval = (ITrade trade, DateTime dateRef) => trade.Value > 1000000 && trade.ClientSector.Equals(ClientSector.Private.ToString(), StringComparison.InvariantCultureIgnoreCase) },
	new Category { Name = "MEDIUMRISK", Eval = (ITrade trade, DateTime dateRef) => trade.Value > 1000000 && trade.ClientSector.Equals(ClientSector.Public.ToString(), StringComparison.InvariantCultureIgnoreCase) },
};


var step = Step.Date;
DateTime dateRef = DateTime.MinValue;
var quantity = 0;
var actual = 1;
var trades = new List<ITrade>();
string input;

while(step != Step.Results)
{
	switch(step)
	{
		case Step.Date:
			Console.Write("Enter a based datetime valid dd/mm/yyyy: ");
			input = Console.ReadLine();

			if (!DateTime.TryParseExact(input, "dd/MM/yyyy", null, DateTimeStyles.None, out dateRef))
			{
				Console.WriteLine("Invalid date");
				continue;
			}

			step = Step.Quantity;

			break;
		case Step.Quantity:
			Console.Write("Enter a trade quantity: ");
			input = Console.ReadLine();

			var quantityIsValid = int.TryParse(input, out quantity);

			if (!quantityIsValid || quantity <= 0)
			{
				Console.WriteLine("Invalid quantity");
				quantity = 0;
				continue;
			}

			step = Step.Trades;
			
			break;
		case Step.Trades:
			Console.WriteLine($"Enter a trade {actual} e.g.: 1000000 Private {DateTime.Now:dd/MM/yyyy}");
			input = Console.ReadLine();

			var inputArr = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

			if (inputArr.Length != 3)
			{
				Console.WriteLine("Invalid input format");
				continue;
			}

			if (!double.TryParse(inputArr[0], out var value))
			{
				Console.WriteLine("Invalid format");
				continue;
			}

			if (!Enum.TryParse<ClientSector>(inputArr[1], true, out var clientSector))
			{
				Console.WriteLine("Invalid client sector");
				continue;
			}

			if (!DateTime.TryParseExact(inputArr[2], "dd/MM/yyyy", null, DateTimeStyles.None, out var nextPaymentDate))
			{
				Console.WriteLine("Invalid date");
				continue;
			}

			ITrade trade = new Trade(value, clientSector.ToString(), nextPaymentDate);
			trades.Add(trade);

			if (actual < quantity)
				actual++;
			else
				step = Step.Results;


			break;
	}
}

trades.ForEach(trade =>
{
	var risk = categories.FirstOrDefault(c => c.Eval.Compile()(trade, dateRef));
	Console.WriteLine(risk?.Name);
});