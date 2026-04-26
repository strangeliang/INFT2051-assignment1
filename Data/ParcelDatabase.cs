using SQLite;
using parcel_station1.Models;

namespace parcel_station1.Data
{
    // ParcelDatabase manages local SQLite storage for users and parcels.
    public class ParcelDatabase
    {
        // SQLite asynchronous database connection.
        private readonly SQLiteAsyncConnection _database;

        // Prevents database tables from being initialized repeatedly.
        private bool _initialized;

        public ParcelDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
        }

        // Creates the required database tables if they do not already exist.
        public async Task InitAsync()
        {
            if (_initialized)
                return;

            await _database.CreateTableAsync<Parcel>();
            await _database.CreateTableAsync<User>();

            _initialized = true;
        }

        // =========================
        // Parcel methods
        // =========================

        // Gets all parcel records from the database.
        public async Task<List<Parcel>> GetParcelsAsync()
        {
            await InitAsync();
            return await _database.Table<Parcel>().ToListAsync();
        }

        // Gets all parcels that belong to a specific user.
        public async Task<List<Parcel>> GetParcelsByUsernameAsync(string username)
        {
            await InitAsync();
            return await _database.Table<Parcel>()
                                  .Where(p => p.Username == username)
                                  .ToListAsync();
        }

        // Inserts a new parcel or updates an existing parcel.
        public async Task<int> SaveParcelAsync(Parcel parcel)
        {
            await InitAsync();

            if (parcel.Id != 0)
                return await _database.UpdateAsync(parcel);

            return await _database.InsertAsync(parcel);
        }

        // Updates an existing parcel, such as changing its status to Collected.
        public async Task<int> UpdateParcelAsync(Parcel parcel)
        {
            await InitAsync();
            return await _database.UpdateAsync(parcel);
        }

        // Deletes a parcel record from the local database.
        public async Task<int> DeleteParcelAsync(Parcel parcel)
        {
            await InitAsync();
            return await _database.DeleteAsync(parcel);
        }

        // Finds a parcel by parcel code across all users.
        public async Task<Parcel?> GetParcelByCodeAsync(string parcelCode)
        {
            await InitAsync();
            return await _database.Table<Parcel>()
                                  .FirstOrDefaultAsync(p => p.ParcelCode == parcelCode);
        }

        // Finds a parcel by parcel code only within the current user's records.
        public async Task<Parcel?> GetParcelByCodeAndUsernameAsync(string parcelCode, string username)
        {
            await InitAsync();
            return await _database.Table<Parcel>()
                                  .FirstOrDefaultAsync(p => p.ParcelCode == parcelCode && p.Username == username);
        }

        // =========================
        // User methods
        // =========================

        // Saves a new user account to the database.
        public async Task<int> SaveUserAsync(User user)
        {
            await InitAsync();
            return await _database.InsertAsync(user);
        }

        // Finds a user by username, mainly used to prevent duplicate registration.
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            await InitAsync();
            return await _database.Table<User>()
                                  .FirstOrDefaultAsync(u => u.Username == username);
        }

        // Finds a user by username and password for login authentication.
        public async Task<User?> GetUserAsync(string username, string password)
        {
            await InitAsync();
            return await _database.Table<User>()
                                  .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }
    }
}