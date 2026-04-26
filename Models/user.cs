using SQLite;

namespace parcel_station1.Models
{
    // User 是用户数据模型
    // 对应 SQLite 数据库中的 User 表
    // 主要用于保存用户注册和登录信息
    public class User
    {
        // 主键 Id
        // PrimaryKey 表示这是数据库表的主键
        // AutoIncrement 表示每新增一个用户，Id 会自动递增
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // 用户名
        // Unique 表示用户名必须唯一，不能重复注册相同用户名
        [Unique]
        public string Username { get; set; } = "";

        // 用户密码
        // 用于登录验证
        // 注意：当前项目中密码是直接保存的，适合作业演示
        // 真实项目中通常需要加密或哈希处理密码
        public string Password { get; set; } = "";
    }
}