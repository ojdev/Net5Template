using CoreTemplate.Infrastructure.NpgSql;
using Microsoft.EntityFrameworkCore;
using System;

namespace IntegrationTests.Data
{
    public class DataSeeder
    {
        public readonly InfrastructureContext _context;

        public DataSeeder(InfrastructureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Seed()
        {
            _context.Database.EnsureCreated();
            _context.Database.Migrate();
            //----------------开始初始化测试数据----------------------//


            //----------------完成初始化测试数据----------------------//
            _context.SaveChanges();
        }
    }
}
