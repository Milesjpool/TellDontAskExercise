using System;
using Main;
using Moq;
using NUnit.Framework;

namespace Bread
{
	[TestFixture]
    public class BreadShopTests
    {
		private const int AccountIdOne = 1;
		private IOutboundEvents _events;
		private BreadShop _breadShop;
		private Mock<IOutboundEvents> _mock;
		private const int OrderIdOne = 1;
		private const int AccountIdTwo = 2;
		private const int OrderIdTwo = 2;

		[SetUp]
		public void SetUp()
		{
			_mock = new Mock<IOutboundEvents>();
			_events = _mock.Object;
			_breadShop = new BreadShop(_events);
		}
		[Test]
		public void CreateAnAccount()
		{
			_breadShop.CreateAccount(AccountIdOne);
			ExpectAccountCreationSuccess(AccountIdOne);
		}

		[Test]
		public void DepositSomeMoney()
		{
			CreateAccount(AccountIdOne);
			const int depositAmount = 300;
			_breadShop.Deposit(AccountIdOne, depositAmount);
			ExpectNewBalance(AccountIdOne, depositAmount);
		}

		[Test]
		public void RejectDepositsForNonExistantAccounts()
		{
			var nonExistentAccountId = -5;
			_breadShop.Deposit(nonExistentAccountId,4000);
			ExpectAccountNotFound(nonExistentAccountId);
		}

		[Test]
		public void DepositesAddUp()
		{
			CreateAccountWithBalance(AccountIdOne,300);
			_breadShop.Deposit(AccountIdOne,300);
			ExpectNewBalance(AccountIdOne,600);
		}

		[Test]
		public void PlaceAnOrderSucceedsIfThereIsEnoughMoney()
		{
			CreateAccountWithBalance(AccountIdOne,500);
			_breadShop.PlaceOrder(AccountIdOne, OrderIdOne, 40);
			ExpectOrderPlaced(AccountIdOne, 40);
			ExpectNewBalance(AccountIdOne,500 - (Cost(40)));
		}

		[Test]
		public void CannotPlaceOrderForNonexistentAccount() 
		{			
			_breadShop.PlaceOrder(-5, OrderIdOne, 40);
			ExpectAccountNotFound(-5);
		}
	
		[Test]
		public void cannot_place_an_order_for_more_than_account_can_afford() 
		{
			CreateAccountWithBalance(AccountIdOne, 500);
			_breadShop.PlaceOrder( AccountIdOne, OrderIdOne, 42 );
			// 42 * 12 = 504
			ExpectOrderRejected(AccountIdOne);
			
		}

		[Test]
		public void CancelAnOrderById() 
		{
			const int balance = 500;
			CreateAccountWithBalance(AccountIdOne, balance);

			var amount = 40;
			_breadShop.PlaceOrder(AccountIdOne, OrderIdOne, amount);
			_breadShop.CancelOrder(AccountIdOne, OrderIdOne);

			ExpectOrderCancelled(AccountIdOne, OrderIdOne);
			ExpectNewBalance(AccountIdOne, balance);
		}
		
		[Test]
		public void cannot_cancel_an_order_for_nonexistent_account()
		{
			_breadShop.CancelOrder( -5, OrderIdOne );
			ExpectAccountNotFound( -5 );
		}

		[Test]
		public void CannotCancelNonexistentOrder()
		{
			CreateAccount(AccountIdOne );
			_breadShop.CancelOrder( AccountIdOne, -5 );
			ExpectOrderNotFound( -5 );
			
		}

		[Test]
		public void cancelling_an_allows_balance_to_be_reused()
		{
			int balance = 500;
			CreateAccountWithBalance(AccountIdOne, balance );

			int amount = 40;
			_breadShop.PlaceOrder( AccountIdOne, OrderIdOne, amount);
			_breadShop.CancelOrder(AccountIdOne, OrderIdOne);
						
			// it's entirely possible that the balance in the resulting event doesn't match the internal
			// state of the system, so we ensure the balance has really been restored
			// by trying to place a new order with it.
			_breadShop.PlaceOrder( AccountIdOne, OrderIdTwo, amount );
			
			ExpectOrderPlaced(AccountIdOne, amount );
			ExpectNewBalance(AccountIdOne, balance - ( Cost( amount ) ) );
		}

		[Test, Ignore( "Objective A" )]
		public void an_empty_shop_places_an_empty_wholesale_order()
		{			
			_breadShop.PlaceWholesaleOrder();
			ExpectWholesaleOrder( 0 );
		}

		[Test, Ignore( "Objective A" )]
		public void Wholesale_orders_are_made_for_the_sum_of_the_quantities_of_outstanding_orders_in_one_account()
		{			
			int balance = Cost( 40 + 55 );
			CreateAccountWithBalance(AccountIdOne, balance );
			_breadShop.PlaceOrder(AccountIdOne,OrderIdOne,40);
			_breadShop.PlaceOrder( AccountIdOne, OrderIdOne, 55 );			

			_breadShop.PlaceWholesaleOrder();

			ExpectWholesaleOrder( 40 + 55 );
		}

		[Test, Ignore( "Objective A" )]
		public void wholesale_orders_are_made_for_the_sum_of_the_quantities_of_outstanding_orders_across_accounts()
		{
			CreateAccountAndPlaceOrder( AccountIdOne, OrderIdOne, 40 );
			CreateAccountAndPlaceOrder( AccountIdTwo, OrderIdTwo, 55 );

			_breadShop.PlaceWholesaleOrder();

			ExpectWholesaleOrder( 40 + 55 );
		}

		private void ExpectWholesaleOrder(int quantity)
		{
			_mock.Verify(e=>e.PlaceWholeSaleOrder(quantity));
		}

		private void ExpectOrderNotFound(int orderId)
		{
			_mock.Verify(e=>e.OrderNotFound(AccountIdOne, orderId));
		}

		private void ExpectOrderCancelled(int accountId, int orderId)
		{
			_mock.Verify(e=>e.OrderCancelled(accountId,orderId));
		}

		private void ExpectOrderPlaced(int accountId, int amount)
		{
			_mock.Verify(e=>e.OrderPlaced(accountId,amount));
		}

		private void ExpectOrderRejected(int accountId)
		{
			_mock.Verify(e=>e.OrderRejected(accountId));
		}

		private int Cost(int quantity)
		{
			return quantity*12;
		}

		private void CreateAccountWithBalance(int accountId, int initialBalance)
		{
			CreateAccount(accountId);
			_breadShop.Deposit(accountId,initialBalance);
		}

		private void ExpectAccountNotFound(int accountId)
		{
			_mock.Verify(e=>e.AccountNotFound(accountId));
		}

		private void ExpectNewBalance(int accountId, int newBalanceDepositAmount)
		{
			_mock.Verify(e=>e.NewAccountBalance(accountId, newBalanceDepositAmount));
		}

		private void CreateAccount(int accountId)
		{
			 _breadShop.CreateAccount(accountId);
		}

		private void ExpectAccountCreationSuccess(int accountId)
		{
			_mock.Verify(e=>e.AccountCreatedSuccessfully(accountId));
		}

		private void CreateAccountAndPlaceOrder(int accountIdOne, int orderIdOne, int p2)
		{
			throw new NotImplementedException();
		}
    }
}
