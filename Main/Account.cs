using System.Collections.Generic;

namespace Main
{
	public class Account
	{
		private int _balance;
		private readonly IDictionary<int,int> _orders = new Dictionary<int, int>();
		private readonly IOutboundEvents _events;
		private readonly int _accountId;

		public Account(int accountId, IOutboundEvents events)
		{
			_accountId = accountId;
			_events = events;
		}

		public void Deposit(int creditAmount)
		{
			_balance += creditAmount;
			_events.NewAccountBalance(_accountId, _balance);
		}

		public void AddOrder(int orderId, int amount, int priceOfBread)
		{
			var cost = amount * priceOfBread;
			if (_balance >= cost)
			{
				_orders.Add(orderId, amount);
				Deposit(-cost);
				_events.OrderPlaced(_accountId, amount);
			}
			else
			{
				_events.OrderRejected(_accountId);
			}
		}

		public int? CancelOrder(int orderId)
		{
			if (!_orders.ContainsKey(orderId)) return null; 
			_orders.Remove(orderId);
			return orderId;
		}
	}
}