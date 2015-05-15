using System.Collections.Generic;

namespace Main
{
	public class Account
	{
		private int _balance = 0;
		private IDictionary<int,int> _orders = new Dictionary<int, int>();

		public void Deposit(int creditAmount, IOutboundEvents events, int accountId)
		{
			_balance += creditAmount;
			events.NewAccountBalance(accountId, _balance);
		}

		public void AddOrder(int accountId, int orderId, int amount, IOutboundEvents events, int priceOfBread)
		{
			var cost = amount * priceOfBread;
			if (_balance >= cost)
			{
				_orders.Add(orderId, amount);
				Deposit(-cost, events, accountId);
				events.OrderPlaced(accountId, amount);
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