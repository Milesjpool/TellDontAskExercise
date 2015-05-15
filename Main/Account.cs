using System.Collections.Generic;

namespace Main
{
	public class Account
	{
		private int _balance = 0;
		private IDictionary<int,int> _orders = new Dictionary<int, int>();

		public int Deposit(int creditAmount)
		{
			_balance += creditAmount;
			return _balance;
		}

		public int GetBalance()
		{
			return _balance;
		}

		public void AddOrder(int accountId, int orderId, int amount, IOutboundEvents events, int priceOfBread)
		{
			var cost = amount * priceOfBread;
			if (GetBalance() >= cost)
			{
				_orders.Add(orderId, amount);
				var newBalance = Deposit(-cost);
				events.OrderPlaced(accountId, amount);
				events.NewAccountBalance(accountId, newBalance);
			}
			else
			{
				events.OrderRejected(accountId);
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