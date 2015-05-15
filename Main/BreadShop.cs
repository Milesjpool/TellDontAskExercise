using System;
using Main;

namespace Bread
{
	public class BreadShop
	{
		private IOutboundEvents _events;
		private const int PriceOfBread = 12;
		private readonly AccountRepository _accountRepository = new AccountRepository();

		public BreadShop(IOutboundEvents events)
		{
			_events = events;
		}

		public void CreateAccount(int accountId)
		{
			var newAccount = new Account();
			_accountRepository.AddAccount(accountId, newAccount);
			_events.AccountCreatedSuccessfully(accountId);
		}

		public void Deposit(int accountId, int creditAmount)
		{
			var account = _accountRepository.GetAccount(accountId);
			if (account != null)
			{
				var newBalance = account.Deposit(creditAmount);
				_events.NewAccountBalance(accountId,newBalance);
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
				account.AddOrder(accountId, orderId, amount, _events, PriceOfBread);
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

			int? cancelledQuantity = account.CancelOrder( orderId );
			if ( cancelledQuantity == null )
			{
				_events.OrderNotFound( accountId, orderId );
				return;
			}

			var newBalance = account.Deposit( cancelledQuantity.Value * PriceOfBread );
			_events.OrderCancelled( accountId, orderId );
			_events.NewAccountBalance( accountId, newBalance );
		}

		public void PlaceWholesaleOrder()
		{
			throw new NotImplementedException("Implement me in Object A");
		}
	}
}