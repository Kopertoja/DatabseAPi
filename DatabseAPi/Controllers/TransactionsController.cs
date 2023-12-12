using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using DatabseAPi;
using Npgsql;
using Transaction = DatabseAPi.Transaction;

namespace DatabaseAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TransactionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Metoda do tworzenia nowego rekordu w tabeli "transaction"
        [HttpPost]
        public IActionResult CreateTransaction(DatabseAPi.Transaction transaction)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "INSERT INTO transaction (customer_id, product_id, quantity, transaction_date) VALUES (@CustomerId, @ProductId, @Quantity, @TransactionDate)";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerId", transaction.CustomerId);
                command.Parameters.AddWithValue("@ProductId", transaction.ProductId);
                command.Parameters.AddWithValue("@Quantity", transaction.Quantity);
                command.Parameters.AddWithValue("@TransactionDate", transaction.TransactionDate);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return Ok();
        }

        [HttpGet("all")]
        public IActionResult ReadAllTransactions()
        {
            List<DatabseAPi.Transaction> transactions = new List<Transaction>();

            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "SELECT * FROM transaction";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);

                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Transaction transaction = new Transaction
                    {
                        Id = (int)reader["id"],
                        CustomerId = (int)reader["customer_id"],
                        ProductId = (int)reader["product_id"],
                        Quantity = (int)reader["quantity"],
                        TransactionDate = (DateTime)reader["transaction_date"]


                        
                    };
                    transactions.Add(transaction);
                }
            }

            return Ok(transactions);
        }


        // Metoda do odczytywania rekordu z tabeli "transaction" na podstawie ID
        [HttpGet("{id}")]
        public IActionResult ReadTransaction(int id)
        {
            DatabseAPi.Transaction transaction = new DatabseAPi.Transaction();

            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "SELECT * FROM transaction WHERE id = @Id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    transaction.Id = (int)reader["id"];
                    transaction.CustomerId = (int)reader["customer_id"];
                    transaction.ProductId = (int)reader["product_id"];
                    transaction.Quantity = (int)reader["quantity"];
                    transaction.TransactionDate = (DateTime)reader["transaction_date"];
                }
                else
                {
                    return NotFound();
                }
            }

            return Ok(transaction);
        }


        // Metoda do aktualizacji rekordu w tabeli "transaction"
        [HttpPut("{id}")]
        public IActionResult UpdateTransaction(int id, DatabseAPi.Transaction transaction)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "UPDATE transaction SET customer_id = @CustomerId, product_id = @ProductId, quantity = @Quantity, transaction_date = @TransactionDate WHERE id = @Id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@CustomerId", transaction.CustomerId);
                command.Parameters.AddWithValue("@ProductId", transaction.ProductId);
                command.Parameters.AddWithValue("@Quantity", transaction.Quantity);
                command.Parameters.AddWithValue("@TransactionDate", transaction.TransactionDate);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return NotFound();
                }
            }

            return Ok();
        }

        // Metoda do usuwania rekordu z tabeli "transaction" na podstawie ID
        [HttpDelete("{id}")]
        public IActionResult DeleteTransaction(int id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "DELETE FROM transaction WHERE id = @Id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return NotFound();
                }
            }

            return Ok();
        }
    }
}
