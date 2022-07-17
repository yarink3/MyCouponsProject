using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace MyCouponsServer.Models
{
    public class CouponContext :DbContext
    {
        public CouponContext(DbContextOptions<CouponContext> options) : base(options)
        {
        }

        public DbSet<Coupon> CouponsList { get; set; } = null!;
    }
}

