using System.Collections;
using System.Runtime.CompilerServices;
using LoginApi.Data;
using LoginApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace LoginApi.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        //readonly
        private readonly ApplicationDbContext _context;

        private readonly string _connectionString;

        public AdminRepository(ApplicationDbContext context,IConfiguration _configuration)
        {
            _context = context;

            _connectionString = _configuration.GetConnectionString("AuthConnection")
            ?? throw new  InvalidOperationException("Connection String 'AuthConnection' not found");
        }

        //List of all Users in database
        public async Task<IEnumerable<UserDto>> ListAllUsers()
        {
            return await _context.Users
                        .Select(u => new UserDto
                        {
                            FullName = u.FullName,
                            Email = u.Email,
                            Role = u.Role,
                        })
                        .ToListAsync();
            
        }

        //Using SQL connection and transaction
        public async Task CreateUser(User user)
        {
            //Create a new SQL connection
            await using var connection = new SqlConnection(_connectionString);

            //We open the connection.
            await connection.OpenAsync();

            //Begin a SQL transaction to ensure the operation is atomic
            await using SqlTransaction transaction = (SqlTransaction) await connection.BeginTransactionAsync();

            try
            {
                //SQL query to insert a new user
                var sql = @"
                INSERT INTO Users
                    (FullName,Email,Password,ContactNumber,Address,Role,ResetToken,ResetTokenExpiry)
                VALUES
                     (@FullName,@Email,@Password,@ContactNumber,@Address,@Role,@ResetToken,@ResetTokenExpiry)";
                
                //Create SQL command with connection and transaction
                await using var cmd = new SqlCommand(sql,connection,transaction);

                //Add parameters to prevent SQL injection
                cmd.Parameters.AddWithValue("@FullName",user.FullName);
                cmd.Parameters.AddWithValue("@Email",user.Email);
                cmd.Parameters.AddWithValue("@Password",user.Password);
                cmd.Parameters.AddWithValue("@ContactNumber",user.ContactNumber);
                cmd.Parameters.AddWithValue("@Address",user.Address);
                cmd.Parameters.AddWithValue("@Role",user.Role);
                cmd.Parameters.AddWithValue("@ResetToken",user.ResetToken ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FullName",user.ResetTokenExpiry ?? (object)DBNull.Value);

                //Execute the insert command
                await cmd.ExecuteNonQueryAsync();

                //Commit transaction (save changes permanently)
                await transaction.CommitAsync();

            }
            catch
            {   
                //If an error occurs, rollback the transaction
                await transaction.RollbackAsync();
                throw; 
            }
        }


    }


}