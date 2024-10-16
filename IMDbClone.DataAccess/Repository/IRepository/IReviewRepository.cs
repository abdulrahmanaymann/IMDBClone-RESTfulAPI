﻿using IMDbClone.Core.Entities;

namespace IMDbClone.DataAccess.Repository.IRepository
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task UpdateAsync(Review review);
    }
}
