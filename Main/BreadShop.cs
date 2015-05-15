using System;
using Main;

namespace Bread
{
	public class BreadShop
	{
		private readonly IOutboundEvents _events;
		private const int PriceOfBread = 12;
		private readonly AccountRepository _accountRepository = new AccountRepository();

		public BreadShop(IOutboundEvents events)
		{
			_events = events;
		}

		public void CreateAccount(int accountId)
		{
			var newAccount = new Account(accountId, _events);
			_accountRepository.AddAccount(accountId, newAccount);
			_events.AccountCreatedSuccessfully(accountId);
		}

		public void Deposit(int accountId, int creditAmount)
		{
			var account = _accountRepository.GetAccount(accountId);
			if (account != null)
			{
				account.Deposit(creditAmount);
			}
			else
			{
				_events.AccountNotFound(accountId);
			}
		}

		public void PlaceOrder(int accountId, int orderId, int amount)
		{
			var account = _accountRepository.GetAccount(accountId);
			if (account != null)
			{
				account.AddOrder(orderId, amount, PriceOfBread);
			}
			else
			{
				_events.AccountNotFound( accountId );
			}
		}


		public void CancelOrder(int accountId, int orderId)
		{
			var account = _accountRepository.GetAccount( accountId );
			if ( account == null )
			{
				_events.AccountNotFound( accountId );
				return;
			}

			account.CancelOrder(accountId, orderId, PriceOfBread);
		}

		public void PlaceWholesaleOrder()
		{
			throw new NotImplementedException("Implement me in Object A");
		}
	}
}