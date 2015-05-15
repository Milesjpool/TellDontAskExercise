namespace Main
{
	public interface IOutboundEvents
	{
		void AccountCreatedSuccessfully(int accountId);
		void NewAccountBalance(int accountId, int newBalanceDepositAmount);
		void AccountNotFound(int accountId);
		void OrderPlaced(int accountId, int amount);
		void PlaceWholeSaleOrder(int quantity);
		void OrderNotFound(int accountId, int orderId);
		void OrderCancelled(int accountId, int orderId);
		void OrderRejected(int accountId);
	}
}