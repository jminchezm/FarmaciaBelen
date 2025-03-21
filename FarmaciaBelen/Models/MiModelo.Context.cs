namespace FarmaciaBelen.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DBFARMACIABELENEntities : DbContext
    {
        public DBFARMACIABELENEntities()
            : base("name=DBFARMACIABELENEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AREA> AREA { get; set; }
        public virtual DbSet<EMPLEADO> EMPLEADO { get; set; }
        public virtual DbSet<PERSONA> PERSONA { get; set; }
        public virtual DbSet<PUESTO> PUESTO { get; set; }
        public virtual DbSet<ROL> ROL { get; set; }
        public virtual DbSet<USUARIO> USUARIO { get; set; }
    }
}
