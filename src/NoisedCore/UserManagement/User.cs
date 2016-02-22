namespace Noised.Core.UserManagement
{
    /// <summary>
    /// A user using noised
    /// </summary>
    public class User
    {
        #region Properties

        internal string Name { get; set; }

        internal string Password { get; set; }

        #endregion

        #region Constructors

        public User(string name)
        {
            Name = name;
        }

        #endregion

        public override string ToString()
        {
            return Name;
        }
    };
}
