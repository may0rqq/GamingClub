using System;
using System.Collections.Generic;
using System.Text;

namespace GamingClub
{
        public abstract class Entity
        {
            public Guid Id { get; } = Guid.NewGuid();
            public DateTime CreatedAt { get; } = DateTime.Now;

            public abstract string GetInfo();
        }
    }


