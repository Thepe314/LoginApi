using LoginApi.Models;
using Microsoft.Data.SqlClient;



namespace LoginApi.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        //readonly

        private readonly string _connectionString;

        public AuthRepository(IConfiguration configuration)
        {
           

            _connectionString = configuration.GetConnectionString("AuthConnection")
            ?? throw new InvalidOperationException("Connection String 'AuthConnection' does not exist");
        }

        //Signup (EF)

        //   public async Task RegisterUser (User user)
        // {
       
        //     _context.Users.Add(user);
        //    await _context.SaveChangesAsync();

        // }


    //     public async Task<User?> CheckByEmail(string email)
    //     {
    //         return await _context.Users
    //         .FirstOrDefaultAsync(e=> e.Email == email);
    //     }

    //     public async Task UpdateUser(User user)
    //     {
    //           _context.Users.Update(user);
    //           await _context.SaveChangesAsync();

    //     }


    //    public async Task<User?> GetByResetToken (string token)
    //     {
    //         return await _context.Users
    //         .FirstOrDefaultAsync(u => u.ResetToken == token);
    //     }



     //Signup(Sql Connection/Transmission)
        public async Task RegisterUser (User user)
        {
       
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

             await using SqlTransaction transaction = (SqlTransaction)await connection.BeginTransactionAsync();

            try
            {
                var sql = 
                @"
                    INSERT INTO Users
                        (FullName,Email,Password,ContactNumber,Address,Role,ResetToken,ResetTokenExpiry) 
                    VALUES
                    (@FullName,@Email,@Password,@ContactNumber,@Address,@Role,@ResetToken,@ResetTokenExpiry)";
                
                     await using var cmd = new SqlCommand(sql, connection, transaction);

                    cmd.Parameters.AddWithValue("@FullName",user.FullName);
                    cmd.Parameters.AddWithValue("@Email",user.Email);
                    cmd.Parameters.AddWithValue("@Password",user.Password);
                    cmd.Parameters.AddWithValue("@ContactNumber",user.ContactNumber);
                    cmd.Parameters.AddWithValue("@Address",user.Address);
                    cmd.Parameters.AddWithValue("@Role",user.Role);
                    cmd.Parameters.AddWithValue("@ResetToken",user.ResetToken ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ResetTokenExpiry",user.ResetTokenExpiry ?? (Object)DBNull.Value);


                    await cmd.ExecuteNonQueryAsync();
                    await transaction.CommitAsync();
            
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

          public async Task<User?> CheckByEmail(string email)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM Users WHERE Email = @Email";

            await using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", email);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FullName = reader.GetString(reader.GetOrdinal("FullName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Password = reader.GetString(reader.GetOrdinal("Password")),
                    ContactNumber = reader.GetString(reader.GetOrdinal("ContactNumber")),
                    Address = reader.GetString(reader.GetOrdinal("Address")),
                    Role = reader.GetString(reader.GetOrdinal("Role")),
                    ResetToken = reader.IsDBNull(reader.GetOrdinal("ResetToken")) ? null : reader.GetString(reader.GetOrdinal("ResetToken")),
                    ResetTokenExpiry = reader.IsDBNull(reader.GetOrdinal("ResetTokenExpiry")) ? null : reader.GetDateTime(reader.GetOrdinal("ResetTokenExpiry"))
                };
            }

            return null;
        }


           public async Task<User?> LoginUser(User user)
        {
       
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM Users WHERE Email = @Email AND Password = @Password";

            await using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", user.Password);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FullName = reader.GetString(reader.GetOrdinal("FullName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Password = reader.GetString(reader.GetOrdinal("Password")),
                    ContactNumber = reader.GetString(reader.GetOrdinal("ContactNumber")),
                    Address = reader.GetString(reader.GetOrdinal("Address")),
                    Role = reader.GetString(reader.GetOrdinal("Role")),
                    ResetToken = reader.IsDBNull(reader.GetOrdinal("ResetToken")) ? null : reader.GetString(reader.GetOrdinal("ResetToken")),
                    ResetTokenExpiry = reader.IsDBNull(reader.GetOrdinal("ResetTokenExpiry")) ? null : reader.GetDateTime(reader.GetOrdinal("ResetTokenExpiry"))
                };
            }

             return null;
        }


          public async Task UpdateUser (User user)
        {
       
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

             await using SqlTransaction transaction = (SqlTransaction)await connection.BeginTransactionAsync();

            try
            {
                var sql = 
                @"
                    UPDATE Users SET
                        FullName = @FullName,
                        Email = @Email,
                        Password = @Password,
                        ContactNumber = @ContactNumber,
                        Address = @Address,
                        Role =@Role,
                        ResetToken = @ResetToken,
                        ResetTokenExpiry = @RestTokenExpiry
                    WHERE Id = @Id";
                
                    await using var cmd = new SqlCommand(sql, connection, transaction);

                    cmd.Parameters.AddWithValue("@FullName", user.FullName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@ContactNumber", user.ContactNumber);
                    cmd.Parameters.AddWithValue("@Address", user.Address);
                    cmd.Parameters.AddWithValue("@Role", user.Role);
                    cmd.Parameters.AddWithValue("@ResetToken", user.ResetToken ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ResetTokenExpiry", user.ResetTokenExpiry ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", user.Id);


                    await cmd.ExecuteNonQueryAsync();
                    await transaction.CommitAsync();
            
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

            public async Task<User?> GetByResetToken (string token)
        {
       
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM Users WHERE ResetToken =@ResetToken";

            await using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@ResetToken",token);
          

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FullName = reader.GetString(reader.GetOrdinal("FullName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Password = reader.GetString(reader.GetOrdinal("Password")),
                    ContactNumber = reader.GetString(reader.GetOrdinal("ContactNumber")),
                    Address = reader.GetString(reader.GetOrdinal("Address")),
                    Role = reader.GetString(reader.GetOrdinal("Role")),
                    ResetToken = reader.IsDBNull(reader.GetOrdinal("ResetToken")) ? null : reader.GetString(reader.GetOrdinal("ResetToken")),
                    ResetTokenExpiry = reader.IsDBNull(reader.GetOrdinal("ResetTokenExpiry")) ? null : reader.GetDateTime(reader.GetOrdinal("ResetTokenExpiry"))
                };
            }

             return null;
        }


    }


}