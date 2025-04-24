namespace BadCode;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool Active { get; set; }
}

public class UserService
{
    private List<User> _users = new List<User>();
    
    public bool AddUser(string username, string email)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
        {
            return false;
        }
        
        foreach (var user in _users)
        {
            if (user.Username == username || user.Email == email)
            {
                return false;
            }
        }
        
        int newId = _users.Count > 0 ? _users[_users.Count - 1].Id + 1 : 1;
        
        var newUser = new User
        {
            Id = newId,
            Username = username,
            Email = email,
            Active = true
        };
        
        _users.Add(newUser);
        
        Console.WriteLine($"Added user: {username}");
        
        return true;
    }
    
    public User GetUser(int id)
    {
        foreach (var user in _users)
        {
            if (user.Id == id)
            {
                return user;
            }
        }
        
        return null;
    }
    
    public bool UpdateUser(int id, string newUsername, string newEmail, bool updateActive, bool activeStatus)
    {
        User user = GetUser(id);
        
        if (user == null)
        {
            return false;
        }
        
        if (!string.IsNullOrEmpty(newUsername))
        {
            user.Username = newUsername;
        }
        
        if (!string.IsNullOrEmpty(newEmail))
        {
            user.Email = newEmail;
        }
        
        if (updateActive)
        {
            user.Active = activeStatus;
        }
        
        return true;
    }
    
    public List<User> FindUsers(string searchTerm)
    {
        List<User> results = new List<User>();
        
        Console.WriteLine($"Searching for users with term: {searchTerm}");
        
        if (string.IsNullOrEmpty(searchTerm))
        {
            return _users;
        }
        
        foreach (var user in _users)
        {
            if (user.Username.Contains(searchTerm) || user.Email.Contains(searchTerm))
            {
                results.Add(user);
            }
        }
        
        return results;
    }
}