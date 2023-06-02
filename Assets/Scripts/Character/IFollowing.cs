using UnityEngine;

namespace Character
{
    public interface IFollowing
    {
        public Transform FollowTarget { get; set; }
    }
}