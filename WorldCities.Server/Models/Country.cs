using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldCities.Server.Models
{
    [Table("Countries")]
    [Index(nameof(Name))]
    [Index(nameof(ISO2))]
    [Index(nameof(ISO3))]
    public class Country
    {
        #region Properties
        /// <summary>
        /// primary key
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }
        
        /// <summary>
        /// country name in UTF-8
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// country code (in ISO 3166-1 ALPHA-2 format)
        /// </summary>
        public required string ISO2 { get; set; }
        
        /// <summary>
        /// country code (int ISO 3166-1 ALPHA-3 format)
        /// </summary>
        public required string ISO3 { get; set; }
        #endregion

        #region Navigation Properties
        /// <summary>
        /// cities related to country
        /// </summary>
        public ICollection<City> Cities { get; set;}
        #endregion
    }
}
