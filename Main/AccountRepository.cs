using System.Collections.Generic;
using Main;

namespace Bread
{
	public class AccountRepository
	{
		private IDictionary<int, Account> _accounts = new Dictionary<int, Account>();
		public void AddAccount(int accountId, Account account)
		{
			_accounts.Add(accountId,account);
		}

		public Account GetAccount(int accountId)
		{
			return _accounts.ContainsKey(accountId) ? _accounts[accountId] : null;
		}
	}
}