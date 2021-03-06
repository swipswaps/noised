using Mono.Data.Sqlite;

namespace Noised.Core.DB.Sqlite
{
    /// <summary>
    ///		Sqlite implementation of IUnitOfWork
    /// </summary>
    public class SqliteUnitOfWork : IUnitOfWork
    {
        private IPluginRepository pluginRepository;
        private IPlaylistRepository playlistRepository;
        private IUserRepository userRepository;
        private IMetaFileRepository metaFileRepository;
        private readonly SqliteConnection connection;
        private readonly SqliteTransaction transaction;

        public SqliteUnitOfWork()
        {
            var connectionString = "Data Source=" + SqliteFileSource.GetDBFileName();
            connection = new SqliteConnection(connectionString);
            connection.Open();
            transaction = connection.BeginTransaction();
        }

        #region IUnitOfWork implementation

        public IPluginRepository PluginRepository
        {
            get
            {
                if (pluginRepository == null)
                {
                    pluginRepository = new SqlitePluginRepository(connection);
                }
                return pluginRepository;
            }
        }

        /// <summary>
        /// Repository for Playlists
        /// </summary>
        public IPlaylistRepository PlaylistRepository
        {
            get
            {
                if (playlistRepository == null)
                    playlistRepository = new SqlitePlaylistRepository(connection);
                return playlistRepository;
            }
        }

        /// <summary>
        /// Repository for Users
        /// </summary>
        public IUserRepository UserRepository
        {
            get
            {
                if (userRepository == null)
                    userRepository = new SqliteUserRepostiory(connection);
                return userRepository;
            }
        }

        /// <summary>
        /// Repository for MetaFiles
        /// </summary>
        public IMetaFileRepository MetaFileRepository
        {
            get
            {
                if (metaFileRepository == null)
                    metaFileRepository = new SqliteMetaFileRepository(connection);
                return metaFileRepository;
            }
        }

        public void SaveChanges()
        {
            transaction.Commit();
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            if (connection != null)
            {
                connection.Close();
            }
        }

        #endregion
    };
}
