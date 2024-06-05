using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mime;

namespace WorldCities.Server.Models
{
    [Table("Cities")]
    [Index(nameof(Name))]
    [Index(nameof(Lat))]
    [Index(nameof(Lon))]
    public class City
    {
        #region properties
        /// <summary>
        /// primary key for city
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }
        
        /// <summary>
        /// city name in UTF-8
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// city latitude
        /// </summary>
        [Column(TypeName ="decimal(7,4)")]
        public required decimal Lat { get; set; }
        
        /// <summary>
        /// city longtitute
        /// </summary>
        [Column(TypeName ="decimal(7,4)")]
        public required decimal Lon { get; set; }

        /// <summary>
        /// coutry id (foreign key) 
        /// </summary>
        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }
        #endregion

        #region Navigation Properties
        /// <summary>
        /// country related to city
        /// </summary>
        public Country? Country { get; set; }
        #endregion
    }
}
