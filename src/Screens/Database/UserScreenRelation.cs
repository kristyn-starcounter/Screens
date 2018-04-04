using Starcounter;

namespace Screens
{
    [Database]
    public class UserScreenRelation
    {
        public User User { get; set; }
        public Screen Screen { get; set; }
    }
}
