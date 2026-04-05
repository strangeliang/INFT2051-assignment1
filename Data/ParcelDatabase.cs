using SQLite;
using parcel_station1.Models;

namespace parcel_station1.Data
{
    public class ParcelDatabase
    {
        private readonly SQLiteAsyncConnection _database;
        private bool _initialized;

        public ParcelDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
        }

        public async Task InitAsync()
        {
            if (_initialized)
                return;

            await _database.CreateTableAsync<Parcel>();
            _initialized = true;
        }

        public async Task<List<Parcel>> GetParcelsAsync()
        {
            await InitAsync();
            return await _database.Table<Parcel>().ToListAsync();
        }

        public async Task<int> SaveParcelAsync(Parcel parcel)
        {
            await InitAsync();

            if (parcel.Id != 0)
                return await _database.UpdateAsync(parcel);

            return await _database.InsertAsync(parcel);
        }

        public async Task<int> DeleteParcelAsync(Parcel parcel)
        {
            await InitAsync();
            return await _database.DeleteAsync(parcel);
        }
    }
}