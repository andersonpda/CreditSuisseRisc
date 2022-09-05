using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CreditSuisseRisk
{
	public class Category
	{
		public string Name { get; set; }
		public Expression<Func<ITrade, DateTime, bool>> Eval { get; set; }
	}
}
