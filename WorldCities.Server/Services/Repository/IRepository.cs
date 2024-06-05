namespace WorldCities.Server.Services.Repository
{
    public interface IRepository<T>
    {
        /// <summary>
        /// Fetch all T instances from database
        /// </summary>
        /// <returns>T IEnumerable</returns>
        public Task<IEnumerable<T>> GetAll();
        /// <summary>
        /// Fetch T instance by id
        /// </summary>
        /// <param name="id">Fetched T instace id</param>
        /// <returns>T instance</returns>
        public Task<T> GetByIdAsync(int id);
        /// <summary>
        /// Add T instance to db
        /// </summary>
        /// <param name="instance">T instance</param>
        /// <returns>Result of db.SaveChanges(), true if saved > 0 rows</returns>
        public bool AddInstance(T instance);
        /// <summary>
        /// Update T instance
        /// </summary>
        /// <param name="instance">New T instance</param>
        /// <returns>Result of db.SaveChanges(), true if saved > 0 rows</returns>
        public bool UpdateInstance(T instance);
        /// <summary>
        /// Delete T instance by id
        /// </summary>
        /// <param name="id">Deleted T instance id</param>
        /// <returns>Result of db.SaveChanges(), true if saved > 0 rows</returns>
        public bool DeleteInstance(int id);
        /// <summary>
        /// Save Changes
        /// </summary>
        /// <returns>Result of db.SaveChanges(), true if saved > 0 rows</returns>
        public bool Save();
    }
}
