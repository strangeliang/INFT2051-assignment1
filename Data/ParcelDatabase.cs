using SQLite;
using parcel_station1.Models;

namespace parcel_station1.Data
{
    public class ParcelDatabase
    {
        // SQLite 异步连接对象
        private readonly SQLiteAsyncConnection _database;

        // 防止数据库表被重复初始化
        private bool _initialized;

        // 构造函数：接收数据库文件路径
        public ParcelDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
        }

        // 初始化数据库表
        public async Task InitAsync()
        {
            if (_initialized)
                return;

            // 创建包裹表
            await _database.CreateTableAsync<Parcel>();

            // 创建用户表（用于注册和登录）
            await _database.CreateTableAsync<User>();

            _initialized = true;
        }

        // =========================
        // Parcel（包裹）相关方法
        // =========================

        // 获取数据库中的所有包裹数据
        public async Task<List<Parcel>> GetParcelsAsync()
        {
            await InitAsync();
            return await _database.Table<Parcel>().ToListAsync();
        }

        // 根据用户名获取该用户的所有包裹
        public async Task<List<Parcel>> GetParcelsByUsernameAsync(string username)
        {
            await InitAsync();
            return await _database.Table<Parcel>()
                                  .Where(p => p.Username == username)
                                  .ToListAsync();
        }

        // 保存包裹数据
        public async Task<int> SaveParcelAsync(Parcel parcel)
        {
            await InitAsync();

            if (parcel.Id != 0)
                return await _database.UpdateAsync(parcel);

            return await _database.InsertAsync(parcel);
        }

        // 删除一个包裹数据
        public async Task<int> DeleteParcelAsync(Parcel parcel)
        {
            await InitAsync();
            return await _database.DeleteAsync(parcel);
        }

        // 通过包裹编号查找包裹（全局）
        public async Task<Parcel?> GetParcelByCodeAsync(string parcelCode)
        {
            await InitAsync();
            return await _database.Table<Parcel>()
                                  .FirstOrDefaultAsync(p => p.ParcelCode == parcelCode);
        }

        // 通过包裹编号 + 用户名查找该用户自己的包裹
        public async Task<Parcel?> GetParcelByCodeAndUsernameAsync(string parcelCode, string username)
        {
            await InitAsync();
            return await _database.Table<Parcel>()
                                  .FirstOrDefaultAsync(p => p.ParcelCode == parcelCode && p.Username == username);
        }

        // =========================
        // User（用户）相关方法
        // =========================

        // 保存新用户到数据库
        public async Task<int> SaveUserAsync(User user)
        {
            await InitAsync();
            return await _database.InsertAsync(user);
        }

        // 通过用户名查找用户
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            await InitAsync();
            return await _database.Table<User>()
                                  .FirstOrDefaultAsync(u => u.Username == username);
        }

        // 通过用户名和密码查找用户
        public async Task<User?> GetUserAsync(string username, string password)
        {
            await InitAsync();
            return await _database.Table<User>()
                                  .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }
    }
}