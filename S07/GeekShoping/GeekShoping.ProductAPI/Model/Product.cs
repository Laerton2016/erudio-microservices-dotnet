using GeekShoping.ProductAPI.Model.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekShoping.ProductAPI.Model
{
    [Table("TB_Product")]
    public class Product : BaseEntity
    {
        [Column("name"), Required, StringLength(150)]
        public required string Name { get; set; }

        [Column("Price"), Required, Range(1, 10000)]
        public decimal Price { get; set; }

        [Column("Description"), StringLength(500)]
        public string? Description { get; set; }

        [Column("CategoryName"), StringLength(50)]
        public string? CategoryName { get; set; }

        [Column("ImageURL"), StringLength(300)]
        public string? ImageURL { get; set; }
    }
}
