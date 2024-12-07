namespace Es2al.Models.Entites
{
    public class UserFollow
    {
        public int FollowingId { get; set; }//That I Make For Him A Follow
        public int FollowerId { get; set; }//After This Iam The Follower 
        public AppUser Following { get; set; } = null!;
        public AppUser Follower { get; set; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            UserFollow userFollow2 = (obj as UserFollow)!;
            return FollowingId == userFollow2.FollowingId && FollowerId == userFollow2.FollowerId;
        }
        public override int GetHashCode()
        {
            Console.WriteLine(FollowingId.GetHashCode() ^ FollowerId.GetHashCode());
            return FollowingId.GetHashCode() ^ FollowerId.GetHashCode();
        }
    }
}
